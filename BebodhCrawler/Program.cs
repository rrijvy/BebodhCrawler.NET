using Common;
using Core.Data;
using Core.Models;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System.Text;
using Core.Helpers;
using System.Text.Json.Serialization;
using AutoMapper;
using Hangfire.PostgreSql;
using Npgsql;

namespace BebodhCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //var mongoDbSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
            //var pgHangfireConfig = builder.Configuration.GetSection("PgHangfireServer").Get<HangfireDbServerSettings>();
            //var crawlerDbSettings = builder.Configuration.GetSection("PgServer").Get<DbServerSettings>();

            var mongoDbSettings = builder.Configuration.GetSection("MongoDB_Dev").Get<MongoDBSettings>();
            var pgHangfireConfig = builder.Configuration.GetSection("PgHangfireServer_Dev").Get<HangfireDbServerSettings>();
            var crawlerDbSettings = builder.Configuration.GetSection("PgServer_Dev").Get<DbServerSettings>();

            var jwtSettings = builder.Configuration.GetSection("JWTCred").Get<JwtSettings>();
            var crawlerConfig = builder.Configuration.GetSection("CrawlerConfig").Get<CrawlerConfig>();

            BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Local, BsonType.String));

            //builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            //builder.Services.Configure<HangfireDbServerSettings>(builder.Configuration.GetSection("PgHangfireServer"));
            //builder.Services.Configure<DbServerSettings>(builder.Configuration.GetSection("PgServer"));

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB_Dev"));
            builder.Services.Configure<HangfireDbServerSettings>(builder.Configuration.GetSection("PgHangfireServer_Dev"));
            builder.Services.Configure<DbServerSettings>(builder.Configuration.GetSection("PgServer_Dev"));

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWTCred"));
            builder.Services.Configure<CrawlerConfig>(builder.Configuration.GetSection("CrawlerConfig"));

            builder.Services.AddDbContext<CrawlerDbContext>(options =>
            {
                options.UseNpgsql(crawlerDbSettings.ConnectionURI);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<CrawlerDbContext>().AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });
            });

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Crawler API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            HelperService.RegisterDependencies(builder.Services);

            EnsureHangfireDatabaseExists(pgHangfireConfig.ConnectionURI);

            builder.Services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(pgHangfireConfig.ConnectionURI));
            });

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            EnsureCrawlerMasterDatabaseExists(app);

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard();

            app.MapHangfireDashboard();

            app.Run();
        }

        public static void EnsureHangfireDatabaseExists(string connectionString)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                var databaseName = builder.Database;
                builder.Database = "";

                var connectionStr = $"Server={builder.Host};Database=postgres;User Id={builder.Username};Password={builder.Password}";

                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();

                    var result = new NpgsqlCommand($"SELECT COUNT(*) FROM pg_database WHERE datname = '{databaseName}'", connection)
                        .ExecuteScalar();

                    if (result != null)
                    {
                        int.TryParse(result.ToString(), out var db_count);

                        var databaseExists = db_count > 0;

                        if (!databaseExists)
                        {
                            new NpgsqlCommand($"CREATE DATABASE {databaseName}", connection).ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void EnsureCrawlerMasterDatabaseExists(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CrawlerDbContext>();
                    if (!context.Database.EnsureCreated())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying database migrations: {ex.Message}");
                }
            }
        }
    }
}
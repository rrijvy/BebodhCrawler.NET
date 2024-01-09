using Common;
using Core.Models;
using Hangfire;

namespace BebodhCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.Configure<SqlServerSettings>(builder.Configuration.GetSection("SqlServer"));
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            HelperService.RegisterDependencies(builder.Services);

            builder.Services.AddHangfire(config => config
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(builder.Configuration.GetSection("SqlServer")["ConnectionURI"]));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard();

            app.MapHangfireDashboard();

            app.Run();
        }
    }
}
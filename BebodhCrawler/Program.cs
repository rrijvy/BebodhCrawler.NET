using BebodhCrawler.Data;
using Common;
using Core.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BebodhCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.Configure<SqlServerSettings>(builder.Configuration.GetSection("SqlServer"));

            builder.Services.AddDbContext<CrawlerContext>(options => options.UseSqlServer(builder.Configuration.GetSection("CrawlerMaster")["ConnectionURI"]));

            builder.Services.AddIdentityServer().AddApiAuthorization<ApplicationUser, CrawlerContext>();
            builder.Services.AddAuthentication().AddIdentityServerJwt();


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            HelperService.RegisterDependencies(builder.Services);

            builder.Services.AddHangfire(config => config
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(builder.Configuration.GetSection("SqlServer")["ConnectionURI"]));

            builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<CrawlerContext>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard();

            app.MapHangfireDashboard();

            app.Run();
        }
    }
}
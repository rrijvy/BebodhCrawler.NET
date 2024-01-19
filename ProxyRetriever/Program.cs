﻿using Common;
using Core.IServices;
using Core.Models;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace ProxyRetriever
{
    public class Program
    {

        static async Task Main(string[] args)
        {

            //var sqlServerConnectionString = @"Data Source=WIN11-DESKTOP\MSSQLSERVER01;Initial Catalog=ProxyRetrieverDB;Integrated Security=True";
            //GlobalConfiguration.Configuration
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseSqlServerStorage(sqlServerConnectionString);

            //var sqliteConnectionString = @"Data Source=scheduler.db";
            //GlobalConfiguration.Configuration
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseSQLiteStorage(sqliteConnectionString);

            //await RetrieveProxies();


            await RecheckActiveProxies();

            Console.ReadKey();

            //using (var server = new BackgroundJobServer())
            //{
            //    //RecurringJob.AddOrUpdate("___main___", () => RetrieveProxies(), "*/1 * * * *");

            //    BackgroundJob.Enqueue(() => RetrieveProxies());
            //    Console.ReadKey();

            //}
            //using (var server = new BackgroundJobServer())
            //{
            //    RecurringJob.AddOrUpdate("___main___", () => RetrieveProxies(), "* * * * *");

            //    Console.WriteLine("Hangfire Server started. Press any key to exit...");

            //    Console.ReadKey();
            //}
        }

        public static async Task RetrieveProxies()
        {
            var serviceProvider = RegisterDependencies();
            var proxyService = serviceProvider.GetService<IProxyService>();
            if (proxyService == null) return;
            await proxyService.RetrieveProxies();
            Console.WriteLine("Completed!");
        }

        public static async Task RecheckActiveProxies()
        {
            var serviceProvider = RegisterDependencies();
            var proxyService = serviceProvider.GetService<IProxyService>();
            if (proxyService == null) return;
            await proxyService.RecheckActiveProxies();
            Console.WriteLine("Completed!");
        }

        static ServiceProvider RegisterDependencies()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.Configure<MongoDBSettings>(options =>
            {
                options.ConnectionURI = "mongodb://localhost:27017";
                options.DatabaseName = "BebodhCrawler";
                options.CollectionName = "";
            });
            HelperService.RegisterDependencies(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
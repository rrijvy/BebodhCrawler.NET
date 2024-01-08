﻿using Common;
using Core.IServices;
using Core.Models;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyRetriever
{
    public class Program
    {
        static void Main(string[] args)
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

            var proxyService = serviceProvider.GetService<IProxyService>();

            if (proxyService == null) return;

            var sqlServerConnectionString = @"Data Source=DESKTOP-MSFMN85\SQLEXPRESS;Initial Catalog=ProxyRetrieverDB;Integrated Security=True";

            GlobalConfiguration.Configuration.UseSqlServerStorage(sqlServerConnectionString);

            var server = new BackgroundJobServer();

            RecurringJob.AddOrUpdate<IProxyService>("_main_", x => x.RetrieveProxies(), "* */12 * * *");

            Console.WriteLine("Hangfire Server started. Press any key to exit...");

            Console.ReadKey();
        }
    }
}
using Common;
using Core.IServices;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyRetriever
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            await RecheckActiveProxies();

            Console.ReadKey();
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
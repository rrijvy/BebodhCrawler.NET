using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Services;

namespace Common
{
    public static class HelperService
    {
        public static void RegisterDependencies(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IProxyRepository, ProxyRepository>();
            serviceCollection.AddScoped<IProxyBackgroundTaskRepository, ProxyBackgroundTaskRepository>();
            serviceCollection.AddScoped<IProxyService, ProxyService>();
            serviceCollection.AddScoped<IAmazonCrawlerService, AmazonCrawlerService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<ICrawlRepository, CrawlRepository>();

        }
    }
}

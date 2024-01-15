using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using MongoDB.Driver;
using Repositories;
using System.Net;

namespace Services
{
    public class ProxyService : IProxyService
    {
        private readonly IProxyRepository _proxyRepository;
        private readonly IProxyBackgroundTaskRepository _proxyBackgroundTaskRepository;

        public ProxyService(IProxyRepository proxyRepository, IProxyBackgroundTaskRepository proxyBackgroundTaskRepository)
        {
            _proxyRepository = proxyRepository;
            _proxyBackgroundTaskRepository = proxyBackgroundTaskRepository;
        }

        public async Task<List<HttpProxy>> GetProxies()
        {
            var proxies = await _proxyRepository.GetAll();
            return proxies;
        }

        public async Task<List<HttpProxy>> RetrieveProxies()
        {
            Console.WriteLine("Initiate Retrival.");
            var proxyBackgroundTaskHistory = new ProxyBackgroundTaskHistory
            {
                StartedAt = Utility.GetCurrentUnixTime(),
            };
            await _proxyBackgroundTaskRepository.InsertOneAsync(proxyBackgroundTaskHistory);
            proxyBackgroundTaskHistory = _proxyBackgroundTaskRepository.AsQueryable().OrderByDescending(x => x.StartedAt).ToList().FirstOrDefault();
            var proxies = new List<string>();
            var activeProxies = new List<string>();
            var tasks = new List<Task<string>>();
            var sourceUrls = new List<string>()
            {
                "https://raw.githubusercontent.com/prxchk/proxy-list/main/http.txt",
                "https://raw.githubusercontent.com/ErcinDedeoglu/proxies/main/proxies/http.txt",
                "https://raw.githubusercontent.com/saisuiu/Lionkings-Http-Proxys-Proxies/main/free.txt",
                "https://raw.githubusercontent.com/MuRongPIG/Proxy-Master/main/http.txt",

                //"https://raw.githubusercontent.com/caliphdev/Proxy-List/master/http.txt",
                //"https://raw.githubusercontent.com/Zaeem20/FREE_PROXIES_LIST/master/http.txt",
                //"https://raw.githubusercontent.com/proxy4parsing/proxy-list/main/http.txt",

                //"https://raw.githubusercontent.com/ErcinDedeoglu/proxies/main/proxies/https.txt",
                //"https://raw.githubusercontent.com/zloi-user/hideip.me/main/https.txt",

                //"https://raw.githubusercontent.com/TheSpeedX/SOCKS-List/master/http.txt",
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

                foreach (var sourceUrl in sourceUrls)
                {
                    var httpResponse = await httpClient.GetAsync(sourceUrl);
                    httpResponse.EnsureSuccessStatusCode();
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    var proxyList = response.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    proxies.AddRange(proxyList);
                }
            }

            Console.WriteLine($"Total ${proxies.Count} proxy found.");

            Console.WriteLine("Start checking...");

            var client = new HttpClient();

            var index = 0;

            foreach (var proxyAddress in proxies)
            {
                index = index + 1;
                tasks.Add(CheckProxyIsAlive(proxyAddress, client, index));
            }

            client.Dispose();

            var taskResults = await Task.WhenAll(tasks);

            activeProxies = taskResults.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var httpProxies = activeProxies.Select(x => Utility.GetProxy(x)).ToList();

            proxyBackgroundTaskHistory.EndedAt = Utility.GetCurrentUnixTime();

            await _proxyBackgroundTaskRepository.ReplaceOneAsync(proxyBackgroundTaskHistory.Id, proxyBackgroundTaskHistory);

            return httpProxies;
        }

        private async Task<string> CheckProxyIsAlive(string proxyAddress, HttpClient client, int index)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"http://{proxyAddress}"),
                UseProxy = true
            };

            using (client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://httpbin.org/ip");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"{index}. Success - {proxyAddress}");
                        await _proxyRepository.InsertOneAsync(Utility.GetProxy(proxyAddress));
                        return proxyAddress;
                    }
                    return string.Empty;
                }
                catch (Exception)
                {
                    Console.WriteLine($"{index}. Failed - {proxyAddress}");
                    return string.Empty;
                }
            }
        }

        public async Task<HttpProxy> GetUnusedActiveProxy()
        {
            var filterDefination = Builders<HttpProxy>.Filter.Eq(x => x.IsProxyRunning, false);
            var sortDefinition = Builders<HttpProxy>.Sort.Ascending(x => x.UpdatedOn);

            var proxies = await _proxyRepository.FindAllAsync(filterDefination, sortDefinition);

            var proxy = proxies.FirstOrDefault(x =>
            {
                if (x.UpdatedOn <= Utility.GetMinimumUnixTime())
                {
                    return true;
                }

                var elapsedTime = Utility.GetElapsedTimeInSecond(x.UpdatedOn);

                return elapsedTime > 300000 ? true : false;
            });

            return proxy;
        }
    }
}

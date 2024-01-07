using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using MongoDB.Driver;
using System.Net;

namespace Services
{
    public class ProxyService : IProxyService
    {
        private readonly IProxyRepository _proxyRepository;

        public ProxyService(IProxyRepository proxyRepository)
        {
            _proxyRepository = proxyRepository;
        }

        public async Task<List<HttpProxy>> GetProxies()
        {
            var proxies = await _proxyRepository.GetAll();
            return proxies;
        }

        public async Task<List<HttpProxy>> RetriveProxies()
        {

            var proxies = new List<string>();
            var activeProxies = new List<string>();
            var tasks = new List<Task<string>>();
            var sourceUrls = new List<string>()
            {
                ////"https://raw.githubusercontent.com/prxchk/proxy-list/main/http.txt",
                "https://raw.githubusercontent.com/ErcinDedeoglu/proxies/main/proxies/http.txt",
                //"https://raw.githubusercontent.com/saisuiu/Lionkings-Http-Proxys-Proxies/main/free.txt",
                //"https://raw.githubusercontent.com/MuRongPIG/Proxy-Master/main/http.txt",

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

            foreach (var proxyAddress in proxies)
            {
                tasks.Add(CheckProxyIsAlive(proxyAddress));
            }

            var taskResults = await Task.WhenAll(tasks);

            activeProxies = taskResults.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var httpProxies = activeProxies.Select(x=>Utility.GetProxy(x)).ToList();

            await _proxyRepository.InsertManyAsync(httpProxies);

            return httpProxies;
        }

        private async Task<string> CheckProxyIsAlive(string proxyAddress)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"http://{proxyAddress}"),
                UseProxy = true
            };
            var client = new HttpClient(handler);
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://httpbin.org/ip");
                if (response.IsSuccessStatusCode)
                {
                    return proxyAddress;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed - {proxyAddress}");
                return string.Empty;
            }
        }

        public async Task<HttpProxy> GetUnsedActiveProxy()
        {
            var proxies = _proxyRepository.AsQueryable().OrderByDescending(x => x.UpdatedAt).Take(1).ToList();
            var proxy = proxies.FirstOrDefault();
            return proxy;
        }
    }
}

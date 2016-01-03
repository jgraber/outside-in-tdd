using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.SelfHost;

namespace RunningJournalApi.AcceptanceTests
{
    public class HttpClientFactory
    {
        public static HttpClient Create()
        {
            var baseAddress = new Uri("http://localhost:9876");
            var config = new HttpSelfHostConfiguration(baseAddress);
            new Bootstrap().Configure(config);
            var server = new HttpSelfHostServer(config);
            var client = new HttpClient(server);

            try
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "userName=foo");
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }
    }
}
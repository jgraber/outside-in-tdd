﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using Xunit;
using RunningJournalApi;

namespace RunningJournalApi.AcceptanceTests
{
    public class HomeJsonTest
    {
        [Fact]
        public void GetResponseReturnCorrectStatusCode()
        {
            var baseAddress = new Uri("http://localhost:9876");
            var config = new HttpSelfHostConfiguration(baseAddress);
            new Bootstrap().Configure(config);
            var server = new HttpSelfHostServer(config);
            using (var client = new HttpClient(server))
            {
                client.BaseAddress = baseAddress;
                var response = client.GetAsync("").Result;

                Assert.True(
                    response.IsSuccessStatusCode,
                    "Actual status code: " + response.StatusCode);
            }
        }
    }
}

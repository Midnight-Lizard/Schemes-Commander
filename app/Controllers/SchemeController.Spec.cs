using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    public class SchemeControllerSpec
    {
        private TestServer testServer;
        private HttpClient testClient;

        public SchemeControllerSpec()
        {
            this.testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            this.testClient = this.testServer.CreateClient();
            this.testClient.DefaultRequestHeaders.Add("version", "1.3");
        }

        public class PublishSpec : SchemeControllerSpec
        {
            [It(nameof(SchemeController.Publish))]
            public async Task Should()
            {
                HttpContent json = new StringContent($"{{Id:\"{Guid.NewGuid()}\"}}", Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", json);

                result.IsSuccessStatusCode.Should().BeTrue();
            }
        }
    }
}

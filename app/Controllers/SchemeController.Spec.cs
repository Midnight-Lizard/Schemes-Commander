using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using Microsoft.Extensions.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Newtonsoft.Json;
using System.Net;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using MidnightLizard.Schemes.Commander.Requests.Base;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    public class SchemeControllerSpec
    {
        private readonly IRequestQueuer<SCHEMES_QUEUE_CONFIG> testQueuer;
        private readonly TestServer testServer;
        private readonly HttpClient testClient;

        public SchemeControllerSpec()
        {
            this.testQueuer = Substitute.For<IRequestQueuer<SCHEMES_QUEUE_CONFIG>>();
            this.testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => x.AddAutofac())
                .ConfigureTestServices(services => services
                    .AddSingleton<IRequestQueuer<SCHEMES_QUEUE_CONFIG>>(testQueuer))
                .UseSetting(nameof(SCHEMES_QUEUE_CONFIG), JsonConvert.SerializeObject(
                    new SCHEMES_QUEUE_CONFIG
                    {
                        TopicName = "test",
                        ProducerSettings = new Dictionary<string, object>
                        {
                            ["bootstrap.servers"] = "test:123"
                        }
                    }))
                .UseStartup<Startup>());
            this.testClient = this.testServer.CreateClient();
            this.testClient.DefaultRequestHeaders.Add("version", "1.3");
        }

        public class PublishSpec : SchemeControllerSpec
        {
            [It(nameof(SchemeController.Publish))]
            public async Task Should_successfuly_process_correct_request()
            {
                var json = JsonConvert.SerializeObject(PublishSchemeRequestSpec.CorrectPublishSchemeRequest);
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.Accepted);

                await this.testQueuer.Received(1).QueueRequest(Arg.Any<Request>());

            }

            [It(nameof(SchemeController.Publish))]
            public async Task Should_return_BadRequest_response_when_request_is_incorrect()
            {
                var json = JsonConvert.SerializeObject(new PublishSchemeRequest());
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}

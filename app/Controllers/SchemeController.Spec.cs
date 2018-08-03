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
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using Microsoft.AspNetCore.Builder;
using MidnightLizard.Schemes.Commander.Infrastructure.Middlewares;

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
                .UseStartup<StartupStub>());
            this.testClient = this.testServer.CreateClient();
            this.testClient.DefaultRequestHeaders.Add("api-version", "1.0");
            this.testClient.DefaultRequestHeaders.Add("schema-version", AppVersion.Latest.ToString());
        }

        public class PublishSpec : SchemeControllerSpec
        {
            [It(nameof(SchemeController.Publish))]
            public async Task Should_successfuly_process_correct_PublishSchemeRequest()
            {
                var json = JsonConvert.SerializeObject(PublishSchemeRequestSpec.CorrectPublishSchemeRequest);
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.Accepted, await result.Content.ReadAsStringAsync());

                await this.testQueuer.Received(1).QueueRequest(Arg.Any<Request>(), Arg.Any<UserId>());
            }

            [It(nameof(SchemeController.Publish))]
            public async Task Should_return_BadRequest_response_when_PublishSchemeRequest_is_incorrect()
            {
                var json = JsonConvert.SerializeObject(new PublishSchemeRequest());
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            [It(nameof(SchemeController.Publish))]
            public async Task Should_return_BadRequest_response_when_SchemaVersion_is_missing()
            {
                this.testClient.DefaultRequestHeaders.Remove("schema-version");
                var json = JsonConvert.SerializeObject(PublishSchemeRequestSpec.CorrectPublishSchemeRequest);
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await this.testClient.PostAsync("scheme", jsonContent);
                result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        public class UnpublishSpec : SchemeControllerSpec
        {
            [It(nameof(SchemeController.Unpublish))]
            public async Task Should_successfuly_process_correct_UnpublishSchemeRequest()
            {
                var testGuid = Guid.NewGuid();

                var result = await this.testClient.DeleteAsync($"scheme/{testGuid}");

                result.StatusCode.Should().Be(HttpStatusCode.Accepted, await result.Content.ReadAsStringAsync());

                await this.testQueuer.Received(1).QueueRequest(
                    Arg.Is<Request>(r => r.AggregateId == testGuid),
                    Arg.Any<UserId>());

            }

            [It(nameof(SchemeController.Unpublish))]
            public async Task Should_return_BadRequest_response_when_UnpublishSchemeRequest_is_incorrect()
            {
                var result = await this.testClient.DeleteAsync("scheme/123");

                result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}

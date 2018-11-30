using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MidnightLizard.Schemes.Commander.Configuration;
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using MidnightLizard.Schemes.Commander.Requests.Base;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Testing.Utilities;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

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
                    .AddSingleton<IRequestQueuer<SCHEMES_QUEUE_CONFIG>>(this.testQueuer))
                .UseSetting(nameof(SCHEMES_QUEUE_CONFIG), JsonConvert.SerializeObject(
                    new SCHEMES_QUEUE_CONFIG
                    {
                        TopicName = "test",
                        ProducerSettings = new Dictionary<string, object>
                        {
                            ["bootstrap.servers"] = "test:123"
                        }
                    }))
                .UseSetting(nameof(CorsConfig.ALLOWED_ORIGINS), JsonConvert.SerializeObject(new CorsConfig
                {
                    ALLOWED_ORIGINS = "localhost"
                }))
                .UseStartup<StartupStub>());
            this.testClient = this.testServer.CreateClient();
            this.testClient.DefaultRequestHeaders.Add("api-version", "1.0");
            this.testClient.DefaultRequestHeaders.Add("schema-version", SchemaVersion.Latest.ToString());
        }

        public class PublishSpec : SchemeControllerSpec
        {
            [It(nameof(SchemeController.Publish))]
            public async Task Should_successfuly_process_correct_PublishSchemeRequest()
            {
                var json = JsonConvert.SerializeObject(PublishSchemeRequestSpec.CorrectPublishSchemeRequest);
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.Accepted, await result.Content.ReadAsStringAsync());

                await this.testQueuer.Received(1).QueueRequest(
                    Arg.Is<Request>(x => x.DeserializerType == typeof(PublishSchemeRequestDeserializer_v10_1)),
                    Arg.Any<UserId>());
            }

            [It(nameof(SchemeController.Publish))]
            public async Task Should_return_BadRequest_response_when_PublishSchemeRequest_is_incorrect()
            {
                var json = JsonConvert.SerializeObject(new PublishSchemeRequest());
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
                var result = await this.testClient.PostAsync("scheme", jsonContent);

                result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            [It(nameof(SchemeController.Publish))]
            public async Task Should_return_BadRequest_response_when_SchemaVersion_is_missing()
            {
                this.testClient.DefaultRequestHeaders.Remove("schema-version");
                var json = JsonConvert.SerializeObject(PublishSchemeRequestSpec.CorrectPublishSchemeRequest);
                HttpContent jsonContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
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

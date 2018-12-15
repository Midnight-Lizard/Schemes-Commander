using FluentAssertions;
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Testing.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public class RequestSerializerSpec
    {
        private readonly RequestSerializer serialiser;
        private readonly UserId testUserId = new UserId("test-user-id");
        private readonly Guid id = Guid.NewGuid(), aggregateId = Guid.NewGuid();
        private readonly string testDescription = "test desc";
        private readonly string snapshot;

        public RequestSerializerSpec()
        {
            var correctColorSchemeJson = JsonConvert.SerializeObject(
                ColorSchemeSpec.CorrectColorScheme, new[] {
                new StringEnumConverter(true)
            });
            this.serialiser = new RequestSerializer(SchemaVersion.Latest);
            this.snapshot =
                $@"{{" +
                    $@"""CorrelationId"":""{this.id}""," +
                    $@"""Type"":""PublishSchemeRequest""," +
                    $@"""Version"":""{SchemaVersion.Latest}""," +
                    $@"""UserId"":""{this.testUserId.Value}""," +
                    $@"""Payload"":" +
                    $@"{{" +
                        $@"""AggregateId"":""{this.aggregateId}""," +
                        $@"""Id"":""{this.id}""," +
                        $@"""Description"":""{this.testDescription}""," +
                        $@"""ColorScheme"":{correctColorSchemeJson}" +
                    $@"}}" +
                $@"}}";
        }

        [It(nameof(RequestSerializer))]
        public void Should_serialize_request_the_same_way_as_in_snapshot()
        {
            var testRequest = PublishSchemeRequestSpec.CorrectPublishSchemeRequest;

            testRequest.AggregateId = this.aggregateId;
            testRequest.Id = this.id;
            testRequest.Description = this.testDescription;

            var json = this.serialiser.Serialize(testRequest, this.testUserId);
            json.Should().Be(this.snapshot);
        }
    }
}

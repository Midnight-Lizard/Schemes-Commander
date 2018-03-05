using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using MidnightLizard.Schemes.Commander.Requests.Base;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Queue
{
    public class RequestQueuerSpec : RequestQueuer<SCHEMES_QUEUE_CONFIG>
    {
        private readonly Request testRequest = PublishSchemeRequestSpec.CorrectPublishSchemeRequest;
        private readonly UserId testUserId = new UserId("test-user-id");
        private readonly Message<string, string> errorMessage =
            new Message<string, string>("", 0, 0, "", "", new Timestamp(),
                new Error(ErrorCode.Unknown, "test"));

        public RequestQueuerSpec() : base(
            new SCHEMES_QUEUE_CONFIG
            {
                TopicName = "test",
                ProducerSettings = new Dictionary<string, object>
                {
                    ["bootstrap.servers"] = "test:123"
                }
            },
            Substitute.For<ILogger<RequestQueuer<SCHEMES_QUEUE_CONFIG>>>(),
            Substitute.For<IRequestSerializer>())
        {
            this.producer = Substitute.For<ISerializingProducer<string, string>>();
            this.producer.ProduceAsync("test", this.testRequest.AggregateId.ToString(), Arg.Any<string>())
                .Returns(new Message<string, string>("", 0, 0, "", "", new Timestamp(), new Error(ErrorCode.NoError)));
        }

        public class QueueRequestSpec : RequestQueuerSpec
        {
            [It(nameof(QueueRequest))]
            public async Task Should_call_KafkaProducer__ProduceAsync()
            {
                await this.QueueRequest(this.testRequest, this.testUserId);

                await this.producer.Received(1).ProduceAsync("test", this.testRequest.AggregateId.ToString(), Arg.Any<string>());
            }

            [It(nameof(QueueRequest))]
            public void Should_throw_ApplicationException_when_KafkaProducer__ProduceAsync_returns_Error()
            {
                this.producer.ProduceAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(errorMessage);

                Func<Task> act = async () => await this.QueueRequest(this.testRequest, this.testUserId);

                act.Should().Throw<ApplicationException>();
            }

            [It(nameof(QueueRequest))]
            public async Task Should_log_Error_when_KafkaProducer__ProduceAsync_returns_Error()
            {
                this.producer.ProduceAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(errorMessage);

                try
                {
                    await this.QueueRequest(this.testRequest, this.testUserId);
                }
                catch { }

                this.logger.Received(1).Log(LogLevel.Error, 0, Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<object, Exception, string>>());
            }
        }
    }
}

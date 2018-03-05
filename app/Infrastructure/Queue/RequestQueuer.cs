using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using MidnightLizard.Schemes.Commander.Requests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Queue
{
    public interface IRequestQueuer<TQueueConfig>
        where TQueueConfig : QueueConfig
    {
        Task QueueRequest(Request request, UserId userId);
    }

    public class RequestQueuer<TQueueConfig>
        : IRequestQueuer<TQueueConfig>, IDisposable
        where TQueueConfig : QueueConfig
    {
        protected ISerializingProducer<string, string> producer;
        protected readonly TQueueConfig queueConfig;
        protected readonly ILogger logger;
        protected readonly IRequestSerializer requestSerializer;

        public RequestQueuer(
            TQueueConfig queueConfig,
            ILogger<RequestQueuer<TQueueConfig>> logger,
            IRequestSerializer requestSerializer)
        {
            this.producer = new Producer<string, string>(queueConfig.ProducerSettings,
                new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
            this.queueConfig = queueConfig;
            this.logger = logger;
            this.requestSerializer = requestSerializer;
        }

        public async Task QueueRequest(Request request, UserId userId)
        {
            var transRequestJson = this.requestSerializer.Serialize(request, userId);

            var result = await this.producer.ProduceAsync(
                this.queueConfig.TopicName,
                request.AggregateId.ToString(),
                transRequestJson);

            if (result.Error.HasError)
            {
                this.logger.LogError($"Failed to queue request {request.Id}: {result.Error.Reason}");
                throw new ApplicationException("Unable to accept a request due to the internal failure");
            }
        }

        public void Dispose()
        {
            if (producer is IDisposable disposable)
            {
                this.producer = null;
                disposable.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Confluent.Kafka.Serialization;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class SchemesController : Controller
    {
        protected class DeliveryHandler : IDeliveryHandler<string, string>
        {
            protected readonly ILogger logger;
            protected readonly TaskCompletionSource<IActionResult> result;
            public DeliveryHandler(ILogger logger)
            {
                this.logger = logger;
                this.result = new TaskCompletionSource<IActionResult>();
            }

            public bool MarshalData => true;

            public Task<IActionResult> Result => this.result.Task;

            public void HandleError(object sender, Error e)
            {
                result.TrySetResult(new BadRequestObjectResult(e.Reason));
                this.logger.LogError("OnError: " + e.Reason);
            }

            public void HandleDeliveryReport(Message<string, string> msg)
            {
                if (msg.Error.HasError)
                {
                    result.TrySetResult(new BadRequestObjectResult(msg.Error.Reason));
                    this.logger.LogError("OnError: " + msg.Error.Reason);
                }
                else
                {
                    result.TrySetResult(new AcceptedResult("", msg));
                    this.logger.LogInformation("Message sent:");
                    this.logger.LogInformation($"\tTopic: {msg.Topic}");
                    this.logger.LogInformation($"\tKey: {msg.Key}");
                    this.logger.LogInformation($"\tValue: {msg.Value}");
                }
            }
        }

        protected readonly ILogger logger;

        public SchemesController(ILogger<SchemesController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Command([FromBody]Dictionary<string, string> obj)
        {
            if (ModelState.IsValid)
            {
                var msg = JsonConvert.SerializeObject(obj);
                using (var producer = new Producer<string, string>(
                       new Dictionary<string, object>() {
                       { "group.id", "schemes-commander" },
                       { "bootstrap.servers", "bootstrap.kafka:9092" }
                       },
                       new StringSerializer(Encoding.UTF8),
                       new StringSerializer(Encoding.UTF8)))
                {
                    // var handler = new DeliveryHandler(logger);
                    // producer.OnError += handler.HandleError;
                    var result = await producer.ProduceAsync(obj["topic"], obj["color"], msg);
                    // producer.Flush(TimeSpan.FromSeconds(1));
                    if (result.Error.HasError)
                    {
                        return BadRequest(result.Error.Reason);
                    }
                    else
                    {
                        return Accepted(result);
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}

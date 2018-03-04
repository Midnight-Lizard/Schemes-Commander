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
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using System.Net;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("1.2")]
    [ApiVersion("1.3")]
    [Route("[controller]")]
    public class SchemeController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer;

        public SchemeController(
            ILogger<SchemeController> logger,
            IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer)
        {
            this.logger = logger;
            this.requestQueuer = requestQueuer;
        }

        //[HttpPost]
        //public async Task<IActionResult> Command([FromBody]Dictionary<string, string> obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var msg = JsonConvert.SerializeObject(obj);
        //        using (var producer = new Producer<string, string>(
        //               new Dictionary<string, object>() {
        //               { "group.id", "schemes-commander" },
        //               { "bootstrap.servers", "bootstrap.kafka:9092" }
        //               },
        //               new StringSerializer(Encoding.UTF8),
        //               new StringSerializer(Encoding.UTF8)))
        //        {
        //            // var handler = new DeliveryHandler(logger);
        //            // producer.OnError += handler.HandleError;
        //            var result = await producer.ProduceAsync(obj["topic"], obj["color"], msg);
        //            // producer.Flush(TimeSpan.FromSeconds(1));
        //            if (result.Error.HasError)
        //            {
        //                return BadRequest(result.Error.Reason);
        //            }
        //            else
        //            {
        //                return Accepted(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }
        //}

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Publish(PublishSchemeRequest request)
        {
            if (ModelState.IsValid)
            {
                await this.requestQueuer.QueueRequest(request);
                return Accepted();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            return Ok();
        }
    }
}

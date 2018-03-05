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
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("1.2")]
    [ApiVersion("1.3")]
    [Route("[controller]")]
    public class SchemeController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer;
        protected readonly UserId userId;

        public SchemeController(
            ILogger<SchemeController> logger,
            IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer)
        {
            this.logger = logger;
            this.requestQueuer = requestQueuer;
            //this.userId = new UserId(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
            // TODO: Get real user id!
            this.userId = new UserId("none");
        }

        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] PublishSchemeRequest request)
        {
            if (ModelState.IsValid)
            {
                await this.requestQueuer.QueueRequest(request, this.userId);
                return Accepted(request.Id);
            }
            return BadRequest(ModelState);
        }

        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{" + nameof(UnpublishSchemeRequest.AggregateId) + "}")]
        public async Task<IActionResult> Unpublish(
            [FromRoute(Name = nameof(UnpublishSchemeRequest.AggregateId))]
            UnpublishSchemeRequest request)
        {
            if (ModelState.IsValid)
            {
                await this.requestQueuer.QueueRequest(request, this.userId);
                return Accepted(request.Id);
            }
            return BadRequest(ModelState);
        }
    }
}

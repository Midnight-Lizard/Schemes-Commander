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
using MidnightLizard.Schemes.Commander.Infrastructure.ActionFilters;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    [Authorize]
    [ValidateModelState]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("1.2")]
    [ApiVersion("1.3")]
    [Route("[controller]")]
    public class SchemeController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer;

        protected UserId GetUserId()
        {
            if (User != null)
            {
                var subClaim = User.FindFirst("sub");
                if (subClaim != null)
                {
                    return new UserId(subClaim.Value);
                }
            }
            throw new UnauthorizedAccessException();
        }

        public SchemeController(
            ILogger<SchemeController> logger,
            IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer)
        {
            this.logger = logger;
            this.requestQueuer = requestQueuer;
        }

        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] PublishSchemeRequest request)
        {
            await this.requestQueuer.QueueRequest(request, this.GetUserId());
            return Accepted(request.Id);
        }

        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{" + nameof(UnpublishSchemeRequest.AggregateId) + "}")]
        public async Task<IActionResult> Unpublish(
            [FromRoute(Name = nameof(UnpublishSchemeRequest.AggregateId))]
            UnpublishSchemeRequest request)
        {
            await this.requestQueuer.QueueRequest(request, this.GetUserId());
            return Accepted(request.Id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MidnightLizard.Schemes.Commander.Infrastructure.ActionFilters;
using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Controllers
{
    [Authorize]
    [ValidateModelState]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class SchemeController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IRequestQueuer<SCHEMES_QUEUE_CONFIG> requestQueuer;

        protected UserId GetUserId()
        {
            if (this.User != null)
            {
                var subClaim = this.User.FindFirst("sub");
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

        /// <summary>
        /// Publishes first or new version of Color Scheme
        /// </summary>
        /// <param name="request">Publish Scheme Request</param>
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] PublishSchemeRequest request)
        {
            await this.requestQueuer.QueueRequest(request, this.GetUserId());
            return this.Accepted(request.Id);
        }

        /// <summary>
        /// Removes color scheme from the registry
        /// </summary>
        /// <param name="request">AggregateId from path</param>
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{" + nameof(UnpublishSchemeRequest.AggregateId) + "}")]
        public async Task<IActionResult> Unpublish(UnpublishSchemeRequest request)
        {
            await this.requestQueuer.QueueRequest(request, this.GetUserId());
            return this.Accepted(request.Id);
        }
    }
}

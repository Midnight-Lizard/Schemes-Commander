using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Commander.Requests.Base;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class UnpublishSchemeRequest : Request
    {
        /// <summary>
        /// Request ID
        /// </summary>
        [FromQuery(Name = nameof(UnpublishSchemeRequest.Id))]
        public override Guid Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// ID of the color scheme you want to delete
        /// </summary>
        [FromRoute(Name = nameof(UnpublishSchemeRequest.AggregateId))]
        public override Guid AggregateId { get => base.AggregateId; set => base.AggregateId = value; }
    }
}

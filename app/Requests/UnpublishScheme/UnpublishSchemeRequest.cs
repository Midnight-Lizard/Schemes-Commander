using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Commander.Requests.Base;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class UnpublishSchemeRequest : Request
    {
        [FromQuery(Name = nameof(UnpublishSchemeRequest.Id))]
        public override Guid Id { get => base.Id; set => base.Id = value; }

        [FromRoute(Name = nameof(UnpublishSchemeRequest.AggregateId))]
        public override Guid AggregateId { get => base.AggregateId; set => base.AggregateId = value; }
    }
}

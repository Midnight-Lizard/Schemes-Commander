using MediatR;
using MidnightLizard.Commons.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequest : IRequest<DomainResult>
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public Guid PublisherId { get; set; }

        public ColorScheme ColorScheme { get; set; }
    }
}

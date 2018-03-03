using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class RequestValidator: AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(r => r.AggregateId).NotEmpty();
            RuleFor(r => r.Id).NotEmpty();
        }
    }
}

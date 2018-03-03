using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequestValidator : AbstractValidator<PublishSchemeRequest>
    {
        public PublishSchemeRequestValidator()
        {
            RuleFor(r => r.AggregateId).NotEmpty();
            RuleFor(r => r.Id).NotEmpty();
            RuleFor(r => r.PublisherId).NotEmpty();
            RuleFor(r => r.ColorScheme).NotNull();
        }
    }
}

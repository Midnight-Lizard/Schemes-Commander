using FluentValidation;
using MidnightLizard.Schemes.Commander.Requests.Base;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequestValidator : AbstractValidator<PublishSchemeRequest>
    {
        public PublishSchemeRequestValidator()
        {
            this.Include(new RequestValidator());
            this.RuleFor(x => x.Description).MaximumLength(2000);
            this.RuleFor(r => r.ColorScheme).NotNull()
                .SetValidator(new ColorSchemeValidator());
        }
    }
}

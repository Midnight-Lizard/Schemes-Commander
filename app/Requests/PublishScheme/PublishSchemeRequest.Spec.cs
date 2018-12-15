using FluentValidation.TestHelper;
using MidnightLizard.Testing.Utilities;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequestSpec
    {
        public static PublishSchemeRequest CorrectPublishSchemeRequest => new PublishSchemeRequest
        {
            AggregateId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            ColorScheme = ColorSchemeSpec.CorrectColorScheme
        };

        public class ValidatorSpec
        {
            private readonly PublishSchemeRequestValidator validator = new PublishSchemeRequestValidator();

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_with_correct_request()
            {
                this.validator.ShouldNotHaveValidationErrorFor(x => x as object,
                    CorrectPublishSchemeRequest);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_some_required_fields_are_empty()
            {
                this.validator.ShouldHaveValidationErrorFor(x => x.Id, Guid.Empty);
                this.validator.ShouldHaveValidationErrorFor(x => x.AggregateId, Guid.Empty);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_when_required_fields_have_values()
            {
                this.validator.ShouldNotHaveValidationErrorFor(x => x.Id, Guid.NewGuid());
                this.validator.ShouldNotHaveValidationErrorFor(x => x.AggregateId, Guid.NewGuid());
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_ColorScheme_is_null()
            {
                this.validator.ShouldHaveValidationErrorFor(x => x.ColorScheme, null as ColorScheme);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_Description_is_too_long()
            {
                this.validator.ShouldHaveValidationErrorFor(x => x.Description, new string('-', 2001));
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_when_ColorScheme_is_not_null()
            {
                this.validator.ShouldNotHaveValidationErrorFor(x => x.ColorScheme, new ColorScheme());
            }
        }
    }
}

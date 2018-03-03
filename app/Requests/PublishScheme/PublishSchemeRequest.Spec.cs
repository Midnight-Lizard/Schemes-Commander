using FluentValidation.TestHelper;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequestSpec
    {
        public static PublishSchemeRequest CorrectPublishSchemeRequest
        {
            get => new PublishSchemeRequest
            {
                AggregateId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                PublisherId = Guid.NewGuid(),
                ColorScheme = ColorSchemeSpec.CorrectColorScheme
            };
        }

        public class ValidatorSpec
        {
            private readonly PublishSchemeRequestValidator validator = new PublishSchemeRequestValidator();

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_some_required_fields_are_empty()
            {
                validator.ShouldHaveValidationErrorFor(x => x.Id, Guid.Empty);
                validator.ShouldHaveValidationErrorFor(x => x.AggregateId, Guid.Empty);
                validator.ShouldHaveValidationErrorFor(x => x.PublisherId, Guid.Empty);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_ColorScheme_is_null()
            {
                validator.ShouldHaveValidationErrorFor(x => x.ColorScheme, null as ColorScheme);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_when_required_fields_have_values()
            {
                validator.ShouldNotHaveValidationErrorFor(x => x.Id, Guid.NewGuid());
                validator.ShouldNotHaveValidationErrorFor(x => x.AggregateId, Guid.NewGuid());
                validator.ShouldNotHaveValidationErrorFor(x => x.PublisherId, Guid.NewGuid());
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_when_ColorScheme_is_not_null()
            {
                validator.ShouldNotHaveValidationErrorFor(x => x.ColorScheme, new ColorScheme());
            }
        }
    }
}

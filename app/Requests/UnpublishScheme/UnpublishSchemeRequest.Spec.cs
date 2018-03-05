using FluentValidation.TestHelper;
using MidnightLizard.Schemes.Commander.Requests.Base;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class UnpublishSchemeRequestSpec
    {
        public static UnpublishSchemeRequest CorrectUnpublishSchemeRequest
        {
            get => new UnpublishSchemeRequest
            {
                AggregateId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };
        }

        public class ValidatorSpec
        {
            private readonly UnpublishSchemeRequestValidator validator = new UnpublishSchemeRequestValidator();

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_with_correct_request()
            {
                validator.ShouldNotHaveValidationErrorFor(x => x as object,
                    CorrectUnpublishSchemeRequest);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_fail_when_some_required_fields_are_empty()
            {
                validator.ShouldHaveValidationErrorFor(x => x.Id, Guid.Empty);
                validator.ShouldHaveValidationErrorFor(x => x.AggregateId, Guid.Empty);
            }

            [It(nameof(PublishSchemeRequestValidator))]
            public void Should_succeed_when_required_fields_have_values()
            {
                validator.ShouldNotHaveValidationErrorFor(x => x.Id, Guid.NewGuid());
                validator.ShouldNotHaveValidationErrorFor(x => x.AggregateId, Guid.NewGuid());
            }
        }
    }
}

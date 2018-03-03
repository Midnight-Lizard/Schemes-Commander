using FluentValidation.TestHelper;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class RequestSpec
    {
        public class ValidatorSpec
        {
            private readonly RequestValidator validator  = new RequestValidator();

            [It(nameof(RequestValidator))]
            public void Should_fail_when_some_required_fields_are_empty()
            {
                validator.ShouldHaveValidationErrorFor(x => x.Id, Guid.Empty);
                validator.ShouldHaveValidationErrorFor(x => x.AggregateId, Guid.Empty);
            }

            [It(nameof(RequestValidator))]
            public void Should_succeed_when_required_fields_have_values()
            {
                validator.ShouldNotHaveValidationErrorFor(x => x.Id, Guid.NewGuid());
                validator.ShouldNotHaveValidationErrorFor(x => x.AggregateId, Guid.NewGuid());
            }
        }
    }
}

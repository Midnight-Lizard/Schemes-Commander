using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.Base;
using System;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public class AggregateIdRequestDeserializer<TRequest> : BaseRequestDeserializer<TRequest>
        where TRequest : Request, new()
    {
        public override void StartAdvancingToTheLatestVersion(TRequest message)
        {
        }

        protected override TRequest DeserializeRequest(ModelBindingContext bindingContext)
        {
            var aggId = Guid.Parse(bindingContext.ValueProvider
                            .GetValue(bindingContext.BinderModelName)
                            .FirstValue);
            return new TRequest
            {
                AggregateId = aggId
            };
        }
    }
}

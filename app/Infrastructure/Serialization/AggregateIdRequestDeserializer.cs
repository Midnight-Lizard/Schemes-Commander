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
                            .GetValue(nameof(Request.AggregateId))
                            .FirstValue);
            var reqIdStr = bindingContext.ValueProvider
                            .GetValue(nameof(Request.Id))
                            .FirstValue;
            var reqId = string.IsNullOrEmpty(reqIdStr)
                ? Guid.NewGuid() : Guid.Parse(reqIdStr);

            return new TRequest
            {
                AggregateId = aggId,
                Id = reqId
            };
        }
    }
}

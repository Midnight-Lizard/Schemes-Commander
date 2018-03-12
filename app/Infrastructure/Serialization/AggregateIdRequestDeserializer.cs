using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public class AggregateIdRequestDeserializer<TRequest> : BaseRequestDeserializer<TRequest>
        where TRequest : Request, new()
    {
        public override void StartAdvancingToTheLatestVersion(TRequest message)
        {
        }

        protected override TRequest DeserializeRequest(string aggregateId)
        {
            return new TRequest
            {
                AggregateId = Guid.Parse(aggregateId)
            };
        }
    }
}

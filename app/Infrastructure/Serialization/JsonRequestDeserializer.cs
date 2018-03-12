using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public abstract class JsonRequestDeserializer<TRequest> : BaseRequestDeserializer<TRequest>
        where TRequest : Request
    {
        protected override TRequest DeserializeRequest(string requestJson)
        {
            return JsonConvert.DeserializeObject<TRequest>(requestJson);
        }
    }
}

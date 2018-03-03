using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public interface IRequestSerializer
    {
        string Serialize(Request request);
    }

    public class RequestSerializer : IRequestSerializer
    {
        private readonly AppVersion version;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            //Formatting = Formatting.Indented,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = MessageContractResolver.Default,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        };

        public RequestSerializer(AppVersion version)
        {
            this.version = version;
        }

        public string Serialize(Request request)
        {
            return JsonConvert.SerializeObject(new
            {
                CorrelationId = request.Id,
                Type = request.GetType().Name,
                Version = this.version.ToString(),
                Payload = request
            }, serializerSettings);
        }
    }
}

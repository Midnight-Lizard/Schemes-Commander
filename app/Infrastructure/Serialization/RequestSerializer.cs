using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public interface IRequestSerializer
    {
        string Serialize(Request request, UserId userId);
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
            Converters = new JsonConverter[] {
                new StringEnumConverter(camelCaseText:true)
            }
        };

        public RequestSerializer(AppVersion version)
        {
            this.version = version;
        }

        public string Serialize(Request request, UserId userId)
        {
            return JsonConvert.SerializeObject(new
            {
                CorrelationId = request.Id,
                Type = request.GetType().Name,
                Version = this.version.ToString(),
                UserId = userId.Value,
                Payload = request
            }, serializerSettings);
        }
    }
}

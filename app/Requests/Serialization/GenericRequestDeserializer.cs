using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public interface IRequestDeserializer { }
    public interface IRequestDeserializer<out TRequest> : IRequestDeserializer
    {
        TRequest Deserialize(string requestJson);
    }

    public class GenericRequestDeserializer<TRequest> : IRequestDeserializer<TRequest>
    {
        public virtual TRequest Deserialize(string requestJson)
        {
            return JsonConvert.DeserializeObject<TRequest>(requestJson);
        }

        protected virtual void AdvanceToTheLatestVersion(TRequest message) { }
    }
}

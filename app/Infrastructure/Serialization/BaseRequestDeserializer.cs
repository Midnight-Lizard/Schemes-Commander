using MidnightLizard.Schemes.Commander.Infrastructure.Authentication;
using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public interface IRequestDeserializer { }
    public interface IRequestDeserializer<out TRequest> : IRequestDeserializer
        where TRequest : Request
    {
        TRequest Deserialize(string data);
    }

    public abstract class BaseRequestDeserializer<TRequest> : IRequestDeserializer<TRequest>
        where TRequest : Request
    {
        public virtual TRequest Deserialize(string requestData)
        {
            var request = DeserializeRequest(requestData);
            AdvanceToTheLatestVersion(request);
            request.Id = request.Id == default ? Guid.NewGuid() : request.Id;
            return request;
        }

        protected abstract TRequest DeserializeRequest(string data);

        protected virtual void AdvanceToTheLatestVersion(TRequest request) { }
    }
}

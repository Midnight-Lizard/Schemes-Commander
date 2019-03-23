using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.Base;
using System;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public interface IRequestDeserializer { }
    public interface IRequestDeserializer<out TRequest> : IRequestDeserializer
        where TRequest : Request
    {
        TRequest Deserialize(ModelBindingContext bindingContext);
    }

    public abstract class BaseRequestDeserializer<TRequest> : IRequestDeserializer<TRequest>
        where TRequest : Request
    {
        public virtual TRequest Deserialize(ModelBindingContext bindingContext)
        {
            var request = DeserializeRequest(bindingContext);
            this.StartAdvancingToTheLatestVersion(request);
            request.Id = request.Id == default ? Guid.NewGuid() : request.Id;
            request.DeserializerType = this.GetType();
            return request;
        }

        protected abstract TRequest DeserializeRequest(ModelBindingContext bindingContext);

        public abstract void StartAdvancingToTheLatestVersion(TRequest message);
        protected virtual void AdvanceToTheLatestVersion(TRequest request) { }
    }
}

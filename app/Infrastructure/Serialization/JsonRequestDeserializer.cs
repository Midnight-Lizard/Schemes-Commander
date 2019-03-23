using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.Base;
using Newtonsoft.Json;
using System.IO;

namespace MidnightLizard.Schemes.Commander.Infrastructure.Serialization
{
    public abstract class JsonRequestDeserializer<TRequest> : BaseRequestDeserializer<TRequest>
        where TRequest : Request
    {
        protected override TRequest DeserializeRequest(ModelBindingContext bindingContext)
        {
            using (var bodyReader = new StreamReader(bindingContext.HttpContext.Request.Body))
            using (var bodyJsonReader = new JsonTextReader(bodyReader))
            {
                var serializer = new JsonSerializer();

                return serializer.Deserialize<TRequest>(bodyJsonReader);
            }
        }
    }
}

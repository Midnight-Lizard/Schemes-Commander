using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public interface IRequestMetaDeserializer
    {
        object Deserialize(Type requestType, ApiVersion apiVersion, string requestJson);
    }

    public class RequestMetaDeserializer: IRequestMetaDeserializer
    {
        protected readonly IEnumerable<Meta<Lazy<IRequestDeserializer>>> deserializers;

        public RequestMetaDeserializer(IEnumerable<Meta<Lazy<IRequestDeserializer>>> deserializers)
        {
            this.deserializers = deserializers;
        }

        public virtual object Deserialize(Type requestType, ApiVersion apiVersion, string requestJson)
        {
            var deserializer = this.deserializers.FirstOrDefault(d =>
                d.Metadata[nameof(Type)] as Type == requestType &&
                (d.Metadata[nameof(Version)] as IReadOnlyList<ApiVersion>).Any(v => v == apiVersion));
            if (deserializer != null)
            {
               return (deserializer.Value.Value as IRequestDeserializer<object>).Deserialize(requestJson);
            }
            throw new ArgumentException($"Deserializer for {requestType} and version {apiVersion} has not been found");
        }
    }
}

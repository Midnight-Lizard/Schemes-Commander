using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public class RequestDeserializer
    {
        protected readonly IEnumerable<Meta<Lazy<IRequestDeserializer>>> deserializers;

        public RequestDeserializer(IEnumerable<Meta<Lazy<IRequestDeserializer>>> deserializers)
        {
            this.deserializers = deserializers;
        }

        public virtual object Deserialize(Type requestType, ApiVersion apiVersion, string requestJson)
        {
            var deserializer = this.deserializers.FirstOrDefault(d =>
                d.Metadata[nameof(Type)] as Type == requestType &&
                (d.Metadata[nameof(Version)] as IReadOnlyList<ApiVersion>).Any(v => v == apiVersion));

            return null;
        }
    }
}

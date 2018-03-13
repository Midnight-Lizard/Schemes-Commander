using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.AutofacModules
{
    public class SerializationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestMetaDeserializer>()
                .AsImplementedInterfaces();

            builder.RegisterType<RequestSerializer>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(SerializationModule).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestDeserializer<>))
                .As<IRequestDeserializer>()
                .WithMetadata(t => new Dictionary<string, object>
                {
                    [nameof(Type)] = t.GetInterfaces().First().GetGenericArguments()[0],
                    [nameof(Version)] = t.GetCustomAttribute<ApiVersionsBaseAttribute>().Versions
                });
        }
    }
}

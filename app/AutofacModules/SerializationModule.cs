using Autofac;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Commander.Requests.Serialization;
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
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(SerializationModule).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestDeserializer<>))
                .As<IRequestDeserializer>()
                .WithMetadata(t => new Dictionary<string, object>
                {
                    [nameof(Type)] = t.GetInterfaces().First().GetGenericArguments()[0],
                    [nameof(Version)] = t.GetCustomAttribute<ApiVersionAttribute>().Versions
                });
        }
    }
}

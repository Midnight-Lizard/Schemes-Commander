using Autofac;
using MidnightLizard.Schemes.Commander.Requests.ModelBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.AutofacModules
{
    public class ModelBinderModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestVersionAccessor>().AsSelf();
            builder.RegisterType<RequestBodyAccessor>().AsSelf();
        }
    }
}

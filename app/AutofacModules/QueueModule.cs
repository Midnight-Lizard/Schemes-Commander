using Autofac;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.AutofacModules
{
    public class QueueModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RequestQueuer<>))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}

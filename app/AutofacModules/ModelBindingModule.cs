using Autofac;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;

namespace MidnightLizard.Schemes.Commander.AutofacModules
{
    public class ModelBindingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestSchemaVersionAccessor>().AsSelf();
        }
    }
}

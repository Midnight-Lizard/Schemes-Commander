using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests
{
    public class RequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(PublishSchemeRequest))
            {
                return new BinderTypeModelBinder(typeof(RequestModelBinder));
            }

            return null;
        }
    }
}

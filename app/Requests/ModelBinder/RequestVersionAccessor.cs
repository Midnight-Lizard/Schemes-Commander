using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.ModelBinder
{
    public class RequestVersionAccessor
    {
        public virtual ApiVersion GetApiVersion(ModelBindingContext bindingContext)
        {
            return bindingContext.HttpContext.GetRequestedApiVersion();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding
{
    public class RequestSchemaVersionAccessor
    {
        private static readonly string versionKey = "schema-version";

        public virtual AppVersion GetSchemaVersion(ModelBindingContext bindingContext)
        {
            var versionValue = string.Empty;
            if (bindingContext.HttpContext.Request.Query.Keys.Contains(versionKey))
            {
                versionValue = bindingContext.HttpContext.Request.Query[versionKey];
            }
            if (versionValue == string.Empty &&
                bindingContext.HttpContext.Request.Headers.Keys.Contains(versionKey))
            {
                versionValue = bindingContext.HttpContext.Request.Headers[versionKey];
            }
            if (versionValue != string.Empty)
            {
                return new AppVersion(versionValue);
            }
            return AppVersion.Unspecified;
        }
    }
}

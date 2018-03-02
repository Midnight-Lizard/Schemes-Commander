using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.ModelBinder
{
    public class RequestBodyAccessor
    {
        public virtual async Task<string> ReadAsync(ModelBindingContext bindingContext)
        {
            using (var streamReader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.ModelBinder;
using MidnightLizard.Schemes.Commander.Requests.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class RequestModelBinder : IModelBinder
    {
        protected readonly RequestDeserializer requestSerializer;
        protected readonly RequestVersionAccessor requestVersionAccessor;
        protected readonly RequestBodyAccessor requestBodyAccessor;

        public RequestModelBinder(
            RequestDeserializer requestSerializer,
            RequestVersionAccessor requestVersionAccessor,
            RequestBodyAccessor requestBodyAccessor)
        {
            this.requestBodyAccessor = requestBodyAccessor;
            this.requestSerializer = requestSerializer;
            this.requestVersionAccessor = requestVersionAccessor;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var version = this.requestVersionAccessor.GetApiVersion(bindingContext);
            var modelType = bindingContext.ModelType;
            var requestJson = await this.requestBodyAccessor.ReadAsync(bindingContext);

            try
            {
                var request = this.requestSerializer.Deserialize(modelType, version, requestJson);

                bindingContext.Result = ModelBindingResult.Success(request);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.FieldName, ex.Message);
            }
        }
    }
}

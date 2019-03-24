using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace MidnightLizard.Schemes.Commander.Configuration
{
    public class SwaggerDefaultValuesFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = apiDescription.ParameterDescriptions
                                                .First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Default == null)
                {
                    parameter.Default = description.DefaultValue;
                }

                parameter.Required |= description.IsRequired;
            }

            operation.Parameters.Add(new NonBodyParameter
            {
                In = "header",
                Default = "1.0",
                Type = "string",
                Required = true,
                Name = "api-version",
                Description = "API Version"
            });

            operation.Parameters.Add(new NonBodyParameter
            {
                In = "header",
                Default = SchemaVersion.Latest.ToString(),
                Type = "string",
                Required = true,
                Name = RequestSchemaVersionAccessor.VersionKey,
                Description = "Midnight Lizard global schema version"
            });
        }
    }
}

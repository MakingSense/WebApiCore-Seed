using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Seed.Api.Filters
{
    public class ValidateModelResponseOperationFilter : IOperationFilter
    {
        private Dictionary<string, List<string>> responseExample = new Dictionary<string, List<string>>()
        {
            {
                "property1",
                new List<string>(){"error1", "error2"}
            },
            {
                "property2",
                new List<string>(){"error1", "error2"}
            }
        };

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var attributes = context.ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .OfType<ValidateModelAttribute>();

            if (attributes.Any())
            {
                operation.Responses.Add("400", new Response()
                {
                    Description = "Invalid Input",
                    Schema = new Schema()
                    {
                        Example = responseExample
                    }
                });
            }

        }
    }
}

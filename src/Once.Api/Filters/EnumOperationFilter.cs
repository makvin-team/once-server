using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Once.Api.Filters;

public class EnumOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Description ??= string.Empty; // prevent NRE

        foreach (var param in context.ApiDescription.ParameterDescriptions)
        {
            var paramType = param.Type;
            if (paramType == null) continue; // safety check

            if (!paramType.IsPrimitive && !paramType.IsEnum && !paramType.IsValueType)
            {
                foreach (var prop in paramType.GetProperties())
                {
                    if (prop.PropertyType.IsEnum)
                    {
                        var enumNames = Enum.GetNames(prop.PropertyType);
                        var enumValues = Enum.GetValues(prop.PropertyType).Cast<int>().ToArray();

                        var enumMapping = string.Join(", ", enumNames.Zip(enumValues, (n, v) => $"{n} = {v}"));

                        operation.Description += $"\n\n**{prop.Name} :** {enumMapping}";
                    }
                }
            }
        }
    }

}

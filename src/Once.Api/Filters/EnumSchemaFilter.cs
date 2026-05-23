using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Once.Api.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        
        schema.Description += "<p>Possible values:</p><ul>";
        foreach (var name in Enum.GetNames(context.Type))
        {
            var value = (int)Enum.Parse(context.Type, name);
            schema.Description += $"<li>{name} = {value}</li>";
        }
        schema.Description += "</ul>";
    }
}
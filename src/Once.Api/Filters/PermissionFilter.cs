using System.Reflection;
using Once.Infrastructure.Authentication;
using Microsoft.OpenApi.Models;
using Once.Domain.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Once.Api.Filters;

public class PermissionFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var hasPermissionAttribute = context.MethodInfo.GetCustomAttribute<HasPermissionAttribute>();

        if(hasPermissionAttribute is {})
        {
            if (Enum.TryParse(hasPermissionAttribute.Policy, out EnumPermission permission))
            {
                operation.Description += $"REQUIRED PERMISSION ID: { (int)permission }";
            }
        }
    }
}
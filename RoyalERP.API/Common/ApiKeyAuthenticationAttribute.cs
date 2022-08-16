using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoyalERP.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthenticationAttribute : Attribute, IAsyncActionFilter {

    private const string ApiKeyHeader = "X-API-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var potentialapikey)) {
            context.Result = new UnauthorizedResult();
            return;
        }

        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var actualapikey = config.GetValue<string>("APIKey");

        if (!actualapikey.Equals(potentialapikey)) {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();

    }

}
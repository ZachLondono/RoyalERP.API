using System.Text;
using System.Text.Json;

namespace RoyalERP.API.Common.Domain;

public class DomainExceptionMiddleware {

    private readonly RequestDelegate _next;

    public DomainExceptionMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {

        try {

            await _next.Invoke(context);

        } catch (DomainInvariantViolatedException ex) {

            var problem = ex.ProblemDetails;
            var responseBody = JsonSerializer.Serialize(problem);
            var responseData = Encoding.UTF8.GetBytes(responseBody);

            context.Response.StatusCode = problem.Status ?? 400;
            context.Response.ContentType = "application/json";
            await context.Response.Body.WriteAsync(responseData);

        }

    }

}

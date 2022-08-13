namespace RoyalERP.Common.Domain;

public static class MiddlewareExtensions {
    
    public static IApplicationBuilder UseDomainExceptionMiddleware(this IApplicationBuilder app) => app.UseMiddleware<DomainExceptionMiddleware>();

}

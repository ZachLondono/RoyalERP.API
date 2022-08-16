using MediatR;

namespace RoyalERP.Common;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> {

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {

        _logger.LogInformation("Handling request {Request}", request);
        try { 
            var response = await next();
            _logger.LogTrace("Request handled successfully");
            return response;
        } catch (Exception ex) {
            _logger.LogError("Error handling request {Request} {Exception}", request, ex);
            throw;
        }

    }

}

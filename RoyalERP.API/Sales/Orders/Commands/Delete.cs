using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.Sales.Orders.Commands;

public class Delete {

    public record Command(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {
        
        private readonly ISalesUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(ISalesUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.Orders.GetAsync(request.OrderId);

            if (order is null) {
                _logger.LogWarning("Tried to delete order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            await _work.Orders.RemoveAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Deleted order with id: {OrderId}", request.OrderId);

            return new NoContentResult();


        }
    }

}
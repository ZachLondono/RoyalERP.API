using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class Delete {

    public record Command(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IManufacturingUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(IManufacturingUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.WorkOrders.GetAsync(request.OrderId);

            if (order is null) {
                _logger.LogWarning("Tried to delete order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            await _work.WorkOrders.RemoveAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Deleted order with id: {OrderId}", request.OrderId);

            return new NoContentResult();


        }
    }

}

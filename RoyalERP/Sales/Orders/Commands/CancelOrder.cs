using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Commands;

public class CancelOrder {

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
                _logger.LogWarning("Tried to cancel order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            order.Cancel();
            await _work.Orders.UpdateAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Canceled order with id {OrderId}", request.OrderId);

            return new OkObjectResult(new OrderDTO() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                CustomerId = order.CustomerId,
                VendorId = order.VendorId,
                PlacedDate = order.PlacedDate,
                ConfirmedDate = order.ConfirmedDate,
                CompletedDate = order.CompletedDate,
                Status = order.Status
            });

        }
    }

}
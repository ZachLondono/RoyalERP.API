using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Contracts.Orders;

namespace RoyalERP.API.Sales.Orders.Commands;

public class AddItemToOrder {

    public record Command(Guid OrderId, NewItem NewItem) : IRequest<IActionResult>;

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
                _logger.LogWarning("Tried to add item to order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            var newitem = order.AddItem(request.NewItem.ProductName, request.NewItem.Quantity, request.NewItem.Properties);
            await _work.Orders.UpdateAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Added new item to order {OrderId} with id {OrderedItemId}", request.OrderId, newitem.Id);

            var items = order.Items.Select(item => new OrderedItemDTO() {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Properties = item.Properties,
            });

            return new OkObjectResult(new OrderDetails() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                CustomerId = order.CustomerId,
                VendorId = order.VendorId,
                PlacedDate = order.PlacedDate,
                ConfirmedDate = order.ConfirmedDate,
                CompletedDate = order.CompletedDate,
                Status = order.Status.ToString(),
                Items = items
            });

        }
    }

}

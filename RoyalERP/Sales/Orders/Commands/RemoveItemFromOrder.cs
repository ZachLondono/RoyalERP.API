using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Commands;

public class RemoveItemFromOrder {

    public record Command(Guid OrderId, Guid ItemId) : IRequest<IActionResult>;

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
                _logger.LogWarning("Tried to remove ordered item from order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            var itemToDelete = order.Items.Where(i => i.Id.Equals(request.ItemId)).FirstOrDefault();

            if (itemToDelete is null) {
                _logger.LogWarning("Tried to remove ordered item not exist with id: {OrderedItemId}", request.ItemId);
                return new NotFoundResult();
            }

            var result = order.RemoveItem(itemToDelete);
            if (!result) _logger.LogWarning($"RemoveItem returned false while removing an item that exists in the order");
            await _work.Orders.UpdateAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Removed item from order {OrderId} with id {OrderedItemId}", request.OrderId, request.ItemId);

            var items = new List<OrderedItemDTO>();
            foreach (var item in order.Items) {
                items.Add(new OrderedItemDTO() {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Properties = item.Properties,
                });
            }

            return new OkObjectResult(new OrderDetails() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                CustomerId = order.CustomerId,
                VendorId = order.VendorId,
                PlacedDate = order.PlacedDate,
                ConfirmedDate = order.ConfirmedDate,
                CompletedDate = order.CompletedDate,
                Status = order.Status,
                Items = items
            });

        }
    }

}

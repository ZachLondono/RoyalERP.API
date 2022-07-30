using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Commands;

public class CompleteOrder {

    public record Command(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.Orders.GetAsync(request.OrderId);

            if (order is null) return new NotFoundResult();

            order.Complete();
            await _work.Orders.UpdateAsync(order);

            await _work.CommitAsync();

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
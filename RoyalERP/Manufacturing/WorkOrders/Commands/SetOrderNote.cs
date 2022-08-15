using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Contracts.WorkOrders;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class SetOrderNote {

    public record Command(Guid OrderId, WorkOrderNote Update) : IRequest<IActionResult>;

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
                _logger.LogWarning("Tried to update order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            order.SetNote(request.Update.Note);
            await _work.WorkOrders.UpdateAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Updated order with id: {OrderId}", request.OrderId);

            return new OkObjectResult(new WorkOrderDTO() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                Note = order.Note,
                CustomerName = order.CustomerName,
                VendorName = order.VendorName,
                ReleasedDate = order.ReleasedDate,
                ScheduledDate = order.ScheduledDate,
                FulfilledDate = order.FulfilledDate,
                Status = order.Status.ToString()
            });


        }
    }

}
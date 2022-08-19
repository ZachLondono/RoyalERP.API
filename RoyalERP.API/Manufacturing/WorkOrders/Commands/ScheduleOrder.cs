using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Contracts.WorkOrders;

namespace RoyalERP.API.Manufacturing.WorkOrders.Commands;

public class ScheduleOrder {

    public record Command(Guid OrderId, DateTime ScheduledDate) : IRequest<IActionResult>;

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
                _logger.LogWarning("Tried to schedule order that does not exist with id: {OrderId}", request.OrderId);
                return new NotFoundResult();
            }

            order.Schedule(request.ScheduledDate);
            await _work.WorkOrders.UpdateAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Scheduled order with id: {OrderId}", request.OrderId);

            return new OkObjectResult(new WorkOrderDTO() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
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

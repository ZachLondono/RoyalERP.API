using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using RoyalERP.Manufacturing.WorkOrders.DTO;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class Create {

    public record Command(NewWorkOrder NewWorkOrder) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IManufacturingUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(IManufacturingUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var newOrder = WorkOrder.Create(request.NewWorkOrder.Number, request.NewWorkOrder.Name, request.NewWorkOrder.CustomerName, request.NewWorkOrder.VendorName);

            await _work.WorkOrders.AddAsync(newOrder);

            await _work.CommitAsync();

            _logger.LogTrace("Created order with id: {OrderId}", newOrder.Id);

            return new CreatedResult($"workorders/{newOrder.Id}", new WorkOrderDTO() {
                Id = newOrder.Id,
                Number = newOrder.Number,
                Name = newOrder.Name,
                CustomerName = newOrder.CustomerName,
                VendorName = newOrder.VendorName,
                ReleasedDate = newOrder.ReleasedDate,
                ScheduledDate = newOrder.ScheduledDate,
                FulfilledDate = newOrder.FulfilledDate,
                Status = newOrder.Status
            });

        }
    }

}
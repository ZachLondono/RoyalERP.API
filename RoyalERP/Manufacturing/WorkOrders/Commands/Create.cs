using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using RoyalERP.Manufacturing.WorkOrders.DTO;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class Create {

    public record Command(NewWorkOrder NewWorkOrder) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {
        
        private readonly IManufacturingUnitOfWork _work;

        public Handler(IManufacturingUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var newOrder = WorkOrder.Create(request.NewWorkOrder.Number, request.NewWorkOrder.Name, request.NewWorkOrder.CustomerName, request.NewWorkOrder.VendorName);

            await _work.WorkOrders.AddAsync(newOrder);

            await _work.CommitAsync();

            return new CreatedResult($"workorders/{newOrder.Id}", new WorkOrderDTO() {
                Id = newOrder.Id,
                Number = newOrder.Number,
                Name = newOrder.Name,
                CustomerName = newOrder.CustomerName,
                VendorName = newOrder.VendorName,
                ReleasedDate = newOrder.ReleasedDate,
                FulfilledDate = newOrder.FulfilledDate,
                Status = newOrder.Status
            });

        }
    }

}
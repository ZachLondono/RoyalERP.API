﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Manufacturing.WorkOrders.DTO;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class ScheduleOrder {

    public record Command(Guid OrderId, DateTime ScheduledDate) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IManufacturingUnitOfWork _work;

        public Handler(IManufacturingUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.WorkOrders.GetAsync(request.OrderId);

            if (order is null) return new NotFoundResult();

            order.Schedule(request.ScheduledDate);
            await _work.WorkOrders.UpdateAsync(order);

            await _work.CommitAsync();

            return new OkObjectResult(new WorkOrderDTO() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                ReleasedDate = order.ReleasedDate,
                ScheduledDate = order.ScheduledDate,
                FulfilledDate = order.FulfilledDate,
                Status = order.Status
            });


        }
    }

}
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.Manufacturing.WorkOrders.Commands;

public class Delete {

    public record Command(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IManufacturingUnitOfWork _work;

        public Handler(IManufacturingUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.WorkOrders.GetAsync(request.OrderId);

            if (order is null) return new NotFoundResult();

            await _work.WorkOrders.RemoveAsync(order);

            await _work.CommitAsync();

            return new NoContentResult();


        }
    }

}

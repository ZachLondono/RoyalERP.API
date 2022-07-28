using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.Sales.Companies.Commands;

public class Delete {

    public record Command(Guid Id) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {
        
        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.Companies.GetAsync(request.Id);

            if (order is null)
                return new NotFoundResult();

            await _work.Companies.RemoveAsync(order);

            await _work.CommitAsync();

            return new NoContentResult();

        }

    }
}
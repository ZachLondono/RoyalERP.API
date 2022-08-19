using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Sales.Companies.Commands;

public class Delete {

    public record Command(Guid Id) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {
        
        private readonly ISalesUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(ISalesUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _work.Companies.GetAsync(request.Id);

            if (order is null) {
                _logger.LogWarning("Tried to delete a company that does not exist");
                return new NotFoundResult();
            }

            await _work.Companies.RemoveAsync(order);

            await _work.CommitAsync();
            
            _logger.LogTrace("Deleted company with id: {CompanyId}", request.Id);

            return new NoContentResult();

        }

    }
}
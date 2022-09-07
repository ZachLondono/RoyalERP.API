using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Sales.Companies.Commands;

public class RemoveInfoField {

    public record Command(Guid CompanyId, string Field) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(ISalesUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var company = await _work.Companies.GetAsync(request.CompanyId);

            if (company is null) return new NotFoundResult();
            
            company.RemoveInfo(request.Field);
            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            _logger.LogTrace("Removed company info: {CompanyId} {Field}", company.Id, request.Field);

            return new OkObjectResult(company.AsDTO());

        }

    }
}

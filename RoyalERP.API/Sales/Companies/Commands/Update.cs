using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Commands;

public class Update {

    public record Command(Guid CompanyId, UpdateCompany Update) : IRequest<IActionResult>;

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

            company.Update(request.Update.Name ?? company.Name, request.Update.Contact ?? company.Contact, request.Update.Email ?? company.Email);
            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            _logger.LogTrace("Updated company: {CompanyId}", company.Id);

            return new OkObjectResult(company.AsDTO());

        }

    }
}
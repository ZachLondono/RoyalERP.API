using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Commands;

public class UpdateCompanyAddress {

    public record Command(Guid CompanyId, UpdateAddress Update) : IRequest<IActionResult>;

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

            company.SetAddress(request.Update.Line1, request.Update.Line2, request.Update.Line3, request.Update.City, request.Update.State, request.Update.Zip);
            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            _logger.LogTrace("Updated company: {CompanyId}", company.Id);

            return new OkObjectResult(new CompanyDTO() {
                Id = company.Id,
                Version = company.Version,
                Name = company.Name,
                Contact = company.Contact,
                Email = company.Email,
                Address = new() {
                    Line1 = company.Address.Line1,
                    Line2 = company.Address.Line2,
                    Line3 = company.Address.Line3,
                    City = company.Address.City,
                    State = company.Address.State,
                    Zip = company.Address.Zip,
                }
            });

        }

    }
}
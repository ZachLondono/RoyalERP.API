using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Contracts.Companies;

namespace RoyalERP.Sales.Companies.Commands;

public class Create {

    public record Command(NewCompany NewCompany) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(ISalesUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var newCompany = Company.Create(request.NewCompany.Name);

            await _work.Companies.AddAsync(newCompany);

            await _work.CommitAsync();

            _logger.LogTrace("Created new company with id: {CompanyId}", newCompany.Id);

            return new CreatedResult($"/companies/{newCompany.Id}", new CompanyDTO() {
                Id = newCompany.Id,
                Name = newCompany.Name
            });

        }

    }
}
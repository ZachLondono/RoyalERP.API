using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Companies.DTO;

namespace RoyalERP.Sales.Companies.Commands;

public class Create {

    public record Command(NewCompany NewCompany) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var newCompany = Company.Create(request.NewCompany.Name);

            await _work.Companies.AddAsync(newCompany);

            await _work.CommitAsync();

            return new CreatedResult($"/companies/{newCompany.Id}", new CompanyDTO() {
                Id = newCompany.Id,
                Name = newCompany.Name
            });

        }

    }
}
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Sales.Companies.Commands;

public class RemoveCompanyContact {

    public record Command(Guid CompanyId, Guid ContactId) : IRequest<IActionResult>;

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

            var contact = company.Contacts.FirstOrDefault(c => c.Id.Equals(request.ContactId));

            if (contact is null) return new NotFoundResult();

            company.RemoveContact(contact);
            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            return new OkObjectResult(company.AsDTO());

        }

    }

}
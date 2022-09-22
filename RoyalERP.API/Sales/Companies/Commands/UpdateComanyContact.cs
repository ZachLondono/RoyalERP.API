using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Commands;

public class UpdateComanyContact {

    public record Command(Guid CompanyId, Guid ContactId, UpdateContact Update) : IRequest<IActionResult>;

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

            if (request.Update.Name is not null)
                contact.SetName(request.Update.Name);

            if (request.Update.Email is not null)
                contact.SetEmail(request.Update.Email);

            if (request.Update.Phone is not null)
                contact.SetPhone(request.Update.Phone);

            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            return new OkObjectResult(company.AsDTO());

        }

    }

}

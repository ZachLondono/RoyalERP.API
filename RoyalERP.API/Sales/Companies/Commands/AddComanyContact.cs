using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Sales.Companies.Commands;

public class AddComanyContact {

    public record Command(Guid CompanyId, string Name, string Email, string Phone) : IRequest<IActionResult>;

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

            company.AddContact(request.Name, request.Email, request.Phone);
            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            return new OkObjectResult(company.AsDTO());

        }

    }

}

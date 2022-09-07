using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Commands;

public class RemoveDefault {

    public record Command(Guid CompanyId, RemoveDefaultValue Update) : IRequest<IActionResult>;

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

            var config = company.DefaultConfigurations
                                .Where(c => c.ProductId.Equals(request.Update.ProductId) && c.AttributeId.Equals(request.Update.AttributeId))
                                .FirstOrDefault();

            if (config is not null) {
                company.RemoveDefault(config);
                await _work.Companies.UpdateAsync(company);
                await _work.CommitAsync();

                _logger.LogTrace("Set company default: {CompanyId} {DefaultConfiguration}", company.Id, request.Update);
            }

            return new OkObjectResult(company.AsDTO());

        }

    }
}

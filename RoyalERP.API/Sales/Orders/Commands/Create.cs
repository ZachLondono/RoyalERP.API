using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Sales.Companies.Domain;
using RoyalERP.API.Sales.Orders.Domain;
using RoyalERP.Contracts.Orders;

namespace RoyalERP.API.Sales.Orders.Commands;

public class Create {

    public record Command(NewOrder NewOrder) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;
        private readonly ILogger<Handler> _logger;

        public Handler(ISalesUnitOfWork work, ILogger<Handler> logger) {
            _work = work;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            Guid customerId = await GetCompanyId(request.NewOrder.CustomerId, request.NewOrder.CustomerName);
            Guid vendorId = await GetCompanyId(request.NewOrder.VendorId, request.NewOrder.VendorName);

            var order = Order.Create(request.NewOrder.Number, request.NewOrder.Name, customerId, vendorId);

            await _work.Orders.AddAsync(order);

            await _work.CommitAsync();

            _logger.LogTrace("Created order with id {OrderId}", order.Id);

            return new CreatedResult($"/orders/{order.Id}", new OrderDetails() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                CustomerId = order.CustomerId,
                VendorId = order.VendorId,
                PlacedDate = order.PlacedDate,
                ConfirmedDate = order.ConfirmedDate,
                CompletedDate = order.CompletedDate,
                Status = order.Status.ToString()
            });


        }

        /// <summary>
        /// Tries to get the company with the given name or id, if one does not exist a new company is created with the given name. If a name is not provided, a new company is created with a default name
        /// </summary>
        /// <returns>The id of the company</returns>
        private async Task<Guid> GetCompanyId(Guid? potentialId, string? potentialName) {
            
            Guid companyId = Guid.Empty;
            if (potentialId is not null) {
                
                var customer = await _work.Companies.GetAsync((Guid)potentialId);
                if (customer is not null) {
                    companyId = (Guid)potentialId;
                } else {
                    _logger.LogWarning("An invalid company id was given while attempting to create a new order {CompanyId}", potentialId);
                }

            } else if (potentialName is not null) {
                companyId = await GetCompanyIdByName(potentialName);
            } 
            
            if (companyId.Equals(Guid.Empty)) {
                _logger.LogTrace("Using default company name for new order");
                companyId = await GetCompanyIdByName("Unknown Company");
            }

            return companyId;

        }

        /// <summary>
        /// Searches the company list for a company with the given name (not case sensitive). The first matching company is returned. If a company is not found, a new company is created
        /// </summary>
        private async Task<Guid> GetCompanyIdByName(string name) {

            var ids = await _work.Companies.GetCompanyIdsWithName(name);

            if (ids.Any()) return ids.First();

            var newCompany = Company.Create(name);
            await _work.Companies.AddAsync(newCompany);

            _logger.LogTrace("Created new company with name {CompanyName}, while creating a new posting", name);

            return newCompany.Id;

        }

    }

}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Orders.Domain;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Commands;

public class Create {

    public record Command(NewOrder NewOrder) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {


            Guid customerId;
            if (request.NewOrder.CustomerId is not null) {
                // TODO: Check that company exists
                customerId = Guid.NewGuid();
            } else if (request.NewOrder.CustomerName is not null) {
                customerId = await GetCompanyIdByName(request.NewOrder.CustomerName);
            } else {
                _work.Rollback();
                return new BadRequestObjectResult(new ProblemDetails() {
                    Title = "Missing customer",
                    Detail = "Either the Customer's Id or Name must be provided.",
                    Status = 400
                });
            }

            Guid vendorId;
            if (request.NewOrder.VendorId is not null) {
                // TODO: Check that company exists
                vendorId = Guid.NewGuid();
            } else if (request.NewOrder.VendorName is not null) {
                vendorId = await GetCompanyIdByName(request.NewOrder.VendorName);
            } else {
                _work.Rollback(); // if a company was created, it should be rolled back
                return new BadRequestObjectResult(new ProblemDetails() {
                    Title = "Missing vendor",
                    Detail = "Either the Vendor's Id or Name must be provided.",
                    Status = 400
                });
            }

            var order = Order.Create(request.NewOrder.Number, request.NewOrder.Name, customerId, vendorId);

            await _work.Orders.AddAsync(order);

            await _work.CommitAsync();

            return new CreatedResult($"/orders/{order.Id}", new OrderDTO() {
                Id = order.Id,
                Number = order.Number,
                Name = order.Name,
                CustomerId = order.CustomerId,
                VendorId = order.VendorId,
                PlacedDate = order.PlacedDate,
                ConfirmedDate = order.ConfirmedDate,
                CompletedDate = order.CompletedDate,
                Status = order.Status
            });


        }

        /// <summary>
        /// Searches the company list for a company with the given name (not case sensitive). The first matching company is returned. If a company is not found, a new company is created
        /// </summary>
        private async Task<Guid> GetCompanyIdByName(string name) {

            var ids = await _work.Companies.GetCompanyIdsWithName(name);

            if (ids.Any()) return ids.First();

            var newCompany = Company.Create(name);
            await _work.Companies.AddAsync(newCompany);

            return newCompany.Id;

        }

    }

}

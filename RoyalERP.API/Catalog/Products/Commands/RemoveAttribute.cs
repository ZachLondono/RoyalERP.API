using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Products;

namespace RoyalERP.API.Catalog.Products.Commands;

public class RemoveAttribute {

    public record Command(Guid ProductId, Guid AttributeId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ICatalogUnitOfWork _work;

        public Handler(ICatalogUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var product = await _work.Products.GetAsync(request.ProductId);

            if (product is null) return new NotFoundResult();

            product.RemoveAttribute(request.AttributeId);

            await _work.Products.UpdateAsync(product);
            await _work.CommitAsync();

            return new OkObjectResult(new ProductDTO() {
                Id = product.Id,
                Name = product.Name,
                Attributes = product.AttributeIds
            });

        }
    }

}

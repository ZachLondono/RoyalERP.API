using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Products;

namespace RoyalERP.API.Catalog.Products.Commands;

public class AddAttribute {

    public record Command(Guid ProductId, ProductAttributeAdd AttributeAdd) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ICatalogUnitOfWork _work;

        public Handler(ICatalogUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var product = await _work.Products.GetAsync(request.ProductId);

            if (product is null) return new NotFoundResult();

            product.AddAttribute(request.AttributeAdd.AttributeId);

            await _work.Products.UpdateAsync(product);
            await _work.CommitAsync();

            return new OkObjectResult(new ProductDTO() {
                Id = product.Id,
                Name = product.Name,
                ClassId = product.ClassId,
                Attributes = product.AttributeIds
            });

        }
    }

}

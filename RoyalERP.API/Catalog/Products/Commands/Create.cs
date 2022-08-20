using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.API.Contracts.Products;

namespace RoyalERP.API.Catalog.Products.Commands;

public class Create {

    public record Command(NewProduct NewProduct) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ICatalogUnitOfWork _work;

        public Handler(ICatalogUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var product = Product.Create(request.NewProduct.Name);
            await _work.Products.AddAsync(product);
            await _work.CommitAsync();

            return new CreatedResult($"/products/{product.Id}", new ProductDTO() {
                Id = product.Id,
                Name = product.Name,
                ClassId = product.ClassId,
                Attributes = Enumerable.Empty<Guid>()
            });

        }
    }

}

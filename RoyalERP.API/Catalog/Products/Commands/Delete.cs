using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Catalog.Products.Commands;

public class Delete {

    public record Command(Guid ProductId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ICatalogUnitOfWork _work;

        public Handler(ICatalogUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var product = await _work.Products.GetAsync(request.ProductId);

            if (product is null) return new NotFoundResult();

            await _work.Products.RemoveAsync(product);
            await _work.CommitAsync();

            return new NoContentResult();

        }
    }

}
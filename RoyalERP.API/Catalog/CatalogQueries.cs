using MediatR;
using RoyalERP.API.Catalog.Products.Queries;

namespace RoyalERP.API.Catalog;

public class CatalogQueries {

    private readonly ISender _sender;

    public CatalogQueries(ISender sender) {
        _sender = sender;
    }

    public Task<Guid?> GetProductClassFromProductId(Guid productId) {
        return _sender.Send(new GetProductClass.Query(productId));
    }

}

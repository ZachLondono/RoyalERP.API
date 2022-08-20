using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.ProductClasses;
using RoyalERP.Common;
using RoyalERP.Contracts.Orders;

namespace RoyalERP.API.Catalog.ProductClasses;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class ProductClassController : ControllerBase {

    private readonly ICatalogConnectionFactory _connectionFactory;

    public ProductClassController(ICatalogConnectionFactory connectionFactory) {
        _connectionFactory = connectionFactory;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductClassDTO))]
    public void Create(NewProductClass newClass) {
        throw new NotImplementedException();
    }

    [Route("{classId}")]
    [HttpGet]
    public void Read(Guid classId) {
        throw new NotImplementedException();
    }

    [Route("{classId}")]
    [HttpPut]
    public void Update(Guid classId, ProductClassUpdate update) {
        throw new NotImplementedException();
    }

    [Route("{classId}")]
    [HttpDelete]
    public void Delete(Guid classId) {
        throw new NotImplementedException();
    }

}

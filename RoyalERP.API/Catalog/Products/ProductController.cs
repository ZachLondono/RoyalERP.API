using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Catalog.Products.Commands;
using RoyalERP.API.Catalog.Products.Queries;
using RoyalERP.API.Contracts.Product;
using RoyalERP.Common;

namespace RoyalERP.API.Catalog.Products;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class ProductsController : ControllerBase {

    private readonly ISender _sender;

    public ProductsController(ISender sender) {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDTO))]
    public void Create([FromBody] NewProduct newProduct) => _sender.Send(new Create.Command(newProduct));

    [Route("{productId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateName(Guid productId, ProductNameUpdate nameUpdate)
        => _sender.Send(new UpdateName.Command(productId, nameUpdate));

    [Route("{productId}/attributes")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AddAttribute(Guid productId, [FromBody] ProductAttributeAdd attributeAdd)
        => _sender.Send(new AddAttribute.Command(productId, attributeAdd));

    [Route("{productId}/attributes/{attributeId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveAttribute(Guid productId, Guid attributeId)
        => _sender.Send(new RemoveAttribute.Command(productId, attributeId));

    [Route("{productId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(Guid productId) => _sender.Send(new Delete.Command(productId));

    [Route("{productId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(Guid productId) => _sender.Send(new GetById.Query(productId));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDTO>))]
    public Task<IActionResult> GetAll() => _sender.Send(new GetAll.Query());

}
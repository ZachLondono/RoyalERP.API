using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Contracts.ProductAttributes;
using RoyalERP.API.Common;

namespace RoyalERP.API.Catalog.ProductAttributes;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class AttributesController : ControllerBase {

    private readonly ICatalogUnitOfWork _uow;

    public AttributesController(ICatalogUnitOfWork uow) {
        _uow = uow;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductAttributeDTO))]
    public async Task<IActionResult> Create(NewProductAttribute newAttribute) {

        var createdAttribute = ProductAttribute.Create(newAttribute.Name);
        await _uow.ProductAttributes.AddAsync(createdAttribute);
        await _uow.CommitAsync();

        return Created($"/attributes/{createdAttribute.Id}", new ProductAttributeDTO {
            Id = createdAttribute.Id,
            Name = createdAttribute.Name
        });

    }

    [Route("{attributeId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductAttributeDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(Guid attributeId) {

        // TODO: custom query here so there is no need to map to dto

        var prodAttribute = await _uow.ProductAttributes.GetAsync(attributeId);

        if (prodAttribute is null) return NotFound();

        return Ok(new ProductAttributeDTO {
            Id = prodAttribute.Id,
            Name = prodAttribute.Name
        });

    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductAttributeDTO>))]
    public async Task<IActionResult> ReadAll() {

        // TODO: custom query here so there is no need to map to dto

        var prodAttributes = await _uow.ProductAttributes.GetAllAsync();

        List<ProductAttributeDTO> dtos = new();

        foreach (var prodAttribute in prodAttributes) {
            dtos.Add(new ProductAttributeDTO {
                Id = prodAttribute.Id,
                Name = prodAttribute.Name
            });
        }

        return Ok(dtos);
    }

    [Route("{attributeId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductAttributeDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid attributeId, ProductAttributeUpdate update) {

        var prodAttribute = await _uow.ProductAttributes.GetAsync(attributeId);

        if (prodAttribute is null) return NotFound();

        prodAttribute.SetName(update.Name);
        await _uow.ProductAttributes.UpdateAsync(prodAttribute);
        await _uow.CommitAsync();

        return Ok(new ProductAttributeDTO {
            Id = prodAttribute.Id,
            Name = prodAttribute.Name
        });

    }

    [Route("{attributeId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid attributeId) {

        var prodAttribute = await _uow.ProductAttributes.GetAsync(attributeId);

        if (prodAttribute is null) return NotFound();

        await _uow.ProductAttributes.RemoveAsync(prodAttribute);
        await _uow.CommitAsync();

        return NoContent();

    }

}
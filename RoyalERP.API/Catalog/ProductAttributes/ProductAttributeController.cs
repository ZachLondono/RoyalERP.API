using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Contracts.ProductAttributes;
using RoyalERP.Common;

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
    public async Task<IActionResult> Create(NewProductAttribute newClass) {

        var createdClass = ProductAttribute.Create(newClass.Name);
        await _uow.ProductAttributes.AddAsync(createdClass);
        await _uow.CommitAsync();

        return Created($"/attributes/{createdClass.Id}", new ProductAttributeDTO {
            Id = createdClass.Id,
            Name = createdClass.Name
        });

    }

    [Route("{attributeId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductAttributeDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(Guid attributeId) {

        // TODO: custom query here so there is no need to map to dto

        var prodClass = await _uow.ProductClasses.GetAsync(attributeId);

        if (prodClass is null) return NotFound();

        return Ok(new ProductAttributeDTO {
            Id = prodClass.Id,
            Name = prodClass.Name
        });

    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductAttributeDTO>))]
    public async Task<IActionResult> ReadAll() {

        // TODO: custom query here so there is no need to map to dto

        var prodClasses = await _uow.ProductClasses.GetAllAsync();

        List<ProductAttributeDTO> dtos = new();

        foreach (var prodClass in prodClasses) {
            dtos.Add(new ProductAttributeDTO {
                Id = prodClass.Id,
                Name = prodClass.Name
            });
        }

        return Ok(dtos);
    }

    [Route("{attributeId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductAttributeDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid attributeId, ProductAttributeUpdate update) {

        var prodClass = await _uow.ProductClasses.GetAsync(attributeId);

        if (prodClass is null) return NotFound();

        prodClass.SetName(update.Name);
        await _uow.ProductClasses.UpdateAsync(prodClass);
        await _uow.CommitAsync();

        return Ok(new ProductAttributeDTO {
            Id = prodClass.Id,
            Name = prodClass.Name
        });

    }

    [Route("{attributeId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid attributeId) {

        var prodClass = await _uow.ProductClasses.GetAsync(attributeId);

        if (prodClass is null) return NotFound();

        await _uow.ProductClasses.RemoveAsync(prodClass);
        await _uow.CommitAsync();

        return NoContent();

    }

}
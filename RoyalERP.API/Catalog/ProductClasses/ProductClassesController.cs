using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using RoyalERP.API.Contracts.ProductAttributes;
using RoyalERP.API.Contracts.ProductClasses;
using RoyalERP.Common;

namespace RoyalERP.API.Catalog.ProductClasses;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class ProductClassesController : ControllerBase {

    private readonly ICatalogUnitOfWork _uow;

    public ProductClassesController(ICatalogUnitOfWork uow) {
        _uow = uow;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductClassDTO))]
    public async Task<IActionResult> Create(NewProductClass newClass) {

        var createdClass = ProductClass.Create(newClass.Name);
        await _uow.ProductClasses.AddAsync(createdClass);
        await _uow.CommitAsync();

        return Created($"/productclasses/{createdClass.Id}", new ProductClassDTO {
            Id = createdClass.Id,
            Name = createdClass.Name
        });

    }

    [Route("{classId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductAttributeDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(Guid classId) {

        // TODO: custom query here so there is no need to map to dto

        var prodClass = await _uow.ProductClasses.GetAsync(classId);

        if (prodClass is null) return NotFound();

        return Ok(new ProductClassDTO {
            Id = prodClass.Id,
            Name = prodClass.Name
        });

    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductClassDTO>))]
    public async Task<IActionResult> ReadAll() {

        // TODO: custom query here so there is no need to map to dto

        var prodClasses = await _uow.ProductClasses.GetAllAsync();

        List<ProductClassDTO> dtos = new();

        foreach (var prodClass in prodClasses) {
            dtos.Add(new ProductClassDTO {
                Id = prodClass.Id,
                Name = prodClass.Name
            });
        }

        return Ok(dtos);

    }

    [Route("{classId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductClassDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid classId, ProductClassUpdate update) {
        
        var prodClass = await _uow.ProductClasses.GetAsync(classId);

        if (prodClass is null) return NotFound();

        prodClass.SetName(update.Name);
        await _uow.ProductClasses.UpdateAsync(prodClass);
        await _uow.CommitAsync();

        return Ok(new ProductClassDTO {
            Id = prodClass.Id,
            Name = prodClass.Name
        });

    }

    [Route("{classId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid classId) {

        var prodClass = await _uow.ProductClasses.GetAsync(classId);

        if (prodClass is null) return NotFound();

        await _uow.ProductClasses.RemoveAsync(prodClass);
        await _uow.CommitAsync();

        return NoContent();

    }

}

using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common;

namespace RoyalERP.API.Catalog.Products;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class ProductController : ControllerBase {

    [HttpPost]
    public void Create() => throw new NotImplementedException();

    [Route("{productId}")]
    [HttpPut]
    public void UpdateName() => throw new NotImplementedException();

    [Route("{productId}/attributes")]
    [HttpPut]
    public void AddAttribute() => throw new NotImplementedException();

    [Route("{productId}/attributes/{attribute}")]
    [HttpDelete]
    public void RemoveAttribute() => throw new NotImplementedException();

    [Route("{productId}")]
    [HttpDelete]
    public void Delete() => throw new NotImplementedException();

    [Route("{productId}")]
    [HttpGet]
    public void GetById() => throw new NotImplementedException();

    [HttpGet]
    public void GetAll() => throw new NotImplementedException();

}
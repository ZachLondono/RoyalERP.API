using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common;
using RoyalERP.Sales.Companies.Commands;
using RoyalERP.Contracts.Companies;
using RoyalERP.Sales.Companies.Queries;

namespace RoyalERP.Sales.Companies;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class CompaniesController : ControllerBase {

    private readonly ISender _sender;

    public CompaniesController(ISender sender) {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CompanyDTO))]
    public Task<IActionResult> Create([FromBody]NewCompany newCompany) {
        return _sender.Send(new Create.Command(newCompany));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CompanyDTO>))]
    public Task<IActionResult> GetAll() {
        return _sender.Send(new GetAll.Query());
    }

    [Route("{companyId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid companyId) {
        var order = await _sender.Send(new GetById.Query(companyId));
        if (order is null) return NotFound();
        return Ok(order);
    }

    [Route("{companyId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateCompany(Guid companyId, [FromBody] UpdateCompany update) {
        return _sender.Send(new Update.Command(companyId, update));
    }

    [Route("{companyId}/address")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateAddress(Guid companyId, [FromBody] UpdateAddress update) {
        return _sender.Send(new UpdateCompanyAddress.Command(companyId, update));
    }

    [Route("{companyId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(Guid companyId) {
        return _sender.Send(new Delete.Command(companyId));
    }

}

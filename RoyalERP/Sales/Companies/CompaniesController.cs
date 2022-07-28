using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Companies.Commands;
using RoyalERP.Sales.Companies.DTO;
using RoyalERP.Sales.Companies.Queries;

namespace RoyalERP.Sales.Companies;

[ApiController]
[Route("[controller]")]
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
    public Task<IActionResult> GetById(Guid companyId) {
        return _sender.Send(new GetById.Query(companyId));
    }

    [Route("{companyId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(Guid companyId) {
        return _sender.Send(new Delete.Command(companyId));
    }

}

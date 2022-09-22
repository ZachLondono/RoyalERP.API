using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Common;
using RoyalERP.API.Sales.Companies.Commands;
using RoyalERP.API.Contracts.Companies;
using RoyalERP.API.Sales.Companies.Queries;

namespace RoyalERP.API.Sales.Companies;

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

    [Route("{companyId}/defaults")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetDefault(Guid companyId, [FromBody] SetDefaultValue defaultValue) {
        return _sender.Send(new SetDefault.Command(companyId, defaultValue));
    }

    [Route("{companyId}/defaults/{productId}/{attributeId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveDefault(Guid companyId, Guid productId, Guid attributeId) {
        return _sender.Send(new RemoveDefault.Command(companyId, new RemoveDefaultValue() {
            ProductId = productId,
            AttributeId = attributeId
        }));
    }

    [Route("{companyId}/info/{field}/{value}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetInfoField(Guid  companyId, string field, string value) {
        return _sender.Send(new SetInfoField.Command(companyId, field, value));
    }

    [Route("{companyId}/info/{field}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveInfoField(Guid companyId, string field) {
        return _sender.Send(new RemoveInfoField.Command(companyId, field));
    }

    [Route("{companyId}/contacts/")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AddContact(Guid companyId, NewContact newContact) {
        return _sender.Send(new AddCompanyContact.Command(companyId, newContact));
    }

    [Route("{companyId}/contacts/{contactId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveContact(Guid companyId, Guid contactId) {
        return _sender.Send(new RemoveCompanyContact.Command(companyId, contactId));
    }

    [Route("{companyId}/contacts/{contactId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateContact(Guid companyId, Guid contactId, UpdateContact update) {
        return _sender.Send(new UpdateCompanyContact.Command(companyId, contactId, update));
    }

    [Route("{companyId}/contacts/{contactId}/roles")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AddRoleToContact(Guid companyId, Guid contactId, ContactRole update) {
        return _sender.Send(new AddRoleToContact.Command(companyId, contactId, update));
    }

    [Route("{companyId}/contacts/{contactId}/roles")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveRoleFromContact(Guid companyId, Guid contactId, ContactRole update) {
        return _sender.Send(new RemoveRoleFromContact.Command(companyId, contactId, update));
    }

}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Manufacturing.WorkOrders.Commands;
using RoyalERP.Manufacturing.WorkOrders.Queries;
using RoyalERP.Manufacturing.WorkOrders.DTO;
using RoyalERP.Common;

namespace RoyalERP.Manufacturing.WorkOrders;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class WorkOrdersController {

    private readonly ISender _sender;

    public WorkOrdersController(ISender sender) {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkOrderDTO))]
    public Task<IActionResult> Create([FromBody] NewWorkOrder newOrder) {
        return _sender.Send(new Create.Command(newOrder));
    }

    [HttpPut]
    [Route("{workorderId}/release")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Release(Guid workorderId) {
        return _sender.Send(new ReleaseOrder.Command(workorderId));
    }

    [HttpPut]
    [Route("{workorderId}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Schedule(Guid workorderId, [FromBody] WorkOrderSchedule schedule) {
        return _sender.Send(new ScheduleOrder.Command(workorderId, schedule.ScheduledDate));
    }

    [HttpPut]
    [Route("{workorderId}/fulfill")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Fulfill(Guid workorderId) {
        return _sender.Send(new FulfillOrder.Command(workorderId));
    }

    [HttpPut]
    [Route("{workorderId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Cancel(Guid workorderId) {
        return _sender.Send(new CancelOrder.Command(workorderId));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkOrderDTO>))]
    public Task<IActionResult> GetAll() {
        return _sender.Send(new GetAll.Query());
    }

    [Route("{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(Guid orderId) {
        return _sender.Send(new GetById.Query(orderId));
    }

    [Route("{orderId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(Guid orderId) {
        return _sender.Send(new Delete.Command(orderId));
    }

}

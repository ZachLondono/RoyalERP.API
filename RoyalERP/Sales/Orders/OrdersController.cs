﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common;
using RoyalERP.Sales.Orders.Commands;
using RoyalERP.Sales.Orders.DTO;
using RoyalERP.Sales.Orders.Queries;

namespace RoyalERP.Sales.Orders;

[ApiController]
[Route("[controller]")]
[ApiKeyAuthentication]
public class OrdersController : ControllerBase {

    private readonly ISender _sender;

    public OrdersController(ISender sender) {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDTO))]
    public Task<IActionResult> Create([FromBody] NewOrder newOrder) {
        return _sender.Send(new Create.Command(newOrder));
    }

    [HttpPut]
    [Route("{orderId}/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Confirm(Guid orderId) {
        return _sender.Send(new ConfirmOrder.Command(orderId));
    }

    [HttpPut]
    [Route("{orderId}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Complete(Guid orderId) {
        return _sender.Send(new CompleteOrder.Command(orderId));
    }

    [HttpPut]
    [Route("{orderId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Cancel(Guid orderId) {
        return _sender.Send(new CancelOrder.Command(orderId));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderDTO>))]
    public Task<IActionResult> GetAll() {
        return _sender.Send(new GetAll.Query());
    }

    [Route("{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid orderId) {
        var order = await _sender.Send(new GetById.Query(orderId));
        if (order is null) return NotFound();
        return Ok(order);
    }

    [Route("search/{orderNumber}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderSearchResult>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(string orderNumber) {
        return _sender.Send(new GetByNumber.Query(orderNumber));
    }

    [Route("{orderId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(Guid orderId) {
        return _sender.Send(new Delete.Command(orderId));
    }

}

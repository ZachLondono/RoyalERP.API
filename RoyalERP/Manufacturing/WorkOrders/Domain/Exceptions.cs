using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common.Domain;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public static class Exceptions {

    public class CantUpdateOrderException : DomainInvariantViolatedException {

        public override ProblemDetails ProblemDetails => new() {
            Title = "Can't Update Order",
            Detail = "Work order cannot be updated from current state",
            Status = 400
        };

    }

}

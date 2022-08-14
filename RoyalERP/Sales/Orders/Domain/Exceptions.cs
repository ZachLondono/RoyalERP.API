using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common.Domain;

namespace RoyalERP.Sales.Orders.Domain;

public static class Exceptions {

    public class CantUpdateCancelledOrderException : DomainInvariantViolatedException {
        
        public override ProblemDetails ProblemDetails => new() {
            Title = "Can't Update",
            Detail = "Can not update order that has been canceled",
            Status = 400
        };

    }

    public class CantAddToConfirmedOrderException : DomainInvariantViolatedException {

        public override ProblemDetails ProblemDetails => new() {
            Title = "Can't Add To Order",
            Detail = "Can not add items to orders that are already confirmed",
            Status = 400
        };

    }

}

using Microsoft.AspNetCore.Mvc;

namespace RoyalERP.API.Common.Domain;

public abstract class DomainInvariantViolatedException : InvalidOperationException {

    public abstract ProblemDetails ProblemDetails { get; }

}

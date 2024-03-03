using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApi.Exceptions;

namespace WebApi.Middleware;

//https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            LineNotFoundException e => 
                CreateProblemDetails(
                    StatusCodes.Status404NotFound, "Sales Order Line not found", e.Message),
            InvalidEntityStateException e =>
                CreateInternalServerError(),
            EntityNotFoundException e =>
                CreateProblemDetails(StatusCodes.Status404NotFound, 
                    "Entity not found.", e.Message),
            { } e => CreateProblemDetails(StatusCodes.Status400BadRequest,
                "Something went wrong.", e.Message)
        };
        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(
        int statusCode, string title, string? detail = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail
        };
    }

    private ProblemDetails CreateInternalServerError()
    {
        return CreateProblemDetails(StatusCodes.Status500InternalServerError, "Something went wrong.");
    }
    
}
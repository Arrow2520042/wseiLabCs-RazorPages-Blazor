using ApplicationCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BackendLab01.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var title = "An error occurred while processing your request.";
        var status = StatusCodes.Status500InternalServerError;

        if (exception is ContactNotFoundException || exception is KeyNotFoundException)
        {
            status = StatusCodes.Status404NotFound;
            title = "Resource not found.";
        }
        else if (exception is NoteNotFoundException)
        {
            status = StatusCodes.Status404NotFound;
            title = "Note not found.";
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = exception.Message
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}

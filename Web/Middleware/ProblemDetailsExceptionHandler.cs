using ApplicationCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BackendLab01.Middleware
{
    public class ProblemDetailsExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _factory;
        private readonly ILogger<ProblemDetailsExceptionHandler> _logger;

        public ProblemDetailsExceptionHandler(ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ContactNotFoundException || exception is NoteNotFoundException)
            {
                _logger.LogInformation("Exception '{ExceptionMessage}' handled!", exception.Message);

                var problem = _factory.CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Contact service error!",
                    "Service error",
                    detail: exception.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            }

            return false;
        }
    }
}
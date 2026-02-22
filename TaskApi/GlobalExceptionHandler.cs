using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskApi;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger;

  public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    // 1. Log the error for the developers to see in the logs
    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

    // 2. Prepare a "ProblemDetails" response (an industry standard)
    var problemDetails = new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "Server Error",
      Detail = "An unexpected error occurred on our end. Please try again later."
    };

    // 3. Send the clean JSON back to the user
    httpContext.Response.StatusCode = problemDetails.Status.Value;
    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    // Return true to signal that we have handled the error
    return true;
  }
}

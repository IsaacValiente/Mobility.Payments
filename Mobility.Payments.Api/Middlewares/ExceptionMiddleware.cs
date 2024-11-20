namespace Mobility.Payments.Api.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using System;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Crosscuting.Exception;

    /// <summary>
    /// Middleware for handling exceptions globally in the application.
    /// Captures exceptions thrown in the pipeline and returns appropriate HTTP responses.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">Logger instance for recording error information.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP context and handle exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Handles specific exception types:
        /// - <see cref="BadRequestException"/>: Returns a 400 Bad Request response.
        /// - <see cref="NotFoundException"/>: Returns a 404 Not Found response.
        /// - <see cref="UnauthorizedException"/>: Returns a 401 Unauthorized response.
        /// - <see cref="Exception"/>: Returns a 500 Internal Server Error response for any unhandled exceptions.
        /// </remarks>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (BadRequestException ex)
            {
                logger.LogError(ex, "Bad request error");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                logger.LogError(ex, "Not found error");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                logger.LogError(ex, "Unauthorized request error");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
            }
        }
    }
}

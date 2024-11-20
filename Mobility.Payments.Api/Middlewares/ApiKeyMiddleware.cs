namespace Mobility.Payments.Api.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Mobility.Payments.Crosscuting.Options;
    using System.Threading.Tasks;

    using Mobility.Payments.Crosscuting.Exception;
    /// <summary>
    /// Middleware for validating the API key in incoming HTTP requests.
    /// Ensures requests contain a valid API key in the "X-API-Key" header.
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware invocation method to validate the API key.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="apiToken">The API key configuration containing the expected API key value.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the API key is missing or invalid.</exception>
        public async Task Invoke(HttpContext context, IOptions<ApiKeyConfiguration> apiToken)
        {

            if (context.Request.Headers.TryGetValue("X-API-Key", out StringValues apiKey))
            {
                if (apiKey == apiToken.Value.Value)
                    await _next(context);
                else
                    throw new UnauthorizedException("Invalid API Key.");
            }
            else
            {
                throw new UnauthorizedException("The API Key is missing.");
            }
        }
    }
}

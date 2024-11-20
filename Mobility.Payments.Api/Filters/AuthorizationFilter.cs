namespace Mobility.Payments.Api.Filters
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Api.Extensions;
    using Mobility.Payments.Application.Interfaces;

    /// <summary>
    /// Custom authorization filter to validate user requests and set the current user in the application context.
    /// </summary>
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Logger instance to record authorization activities and errors.
        /// </summary>
        protected readonly ILogger<AuthorizationFilter> logger;
        /// <summary>
        /// Service to manage and provide the current user context.
        /// </summary>
        protected readonly IUserProviderService userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationFilter"/> class.
        /// </summary>
        /// <param name="userProvider">Service to manage and provide the current user context.</param>
        /// <param name="logger">Logger instance for recording authorization activities.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userProvider"/> or <paramref name="logger"/> is null.
        /// </exception>
        public AuthorizationFilter(IUserProviderService userProvider, ILogger<AuthorizationFilter> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
        }

        /// <summary>
        /// Asynchronous method to perform custom authorization logic.
        /// </summary>
        /// <param name="context">The context of the current action being executed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// - If the endpoint is marked with <see cref="AllowAnonymousAttribute"/>, the filter is bypassed.
        /// - If the user context is null, the request is denied with a 401 Unauthorized response.
        /// - Otherwise, the user context is set for the application using the provided <see cref="IUserProviderService"/>.
        /// </remarks>
        public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                this.logger.LogInformation("Skipping the filter and continue as Anonymous request");
                return;
            }

            this.logger.LogInformation("Authorizing user request to allow access to the application.");

            var user = await Task.FromResult(context.HttpContext.User.GetUserContext());

            if (user == null)
            {
                this.logger.LogError($"The user in token doesn't exist.");
                context.Result = new UnauthorizedObjectResult($"The user in token doesn't exist.");
                return;
            }

            this.userProvider.SetCurrentUser(user);
        }
    }
}

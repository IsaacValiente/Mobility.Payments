namespace Mobility.Payments.Application.Services
{
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;
    using System;

    /// <summary>
    /// Service for managing and providing the current user's context.
    /// </summary>
    public class UserContextProviderService : IUserProviderService, IUserContextProviderService
    {
        private readonly ILogger<UserContextProviderService> logger;
        private UserContext currentUser;

        public UserContextProviderService(ILogger<UserContextProviderService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool HasCurrentUser => this.currentUser != null;


        /// <summary>
        /// Retrieves the current user.
        /// </summary>
        /// <returns>The current <see cref="UserContext"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no current user is set.</exception>
        public UserContext GetCurrentUser()
        {
            if (!this.HasCurrentUser)
            {
                var errorMessage = "The current user is not set. Please set it before using it.";
                this.logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            this.logger.LogInformation("Returning current user '{username}'", this.currentUser.Username);
            return this.currentUser;
        }

        /// <summary>
        /// Retrieves the username of the current user.
        /// </summary>
        /// <returns>The current user's username.</returns>
        public string GetCurrentUserUsername()
        {
            if (!this.HasCurrentUser)
            {
                var warningMessage = "Current user is not set. Returning an empty string.";
                this.logger.LogWarning(warningMessage);
                return string.Empty;
            }

            this.logger.LogInformation("Returning current user username '{username}'", this.currentUser.Username);
            return this.currentUser.Username;
        }

        /// <summary>
        /// Sets the current user.
        /// </summary>
        /// <param name="user">The user context to set as the current user.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided user is null.</exception>
        public void SetCurrentUser(UserContext user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Cannot set the current user to null.");
            }

            this.logger.LogInformation("Setting current user '{username}'", user.Username);
            this.currentUser = user;
        }
    }
}

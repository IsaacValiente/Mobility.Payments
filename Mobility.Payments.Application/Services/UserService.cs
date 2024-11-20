namespace Mobility.Payments.Application.Services
{
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Crosscuting.Exception;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Domain.Repositories;
    using System;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenProviderService tokenProviderService;
        private readonly ILogger<UserService> logger;

        public UserService(IUserRepository userRepository, ITokenProviderService tokenProviderService, ILogger<UserService> logger)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.tokenProviderService = tokenProviderService ?? throw new ArgumentNullException(nameof(tokenProviderService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new user with the specified user details.
        /// </summary>
        /// <param name="userDto">User data transfer object containing user details.</param>
        /// <exception cref="BadRequestException">Thrown when the user already exists.</exception>
        public async Task CreateUserAsync(UserDto userDto)
        {
            this.logger.LogInformation("Attempting to create user with username: {UserName}", userDto.UserName);

            var existingUser = await this.userRepository.GetByUsernameAsync(userDto.UserName);
            if (existingUser != null)
            {
                var message = $"User with username '{userDto.UserName}' already exists.";
                this.logger.LogWarning(message);
                throw new BadRequestException(message);
            }

            var user = new User(userDto.UserName, userDto.Password, userDto.FirstName, userDto.LastName, userDto.UserRole);

            await this.userRepository.AddAsync(user);
            await this.userRepository.SaveChangesAsync(user);

            this.logger.LogInformation("User '{UserName}' created successfully.", userDto.UserName);
        }

        /// <summary>
        /// Logs in a user by verifying their credentials and returns an authentication token.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns>Authentication token if login is successful.</returns>
        /// <exception cref="UnauthorizedException">Thrown when credentials are incorrect.</exception>
        public async Task<string> Login(string username, string password)
        {
            this.logger.LogInformation("Attempting login for user: {UserName}", username);
            var user = await this.userRepository.GetByUsernameAsync(username);
            if (user is null || !user.VerifyPassword(password))
            {
                var message = "Invalid username or password.";
                this.logger.LogWarning(message);
                throw new UnauthorizedException(message);
            }

            var authToken = this.tokenProviderService.Create(user);
            this.logger.LogInformation("Login successful for user: {UserName}", username);

            return authToken;
        }

    }
}

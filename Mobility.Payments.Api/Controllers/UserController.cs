namespace Mobility.Payments.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Api.Contracts.Request;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;

    /// <summary>
    /// Controller for managing user-related operations, such as creating a user and logging in.
    /// </summary>
    [ApiController]
    [Route("user")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The service responsible for user operations.</param>
        /// <param name="logger">The logger for recording application events and messages.</param>
        /// <param name="mapper">The mapper for converting between models and DTOs.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the dependencies are null.</exception>
        public UserController(
            IUserService userService,
            ILogger<UserController> logger,
            IMapper mapper)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="userRequest">The user details provided in the request body.</param>
        /// <returns>A status indicating whether the user was successfully created.</returns>
        /// <response code="200">User created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userRequest)
        {
            this.logger.LogInformation("Receieved create user request for username {username}", userRequest.UserName);
            var userDto = this.mapper.Map<UserDto>(userRequest);
            await this.userService.CreateUserAsync(userDto);
            return this.Ok();
        }

        /// <summary>
        /// Authenticates a user and generates an authentication token.
        /// </summary>
        /// <param name="userLoginRequest">The login credentials provided in the request body.</param>
        /// <returns>The authentication token if login is successful.</returns>
        /// <response code="200">Login successful. Returns the authentication token.</response>
        /// <response code="401">Invalid login credentials.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest userLoginRequest)
        {
            this.logger.LogInformation("Receieved login request for user {username}", userLoginRequest.UserName);
            var authToken = await this.userService.Login(userLoginRequest.UserName, userLoginRequest.Password);
            return this.Ok(authToken);
        }
    }
}

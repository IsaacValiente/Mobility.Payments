namespace Mobility.Payments.Application.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Crosscuting.Options;
    using Mobility.Payments.Domain.Entities;
    using System;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// Service for generating JWT tokens for users.
    /// </summary>
    public class TokenProviderService : ITokenProviderService
    {
        private readonly JwtConfiguration jwtConfiguration;

        public TokenProviderService(IOptions<JwtConfiguration> options)
        {
            this.jwtConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Creates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is created.</param>
        /// <returns>A string representing the generated JWT token.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the user is null.</exception>
        public string Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtConfiguration.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(this.jwtConfiguration.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = this.jwtConfiguration.Issuer,
                Audience = this.jwtConfiguration.Audience,

            };

            var handler = new JsonWebTokenHandler();

            return handler.CreateToken(tokenDescriptor);
        }
    }
}

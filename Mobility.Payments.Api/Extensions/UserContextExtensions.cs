namespace Mobility.Payments.Api.Extensions
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Crosscuting.Enums;

    internal static class UserExtensions
    {
        internal static UserContext GetUserContext(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException($"{ClaimTypes.NameIdentifier} claim must be informed");
            var username = nameIdentifier.Value.ToString();
            return new UserContext
            {
                Username = username,
                UserRole = user.GetUserRole(),
            };
        }

        public static UserRole GetUserRole(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var roles = user.FindAll(ClaimTypes.Role);

            if (Enum.TryParse(roles.First().Value, out UserRole userRole))
            {
                return userRole;
            }

            throw new ArgumentNullException(nameof(user));
        }
    }
}

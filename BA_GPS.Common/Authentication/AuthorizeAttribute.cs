using System;
using Microsoft.AspNetCore.Authorization;

namespace BA_GPS.Common.Authentication
{
	public class AuthorizeAttribute : IAuthorizationRequirement
    {
		public byte UserType { get; }
		public AuthorizeAttribute(byte userType)
		{
			UserType = userType;
		}
	}

    public class UserTypeAuthorizationHandler : AuthorizationHandler<AuthorizeAttribute>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeAttribute requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "UserType"))
            {
                return Task.CompletedTask;
            }

            var userType = int.Parse(context.User.FindFirst(c => c.Type == "UserType").Value);

            if (userType == requirement.UserType)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}


using System;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   09/03/2024 Created
/// </Modified>
namespace BA_GPS.Common.Authentication
{
	//public class AuthorizeAttribute : IAuthorizationRequirement
 //   {
	//	public byte PermissionId { get; }
	//	public AuthorizeAttribute(byte permissionId)
	//	{
 //           PermissionId = permissionId;
	//	}
	//}

 //   public class UserTypeAuthorizationHandler : AuthorizationHandler<AuthorizeAttribute>
 //   {
 //       protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeAttribute requirement)
 //       {
 //           if (!context.User.HasClaim(c => c.Type == "PermissionId"))
 //           {
 //               return Task.CompletedTask;
 //           }

 //           var userType = int.Parse(context.User.FindFirst(c => c.Type == "PermissionId").Value);

 //           if (userType == requirement.PermissionId)
 //           {
 //               context.Succeed(requirement);
 //           }

 //           return Task.CompletedTask;
 //       }
 //   }
}


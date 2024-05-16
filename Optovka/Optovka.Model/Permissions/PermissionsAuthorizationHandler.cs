using Microsoft.AspNetCore.Authorization;

namespace Optovka.Model
{
    public class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
            if (requirement.RequiredPermissions.All(permission =>
                context.User.HasClaim("permission", permission)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

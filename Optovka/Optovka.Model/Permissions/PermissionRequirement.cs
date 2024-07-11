using Microsoft.AspNetCore.Authorization;

namespace Optovka.Model
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public string[] RequiredPermissions { get; }

        public PermissionsRequirement(params string[] requiredPermissions)
        {
            RequiredPermissions = requiredPermissions;
        }
    }
}
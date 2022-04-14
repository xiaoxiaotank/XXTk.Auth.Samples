using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.Permission.HttpApi.Authorizations
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly DataSeeds _dataSeeds;

        public PermissionAuthorizationHandler(DataSeeds dataSeeds)
        {
            _dataSeeds = dataSeeds;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!requirement.Permissions.Any())
            {
                context.Succeed(requirement);
            }

            var userPermissions = _dataSeeds.GetUserPermissions(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            foreach (var permission in requirement.Permissions)
            {
                if (userPermissions.Contains(permission))
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}

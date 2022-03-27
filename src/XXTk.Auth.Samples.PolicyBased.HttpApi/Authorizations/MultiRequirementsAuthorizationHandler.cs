using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    /// <summary>
    /// Handler : Requirement = 1 : n
    /// </summary>
    public class MultiRequirementsAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements;

            foreach (var requirement in pendingRequirements)
            {
                if (requirement is Custom1Requirement)
                {
                    // ... 一些校验

                    context.Succeed(requirement);
                }
                else if (requirement is Custom2Requirement)
                {
                    // ... 一些校验

                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public class Custom1Requirement : IAuthorizationRequirement
    {
    }

    public class Custom2Requirement : IAuthorizationRequirement
    {
    }
}

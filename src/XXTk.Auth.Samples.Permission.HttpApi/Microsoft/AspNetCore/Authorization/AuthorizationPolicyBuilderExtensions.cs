using System;
using XXTk.Auth.Samples.Permission.HttpApi.Authorizations;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequirePermissions(this AuthorizationPolicyBuilder builder, params string[] permissions)
        {
            if(builder is null) throw new ArgumentNullException(nameof(builder));

            return builder.AddRequirements(new PermissionRequirement(permissions));
        }
    }
}
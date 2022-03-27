using XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireMinimumAge(this AuthorizationPolicyBuilder builder, int minimumAge)
        {
            return builder.AddRequirements(new MinimumAgeRequirement(minimumAge));
        }
    }
}
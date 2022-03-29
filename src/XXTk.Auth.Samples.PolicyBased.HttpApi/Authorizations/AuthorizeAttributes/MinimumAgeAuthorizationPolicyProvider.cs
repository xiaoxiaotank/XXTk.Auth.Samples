using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    public class MinimumAgeAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public MinimumAgeAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(MinimumAgeAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase) 
                && int.TryParse(policyName[MinimumAgeAuthorizeAttribute.PolicyPrefix.Length..], out var age))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new MinimumAgeRequirement(age));
                return Task.FromResult(policy.Build());
            }

            return BackupPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return BackupPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return BackupPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    /// <summary>
    /// 注意：只能注册一个 IAuthorizationPolicyProvider，所以要将所有逻辑写到一个类中
    /// </summary>
    public class AppAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {       
        public AppAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        private ConcurrentDictionary<string, AuthorizationPolicy> PolicyMap { get; } = new(StringComparer.OrdinalIgnoreCase);

        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if(policyName is null) throw new ArgumentNullException(nameof(policyName));

            if(PolicyMap.TryGetValue(policyName, out var policy))
            {
                return Task.FromResult(policy);
            }

            if (policyName.StartsWith(MinimumAgeAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase) 
                && int.TryParse(policyName[MinimumAgeAuthorizeAttribute.PolicyPrefix.Length..], out var age))
            {
                var builder = new AuthorizationPolicyBuilder();
                builder.AddRequirements(new MinimumAgeRequirement(age));
                policy = builder.Build();
                PolicyMap[policyName] = policy;

                return Task.FromResult(policy);
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

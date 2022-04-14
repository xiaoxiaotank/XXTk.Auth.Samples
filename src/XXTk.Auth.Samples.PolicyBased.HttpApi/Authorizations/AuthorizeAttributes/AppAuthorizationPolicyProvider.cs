using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
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
        private static readonly AsyncLock _mutex = new();
        private readonly AuthorizationOptions _authorizationOptions;

        public AppAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            _authorizationOptions = options.Value;
        }

        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName is null) throw new ArgumentNullException(nameof(policyName));

            var policy = await BackupPolicyProvider.GetPolicyAsync(policyName);
            if (policy is not null)
            {
                return policy;
            }

            using (await _mutex.LockAsync())
            {
                policy = await BackupPolicyProvider.GetPolicyAsync(policyName);
                if (policy is not null)
                {
                    return policy;
                }

                if (policyName.StartsWith(MinimumAgeAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(policyName[MinimumAgeAuthorizeAttribute.PolicyPrefix.Length..], out var age))
                {
                    var builder = new AuthorizationPolicyBuilder();
                    builder.RequireMinimumAge(age);
                    policy = builder.Build();
                    _authorizationOptions.AddPolicy(policyName, policy);

                    return policy;
                }
            }

            return null;
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

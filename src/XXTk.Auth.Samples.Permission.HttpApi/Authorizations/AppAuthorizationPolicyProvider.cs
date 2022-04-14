using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.Permission.HttpApi.Authorizations
{
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

                if (PermissionAuthorizeAttribute.TryGetPermissions(policyName, out var permissions))
                {
                    var builder = new AuthorizationPolicyBuilder();
                    builder.RequirePermissions(permissions);
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

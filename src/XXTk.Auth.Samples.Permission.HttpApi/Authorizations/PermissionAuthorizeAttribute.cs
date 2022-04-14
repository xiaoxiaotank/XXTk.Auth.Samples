using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace XXTk.Auth.Samples.Permission.HttpApi.Authorizations
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PermissionSeparator = ",";
        public const string PolicyPrefix = "Permission:";

        public PermissionAuthorizeAttribute(params string[] permissions) =>
            Permissions = permissions ?? Array.Empty<string>();

        public string[] Permissions
        {
            get
            {
                return Policy[PolicyPrefix.Length..].Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                Policy = $"{PolicyPrefix}{string.Join(PermissionSeparator, value.OrderBy(p => p))}";
            }
        }

        public static bool TryGetPermissions(string policyName, out string[] permissions)
        {
            var result = false;
            permissions = null;

            if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                result = true;
                permissions = policyName[PolicyPrefix.Length..]
                    .Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
            }

            return result;
        }
    }
}

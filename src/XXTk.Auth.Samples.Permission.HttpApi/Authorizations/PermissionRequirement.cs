using Microsoft.AspNetCore.Authorization;
using System;

namespace XXTk.Auth.Samples.Permission.HttpApi.Authorizations
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(params string[] permissions) =>
            Permissions = permissions ?? Array.Empty<string>();

        public string[] Permissions { get; set; }
    }
}

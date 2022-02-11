using System;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Authentication
{
    public class AuthUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public DateTime? LastModifyPasswordTime { get; set; }
    }
}

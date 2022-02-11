using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Authentication.JwtBearer
{
    public class AppJwtBearerPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly IDistributedCache _distributedCache;

        public AppJwtBearerPostConfigureOptions(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new AppJwtSecurityTokenHandler(_distributedCache));
            }
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Authentication.JwtBearer
{
    public class AppJwtBearerEvents : JwtBearerEvents
    {
        public override Task MessageReceived(MessageReceivedContext context)
        {
            if (context.Request.Cookies.TryGetValue(HeaderNames.Authorization, out var token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 质询
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            // 添加标记，使前端知晓access token过期，可以使用refresh token了
            if (context.AuthenticateFailure is SecurityTokenExpiredException)
            {
                context.Response.Headers.Add("x-access-token", "expired");
            }

            return Task.CompletedTask;
        }
    }
}

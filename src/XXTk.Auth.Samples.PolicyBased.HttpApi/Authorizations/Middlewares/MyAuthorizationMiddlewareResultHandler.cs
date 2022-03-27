using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    /// <summary>
    /// 自定义授权中间件（实现逻辑与默认的 AuthorizationMiddlewareResultHandler 实现逻辑一致）
    /// </summary>
    public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        /// <inheritdoc />
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await context.ChallengeAsync();
                }

                return;
            }
            else if (authorizeResult.Forbidden)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ForbidAsync(scheme);
                    }
                }
                else
                {
                    await context.ForbidAsync();
                }

                return;
            }

            await next(context);
        }
    }
}

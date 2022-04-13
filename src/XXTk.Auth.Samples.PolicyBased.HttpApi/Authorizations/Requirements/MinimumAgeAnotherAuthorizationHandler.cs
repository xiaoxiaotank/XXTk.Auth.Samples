using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    /// <summary>
    /// Handler : Requirement = 1 : 1
    /// 
    /// 与 MinimumAgeAuthorizationHandler 是 Or 的关系，即两者其中有一个成功即可
    /// </summary>
    public class MinimumAgeAnotherAuthorizationHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var isBoss = context.User.IsInRole("InternetBarBoss");

            if (isBoss)
            {
                context.Succeed(requirement);
            }

            // 如果该校验失败了即视为授权失败，则使用Fail，但如果虽然该校验失败了，但还可以进行管道中后面Handler的授权校验，则不要使用Fail
            //context.Fail();

            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    /// <summary>
    /// Handler : Requirement = 1 : 1
    /// 
    /// 与 MinimumAgeAnotherAuthorizationHandler 是 Or 的关系，即两者其中有一个成功即可
    /// </summary>
    public class MinimumAgeAuthorizationHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            #region 其他说明
            // 说明当前通过终结点路由访问
            if (context.Resource is HttpContext httpContext)
            {
                var endpoint = httpContext.GetEndpoint();
                var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            }
            // 说明当前通过传统mvc路由访问
            else if (context.Resource is AuthorizationFilterContext mvcContext)
            {

            }
            #endregion

            // 这里也可以从数据库中获取生日信息
            var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);

            if (dateOfBirthClaim is null)
            {
                return Task.CompletedTask;
            }

            var today = DateTime.Today;
            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
            int calculatedAge = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            // 如果该校验失败了即视为授权失败，则使用Fail，但如果虽然该校验失败了，但还可以进行管道中后面Handler的授权校验，则不要使用Fail
            //context.Fail();

            return Task.CompletedTask;
        }
    }
}

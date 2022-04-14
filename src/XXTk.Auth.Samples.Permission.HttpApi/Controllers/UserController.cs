using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XXTk.Auth.Samples.Permission.HttpApi.Authorizations;

namespace XXTk.Auth.Samples.Permission.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [PermissionAuthorize(AppPermissions.User.Default)]
        public string Get() => "Get";

        [HttpPost]
        [PermissionAuthorize(AppPermissions.User.Create)]
        public string Create() => "Create";

        [HttpPut]
        [PermissionAuthorize(AppPermissions.User.Update)]
        public string Update() => "Update";

        [HttpDelete]
        [PermissionAuthorize(AppPermissions.User.Delete)]
        public string Delete() => "Delete";

        [HttpPost("CreateOrUpdate")]
        [PermissionAuthorize(AppPermissions.User.Create, AppPermissions.User.Update)]
        public string CreateOrUpdate() => "CreateOrUpdate";

        [HttpPost("CreateAndDelete")]
        [PermissionAuthorize(AppPermissions.User.Create)]
        [PermissionAuthorize(AppPermissions.User.Delete)]
        public string CreateAndDelete() => "CreateAndDelete";
    }
}

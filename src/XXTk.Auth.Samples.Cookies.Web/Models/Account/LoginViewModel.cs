using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XXTk.Auth.Samples.Cookies.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}

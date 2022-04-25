using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreJwtAuthorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public class LoginModel
        {
            [Display(Name = "用户名")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string? UserName { get; set; }
            [Display(Name = "密码")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string? Password { get; set; }
        }
        [HttpPost]
        public IActionResult Post([FromBody] LoginModel loginModel)
        {
            if (loginModel.UserName != "linyisonger" || loginModel.Password != "123456")
            {
                ModelState.AddModelError(nameof(loginModel.UserName), "用户名或密码错误!");
                return ValidationProblem(ModelState);
            }
            return new JsonResult(new
            {
                loginModel.UserName,
                token = new JwtSecurityTokenHandler().WriteToken(JwtServices.GetToken(new Claim[] { new Claim(nameof(loginModel.UserName), loginModel.UserName) }))
            });
        }
    }
}

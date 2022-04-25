## AspNetCoreJwtAuthorize

#### 环境

- .Net Core 6.0

#### 工具

- Visual Studio 2022

#### 流程

##### 新建项目

我这里是这样新建的，当然也可以使用Visual Studio中的可视化新建。

``` shell
dotnet new webapi --name AspNetCoreJwtAuthorize
```

##### 添加依赖

编辑```.csproj```文件

``` xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="6.0.3" />
  </ItemGroup>

</Project>
```

##### 新增Jwt服务类

新建`JwtServices.cs`文件

关于JwtSettings中的值，根据自己所需进行配置~

``` csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreJwtAuthorize
{
    public static class JwtServices
    {
        class JwtSettings
        {
            /// <summary>
            /// 颁发者
            /// </summary>
            public string Issuer { get; set; } = "http://localhost:5000";
            /// <summary>
            /// 使用者
            /// </summary>
            public string Audience { get; set; } = "http://localhost:5000";
            /// <summary>
            /// 密钥
            /// </summary>
            public string SecretKey { get; set; } = "xxxxxxxxxxxxxxxxxxxxxxx";
        }

        static readonly JwtSettings jwtSettings = new();

        public static JwtSecurityToken GetToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // 我这里设置的有效时常为1分钟便于测试
            return new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(1), creds);
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer, // 颁发者
                    ValidAudience = jwtSettings.Audience, // 使用者
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)), // 颁发者密钥
                    ValidateIssuerSigningKey = true, //  验证颁发者密钥
                    ValidateIssuer = true,  // 验证颁发者
                    ValidateLifetime = true,  // 验证使用时限 
                    ClockSkew = TimeSpan.FromSeconds(30) // 缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                };
            });
            return services;
        }
    }
}

```

##### 修改Program类

``` csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetCoreJwtAuthorize;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// 添加jwt服务
builder.Services.AddJwtAuthentication();

var app = builder.Build();
// 增加默认验证中间件
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

##### 新增两个接口进行测试

新增`LoginController.cs`文件

``` csharp
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
            public string UserName { get; set; }
            [Display(Name = "密码")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string Password { get; set; }
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
```

新增`ValuesController.cs`文件

``` csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreJwtAuthorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
```

##### 验证

打开期待已久的postman

- [x] 没有令牌请求需要授权接口401

![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220411165609691-1498108294.png)



- [x] 请求登录返回令牌

![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220411165806839-1491112573.png)

- [x] 拿着令牌去请求需要授权接口

![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220411170046877-2143302700.png)

知识点 +1


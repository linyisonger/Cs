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

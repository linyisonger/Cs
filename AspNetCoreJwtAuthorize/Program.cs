using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetCoreJwtAuthorize;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// ���jwt����
builder.Services.AddJwtAuthentication();

var app = builder.Build();
// ����Ĭ����֤�м��
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

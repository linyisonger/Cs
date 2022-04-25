using AspNetCoreWeChatCode2Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<WeChat>(new WeChat("app", "secret"));

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//                builder =>
//                {
//                    builder
//                    .AllowAnyHeader()
//                    .AllowAnyMethod()
//                    .AllowAnyOrigin();
//                });
//});

var app = builder.Build();

//app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

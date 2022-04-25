using AspNetCoreWeChatSubscribeMessage;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

builder.Services.AddSingleton<WeChat>(new WeChat("appid", "secret"));

var app = builder.Build();

//app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

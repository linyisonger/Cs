using AspNetCoreConnectionMySQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// 连接数据库操作
var connectionString = "Data Source=localhost ; Database= test; User ID= root ; Password=root; pooling=true ; port= 3306; sslmode=none ; CharSet =utf8;allowPublicKeyRetrieval=true;";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
builder.Services.AddDbContext<TestDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();

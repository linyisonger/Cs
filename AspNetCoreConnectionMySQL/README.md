## Asp .NetCore 使用EFCore连接MySQL数据库

> 自己要先看得起自己，别人才会看得起你。😉

#### 环境

- .Net Core 6.0

#### 工具

- Visual Studio 2022

#### 流程

##### 新建项目

我这里是这样新建的，当然也可以使用Visual Studio中的可视化新建。

``` shell
dotnet new webapi --name AspNetCoreConnectionMySQL
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
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="6.0.1" />
  </ItemGroup>

</Project>
```

##### 添加DBContext

我这里新建了一个TestDbContext.cs并且新增了一个TestDbContext类和User用户类，用于之后见表使用。

``` csharp
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreConnectionMySQL
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }
}
```

##### 修改Program.cs文件

增加连接数据库的字符串，数据库的版本以及注册在服务中。

``` csharp
using AspNetCoreConnectionMySQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 连接数据库操作
var connectionString = "Data Source= localhost ; Database= test; User ID= root ; Password=root; pooling=true ; port= 3306; sslmode=none ; CharSet =utf8;allowPublicKeyRetrieval=true;";
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
```

##### 打开程序包管理器控制台  <kbd>Ctrl</kbd> + <kbd>`</kbd>

执行安装命令依赖包

``` shell
Install-Package Microsoft.EntityFrameworkCore.Tools
```

添加一个迁移，我这里的名成叫做init

``` shell
add-migration init
```

更新数据库

``` shell
update-database
```

如果以上都很顺利的话~ 

连接数据库已经完成啦

#### 测试

新增一个控制器

``` csharp
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreConnectionMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly TestDbContext _context;
        public HomeController(TestDbContext context)
        {
            _context = context;
        }

        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _context.Users.ToList();
        }

        // GET api/<HomeController>/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _context.Users.Find(id);
        }

        // POST api/<HomeController>
        [HttpPost]
        public void Post([FromBody] User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Users.Remove(Get(id));
            _context.SaveChanges();
        }
    }
}
```

![image](https://img2022.cnblogs.com/blog/1415778/202203/1415778-20220324172710838-1209228645.png)

![image](https://img2022.cnblogs.com/blog/1415778/202203/1415778-20220324172756295-2081338366.png)

![image](https://img2022.cnblogs.com/blog/1415778/202203/1415778-20220324172829501-1548451338.png)





完美~


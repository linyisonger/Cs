### Asp .NetCore 微信小程序授权登录、获取用户信息

> 官方文档
>
> https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/login/auth.code2Session.html

#### 环境

- .Net Core 6.0

#### 工具

- Visual Studio 2022
- 微信开发者工具

#### 流程

##### 新建项目

我这里是这样新建的，当然也可以使用Visual Studio中的可视化新建。

``` shell
dotnet new webapi --name AspNetCoreWeChatCode2Session
```

##### 添加依赖

编辑```.csproj```文件。

``` xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
  </ItemGroup>

</Project>
```

##### 新增WeChat类进行封装

新建WeChat.cs文件。

``` csharp
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCoreWeChatCode2Session
{

    public class WeChat
    {
        /// <summary>
        /// 小程序 appId
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 小程序 appSecret
        /// </summary>
        public string secret { get; set; }

        public class Code2SessionParamter
        {
            /// <summary>
            /// 小程序 appId
            /// </summary>
            public string appid { get; set; }
            /// <summary>
            /// 小程序 appSecret
            /// </summary>
            public string secret { get; set; }
            /// <summary>
            /// 登录时获取的 code
            /// </summary>
            public string js_code { get; set; }
            /// <summary>
            ///  授权类型，此处只需填写 authorization_code
            /// </summary>
            public string grant_type { get; set; } = "authorization_code";

            public Code2SessionParamter(string js_code)
            {
                this.js_code = js_code;
            }

            public Code2SessionParamter(string appid, string secret, string js_code)
            {
                this.appid = appid;
                this.secret = secret;
                this.js_code = js_code;
            }
        }
        public class Code2SessionResult
        {
            /// <summary>
            /// 用户唯一标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 会话密钥
            /// </summary>
            public string session_key { get; set; }

            /// <summary>
            /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台帐号下会返回，详见 UnionID 机制说明。
            /// </summary>
            public string unionid { get; set; }
            /// <summary>
            /// 错误码
            /// </summary>
            public int errcode { get; set; }
            /// <summary>
            ///  错误信息
            /// </summary>
            public string errmsg { get; set; }
        }

        public enum Gender
        {
            unkown = 0,
            man = 1,
            woman = 2
        }
        public class UserInfo
        {
            public string openId { get; set; }
            /// <summary>
            /// 用户昵称
            /// </summary>
            public string nickName { get; set; }
            /// <summary>
            /// 用户性别
            /// </summary>
            public Gender gender { get; set; }
            /// <summary>
            /// 用户所在国家
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 用户所在省份。
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 用户所在城市。
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台帐号下会返回，详见 UnionID 机制说明。
            /// </summary>
            public string unionId { get; set; }
            /// <summary>
            /// 用户头像图片的 URL。URL 最后一个数值代表正方形头像大小（有 0、46、64、96、132 数值可选，0 代表 640x640 的正方形头像，46 表示 46x46 的正方形头像，剩余数值以此类推。默认132），用户没有头像时该项为空。若用户更换头像，原有头像 URL 将失效。
            /// </summary>
            public string avatarUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Watermark watermark { get; set; }
        }
        public class Watermark
        {
            /// <summary>
            /// 敏感数据归属 appId，开发者可校验此参数与自身 appId 是否一致
            /// </summary>
            public string appid { get; set; }
            /// <summary>
            /// 敏感数据获取的时间戳, 开发者可以用于数据时效性校验
            /// </summary>
            public string timestamp { get; set; }
        }

        public WeChat(string appid, string secret)
        {
            this.appid = appid;
            this.secret = secret;
        }

        /// <summary>
        /// 登录凭证校验。通过 wx.login 接口获得临时登录凭证 code 后传到开发者服务器调用此接口完成登录流程。更多使用方法详见 小程序登录。
        /// https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/login.html
        /// </summary>
        /// <param name="code2SessionParamter">参数</param>
        /// <returns></returns>
        public async Task<Code2SessionResult?> GetCode2Session(Code2SessionParamter code2SessionParamter)
        {
            if (string.IsNullOrEmpty(code2SessionParamter.appid)) code2SessionParamter.appid = appid;
            if (string.IsNullOrEmpty(code2SessionParamter.secret)) code2SessionParamter.secret = secret;
            var result = await Get($"https://api.weixin.qq.com/sns/jscode2session?appid={code2SessionParamter.appid}&secret={code2SessionParamter.secret}&js_code={code2SessionParamter.js_code}&grant_type={code2SessionParamter.grant_type}");
            return JsonConvert.DeserializeObject<Code2SessionResult>(result);
        }

        /// <summary>
        /// 内部使用的通用方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static async Task<string> Get(string url)
        {
            try
            {
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : ""; ;
            }
            catch (Exception ex)
            {
                throw new Exception("Get 请求出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 解密获取用户信息 (不验证签名)
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="session_key">会话密钥</param> 
        /// <returns></returns>
        public UserInfo? GetUserInfo(string iv, string encryptedData, string session_key)
        {
            return JsonConvert.DeserializeObject<UserInfo>(AESDecrypt(encryptedData, session_key, iv));
        }
        /// <summary>
        /// 解密获取用户信息 (验证签名)
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="session_key">会话密钥</param>
        /// <param name="rawData">不包括敏感信息的原始数据字符串，用于计算签名</param>
        /// <param name="signature">使用 sha1( rawData + sessionkey ) 得到字符串，用于校验用户信息</param>
        /// <returns></returns>
        public UserInfo? GetUserInfo(string iv, string encryptedData, string session_key, string rawData, string signature)
        {
            CheckSignature(rawData, session_key, signature);
            return GetUserInfo(iv, encryptedData, session_key);
        }


        /// <summary>
        /// 检查签名
        /// </summary>
        /// <param name="rawData">不包括敏感信息的原始数据字符串，用于计算签名</param>
        /// <param name="session_key"></param>
        /// <param name="signature">使用 sha1( rawData + sessionkey ) 得到字符串，用于校验用户信息</param>
        /// <exception cref="Exception"></exception>
        void CheckSignature(string rawData, string session_key, string signature)
        {
            Console.WriteLine(SHA1Encryption(rawData + session_key));
            Console.WriteLine(signature);
            if (SHA1Encryption(rawData + session_key).ToUpper() != signature.ToUpper())
            {
                throw new Exception("CheckSignature 签名校验失败，数据可能损坏。");
            }
        }


        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        string SHA1Encryption(string content, Encoding encode = null)
        {
            try
            {
                if (encode == null) encode = Encoding.UTF8;
                SHA1 sha1 = SHA1.Create();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1Encryption加密出错：" + ex.Message);
            }
        }
        /// <summary>
        /// Aes 解密
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="sessionKey"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        string AESDecrypt(string encryptedData, string sessionKey, string iv)
        {
            try
            {
                var encryptedDataByte = Convert.FromBase64String(encryptedData);
                var aes = Aes.Create();
                aes.Key = Convert.FromBase64String(sessionKey);
                aes.IV = Convert.FromBase64String(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var transform = aes.CreateDecryptor();
                var plainText = transform.TransformFinalBlock(encryptedDataByte, 0, encryptedDataByte.Length);
                var result = Encoding.Default.GetString(plainText);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("AESDecrypt解密出错：" + ex.Message);
            }
        }
    }
}
```

##### 编辑Program文件

修改Program.cs文件。

``` csharp
using AspNetCoreWeChatCode2Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// 添加WeChat单例服务
builder.Services.AddSingleton<WeChat>(new WeChat("appid", "secret"));
 
var app = builder.Build();  
app.UseAuthorization(); 
app.MapControllers(); 
app.Run();
```

##### 获取appid和secret

1. 进入微信公众平台 https://mp.weixin.qq.com/ 。

2. 进入开发设置中获取。

   ![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220413191739742-2136830776.png)

##### 新增接口进行测试

新建LoginController.cs文件放在Controllers文件夹中。

```csharp
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreWeChatCode2Session.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly WeChat _weChat;

        static string session_key = "";
        public LoginController(WeChat weChat)
        {
            _weChat = weChat;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            var res = await _weChat.GetCode2Session(new WeChat.Code2SessionParamter(code));
            session_key = res.session_key;
            // 注意: 这里是为了方便演示，开发的时候不应该去传给前端。
            // 为了数据不被篡改，开发者不应该把 session_key 传到小程序客户端等服务器外的环境。
            return new JsonResult(res);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginParameter parameter)
        {
            if (!string.IsNullOrEmpty(parameter.code)) await Get(parameter.code);
            return new JsonResult(_weChat.GetUserInfo(parameter.iv, parameter.encryptedData, session_key, parameter.rawData, parameter.signature));
        }

        public class LoginParameter
        {
            /// <summary>
            /// 用于解析session_key
            /// </summary>
            public string? code { get; set; }
            /// <summary>
            /// 不包括敏感信息的原始数据字符串，用于计算签名
            /// </summary>
            public string rawData { get; set; }

            /// <summary>
            /// 包括敏感数据在内的完整用户信息的加密数据
            /// </summary>
            public string encryptedData { get; set; }
            /// <summary>
            /// 加密算法的初始向量
            /// </summary>
            public string iv { get; set; }
            /// <summary>
            /// 使用 sha1( rawData + sessionkey ) 得到字符串，用于校验用户信息
            /// </summary>
            public string signature { get; set; }
        }
    }
}
```

##### 新建小程序

![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220413192055830-1249537236.png)

##### 修改app.ts

在app.ts中调用获取openid接口

``` typescript
// app.ts
App<IAppOption>({
  globalData: {},
  onLaunch() {
    wx.hideHomeButton();
    wx.login({
      success: (res) => {
        wx.request({
          url: `http://localhost:5000/api/login/${res.code}`,
          method: "GET",
          success: (res) => {
            console.log(res);
          }
        })
      }
    })
  },
})
```

##### 新建test页面

index.wxml

```html
<view bindtap="getUserInfo">获取用户信息</view>
```

index.ts

``` typescript
let checkSession = (): Promise<WechatMiniprogram.GeneralCallbackResult> => {
  return new Promise((resolve, reject) => {
    wx.checkSession({
      success: resolve,
      fail: reject
    })
  })
}
let getUserProfile = (): Promise<WechatMiniprogram.GetUserProfileSuccessCallbackResult & { iv: string, signature: string, encryptedData: string, rawData: string }> => {
  return new Promise((resolve, reject) => {
    wx.getUserProfile({
      desc: "用于更好的展示。",
      success: (res: any) => resolve(res),
      fail: reject
    })
  })
}
let login = (): Promise<WechatMiniprogram.LoginSuccessCallbackResult> => {
  return new Promise((resolve, reject) => {
    wx.login({
      success: resolve,
      fail: reject
    })
  })
}
Page({
  data: {
    code: '',
  },
  async onLoad() {
    try {
      await checkSession();
    } catch (error) {
      this.setData({ code: (await login()).code })
    }
  },
  async getUserInfo() {
    let code = this.data.code;
    let userRes = await getUserProfile();
    let data = {
      encryptedData: userRes.encryptedData,
      iv: userRes.iv,
      signature: userRes.signature,
      rawData: userRes.rawData,
      code,
    }
    wx.request({
      url: `http://localhost:5000/api/login`,
      method: 'POST',
      data,
      success: async (res) => {
        if (res.statusCode != 200) {
          console.log("error", res);
          this.setData({ code: (await login()).code })
        }
        else {
          // 防止code重复使用
          this.setData({ code: "" })
          console.log("success", res);
        }
      },
      fail: async (err) => {
        console.log("fail", err);
        this.setData({ code: (await login()).code })
      }
    })
    console.log(userRes);
  }
})
```

##### 稍微测试一下

![image](https://img2022.cnblogs.com/blog/1415778/202204/1415778-20220413193009634-11014319.png)

没啥大问题~
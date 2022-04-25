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
            session_key = res?.session_key ?? "";
            // 注意: 这里是为了方便演示，开发的时候不应该去传给前端。
            // 为了数据不被篡改，开发者不应该把 session_key 传到小程序客户端等服务器外的环境。
            return new JsonResult(res);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginParameter parameter)
        {
            if (!string.IsNullOrEmpty(parameter.code)) await Get(parameter.code);
            return new JsonResult(_weChat.GetUserInfo(parameter.iv ?? "", parameter.encryptedData ?? "", session_key, parameter.rawData ?? "", parameter.signature ?? ""));
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
            public string? rawData { get; set; }

            /// <summary>
            /// 包括敏感数据在内的完整用户信息的加密数据
            /// </summary>
            public string? encryptedData { get; set; }
            /// <summary>
            /// 加密算法的初始向量
            /// </summary>
            public string? iv { get; set; }
            /// <summary>
            /// 使用 sha1( rawData + sessionkey ) 得到字符串，用于校验用户信息
            /// </summary>
            public string? signature { get; set; }
        }
    }
}

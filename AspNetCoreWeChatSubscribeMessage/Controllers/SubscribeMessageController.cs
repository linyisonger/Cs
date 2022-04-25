using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreWeChatSubscribeMessage.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscribeMessageController : ControllerBase
    {
        readonly WeChat _weChat;
        static string access_token = "";
        public SubscribeMessageController(WeChat weChat)
        {
            _weChat = weChat;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetOpenId(string code)
        {
            var res = await _weChat.GetCode2Session(code);
            // 注意: 这里是为了方便演示，开发的时候不应该去传给前端。
            // 为了数据不被篡改，开发者不应该把 session_key 传到小程序客户端等服务器外的环境。
            return new JsonResult(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetAccessToken()
        {
            var result = await _weChat.GetAccessToken();
            // 注意: 这里是为了方便演示，开发的时候不应该去传给前端。
            // 建议使用redis，将access_token存储起来
            access_token = result?.access_token ?? "";
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            return new JsonResult(await _weChat.GetCategory(access_token));
        }
        [HttpGet]
        public async Task<IActionResult> GetTemplateList()
        {
            return new JsonResult(await _weChat.GetTemplateList(access_token));
        }
        [HttpGet]
        public async Task<IActionResult> GetPubTemplateTitleList([FromQuery] string ids, [FromQuery] long start)
        {
            return new JsonResult(await _weChat.GetPubTemplateTitleList(access_token, ids, start));
        }
        [HttpGet]
        public async Task<IActionResult> GetPubTemplateKeyWordsById([FromQuery] string tid)
        {
            return new JsonResult(await _weChat.GetPubTemplateKeyWordsById(access_token, tid));
        }
        [HttpPost]
        public async Task<IActionResult> AddTemplate([FromBody] WeChat.AddTemplateParamter paramter)
        {
            return new JsonResult(await _weChat.AddTemplate(access_token, paramter));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTemplate([FromQuery] string priTmplId)
        {
            return new JsonResult(await _weChat.DeleteTemplate(access_token, priTmplId));
        }
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] WeChat.SubscribeMessageSendParamter paramter)
        {
            return new JsonResult(await _weChat.Send(access_token, paramter));
        }
    }
}

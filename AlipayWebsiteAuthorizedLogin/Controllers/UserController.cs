using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlipayWebsiteAuthorizedLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        const string app_id = "2021003124668446";
        const string merchant_private_key = "MIIEpAIBAAKCAQEAwS096M9ywQJlWAD7LKJV65oRaduz4pAX0J6z6C/QbIPBduo/+O1fxpLAAMqy2kxvqbNzMa7/HzbishI/CMR/6ZuTXTZlRoTeHHJlqfDXB9RXkuAaMS5mNke4i8cnNlv0zWmU/V8tHKot0F51iPDgajqrPGk1ojLjL+TzUMgCEJCvpn96Vec9XFgk6dzfT/evMc1IlV6vNvnl0guGwaPljdiWgB6D+rZQP+yP1pVFXTexzsM2fKOqfM/qVag1vAyMDo+QshcnufYMecjqDTpljBRtFv8cOkNjc4QMRmiGvkY4/a5WOA1v3+q1SXb1KdjQdg0kdj/AQMtfFYarHbyNNQIDAQABAoIBAGK8Vg1dosCmEdiGP9tTaekPCWF66xmdHp8BzuCZN8WPHl5CYso/wZlEPqDo+bt8IAKz9ufpEvsIWZw8mT1I+jwTyaF2AROnK8OMcQwITk/xJ4Wpx5llptaNl/TRqOgu3hPzqDRM5kjlbp82+Ioy2/FP5V/uQnyR1+8N7ZNXdz6xYRb2mdi1gq9qdcHq+TZ/Gv3qvJkh3V8ZkrV4NCyYHVwmQp1Q2i9gsvjlwZ99pPLXbLyOOYNb5ufnP9nBEMloivN5e3lOlMYvEE8MiOZzyKtjT+bsSUfK0Ompw8sm3Wr2TvBi2/vB2rZKQlodUzqNkz60X7UYlODxfYgC91PYNE0CgYEA8PDZ2htSBTXKwdvmCAlFnzpcev7oobiJEZfQ6KWwnQi5Jm1BCt7M7hb0yJcHA1Os6AEsWGaepAfW5yvCdsEUBGRZbp3Qe/QbExjGWQf7mIb5UYkfJcCPwnznaRzesSGQszlpF/LimFuwplhywUtRsNj5EVxJxHPtHultpRK2Za8CgYEAzUAlcp+yn+Pe3qNcg/JX1tbBfvxioxCoMUwVHG7mSKXdttyN24rX03SGAimHLXuiQQC/lBZMmkWDAbwHpNbnSSgt12f70ekedgjVFDA6oIwMmZmJLFyk7Uj6L3alXawmIpmqS/++jVJ/VxNwyruOrvYbyGCZGtMf4XCfwA4CGFsCgYEA5R0jDDROmoW8ePwSkjG+8tscdhlQPfis3v3uukxU1f8lkVTzNNMJLJ7HxrdXA+Ld2QDEmot676BEyy7hjOutjT+fi4CmcDiARniYpLDinvg6vT4XUF78VYVQyLObT/ULFGLTfOKKdzc5k/Unqodk9zZcIhYQT6+tHGf8y6wvpiECgYEAl17gf0mRxeObQVz/ZPw8leAqMgnWgOZO9JRk2WRch2V0P2EnaatbAsLj9gJEhGGyxvfTUlajxf7P4F6Y0JQBXO256SvyBtxDL8/RvaLYCgFfZYRbKxFkPO9eO9Tnnk7QjSIA+y2wja8y5Lgcrhdm4lf1I3FCWQstbujNmCl8mBMCgYBxH8B5TR//rS7tC+EMZsk6htwnNnnOdhisvckvWVzJHAsHT3MxsNoCJGiifgo/tixhwJpVZak4j2t544K8Rx1ph/qL1bJDxEXUQ5WcYdstnXQGRaIIpeAagKM7cs1KpokLNcRBksBYh9mAPLq7yqdd7sONmKRbFDc+G3uh5qgsyQ==";
        const string alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApK0cSYMjk0H9tQzCUPH3vJeZolux5rnCJLW2n2PhTqTLuh+mGPs0Qm4fQ8olte3sCSC8BtJImB3OAdpiu7OR7mGTEofufUdX6uAAXmh0vW97zG9q808EDZUxwhTfpuZaJLuHf1/AyLaZvevudaHgCUv4e+Jjcs7FEtLBudkUMFjpsEhhI+eqxFTzjCqE0mLxcRPhjTWclrP3dZIykzXF77HrN/yftOmxu8qUTAmfoBRL3dGMSC8kAJFUrE0TbQgZ73n55o7NG70myXjdRaY+vZoyCAdG/wtV08ElzJEx73D3nuzW3ZGZDZreHfMI4o7BlH93HeUf0jOY/OLBDeC2AQIDAQAB";

        [HttpPost(nameof(Login))]
        public ActionResult<LoginResult> Login([FromBody] LoginParameters parameters)
        {
            IAopClient client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", app_id, merchant_private_key, "json", "1.0", "RSA2", alipay_public_key, "UTF-8", false);
            // 获取AccessToken
            AlipaySystemOauthTokenRequest oauthTokenRequest = new AlipaySystemOauthTokenRequest();
            oauthTokenRequest.GrantType = "authorization_code";
            oauthTokenRequest.Code = parameters.Code;
            AlipaySystemOauthTokenResponse oauthTokenResponse = client.Execute(oauthTokenRequest);
            if (oauthTokenResponse.IsError)
            {
                Console.WriteLine("获取AccessToken失败!", oauthTokenResponse.Msg);
                throw new Exception(oauthTokenResponse.Msg);
            } 
            return new LoginResult
            {
                AccessToken = oauthTokenResponse.AccessToken,
                UserId = oauthTokenResponse.UserId
            };
        }
    }

    public class LoginParameters
    {
        public string Code { get; set; }
    }

    public class LoginResult
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
    }
}

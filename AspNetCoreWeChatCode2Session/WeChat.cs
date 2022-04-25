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
            public string? appid { get; set; }
            /// <summary>
            /// 小程序 appSecret
            /// </summary>
            public string? secret { get; set; }
            /// <summary>
            /// 登录时获取的 code
            /// </summary>
            public string? js_code { get; set; }
            /// <summary>
            ///  授权类型，此处只需填写 authorization_code
            /// </summary>
            public string? grant_type { get; set; } = "authorization_code";

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
            public string? openid { get; set; }
            /// <summary>
            /// 会话密钥
            /// </summary>
            public string? session_key { get; set; }

            /// <summary>
            /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台帐号下会返回，详见 UnionID 机制说明。
            /// </summary>
            public string? unionid { get; set; }
            /// <summary>
            /// 错误码
            /// </summary>
            public int errcode { get; set; }
            /// <summary>
            ///  错误信息
            /// </summary>
            public string? errmsg { get; set; }
        }

        public enum Gender
        {
            unkown = 0,
            man = 1,
            woman = 2
        }
        public class UserInfo
        {
            public string? openId { get; set; }
            /// <summary>
            /// 用户昵称
            /// </summary>
            public string? nickName { get; set; }
            /// <summary>
            /// 用户性别
            /// </summary>
            public Gender gender { get; set; }
            /// <summary>
            /// 用户所在国家
            /// </summary>
            public string? country { get; set; }
            /// <summary>
            /// 用户所在省份。
            /// </summary>
            public string? province { get; set; }
            /// <summary>
            /// 用户所在城市。
            /// </summary>
            public string? city { get; set; }
            /// <summary>
            /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台帐号下会返回，详见 UnionID 机制说明。
            /// </summary>
            public string? unionId { get; set; }
            /// <summary>
            /// 用户头像图片的 URL。URL 最后一个数值代表正方形头像大小（有 0、46、64、96、132 数值可选，0 代表 640x640 的正方形头像，46 表示 46x46 的正方形头像，剩余数值以此类推。默认132），用户没有头像时该项为空。若用户更换头像，原有头像 URL 将失效。
            /// </summary>
            public string? avatarUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Watermark? watermark { get; set; }
        }
        public class Watermark
        {
            /// <summary>
            /// 敏感数据归属 appId，开发者可校验此参数与自身 appId 是否一致
            /// </summary>
            public string? appid { get; set; }
            /// <summary>
            /// 敏感数据获取的时间戳, 开发者可以用于数据时效性校验
            /// </summary>
            public string? timestamp { get; set; }
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
        string SHA1Encryption(string content, Encoding? encode = null)
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

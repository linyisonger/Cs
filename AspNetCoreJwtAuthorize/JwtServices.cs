using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreJwtAuthorize
{
    public static class JwtServices
    {
        class JwtSettings
        {
            /// <summary>
            /// 颁发者
            /// </summary>
            public string Issuer { get; set; } = "http://localhost:5000";
            /// <summary>
            /// 使用者
            /// </summary>
            public string Audience { get; set; } = "http://localhost:5000";
            /// <summary>
            /// 密钥
            /// </summary>
            public string SecretKey { get; set; } = "xxxxxxxxxxxxxxxxxxxxxxx";
        }

        static readonly JwtSettings jwtSettings = new();

        public static JwtSecurityToken GetToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // 我这里设置的有效时常为1分钟便于测试
            return new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(1), creds);
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer, // 颁发者
                    ValidAudience = jwtSettings.Audience, // 使用者
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)), // 颁发者密钥
                    ValidateIssuerSigningKey = true, //  验证颁发者密钥
                    ValidateIssuer = true,  // 验证颁发者
                    ValidateLifetime = true,  // 验证使用时限 
                    ClockSkew = TimeSpan.FromSeconds(30) // 缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                };
            });
            return services;
        }
    }
}

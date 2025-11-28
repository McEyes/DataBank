
using Furion;

using ITPortal.Core.Encrypt;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ITPortal.Core.Extensions
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(UserInfo userInfo)
        {
            var claims = new List<Claim>()
            {
                 new Claim("aud",App.GetConfig<string>("JWTSettings:ValidAudience") ),//_jwtOptions.Audience
                 new Claim("iss",App.GetConfig<string>("JWTSettings:ValidIssuer")  ),// _jwtOptions.Issuer ),
                 new Claim("id", userInfo.UserId.ToString() ),
                 new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString() ),
                 new Claim(ClaimTypes.Name, userInfo.UserName ),
                 new Claim("name", userInfo.UserName ),
                 new Claim("enname", userInfo.EnglishName??"" ),
                 new Claim( ClaimTypes.GivenName, userInfo.Name ),
                 new Claim(ClaimTypes.Email, userInfo.Email ),
                 new Claim(ClaimTypes.Surname, userInfo.Surname ),
                 //new Claim(ClaimTypes.Role,userInfo.Roles ),
                 new Claim(ClaimTypes.MobilePhone,userInfo.PhoneNumber ),
                 new Claim(ClaimTypes.HomePhone,userInfo.PhoneNumber )
            };

            foreach (var item in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            return JwtHelper.GenerateToken(claims.ToArray());
            //return JWTEncryption.Encrypt(claims);
        }


        public static string GenerateToken(Claim[] claims)
        {
            var config = App.Configuration;
            var jwtSettings = config.GetSection("JWTSettings");

            // 读取配置信息
            var issuer = jwtSettings["ValidIssuer"];
            var audience = jwtSettings["ValidAudience"];
            var secretKey = jwtSettings["IssuerSigningKey"]??"";
            var expiration = 259200;
            if (int.TryParse(jwtSettings["ExpiredTime"], out int ex))
            {
                expiration = ex;
            }

            //// 创建对称安全密钥
            //SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            //var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //// 创建 JWT 令牌
            //var token = new JwtSecurityToken(
            //    issuer: issuer,
            //    audience: audience,
            //    claims: claims,
            //    expires: DateTimeOffset.UtcNow.AddMinutes(expiration),
            //    signingCredentials: credentials
            //);

            //// 生成 JWT 字符串
            //var tokenHandler = new JwtSecurityTokenHandler();
            //return tokenHandler.WriteToken(token);

            var Jbkey = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Jbkey),
                  SecurityAlgorithms.HmacSha256Signature)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
        public static string GenerateDatalateToken(string userName)
        {
            var _datalakeOptions = new DatalakeOptions();
            _datalakeOptions.Key = App.GetConfig<string>("DatalakeOptions:Key")?? "b6nqdxNsKzbXQzTKeWz4";
            _datalakeOptions.Duration = App.GetConfig<int>("DatalakeOptions:Duration");
            if (_datalakeOptions == null || string.IsNullOrWhiteSpace(_datalakeOptions.Key))
            {
                return string.Empty;
            }
            var token = $"{userName}#{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}#{_datalakeOptions.Duration}";
            return DESUtil.Encrypt(token, _datalakeOptions.Key);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var config = App.Configuration;
            var jwtSettings = config.GetSection("JWTSettings");

            // 读取配置信息
            var issuer = jwtSettings["ValidIssuer"];
            var audience = jwtSettings["ValidAudience"];
            var secretKey = jwtSettings["IssuerSigningKey"] ?? "";
            var expiration = 259200;
            if (int.TryParse(jwtSettings["ExpiredTime"], out int ex))
            {
                expiration = ex;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
    public class DatalakeOptions
    {
        //Seconds
        public int Duration { get; set; } = 86400;
        public string Key { get; set; }
    }
}

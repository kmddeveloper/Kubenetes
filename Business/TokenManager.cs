using Kubernetes.TransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kubernetes.Business
{
    public class TokenManager : ITokenManager
    {

        readonly IConfiguration _configuration;

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JsonWebToken CreateJWT(string refreshToken = null)
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "userid"),
            new Claim(JwtRegisteredClaimNames.Cid, "customerid"),
            new Claim(JwtRegisteredClaimNames.Lid,"libraryid"),
            new Claim(JwtRegisteredClaimNames.SessionId, "sessionid"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim(ClaimTypes.Role, "SuperUser"),
            new Claim(ClaimTypes.Role, "Kevin"),        
            new Claim("Kevin", "123")
            };

            var signingKey = GetSymmetricSecurityKey();

            if (signingKey == null)
                throw new ArgumentException("Unable to create SymetricSecurityKey!");

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Token:Iss"],
                audience: _configuration["Token:Aud"],
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(Settings.Token.TOKEN_EXPIRE_IN_MINUTES)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);


            return new TransferObjects.JsonWebToken
            {
                Access_Token = encodedJwt,
                Expire_In_Seconds = (int)TimeSpan.FromMinutes(Settings.Token.TOKEN_EXPIRE_IN_MINUTES).TotalSeconds,
                RefreshToken = refreshToken,
            };
        }


        public TransferObjects.JsonWebToken RefreshToken(string encodedJwt)
        {
            if (encodedJwt != null)
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(encodedJwt);
                if (token != null && token.Claims != null)
                {
                    var now = DateTime.UtcNow;

                    var signingKey = GetSymmetricSecurityKey();

                    if (signingKey == null)
                        throw new ArgumentException("Unable to create SymetricSecurityKey!");

                    var jwt = new JwtSecurityToken(
                    issuer: _configuration["Token:Iss"],
                    audience: _configuration["Token:Aud"],
                    claims: token.Claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(Settings.Token.TOKEN_EXPIRE_IN_MINUTES)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
                    encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    return new TransferObjects.JsonWebToken
                    {
                        Access_Token = encodedJwt,
                        Expire_In_Seconds = (int)TimeSpan.FromMinutes(Settings.Token.TOKEN_EXPIRE_IN_MINUTES).TotalSeconds,
                        RefreshToken = null,
                    };
                }
            }
            return null;
        }


        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            try
            {
                var now = DateTime.UtcNow;
                var symmetricKeyAsBase64 = _configuration["Token:Secret"];
                var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
                return new SymmetricSecurityKey(keyByteArray);
            }
            catch (Exception)
            {

            }
            return null;
        }

        public string TokenIssuer()
        {
            return _configuration["Token:Iss"];
        }

        public string TokenAudience()
        {
            return _configuration["Token:Aud"];
        }

        public string TokenSecretKey()
        {
            return _configuration["Token:Secret"];
        }


        JsonWebToken ITokenManager.RefreshToken(string encodedJwt)
        {
            throw new NotImplementedException();
        }
    }
}

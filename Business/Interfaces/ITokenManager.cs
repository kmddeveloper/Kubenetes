
using Kubernetes.TransferObjects;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kubernetes.Business
{
    public interface ITokenManager
    {
        JsonWebToken CreateJWT(string refreshToken = null);
        JsonWebToken RefreshToken(string encodedJwt);
        SymmetricSecurityKey GetSymmetricSecurityKey();
        string TokenIssuer();
        string TokenAudience();
        string TokenSecretKey();
    }
}

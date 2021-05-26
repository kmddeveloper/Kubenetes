using System;
using System.Collections.Generic;
using System.Text;

namespace Kubernetes.TransferObjects
{
    public class JsonWebToken
    {
        public string Access_Token { get; set; }
        public int Expire_In_Seconds { get; set; }
        public string RefreshToken { get; set; }
    }
}

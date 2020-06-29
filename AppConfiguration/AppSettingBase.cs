using System;
using System.Collections.Generic;
using System.Text;

namespace Kubernetes.AppConfiguration
{
    public class AppSettingBase
    {
       static public class AppSection
        {
            public static string SqlServer = "SQL:Server";
            public static string SqlDatabase = "SQL:Database";
            public static string SqlUserId = "SQL:UserId";
            public static string SqlPassword = "SQL:Password";
        }
    }
}

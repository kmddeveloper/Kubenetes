using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kubernetes.AppConfiguration
{
    public class AppSettingManager:AppSettingBase, IAppSettingManager
    {
        readonly IConfiguration _configuration;

        public AppSettingManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region SqlServer
        public string SqlServer
        {
            get
            {
                return _configuration[AppSection.SqlServer];
            }
        }
        #endregion

        #region SqlDatabase
        public string SqlDatabase
        {
            get
            {
                return _configuration[AppSection.SqlDatabase];
            }
        }
        #endregion

        #region SqlUserId
        public string SqlUserId
        {
            get
            {
                return _configuration[AppSection.SqlUserId];
            }
        }
        #endregion

        #region SqlPassword
        public string SqlPassword
        {
            get
            {
                return _configuration[AppSection.SqlPassword];
            }
        }
        #endregion

        #region SqlConnectionString
        public string SqlConnectionString
        {
            get
            {
                return $"Server={SqlServer}; Database={SqlDatabase}; User iD={SqlUserId}; Password={SqlPassword}";
            }
        } 
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kubernetes.AppConfiguration
{
    public interface IAppSettingManager
    {
        public string SqlConnectionString { get;  }
    }
}

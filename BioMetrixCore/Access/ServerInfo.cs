using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Access
{
    /// <summary>
    /// 服务器信息
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// 服务器版本号
        /// </summary>
        public const string ServerVersion = "3.1.1";
        /// <summary>
        /// 服务器名称
        /// </summary>
        public const string ServerName = "ADMS";
        /// <summary>
        /// 服务端依据哪个协议版本开发的
        /// </summary>
        public const string PushProtVer = "3.2.0";
        /// <summary>
        /// 软件是否支持设备推送配置参数请求， 0不支持， 1支持， 未设置时默认不支持。
        /// </summary>
        public const string PushOptionsFlag = "1";

    }
}

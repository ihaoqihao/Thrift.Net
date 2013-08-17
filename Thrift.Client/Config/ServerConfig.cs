using System;
using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// server config
    /// </summary>
    public class ServerConfig : ConfigurationElement
    {
        /// <summary>
        /// 服务器地址。
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return (String)this["host"]; }
        }
        /// <summary>
        /// 端口号。
        /// </summary>
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
        }
        /// <summary>
        /// to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.Host, ":", this.Port.ToString());
        }
    }
}
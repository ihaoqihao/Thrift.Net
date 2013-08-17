using System;
using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// service config
    /// </summary>
    public class ServiceConfig : ConfigurationElement
    {
        /// <summary>
        /// name
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (String)this["name"]; }
        }
        /// <summary>
        /// Client
        /// </summary>
        [ConfigurationProperty("client", IsRequired = true)]
        public string Client
        {
            get { return (String)this["client"]; }
        }

        /// <summary>
        /// socket buffer size
        /// default is 1024 byte
        /// </summary>
        [ConfigurationProperty("socketBufferSize", IsRequired = false, DefaultValue = 8192)]
        public int SocketBufferSize
        {
            get { return (int)this["socketBufferSize"]; }
        }
        /// <summary>
        /// message buffer size
        /// default is 1024 byte
        /// </summary>
        [ConfigurationProperty("messageBufferSize", IsRequired = false, DefaultValue = 8192)]
        public int MessageBufferSize
        {
            get { return (int)this["messageBufferSize"]; }
        }
        /// <summary>
        /// 发送超时值，毫秒单位
        /// </summary>
        [ConfigurationProperty("sendTimeout", IsRequired = false, DefaultValue = 3000)]
        public int SendTimeout
        {
            get { return (int)(this["sendTimeout"]); }
        }
        /// <summary>
        /// 接收超时值，毫秒单位
        /// </summary>
        [ConfigurationProperty("receiveTimeout", IsRequired = false, DefaultValue = 3000)]
        public int ReceiveTimeout
        {
            get { return (int)(this["receiveTimeout"]); }
        }

        /// <summary>
        /// 服务发现配置
        /// </summary>
        [ConfigurationProperty("discovery", IsRequired = true)]
        public DiscoveryConfig Discovery
        {
            get { return (DiscoveryConfig)(this["discovery"]); }
        }
    }
}
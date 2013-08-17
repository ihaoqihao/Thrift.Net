using System;
using System.Configuration;

namespace Thrift.Server.Config
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
        /// 端口号。
        /// </summary>
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
        }
        /// <summary>
        /// socket buffer size
        /// </summary>
        [ConfigurationProperty("socketBufferSize", IsRequired = false, DefaultValue = 8192)]
        public int SocketBufferSize
        {
            get { return (int)this["socketBufferSize"]; }
        }
        /// <summary>
        /// message buffer size
        /// </summary>
        [ConfigurationProperty("messageBufferSize", IsRequired = false, DefaultValue = 8192)]
        public int MessageBufferSize
        {
            get { return (int)this["messageBufferSize"]; }
        }
        /// <summary>
        /// max Message Size
        /// </summary>
        [ConfigurationProperty("maxMessageSize", IsRequired = false, DefaultValue = 4096 * 100)]
        public int MaxMessageSize
        {
            get { return (int)this["maxMessageSize"]; }
        }
        /// <summary>
        /// max Connections
        /// </summary>
        [ConfigurationProperty("maxConnections", IsRequired = false, DefaultValue = 10000)]
        public int MaxConnections
        {
            get { return (int)this["maxConnections"]; }
        }

        /// <summary>
        /// thrift service type
        /// </summary>
        [ConfigurationProperty("serviceType", IsRequired = true)]
        public string ServiceType
        {
            get { return (String)this["serviceType"]; }
        }
        /// <summary>
        /// thrift processor Type
        /// </summary>
        [ConfigurationProperty("processorType", IsRequired = true)]
        public string ProcessorType
        {
            get { return (String)this["processorType"]; }
        }

        /// <summary>
        /// 服务注册配置
        /// </summary>
        [ConfigurationProperty("registry", IsRequired = false)]
        public RegistryConfig Registry
        {
            get { return (RegistryConfig)(this["registry"]); }
        }
    }
}
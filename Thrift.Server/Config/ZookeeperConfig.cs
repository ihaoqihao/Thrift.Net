using System.Configuration;

namespace Thrift.Server.Config
{
    /// <summary>
    /// zookeeper config
    /// </summary>
    public sealed class ZookeeperConfig : ConfigurationElement
    {
        /// <summary>
        /// zookeeper config path
        /// </summary>
        [ConfigurationProperty("path", IsRequired = false, DefaultValue = "zookeeper.config")]
        public string ConfigPath
        {
            get { return (string)this["path"]; }
        }
        /// <summary>
        /// zookeeper config name
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "thrift")]
        public string ConfigName
        {
            get { return (string)this["name"]; }
        }
        /// <summary>
        /// owner
        /// </summary>
        [ConfigurationProperty("owner", IsRequired = false, DefaultValue = "")]
        public string Owner
        {
            get { return (string)this["owner"]; }
        }
        /// <summary>
        /// service node name
        /// </summary>
        [ConfigurationProperty("znode", IsRequired = true)]
        public string ZNode
        {
            get { return (string)this["znode"]; }
        }
    }
}
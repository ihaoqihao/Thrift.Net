using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// discoveryc onfig
    /// </summary>
    public sealed class DiscoveryConfig : ConfigurationElement
    {
        /// <summary>
        /// 服务器集合。
        /// </summary>
        [ConfigurationProperty("servers", IsRequired = false)]
        public ServerCollection Servers
        {
            get { return this["servers"] as ServerCollection; }
        }
        /// <summary>
        /// zookeeper
        /// </summary>
        [ConfigurationProperty("zookeeper", IsRequired = false)]
        public ZookeeperConfig Zookeeper
        {
            get { return this["zookeeper"] as ZookeeperConfig; }
        }
    }
}
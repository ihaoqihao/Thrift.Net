using System.Configuration;

namespace Thrift.Server.Config
{
    /// <summary>
    /// registry onfig
    /// </summary>
    public sealed class RegistryConfig : ConfigurationElement
    {
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
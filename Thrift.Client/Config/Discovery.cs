using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// discoveryc onfig
    /// </summary>
    public sealed class Discovery : ConfigurationElement
    {
        /// <summary>
        /// 服务器集合。
        /// </summary>
        [ConfigurationProperty("server", IsRequired = false)]
        public EndPointCollection EndPoints
        {
            get { return this["server"] as EndPointCollection; }
        }
        /// <summary>
        /// zookeeper
        /// </summary>
        [ConfigurationProperty("zookeeper", IsRequired = false)]
        public Zookeeper Zookeeper
        {
            get { return this["zookeeper"] as Zookeeper; }
        }
    }
}
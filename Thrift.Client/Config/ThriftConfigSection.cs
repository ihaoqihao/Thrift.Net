using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// thrift config section
    /// </summary>
    public class ThriftConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 服务集合。
        /// </summary>
        [ConfigurationProperty("services", IsRequired = true)]
        public ServiceCollection Services
        {
            get { return this["services"] as ServiceCollection; }
        }
    }
}
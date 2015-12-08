using System;
using System.Configuration;
using System.IO;

namespace Thrift.Client
{
    /// <summary>
    /// thrift client factory
    /// </summary>
    static public class ThriftClientFactory
    {
        /// <summary>
        /// create
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public Tuple<ThriftClient, object> Create(string serviceName)
        {
            return Create(null, "thriftClient", serviceName);
        }
        /// <summary>
        /// create
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public Tuple<ThriftClient, object> Create(string sectionName, string serviceName)
        {
            return Create(null, sectionName, serviceName);
        }
        /// <summary>
        /// create
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public Tuple<ThriftClient, object> Create(string configPath, string sectionName, string serviceName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            //load config...
            Config.ThriftConfigSection config = null;
            if (string.IsNullOrEmpty(configPath))
                config = ConfigurationManager.GetSection(sectionName) as Config.ThriftConfigSection;
            else
            {
                config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath)
                }, ConfigurationUserLevel.None).GetSection(sectionName) as Config.ThriftConfigSection;
            }

            foreach (Config.Service service in config.Services)
            {
                if (service.Name != serviceName) continue;

                var client = new ThriftClient(service,
                    service.SocketBufferSize,
                    service.MessageBufferSize,
                    service.SendTimeout,
                    service.ReceiveTimeout);
                client.Start();

                return Tuple.Create(client, Type.GetType(service.Client, true)
                    .GetConstructor(new Type[] { typeof(IThriftClient) })
                    .Invoke(new object[] { client }));
            }
            return null;
        }
    }
}
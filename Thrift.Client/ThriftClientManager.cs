using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;

namespace Thrift.Client
{
    /// <summary>
    /// thrift client manager.
    /// </summary>
    static public class ThriftClientManager
    {
        #region Private Members
        static private readonly Dictionary<string, object> _dicThriftClients = new Dictionary<string, object>();
        static private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        #endregion

        #region Static Methods
        /// <summary>
        /// get thrift config
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">sectionName is null or empty</exception>
        /// <exception cref="ArgumentNullException">config is null</exception>
        static private Config.ThriftConfigSection GetConfig(string configPath, string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");

            Config.ThriftConfigSection thriftConfig = null;
            if (string.IsNullOrEmpty(configPath)) thriftConfig = ConfigurationManager.GetSection(sectionName) as Config.ThriftConfigSection;
            else
            {
                thriftConfig = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath)
                }, ConfigurationUserLevel.None).GetSection(sectionName) as Config.ThriftConfigSection;
            }
            return thriftConfig;
        }
        /// <summary>
        /// get thrift client
        /// </summary>
        /// <param name="config"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">config is null</exception>
        static private object CreateThriftClient(Config.ThriftConfigSection config, string serviceName)
        {
            if (config == null) throw new ArgumentNullException("config");

            foreach (Config.ServiceConfig childService in config.Services)
            {
                if (childService.Name != serviceName) continue;

                var thriftClient = new ThriftClient(childService.SocketBufferSize,
                    childService.MessageBufferSize,
                    childService.SendTimeout,
                    childService.ReceiveTimeout);

                thriftClient.ApplyConfig(childService);

                thriftClient.Start();

                return Type.GetType(childService.Client, true).GetConstructor(new Type[] { typeof(IThriftClient) }).Invoke(new object[] { thriftClient });
            }
            return null;
        }

        /// <summary>
        /// get proxy
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string serviceName)
            where TClient : class
        {
            return GetClient<TClient>(serviceName, null, "thriftClient");
        }
        /// <summary>
        /// get proxy
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string serviceName, string configPath)
            where TClient : class
        {
            return GetClient<TClient>(serviceName, configPath, "thriftClient");
        }
        /// <summary>
        /// get proxy
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string serviceName, string configPath, string sectionName)
            where TClient : class
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            _locker.EnterReadLock();
            try
            {
                if (_dicThriftClients.ContainsKey(serviceName)) return _dicThriftClients[serviceName] as TClient;
            }
            finally { _locker.ExitReadLock(); }

            _locker.EnterWriteLock();
            try
            {
                if (_dicThriftClients.ContainsKey(serviceName)) return _dicThriftClients[serviceName] as TClient;

                var client = CreateThriftClient(GetConfig(configPath, sectionName), serviceName);
                if (client == null) throw new InvalidOperationException("create thrift client failed");

                _dicThriftClients[serviceName] = client;
                return client as TClient;
            }
            finally { _locker.ExitWriteLock(); }
        }
        #endregion
    }
}
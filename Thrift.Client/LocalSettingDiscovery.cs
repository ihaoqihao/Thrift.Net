using System;
using System.Net;

namespace Thrift.Client
{
    /// <summary>
    /// local setting discovery
    /// </summary>
    public sealed class LocalSettingDiscovery : IServiceDiscovery
    {
        private ThriftClient _thriftClient = null;
        private Config.ServiceConfig _config = null;

        /// <summary>
        /// init
        /// </summary>
        /// <param name="thriftClient"></param>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException">thriftClient is null</exception>
        /// <exception cref="ArgumentNullException">config is null</exception>
        public void Init(ThriftClient thriftClient, Config.ServiceConfig config)
        {
            if (thriftClient == null) throw new ArgumentNullException("thriftClient");
            if (config == null) throw new ArgumentNullException("config");
            this._thriftClient = thriftClient;
            this._config = config;
        }
        /// <summary>
        /// start
        /// </summary>
        public void Start()
        {
            if (this._config.Discovery == null ||
                this._config.Discovery.Servers == null || this._config.Discovery.Servers.Count == 0) return;

            foreach (Config.ServerConfig child in this._config.Discovery.Servers)
            {
                this._thriftClient.RegisterServerNode(string.Concat(child.Host, child.Port.ToString()),
                    new IPEndPoint(IPAddress.Parse(child.Host), child.Port));
            }
        }
        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
        }
    }
}
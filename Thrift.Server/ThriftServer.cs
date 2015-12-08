using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Messaging;
using Sodao.FastSocket.Server.Protocol;
using System;

namespace Thrift.Server
{
    /// <summary>
    /// thrift server
    /// </summary>
    public class ThriftServer : SocketServer<ThriftMessage>
    {
        #region Members
        /// <summary>
        /// port
        /// </summary>
        public readonly int Port;
        /// <summary>
        /// config
        /// </summary>
        private readonly Config.ServiceConfig Config;
        /// <summary>
        /// zookeeper registry
        /// </summary>
        private ZookeeperRegistry _registry = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="config"></param>
        /// <param name="port"></param>
        /// <param name="socketService"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="maxConnections"></param>
        /// <exception cref="ArgumentNullException">config is null.</exception>
        public ThriftServer(Config.ServiceConfig config,
            int port,
            ThriftService socketService,
            int socketBufferSize,
            int messageBufferSize,
            int maxMessageSize,
            int maxConnections)
            : base(port, socketService, new ThriftProtocol(), socketBufferSize, messageBufferSize, maxMessageSize, maxConnections)
        {
            if (config == null) throw new ArgumentNullException("config");

            this.Port = port;
            this.Config = config;
        }
        #endregion

        #region Override Methos
        /// <summary>
        /// start
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (this.Config.Registry == null ||
                this.Config.Registry.Zookeeper == null ||
                string.IsNullOrEmpty(this.Config.Registry.Zookeeper.ConfigPath) ||
                string.IsNullOrEmpty(this.Config.Registry.Zookeeper.ConfigName) ||
                string.IsNullOrEmpty(this.Config.Registry.Zookeeper.ZNode)) return;

            this._registry = new ZookeeperRegistry(this.Port, this.Config.ServiceType,
                this.Config.Registry.Zookeeper.ConfigPath,
                this.Config.Registry.Zookeeper.ConfigName,
                this.Config.Registry.Zookeeper.ZNode,
                this.Config.Registry.Zookeeper.Owner);
        }
        /// <summary>
        /// stop
        /// </summary>
        public override void Stop()
        {
            if (this._registry != null)
            {
                this._registry.Dispose();
                this._registry = null;
            }
            base.Stop();
        }
        #endregion
    }
}
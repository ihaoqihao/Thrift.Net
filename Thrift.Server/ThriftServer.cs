using System;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.Server.Protocol;
using Sodao.FastSocket.SocketBase.Utils;

namespace Thrift.Server
{
    /// <summary>
    /// thrift server
    /// </summary>
    public class ThriftServer : SocketServer<ThriftCommandInfo>
    {
        #region Private Members
        private IServiceRegistry[] _arrRegistry = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketService"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="maxConnections"></param>
        public ThriftServer(ThriftService socketService,
            int socketBufferSize,
            int messageBufferSize,
            int maxMessageSize,
            int maxConnections)
            : base(socketService, new ThriftProtocol(), socketBufferSize, messageBufferSize, maxMessageSize, maxConnections)
        {
        }
        #endregion

        #region Override Methos
        /// <summary>
        /// start
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (this._arrRegistry != null)
                foreach (var child in this._arrRegistry) child.Start();
        }
        /// <summary>
        /// stop
        /// </summary>
        public override void Stop()
        {
            if (this._arrRegistry != null)
                foreach (var child in this._arrRegistry) child.Stop();

            base.Stop();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// apply config
        /// </summary>
        /// <param name="config"></param>
        public void ApplyConfig(Config.ServiceConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this._arrRegistry = ReflectionHelper.GetImplementObjects<IServiceRegistry>(this.GetType().Assembly);
            if (this._arrRegistry == null) return;
            foreach (var child in this._arrRegistry) child.Init(config);
        }
        #endregion
    }
}
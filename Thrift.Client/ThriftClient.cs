using System;
using Sodao.FastSocket.SocketBase.Utils;

namespace Thrift.Client
{
    /// <summary>
    /// async thrift request
    /// </summary>
    public sealed class ThriftClient : Sodao.FastSocket.Client.ThriftClient, IThriftClient
    {
        private IServiceDiscovery[] _arrDiscovery = null;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        public ThriftClient(int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(socketBufferSize, messageBufferSize, millisecondsSendTimeout, millisecondsReceiveTimeout)
        {
        }

        /// <summary>
        /// apply config
        /// </summary>
        /// <param name="config"></param>
        public void ApplyConfig(Config.ServiceConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this._arrDiscovery = ReflectionHelper.GetImplementObjects<IServiceDiscovery>(this.GetType().Assembly);
            if (this._arrDiscovery != null)
            {
                foreach (var child in this._arrDiscovery) child.Init(this, config);
            }
        }

        /// <summary>
        /// start
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (this._arrDiscovery == null) return;
            foreach (var child in this._arrDiscovery) child.Start();
        }
        /// <summary>
        /// stop
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            if (this._arrDiscovery == null) return;
            foreach (var child in this._arrDiscovery) child.Stop();
        }
    }
}
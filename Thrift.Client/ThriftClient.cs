using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Thrift.Client
{
    /// <summary>
    /// async thrift request
    /// </summary>
    public sealed class ThriftClient : Sodao.FastSocket.Client.ThriftClient, IThriftClient
    {
        #region Members
        private readonly Config.Service _config = null;
        private ZoomkeeperDiscovery _zkDiscovery = null;
        private readonly object _lockObj = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="config"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        public ThriftClient(Config.Service config, int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(socketBufferSize, messageBufferSize, millisecondsSendTimeout, millisecondsReceiveTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");
            this._config = config;

            if (this._config.Discovery != null &&
                this._config.Discovery.EndPoints != null &&
                this._config.Discovery.EndPoints.Count > 0)
            {
                var arrEndPoints = this._config.Discovery.EndPoints.ToArray();
                foreach (var p in arrEndPoints)
                    this.TryRegisterEndPoint(string.Concat(p.Address.ToString(), ":", p.Port.ToString()), new System.Net.EndPoint[] { p });
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// start
        /// </summary>
        public override void Start()
        {
            if (this._config.Discovery != null &&
                this._config.Discovery.Zookeeper != null &&
                !string.IsNullOrEmpty(this._config.Discovery.Zookeeper.ConfigPath) &&
                !string.IsNullOrEmpty(this._config.Discovery.Zookeeper.ConfigName) &&
                !string.IsNullOrEmpty(this._config.Discovery.Zookeeper.ZNode))
            {
                this._zkDiscovery = new ZoomkeeperDiscovery(this._config.Client,
                    this._config.Discovery.Zookeeper.ConfigPath,
                    this._config.Discovery.Zookeeper.ConfigName,
                    this._config.Discovery.Zookeeper.ZNode, endpoints =>
                    {
                        lock (this._lockObj)
                        {
                            var set = new HashSet<string>(this.GetAllRegisteredEndPoint().Select(c => c.Key).Distinct().ToArray());
                            set.ExceptWith(endpoints.Select(p =>
                                string.Concat(p.Address.ToString(), ":", p.Port.ToString())).Distinct().ToArray());
                            if (set.Count > 0)
                            {
                                foreach (var name in set) this.UnRegisterEndPoint(name);
                            }

                            foreach (var p in endpoints)
                                this.TryRegisterEndPoint(string.Concat(p.Address.ToString(), ":", p.Port.ToString()), new EndPoint[] { p });
                        }
                    });
            }
            base.Start();
        }
        /// <summary>
        /// stop
        /// </summary>
        public override void Stop()
        {
            if (this._zkDiscovery != null)
            {
                this._zkDiscovery.Dispose();
                this._zkDiscovery = null;
            }
            base.Stop();
        }
        #endregion
    }
}
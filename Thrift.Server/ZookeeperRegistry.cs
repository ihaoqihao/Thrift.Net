using System;
using System.Diagnostics;
using System.Linq;
using Sodao.FastSocket.SocketBase.Utils;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;

namespace Thrift.Server
{
    /// <summary>
    /// zookeeper registry
    /// </summary>
    public sealed class ZookeeperRegistry : IServiceRegistry
    {
        #region Private Members
        private Config.ServiceConfig _config = null;
        private string _methods = string.Empty;
        private SessionNode _sessionNode = null;
        #endregion

        #region IServiceRegistry Members
        /// <summary>
        /// set config
        /// </summary>
        /// <param name="config"></param>
        public void Init(Config.ServiceConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            this._config = config;
            this._methods = string.Join(",", Type.GetType(config.ServiceType).GetInterfaces()[0].GetMethods().Select(c => c.Name).ToArray());
        }
        /// <summary>
        /// start
        /// </summary>
        public void Start()
        {
            if (this._config == null ||
                this._config.Registry == null || this._config.Registry.Zookeeper == null) return;

            var keeperConfig = this._config.Registry.Zookeeper;
            var zk = ZookClientPool.Get(keeperConfig.ConfigPath, "zookeeper", keeperConfig.ConfigName);

            //ensure root node...
            var nodes = new NodeInfo[2];
            nodes[0] = new NodeInfo(string.Concat("/", keeperConfig.ZNode), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent);
            nodes[1] = new NodeInfo(string.Concat("/", keeperConfig.ZNode, "/providers"), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent);
            NodeFactory.TryEnsureCreate(zk, nodes, () =>
            {
                var currProcess = Process.GetCurrentProcess();
                var path = string.Concat("/", keeperConfig.ZNode, "/providers/", Uri.EscapeDataString(string.Format(
                    "thrift2://{0}:{1}/{2}?anyhost=true&application={3}&dispatcher=message&dubbo=2.5.1&interface={2}&loadbalance=roundrobin&methods={7}&owner={4}&pid={5}&revision=0.0.2-SNAPSHOT&side=provider&threads=100&timestamp={6}",
                    IPUtility.GetLocalIntranetIP().ToString(),
                    this._config.Port.ToString(),
                    keeperConfig.ZNode,
                    currProcess.ProcessName,
                    keeperConfig.Owner,
                    currProcess.Id.ToString(),
                    Date.ToMillisecondsSinceEpoch(DateTime.UtcNow).ToString(),
                    this._methods)));
                this._sessionNode = new SessionNode(zk, path, null, IDs.OPEN_ACL_UNSAFE);
            });
        }
        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
            if (this._sessionNode == null) return;
            this._sessionNode.Close();
            this._sessionNode = null;
        }
        #endregion
    }
}
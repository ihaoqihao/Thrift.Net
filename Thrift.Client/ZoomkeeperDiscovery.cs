using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Sodao.FastSocket.SocketBase.Utils;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;

namespace Thrift.Client
{
    /// <summary>
    /// zookeeper discovery
    /// </summary>
    public sealed class ZoomkeeperDiscovery : IServiceDiscovery
    {
        #region Private Members
        private ThriftClient _thriftClient = null;
        private Config.ServiceConfig _config = null;
        private string _methods = string.Empty;

        private SessionNode _sessionNode = null;
        private ChildrenWatcher _watcher = null;
        #endregion

        #region IServiceDiscovery Members
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
            this._methods = string.Join(",", Type.GetType(config.Client).GetInterfaces()[0].GetMethods().Select(c => c.Name).ToArray());
        }
        /// <summary>
        /// start
        /// </summary>
        public void Start()
        {
            if (this._config == null || this._config.Discovery == null ||
                this._config.Discovery.Zookeeper == null || string.IsNullOrEmpty(this._config.Discovery.Zookeeper.ZNode)) return;

            var keeperConfig = this._config.Discovery.Zookeeper;
            var zk = ZookClientPool.Get(keeperConfig.ConfigPath, "zookeeper", keeperConfig.ConfigName);

            //ensure root node...
            var nodes = new NodeInfo[2];
            nodes[0] = new NodeInfo(string.Concat("/", keeperConfig.ZNode), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent);
            nodes[1] = new NodeInfo(string.Concat("/", keeperConfig.ZNode, "/consumers"), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent);
            NodeFactory.TryEnsureCreate(zk, nodes, () =>
            {
                var currProcess = Process.GetCurrentProcess();
                var path = string.Concat("/", keeperConfig.ZNode, "/consumers/", Uri.EscapeDataString(string.Format(
                    "consumer://{0}/{1}?application={2}&category=consumers&check=false&dubbo=2.5.1&interface={1}&methods={6}&owner={3}&pid={4}&revision=0.0.2-SNAPSHOT&side=consumer&timestamp={5}",
                    IPUtility.GetLocalIntranetIP().ToString(),
                    keeperConfig.ZNode,
                    currProcess.ProcessName,
                    string.Empty,
                    currProcess.Id.ToString(),
                    Date.ToMillisecondsSinceEpoch(DateTime.UtcNow).ToString(),
                    this._methods)));
                this._sessionNode = new SessionNode(zk, path, null, IDs.OPEN_ACL_UNSAFE);
            });

            this._watcher = new ChildrenWatcher(zk, string.Concat("/", keeperConfig.ZNode, "/providers"), (names) =>
            {
                //已存在的servers
                var arrExistServers = this._thriftClient.GetAllNodeNames();
                //当前从zk获取到servers
                var arrNowServers = names.Select(s =>
                {
                    var t = Uri.UnescapeDataString(s);
                    t = t.Substring(t.IndexOf(":") + 3);
                    return t.Substring(0, t.IndexOf("/"));
                }).ToArray();

                var set = new HashSet<string>(arrExistServers);
                set.ExceptWith(arrNowServers);
                if (set.Count > 0)
                {
                    foreach (var child in set) this._thriftClient.UnRegisterServerNode(child);
                }

                set = new HashSet<string>(arrNowServers);
                set.ExceptWith(arrExistServers);
                if (set.Count > 0)
                {
                    foreach (var child in set)
                    {
                        int index = child.IndexOf(":");
                        this._thriftClient.RegisterServerNode(child,
                            new IPEndPoint(IPAddress.Parse(child.Substring(0, index)), int.Parse(child.Substring(index + 1))));
                    }
                }
            });
        }
        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
            if (this._sessionNode != null)
            {
                this._sessionNode.Close();
                this._sessionNode = null;
            }

            if (this._watcher != null)
            {
                this._watcher.Stop();
                this._watcher = null;
            }
        }
        #endregion
    }
}
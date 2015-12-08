using Sodao.FastSocket.SocketBase.Utils;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Server
{
    /// <summary>
    /// zookeeper registry
    /// </summary>
    public sealed class ZookeeperRegistry : IDisposable
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly SessionNode _sessionNode = null;
        private int _isdisposed = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// free
        /// </summary>
        ~ZookeeperRegistry()
        {
            this.Dispose();
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="port"></param>
        /// <param name="serviceType"></param>
        /// <param name="zkConfigPath"></param>
        /// <param name="zkConfigName"></param>
        /// <param name="zNode"></param>
        /// <param name="owner"></param>
        public ZookeeperRegistry(int port, string serviceType,
            string zkConfigPath, string zkConfigName, string zNode, string owner)
        {
            if (string.IsNullOrEmpty(serviceType)) throw new ArgumentNullException("serviceType");
            if (string.IsNullOrEmpty(zkConfigPath)) throw new ArgumentNullException("zkConfigPath");
            if (string.IsNullOrEmpty(zkConfigName)) throw new ArgumentNullException("zkConfigName");
            if (string.IsNullOrEmpty(zNode)) throw new ArgumentNullException("zNode");

            this._zk = ZookClientPool.Get(zkConfigPath, "zookeeper", zkConfigName);
            this.RegisterZNode(new NodeInfo[]
            { 
                new NodeInfo(string.Concat("/", zNode), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent),
                new NodeInfo(string.Concat("/", zNode, "/providers"), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent)
            });
            this._sessionNode = new SessionNode(this._zk,
                string.Concat("/", zNode, "/providers/", Uri.EscapeDataString(string.Format(
                    @"thrift2://{0}:{1}/{2}?anyhost=true&application={3}&dispatcher=message&dubbo=2.5.1&
                    interface={2}&loadbalance=roundrobin&methods={7}&owner={4}&pid={5}&revision=0.0.2-SNAPSHOT&
                    side=provider&threads=100&timestamp={6}",
                    IPUtility.GetLocalIntranetIP().ToString(),
                    port.ToString(), zNode, Process.GetCurrentProcess().ProcessName, owner ?? "", Process.GetCurrentProcess().Id.ToString(),
                    Date.ToMillisecondsSinceEpoch(DateTime.UtcNow).ToString(),
                    string.Join(",", Type.GetType(serviceType).GetInterfaces()[0].GetMethods().Select(c => c.Name).ToArray())))),
                    null, IDs.OPEN_ACL_UNSAFE);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// register zk node
        /// </summary>
        /// <param name="arrNodes"></param>
        private void RegisterZNode(NodeInfo[] arrNodes)
        {
            NodeCreator.TryCreate(this._zk, arrNodes).ContinueWith(c =>
            {
                if (Thread.VolatileRead(ref this._isdisposed) == 1) return;
                TaskEx.Delay(new Random().Next(100, 1500)).ContinueWith(_ => this.RegisterZNode(arrNodes));
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isdisposed, 1, 0) == 1) return;
            this._sessionNode.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
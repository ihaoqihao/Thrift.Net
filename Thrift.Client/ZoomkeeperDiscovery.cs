using Sodao.FastSocket.SocketBase.Utils;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Client
{
    /// <summary>
    /// zookeeper discovery
    /// </summary>
    public sealed class ZoomkeeperDiscovery : IDisposable
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly SessionNode _sessionNode = null;
        private readonly ChildrenWatcher _watcher = null;
        private readonly Action<IPEndPoint[]> _callback = null;

        private int _isdisposed = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="zkConfigPath"></param>
        /// <param name="zkConfigName"></param>
        /// <param name="zNode"></param>
        /// <param name="callback"></param>
        public ZoomkeeperDiscovery(string serviceType,
            string zkConfigPath, string zkConfigName, string zNode,
            Action<IPEndPoint[]> callback)
        {
            if (string.IsNullOrEmpty(serviceType)) throw new ArgumentNullException("serviceType");
            if (string.IsNullOrEmpty(zkConfigPath)) throw new ArgumentNullException("zkConfigPath");
            if (string.IsNullOrEmpty(zkConfigName)) throw new ArgumentNullException("zkConfigName");
            if (string.IsNullOrEmpty(zNode)) throw new ArgumentNullException("zNode");
            if (callback == null) throw new ArgumentNullException("callback");

            this._callback = callback;
            this._zk = ZookClientPool.Get(zkConfigPath, "zookeeper", zkConfigName);
            this.RegisterZNode(new NodeInfo[]
            {
                new NodeInfo(string.Concat("/", zNode), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent),
                new NodeInfo(string.Concat("/", zNode, "/consumers"), null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent)
            });
            this._sessionNode = new SessionNode(this._zk,
                string.Concat("/", zNode, "/consumers/", Uri.EscapeDataString(string.Format(
                    @"consumer://{0}/{1}?application={2}&category=consumers&check=false&dubbo=2.5.1&
                    interface={1}&methods={6}&owner={3}&pid={4}&revision=0.0.2-SNAPSHOT&
                    side=consumer&timestamp={5}",
                    IPUtility.GetLocalIntranetIP().ToString(),
                    zNode, Process.GetCurrentProcess().ProcessName, string.Empty, Process.GetCurrentProcess().Id.ToString(),
                    Date.ToMillisecondsSinceEpoch(DateTime.UtcNow).ToString(),
                    string.Join(",", Type.GetType(serviceType).GetInterfaces()[0].GetMethods().Select(c => c.Name).ToArray())))),
                    null, IDs.OPEN_ACL_UNSAFE);

            this._watcher = new ChildrenWatcher(this._zk, string.Concat("/", zNode, "/providers"), arrNodes =>
            {
                this._callback(arrNodes.Select(c =>
                {
                    var objUri = new Uri(Uri.UnescapeDataString(c));
                    return new IPEndPoint(IPAddress.Parse(objUri.Host), objUri.Port);
                }).ToArray());
                //lock (this._lockObj)
                //{
                //    var arrExists = this._thrift.GetAllRegisteredEndPoint().Select(c => c.Key).Distinct().ToArray();
                //    var arrCurr = arrNodes.Select(node =>
                //    {
                //        var strUrl = Uri.UnescapeDataString(node);
                //        strUrl = strUrl.Substring(strUrl.IndexOf(":") + 3);
                //        return strUrl.Substring(0, strUrl.IndexOf("/"));
                //    }).ToArray();

                //    var set = new HashSet<string>(arrExists);
                //    set.ExceptWith(arrCurr);
                //    if (set.Count > 0)
                //    {
                //        foreach (var child in set)
                //            this._thrift.UnRegisterEndPoint(child);
                //    }

                //    set = new HashSet<string>(arrCurr);
                //    set.ExceptWith(arrExists);
                //    if (set.Count > 0)
                //    {
                //        foreach (var child in set)
                //        {
                //            var i = child.IndexOf(":");
                //            var endpoint = new IPEndPoint(IPAddress.Parse(child.Substring(0, i)), int.Parse(child.Substring(i + 1)));
                //            this._thrift.TryRegisterEndPoint(child, new EndPoint[] { endpoint });
                //        }
                //    }
                //}
            });
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
            this._watcher.Dispose();
            this._sessionNode.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
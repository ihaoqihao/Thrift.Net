using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;

namespace Thrift.Server
{
    /// <summary>
    /// thrift server manager.
    /// </summary>
    static public class ThriftServerManager
    {
        #region Private Members
        static private readonly List<SocketServer<ThriftCommandInfo>> _serverList = new List<SocketServer<ThriftCommandInfo>>();
        #endregion

        #region Public Methods
        /// <summary>
        /// init
        /// </summary>
        static public void Init()
        {
            Init("thriftServer");
        }
        /// <summary>
        /// init
        /// </summary>
        /// <param name="sectionName"></param>
        static public void Init(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");
            Init(ConfigurationManager.GetSection(sectionName) as Config.ThriftConfigSection);
        }
        /// <summary>
        /// init
        /// </summary>
        /// <param name="config"></param>
        static public void Init(Config.ThriftConfigSection config)
        {
            if (config == null) throw new ArgumentNullException("config");

            foreach (Config.ServiceConfig serviceConfig in config.Services)
            {
                var objThriftService = Type.GetType(serviceConfig.ServiceType)
                    .GetConstructor(new Type[0])
                    .Invoke(new object[0]);

                var objThriftProcessor = Type.GetType(serviceConfig.ProcessorType)
                    .GetConstructor(new Type[] { objThriftService.GetType() })
                    .Invoke(new object[] { objThriftService }) as Thrift.IAsyncProcessor;

                var socketServer = new ThriftServer(new ThriftService(objThriftProcessor),
                    serviceConfig.SocketBufferSize,
                    serviceConfig.MessageBufferSize,
                    serviceConfig.MaxMessageSize,
                    serviceConfig.MaxConnections);

                socketServer.AddListener(serviceConfig.Name, new IPEndPoint(IPAddress.Any, serviceConfig.Port));
                socketServer.ApplyConfig(serviceConfig);

                _serverList.Add(socketServer);
            }
        }
        /// <summary>
        /// start thrift service list.
        /// </summary>
        static public void Start()
        {
            foreach (var child in _serverList) child.Start();
        }
        /// <summary>
        /// stop thrift service list
        /// </summary>
        static public void Stop()
        {
            foreach (var child in _serverList) child.Stop();
        }
        #endregion
    }
}
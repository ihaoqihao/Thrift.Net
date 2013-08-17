using System;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.SocketBase;

namespace Thrift.Server
{
    /// <summary>
    /// thrift service
    /// </summary>
    public sealed class ThriftService : ISocketService<ThriftCommandInfo>
    {
        #region Private Members
        private Thrift.IAsyncProcessor _processor = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        private ThriftService()
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="processor"></param>
        /// <exception cref="ArgumentNullException">processor is null</exception>
        public ThriftService(Thrift.IAsyncProcessor processor)
        {
            if (processor == null) throw new ArgumentNullException("processor");
            this._processor = processor;
        }
        #endregion

        #region ISocketService Members
        /// <summary>
        /// OnConnected
        /// </summary>
        /// <param name="connection"></param>
        public void OnConnected(IConnection connection)
        {
            connection.BeginReceive();
        }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public void OnDisconnected(IConnection connection, Exception ex)
        {
        }
        /// <summary>
        /// OnException
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public void OnException(IConnection connection, Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        /// <summary>
        /// OnReceived
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdInfo"></param>
        public void OnReceived(IConnection connection, ThriftCommandInfo cmdInfo)
        {
            this._processor.Process(cmdInfo.Buffer, (buffer) =>
            {
                if (buffer != null && buffer.Length > 0) connection.BeginSend(new Packet(buffer));
            });
        }
        /// <summary>
        /// OnSendCallback
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        public void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
        }
        /// <summary>
        /// OnStartSending
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        public void OnStartSending(IConnection connection, Packet packet)
        {
        }
        #endregion
    }
}
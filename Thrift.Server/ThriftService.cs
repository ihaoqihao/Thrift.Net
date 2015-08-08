using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Messaging;
using Sodao.FastSocket.SocketBase;
using System;

namespace Thrift.Server
{
    /// <summary>
    /// thrift service
    /// </summary>
    public sealed class ThriftService : ISocketService<ThriftMessage>
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
        /// OnSendCallback
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        /// <param name="isSuccess"></param>
        public void OnSendCallback(IConnection connection, Packet packet, bool isSuccess)
        {
        }
        /// <summary>
        /// OnReceived
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public void OnReceived(IConnection connection, ThriftMessage message)
        {
            this._processor.Process(message.Payload, bytes =>
            {
                if (bytes != null) connection.BeginSend(new Packet(bytes));
            });
        }
        /// <summary>
        /// OnException
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public void OnException(IConnection connection, Exception ex)
        {
        }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public void OnDisconnected(IConnection connection, Exception ex)
        {
        }
        #endregion
    }
}
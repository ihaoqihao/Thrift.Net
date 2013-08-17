using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thrift;
using Thrift.Protocol;
using Thrift.Util;

namespace Example.Service.Thrift
{
    /// <summary>
    /// 异步Service1
    /// </summary>
    public class AsyncService1
    {
		﻿        public interface Iface_client
        {
            ﻿            Task<System.Int32> Sum(System.Int32 x, System.Int32 y, object asyncState = null);
﻿            Task<Example.Service.Thrift.User> GetUser(System.Int32 userId, object asyncState = null);

        }
﻿        /// <summary>
        /// 异步客户端实现
        /// </summary>
        public class Client : Iface_client
        {
            #region Private Members
            /// <summary>
            /// thrift client
            /// </summary>
            private IThriftClient client_ = null;
            #endregion

            #region Constructors
            /// <summary>
            /// new
            /// </summary>
            /// <param name="client"></param>
            /// <exception cref="ArgumentNullException">client is null</exception>
            public Client(IThriftClient client)
            {
                if (client == null)
                    throw new ArgumentNullException("client");

                this.client_ = client;
            }
            #endregion

            #region Iface_client Members
            ﻿            public Task<System.Int32> Sum(System.Int32 x, System.Int32 y, object asyncState = null)
            {
                var taskSource_ = new TaskCompletionSource<System.Int32>(asyncState);

                //构建请求发送buffer
                int seqID_ = this.client_.NextRequestSeqID();
                var sendBuffer_ = ThriftMarshaller.Serialize(new TMessage("Sum", TMessageType.Call, seqID_),
                    new Service1.Sum_args()
                    {
                        X = x, Y = y
                    });

                //开始异步发送
                this.client_.Send("Example.Service.Thrift.Service1+Iface", "Sum", seqID_, sendBuffer_, (ex_) =>
                {
                    //处理异常回调
                    taskSource_.SetException(ex_);
                },
                (payload_) =>
                {
                    if (payload_ == null || payload_.Length == 0)
                    {
                        taskSource_.SetException(new TApplicationException(
                            TApplicationException.ExceptionType.MissingResult, "Sum failed: Did not receive any data."));
                        return;
                    }

                    TMessage recvMsg_;
                    TApplicationException exServer_ = null;
                    Service1.Sum_result result_ = null;

                    var oproto_ = ThriftMarshaller.GetBinaryProtocol(payload_);
                    try
                    {
                        //read TMessage
                        recvMsg_ = oproto_.ReadMessageBegin();
                        //read server return exception
                        if (recvMsg_.Type == TMessageType.Exception)
                            exServer_ = TApplicationException.Read(oproto_);
                        else
                        {
                            //read result
                            result_ = new Service1.Sum_result();
                            result_.Read(oproto_);
                        }
                    }
                    catch (System.Exception ex_)
                    {
                        oproto_.Transport.Close();
                        taskSource_.SetException(ex_);
                        return;
                    }
                    oproto_.Transport.Close();

                    if (exServer_ != null)
                        taskSource_.SetException(exServer_);
                    else
                    {
                        if (result_.__isset.success)
                        {
                            taskSource_.SetResult(result_.Success);
                            return;
                        }
						
                        taskSource_.SetException(new TApplicationException(
                            TApplicationException.ExceptionType.MissingResult, "Sum failed: unknown result"));
                    }
                });

                return taskSource_.Task;
            }
﻿            public Task<Example.Service.Thrift.User> GetUser(System.Int32 userId, object asyncState = null)
            {
                var taskSource_ = new TaskCompletionSource<Example.Service.Thrift.User>(asyncState);

                //构建请求发送buffer
                int seqID_ = this.client_.NextRequestSeqID();
                var sendBuffer_ = ThriftMarshaller.Serialize(new TMessage("GetUser", TMessageType.Call, seqID_),
                    new Service1.GetUser_args()
                    {
                        UserId = userId
                    });

                //开始异步发送
                this.client_.Send("Example.Service.Thrift.Service1+Iface", "GetUser", seqID_, sendBuffer_, (ex_) =>
                {
                    //处理异常回调
                    taskSource_.SetException(ex_);
                },
                (payload_) =>
                {
                    if (payload_ == null || payload_.Length == 0)
                    {
                        taskSource_.SetException(new TApplicationException(
                            TApplicationException.ExceptionType.MissingResult, "GetUser failed: Did not receive any data."));
                        return;
                    }

                    TMessage recvMsg_;
                    TApplicationException exServer_ = null;
                    Service1.GetUser_result result_ = null;

                    var oproto_ = ThriftMarshaller.GetBinaryProtocol(payload_);
                    try
                    {
                        //read TMessage
                        recvMsg_ = oproto_.ReadMessageBegin();
                        //read server return exception
                        if (recvMsg_.Type == TMessageType.Exception)
                            exServer_ = TApplicationException.Read(oproto_);
                        else
                        {
                            //read result
                            result_ = new Service1.GetUser_result();
                            result_.Read(oproto_);
                        }
                    }
                    catch (System.Exception ex_)
                    {
                        oproto_.Transport.Close();
                        taskSource_.SetException(ex_);
                        return;
                    }
                    oproto_.Transport.Close();

                    if (exServer_ != null)
                        taskSource_.SetException(exServer_);
                    else
                    {
                        if (result_.__isset.success)
                        {
                            taskSource_.SetResult(result_.Success);
                            return;
                        }
						if (result_.__isset.ex) {
taskSource_.SetException(result_.Ex);
return;
}

                        taskSource_.SetException(new TApplicationException(
                            TApplicationException.ExceptionType.MissingResult, "GetUser failed: unknown result"));
                    }
                });

                return taskSource_.Task;
            }

            #endregion
        }

﻿        /// <summary>
        /// service interface for server processor
        /// </summary>
		public interface Iface_server
        {
            ﻿            void Sum(System.Int32 x, System.Int32 y, Action<System.Int32> callback);
﻿            void GetUser(System.Int32 userId, Action<Example.Service.Thrift.User> callback);

        }
﻿        /// <summary>
        /// 异步Processor
        /// </summary>
        public class Processor : IAsyncProcessor
        {
            #region Delegates
            /// <summary>
            /// process handle delegate
            /// </summary>
            /// <param name="message"></param>
            /// <param name="iproto"></param>
            /// <param name="callback"></param>
            private delegate void ProcessHandle(TMessage message, TProtocol iproto, Action<byte[]> callback);
            #endregion

            #region Private Members
            /// <summary>
            /// service实现对象
            /// </summary>
            private Iface_server _face = null;
            /// <summary>
            /// process handle dic
            /// </summary>
            private Dictionary<string, ProcessHandle> processMap_ =
                 new Dictionary<string, ProcessHandle>();
            #endregion

            #region Constructors
            /// <summary>
            /// new
            /// </summary>
            /// <param name="face"></param>
            /// <exception cref="ArgumentNullException">face is null</exception>
            public Processor(Iface_server face)
            {
                if (face == null)
                    throw new ArgumentNullException("face");

                this._face = face;
				processMap_["Sum"]=Sum_Process;
processMap_["GetUser"]=GetUser_Process;

            }
            #endregion

            #region IAsyncProcessor Members
            /// <summary>
            /// process
            /// </summary>
            /// <param name="payload"></param>
            /// <param name="callback"></param>
            public void Process(byte[] payload, Action<byte[]> callback)
            {
                var iproto = ThriftMarshaller.GetBinaryProtocol(payload);

                TMessage message;
                try
                {
                    message = iproto.ReadMessageBegin();
                }
                catch (System.Exception)
                {
                    iproto.Transport.Close();
                    return;
                }

                ProcessHandle handle = null;
                if (this.processMap_.TryGetValue(message.Name, out handle))
                {
                    handle(message, iproto, callback);
                }
                else
                {
                    iproto.Transport.Close();
                    callback(ThriftMarshaller.Serialize(new TMessage(message.Name, TMessageType.Exception, message.SeqID),
                        new TApplicationException(TApplicationException.ExceptionType.UnknownMethod,
                            string.Concat("Invalid method name: '", message.Name, "'"))));
                }
            }
            #endregion

            #region Private Methods
			﻿            private void Sum_Process(TMessage message, TProtocol iproto, Action<byte[]> callback)
            {
                var args = new Service1.Sum_args();
                try
                {
                    args.Read(iproto);
                }
                catch (System.Exception ex)
                {
                    iproto.Transport.Close();
                    callback(ThriftMarshaller.Serialize(new TMessage(message.Name, TMessageType.Exception, message.SeqID),
                        new TApplicationException(TApplicationException.ExceptionType.Unknown, ex.Message)));
                    return;
                }
                iproto.Transport.Close();

                int seqID = message.SeqID;
                try
                {
                    this._face.Sum(args.X,args.Y, (result) =>
                    {
                        callback(ThriftMarshaller.Serialize(new TMessage("Sum", TMessageType.Reply, seqID),
                            new Service1.Sum_result
                            {
                                Success = result
                            }));
                    });
                }
                catch (System.Exception ex)
                {
                    callback(ThriftMarshaller.Serialize(new TMessage(message.Name, TMessageType.Exception, message.SeqID),
                        new TApplicationException(TApplicationException.ExceptionType.Unknown, ex.ToString())));
                }
            }﻿            private void GetUser_Process(TMessage message, TProtocol iproto, Action<byte[]> callback)
            {
                var args = new Service1.GetUser_args();
                try
                {
                    args.Read(iproto);
                }
                catch (System.Exception ex)
                {
                    iproto.Transport.Close();
                    callback(ThriftMarshaller.Serialize(new TMessage(message.Name, TMessageType.Exception, message.SeqID),
                        new TApplicationException(TApplicationException.ExceptionType.Unknown, ex.Message)));
                    return;
                }
                iproto.Transport.Close();

                int seqID = message.SeqID;
                try
                {
                    this._face.GetUser(args.UserId, (result) =>
                    {
                        callback(ThriftMarshaller.Serialize(new TMessage("GetUser", TMessageType.Reply, seqID),
                            new Service1.GetUser_result
                            {
                                Success = result
                            }));
                    });
                }
                catch (System.Exception ex)
                {
                    if (ex is Example.Service.Thrift.IllegalityUserIdException) { 
callback(ThriftMarshaller.Serialize(new TMessage("GetUser", TMessageType.Reply, seqID),
new Service1.GetUser_result{ 
 Ex = ex as Example.Service.Thrift.IllegalityUserIdException }));
return;} 
callback(ThriftMarshaller.Serialize(new TMessage(message.Name, TMessageType.Exception, message.SeqID),
                        new TApplicationException(TApplicationException.ExceptionType.Unknown, ex.ToString())));
                }
            }
            #endregion
        }


    }
}
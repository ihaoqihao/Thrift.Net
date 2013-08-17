/**
 * by dev.hong
 */
using System.IO;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift.Util
{
    /// <summary>
    /// thrift marshaller
    /// </summary>
    static public class ThriftMarshaller
    {
        /// <summary>
        /// 序列化<see cref="TBase"/>对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        static public byte[] Serialize<T>(T objectToSerialize) where T : TBase
        {
            using (var stream = new MemoryStream())
            {
                var protocol = new TBinaryProtocol(new TStreamTransport(stream, stream));
                objectToSerialize.Write(protocol);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 序列化<see cref="TMessage"/>以及指定<see cref="TBase"/>对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="objectToSerialize"></param>
        /// <param name="writeLength"></param>
        /// <returns></returns>
        static public byte[] Serialize<T>(TMessage message, T objectToSerialize, bool writeLength = true) where T : TBase
        {
            using (var stream = new MemoryStream())
            {
                var protocol = new TBinaryProtocol(new TStreamTransport(stream, stream));
                if (writeLength) stream.Write(new byte[4], 0, 4);

                protocol.WriteMessageBegin(message);
                objectToSerialize.Write(protocol);

                if (writeLength)
                {
                    stream.Position = 0;
                    protocol.WriteI32((int)stream.Length - 4);
                }
                return stream.ToArray();
            }
        }
        /// <summary>
        /// 序列化<see cref="TApplicationException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="writeLength"></param>
        /// <returns></returns>
        static public byte[] Serialize(TMessage message, TApplicationException ex, bool writeLength = true)
        {
            using (var stream = new MemoryStream())
            {
                var protocol = new TBinaryProtocol(new TStreamTransport(stream, stream));
                if (writeLength) stream.Write(new byte[4], 0, 4);

                protocol.WriteMessageBegin(message);
                ex.Write(protocol);

                if (writeLength)
                {
                    stream.Position = 0;
                    protocol.WriteI32((int)stream.Length - 4);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化指定类型对象 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        static public T Deserialize<T>(byte[] payload) where T : TBase, new()
        {
            using (var stream = new MemoryStream(payload))
            {
                var protocol = new TBinaryProtocol(new TStreamTransport(stream, stream));
                T t = new T();
                t.Read(protocol);
                return t;
            }
        }
        /// <summary>
        /// init binaryProtocol by payload
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        static public Thrift.Protocol.TProtocol GetBinaryProtocol(byte[] payload)
        {
            var stream = new MemoryStream(payload);
            return new TBinaryProtocol(new TStreamTransport(stream, stream));
        }
    }
}
/**
 * by dev.hong
 */
using System;

namespace Thrift
{
    /// <summary>
    /// thrift client interface
    /// </summary>
    public interface IThriftClient
    {
        /// <summary>
        /// 产生不重复的seqID
        /// </summary>
        /// <returns></returns>
        int NextRequestSeqID();
        /// <summary>
        /// send
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cmdName"></param>
        /// <param name="seqID"></param>
        /// <param name="payload"></param>
        /// <param name="onException"></param>
        /// <param name="onResult"></param>
        void Send(string service, string cmdName, int seqID, byte[] payload,
            Action<Exception> onException, Action<byte[]> onResult);
        /// <summary>
        /// send
        /// </summary>
        /// <param name="consistentKey"></param>
        /// <param name="service"></param>
        /// <param name="cmdName"></param>
        /// <param name="seqID"></param>
        /// <param name="payload"></param>
        /// <param name="onException"></param>
        /// <param name="onResult"></param>
        void Send(byte[] consistentKey, string service, string cmdName, int seqID, byte[] payload,
            Action<Exception> onException, Action<byte[]> onResult);
    }
}
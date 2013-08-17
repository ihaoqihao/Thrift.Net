/**
 * by dev.hong
 */
using System;

namespace Thrift
{
    /// <summary>
    /// 异步处理器
    /// </summary>
    public interface IAsyncProcessor
    {
        /// <summary>
        /// process
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        void Process(byte[] buffer, Action<byte[]> callback);
    }
}
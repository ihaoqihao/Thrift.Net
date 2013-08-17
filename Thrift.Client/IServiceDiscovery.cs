
namespace Thrift.Client
{
    /// <summary>
    /// service discovery interface.
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// init
        /// </summary>
        /// <param name="thriftClient"></param>
        /// <param name="config"></param>
        void Init(ThriftClient thriftClient, Config.ServiceConfig config);
        /// <summary>
        /// start
        /// </summary>
        void Start();
        /// <summary>
        /// stop
        /// </summary>
        void Stop();
    }
}
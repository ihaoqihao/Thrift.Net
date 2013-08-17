
namespace Thrift.Server
{
    /// <summary>
    /// 服务注册handle interface.  
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// init
        /// </summary>
        /// <param name="config"></param>
        void Init(Config.ServiceConfig config);
        /// <summary>
        /// 开始注册
        /// </summary>
        void Start();
        /// <summary>
        /// 停止注册
        /// </summary>
        void Stop();
    }
}
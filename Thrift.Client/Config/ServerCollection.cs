using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// server config collection
    /// </summary>
    [ConfigurationCollection(typeof(ServerConfig), AddItemName = "server")]
    public class ServerCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// create new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerConfig();
        }
        /// <summary>
        /// 获取指定元素的Key。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServerConfig).ToString();
        }
        /// <summary>
        /// 获取指定位置的对象。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ServerConfig this[int i]
        {
            get { return BaseGet(i) as ServerConfig; }
        }
    }
}
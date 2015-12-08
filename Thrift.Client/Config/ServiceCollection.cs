using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// service config collection
    /// </summary>
    [ConfigurationCollection(typeof(Service), AddItemName = "service")]
    public class ServiceCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// create new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Service();
        }
        /// <summary>
        /// 获取指定元素的Key。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as Service).Name;
        }
        /// <summary>
        /// 获取指定位置的对象。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Service this[int i]
        {
            get { return BaseGet(i) as Service; }
        }
        /// <summary>
        /// 获取指定key的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Service Get(string key)
        {
            return BaseGet(key) as Service;
        }
    }
}
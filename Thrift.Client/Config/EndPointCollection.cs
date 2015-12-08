using System.Configuration;

namespace Thrift.Client.Config
{
    /// <summary>
    /// endpoint collection
    /// </summary>
    [ConfigurationCollection(typeof(EndPoint), AddItemName = "endpoint")]
    public sealed class EndPointCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// create new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new EndPoint();
        }
        /// <summary>
        /// 获取指定元素的Key。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as EndPoint).ToString();
        }
        /// <summary>
        /// 获取指定位置的对象。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public EndPoint this[int i]
        {
            get { return BaseGet(i) as EndPoint; }
        }
        /// <summary>
        /// to array
        /// </summary>
        /// <returns></returns>
        public System.Net.IPEndPoint[] ToArray()
        {
            var endpoints = new System.Net.IPEndPoint[base.Count];
            for (int i = 0, l = this.Count; i < l; i++)
            {
                var child = this[i];
                endpoints[i] = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(child.Host), child.Port);
            }
            return endpoints;
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Thrift.Client
{
    /// <summary>
    /// thrift client pool
    /// </summary>
    static public class ThriftClientManager
    {
        /// <summary>
        /// key:string.Concat(configPath, sectionName, serviceName)
        /// </summary>
        static private readonly ConcurrentDictionary<string, Lazy<Tuple<ThriftClient, object>>> _dic =
            new ConcurrentDictionary<string, Lazy<Tuple<ThriftClient, object>>>();

        /// <summary>
        /// get
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public ThriftClient GetClient(string serviceName)
        {
            return GetClient(null, "thriftClient", serviceName);
        }
        /// <summary>
        /// get
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public ThriftClient GetClient(string sectionName, string serviceName)
        {
            return GetClient(null, sectionName, serviceName);
        }
        /// <summary>
        /// get
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public ThriftClient GetClient(string configPath, string sectionName, string serviceName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            return _dic.GetOrAdd(string.Concat(configPath ?? "", "/", sectionName, "/", serviceName),
                key => new Lazy<Tuple<ThriftClient, object>>(() =>
                    ThriftClientFactory.Create(configPath, sectionName, serviceName),
                    LazyThreadSafetyMode.ExecutionAndPublication)).Value.Item1;
        }

        /// <summary>
        /// get client
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string serviceName)
            where TClient : class
        {
            return GetClient<TClient>(null, "thriftClient", serviceName);
        }
        /// <summary>
        /// get client
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string sectionName, string serviceName)
            where TClient : class
        {
            return GetClient<TClient>(null, sectionName, serviceName);
        }
        /// <summary>
        /// get client
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static public TClient GetClient<TClient>(string configPath, string sectionName, string serviceName)
            where TClient : class
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            return _dic.GetOrAdd(string.Concat(configPath ?? "", "/", sectionName, "/", serviceName),
                key => new Lazy<Tuple<ThriftClient, object>>(() =>
                    ThriftClientFactory.Create(configPath, sectionName, serviceName),
                    LazyThreadSafetyMode.ExecutionAndPublication)).Value.Item2 as TClient;
        }

        /// <summary>
        /// start all
        /// </summary>
        static public void StartAll()
        {
            _dic.Values.ToList().ForEach(t => t.Value.Item1.Start());
        }
        /// <summary>
        /// stop all
        /// </summary>
        static public void StopAll()
        {
            _dic.Values.ToList().ForEach(t => t.Value.Item1.Stop());
        }
    }
}
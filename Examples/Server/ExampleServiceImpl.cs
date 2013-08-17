using System;

namespace Server
{
    /// <summary>
    /// example thrift service implementation
    /// </summary>
    public sealed class ExampleServiceImpl : Example.Service.Thrift.AsyncService1.Iface_server
    {
        /// <summary>
        /// sum
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="callback"></param>
        public void Sum(int x, int y, Action<int> callback)
        {
            callback(x + y);
        }
        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callback"></param>
        public void GetUser(int userId, Action<Example.Service.Thrift.User> callback)
        {
            if (userId < 1) throw new Example.Service.Thrift.IllegalityUserIdException { UserId = userId, Reason = "userId is less than 1." };
            //Todo ...
            callback(new Example.Service.Thrift.User
            {
                UserId = userId,
                Name = userId.ToString()
            });
        }
    }
}
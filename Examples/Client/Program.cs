using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = Thrift.Client.ThriftClientManager.GetClient<Example.Service.Thrift.AsyncService1.Iface_client>("example");

            //sum
            proxy.Sum(1, 1).ContinueWith(c =>
            {
                if (c.IsFaulted) Console.WriteLine(c.Exception.ToString());
                else Console.WriteLine("1+1=" + c.Result.ToString());
            });
            //get user
            proxy.GetUser(1023).ContinueWith(c =>
            {
                if (c.IsFaulted) Console.WriteLine(c.Exception.ToString());
                else Console.WriteLine("user(1023).Name is " + c.Result.Name);
            });
            //thrift application
            proxy.GetUser(-5).ContinueWith(c =>
            {
                var illex = c.Exception.InnerException as Example.Service.Thrift.IllegalityUserIdException;
                if (illex == null) Console.WriteLine(c.Exception);
                else Console.WriteLine("illegality userId(" + illex.UserId.ToString() + ")");
            }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);

            for (int i = 0; i < 10000; i++)
            {
                try { Console.WriteLine(proxy.Sum(i, i).Result); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.ReadLine();
            Thrift.Client.ThriftClientManager.StopAll();
            Console.WriteLine("exit...");
        }
    }
}
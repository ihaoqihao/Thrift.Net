using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Thrift.Server.ThriftServerManager.Init();
            Thrift.Server.ThriftServerManager.Start();
            Console.WriteLine("thrift server started.");
            Console.ReadLine();
        }
    }
}

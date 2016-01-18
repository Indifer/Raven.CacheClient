using Raven.CacheClient.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient.Test2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (RedisCacheClient client = new RedisCacheClient())
            {
                client.Subscribe<MallCard>("var", (x) =>
                {
                    Console.WriteLine(x.Name);
                });

                Console.ReadLine();
            }

            Console.WriteLine("over...");
        }


    }
}

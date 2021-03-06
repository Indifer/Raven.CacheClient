﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient.PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {

            MallCard mall = new MallCard()
            {
                ID = Guid.NewGuid().ToString("N"),
                MallID = new Random().Next(1, 100),
                UID = new Random().Next(1, 1000)
            };


            int seed = 1;
            Stopwatch sw = new Stopwatch();
            Task[] taskList = new Task[seed];
            string conStr = System.Configuration.ConfigurationManager.AppSettings["conStr"];

            using (RedisCacheClient client = new RedisCacheClient(conStr, 2))
            {
                sw.Restart();

                string key = "123ABC";
                taskList[0] = client.SetAsync<string>(key, "33333");

                //for (var i = 0; i < seed; i++)
                //{
                //    //RedisKey key = "MallCardbcb0878b8e814b8fa7540862729044c9"; //mall.GetKey();
                //    ////RedisValue val = serializer.Serialize(mall);
                //    client.Database.StringSet(key, val);

                //    //var task = client.Database.StringGetAsync(key).ContinueWith(x =>
                //    //{
                //    //    var mall2 = serializer.Deserialize<MallCard2>(x.Result);
                //    //    ;
                //    //});
                //    //taskList[i] = task;

                //    //client.s

                //    string key = "123ABC";
                //    taskList[i] = client.GetAsync<MallCard2>(key);
                //}

                Task.WhenAll(taskList);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                Console.WriteLine(seed / sw.Elapsed.TotalSeconds);
                Console.ReadLine();
                //Assert.AreEqual(mall.MallID, mall2.MallID);
            }

        }
    }

    public class MallCard
    {
        /// <summary>
        ///自增主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///// 商场id
        ///// </summary>
        public long MallID { get; set; }

        ///// <summary>
        ///// 用户UID[为0表示未绑定猫酷用户]
        ///// </summary>
        public long UID { get; set; }

        ///// <summary>
        ///// 会员卡类型ID
        ///// </summary>
        public long? CardTypeID { get; set; }

        public string GetKey()
        {
            return string.Concat("MallCard", ID);
        }
    }


    public class MallCard2
    {

        /// <summary>
        ///// 商场id
        ///// </summary>
        public long MallID;

        /// <summary>
        ///自增主键
        /// </summary>
        public string ID;

        ///// <summary>
        ///// 会员卡类型ID
        ///// </summary>
        public long? CardTypeID;

        public string GetKey()
        {
            return string.Concat("MallCard", ID);
        }
    }
}

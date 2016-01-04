using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Diagnostics;

namespace Raven.CacheClient.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static string host = "127.0.0.1";

        [TestMethod]
        public void StringSet_StringGet()
        {
            MallCard mall = new MallCard()
            {
                ID = Guid.NewGuid().ToString("N"),
                MallID = new Random().Next(1, 100),
                UID = new Random().Next(1, 1000)
            };
            
            var serializer = Raven.Serializer.SerializerFactory.Create(Serializer.SerializerType.MsgPack);
            using (RedisCacheClient client = new RedisCacheClient( "127.0.0.1", 3, serializer))
            {
                RedisKey key = mall.GetKey();
                RedisValue val = serializer.Serialize(mall);
                client.Database.StringSet(key, val);

                RedisValue val2 = client.Database.StringGet(key);
                var mall2 = serializer.Deserialize<MallCard2>(val2);

                Assert.AreEqual(mall.MallID, mall2.MallID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void HashSet_HashGet()
        {
            //var serializer = Raven.Serializer.SerializerFactory.Create(Serializer.SerializerType.MsgPack);
            //using (RedisCacheClient client = new RedisCacheClient(new Raven.Serializer.WithMsgPack.MsgPackSerializer(), "127.0.0.1", 3))
            //{
            //    RedisKey key = mall.GetKey();
            //    RedisValue val = serializer.Serialize(mall);
            //    client.Database.HashSet(key, val);

            //    RedisValue val2 = client.Database.HashGetAll(key);
            //    var mall2 = serializer.Deserialize<MallCard2>(val2);

            //    Assert.AreEqual(mall.MallID, mall2.MallID);
            //}
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
        public long MallID { get; set; }

        /// <summary>
        ///自增主键
        /// </summary>
        public string ID { get; set; }

        ///// <summary>
        ///// 会员卡类型ID
        ///// </summary>
        public long? CardTypeID { get; set; }

        public string GetKey()
        {
            return string.Concat("MallCard", ID);
        }
    }
}

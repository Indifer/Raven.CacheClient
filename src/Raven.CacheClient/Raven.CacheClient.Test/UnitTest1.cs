using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Diagnostics;
using MsgPack.Serialization;

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
                ID = "123abc",//Guid.NewGuid().ToString("N"),
                Name = Guid.NewGuid().ToString("N"),
                MallID = new Random().Next(1, 100),
                UID = new Random().Next(1, 1000)
            };

            var serializer = Raven.Serializer.SerializerFactory.Create(Serializer.SerializerType.MsgPack);
            using (RedisCacheClient client = new RedisCacheClient(host, 3, serializer))
            {
                var key = mall.GetKey();
                //RedisValue val = serializer.Serialize(mall);
                //client.Database.StringSet(key, val);
                client.Set(key, mall);

                //byte[] val2 = client.Database.StringGet(key);
                //var mall2 = serializer.Deserialize<MallCard2>(val2);
                var mall2 = client.Get<MallCard2>(key);

                Assert.AreEqual(mall.Name, mall2.Name);
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

}

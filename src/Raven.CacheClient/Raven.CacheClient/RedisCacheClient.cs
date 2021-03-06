﻿using Raven.CacheClient.Configuration;
using Raven.Serializer;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient
{
    /// <summary>
    /// redis缓存客户端
    /// </summary>
    public class RedisCacheClient : IDisposable
    {
        private ConnectionMultiplexer connectionMultiplexer;
        private readonly IDatabase db;
        private readonly IDataSerializer serializer;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionMultiplexer ConnectionMultiplexer
        {
            get { return this.connectionMultiplexer; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDatabase Database
        {
            get { return db; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataSerializer Serializer
        {
            get { return this.serializer; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        public RedisCacheClient(IDataSerializer serializer = null, IRedisCachingConfiguration configuration = null, string section = RedisCachingSectionHandler.DefaulSection)
        {
            if (serializer == null)
            {
                serializer = SerializerFactory.Create(SerializerType.MsgPack);
            }

            if (configuration == null)
            {
                configuration = RedisCachingSectionHandler.GetConfig(section);
            }

            if (configuration == null)
            {
                throw new ConfigurationErrorsException("Unable to locate <"+ section + "> section into your configuration file.");
            }

            var options = new ConfigurationOptions
            {
                Ssl = configuration.Ssl,
                AllowAdmin = configuration.AllowAdmin,
                Password = configuration.Password
            };

            foreach (RedisHost redisHost in configuration.RedisHosts)
            {
                options.EndPoints.Add(redisHost.Host, redisHost.Port);
            }

            this.connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            db = connectionMultiplexer.GetDatabase(configuration.Database);
            this.serializer = serializer;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="database"></param>
        /// <param name="serializer"></param>
        public RedisCacheClient(string connectionString, int database = 0, IDataSerializer serializer = null)
        {
            if (serializer == null)
            {
                serializer = SerializerFactory.Create(SerializerType.MsgPack);
            }

            this.serializer = serializer;
            this.connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            db = connectionMultiplexer.GetDatabase(database);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private T Deserialize<T>(byte[] bytes)
        {
            try
            {
                T val = serializer.Deserialize<T>(bytes);
                return val;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] Serialize(object obj)
        {
            var bytes = serializer.Serialize(obj);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return db.KeyExists(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string key)
        {
            return db.KeyExistsAsync(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var valueBytes = db.StringGet(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return Deserialize<T>(valueBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var valueBytes = await db.StringGetAsync(key).ConfigureAwait(false);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return Deserialize<T>(valueBytes);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return db.KeyDelete(key);
        }

        /// <summary>
		/// Removes the specified key from Redis Database
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// True if the key has removed. Othwerwise False
		/// </returns>
		public Task<bool> RemoveAsync(string key)
        {
            return db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach (var k in keys)
            {
                Remove(k);
            }
        }

        /// <summary>
        /// Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            return Task.WhenAll(
                keys.Select(x =>
                    Task.Run(
                            () => Remove(x)
                    )
                )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            var entryBytes = Serialize(value);

            return db.StringSet(key, entryBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T value)
        {
            var entryBytes = Serialize(value);

            return db.StringSetAsync(key, entryBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value)
        {
            return Set(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            return SetAsync(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return db.StringSet(key, entryBytes, expiration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return db.StringSetAsync(key, entryBytes, expiration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return Set(key, value, expiresAt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return SetAsync(key, value, expiresAt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = Serialize(value);

            return db.StringSet(key, entryBytes, expiresIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = Serialize(value);

            return db.StringSetAsync(key, entryBytes, expiresIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Set(key, value, expiresIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            return SetAsync(key, value, expiresIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = db.StringGet(redisKeys);
            return redisKeys.ToDictionary(key => (string)key, key =>
            {
                {
                    var index = Array.IndexOf(redisKeys, key);
                    var value = result[index];
                    return value == RedisValue.Null ? default(T) : Deserialize<T>(result[index]);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = await db.StringGetAsync(redisKeys).ConfigureAwait(false);
            return redisKeys.ToDictionary(key => (string)key, key =>
            {
                {
                    var index = Array.IndexOf(redisKeys, key);
                    var value = result[index];
                    return value == RedisValue.Null ? default(T) : Deserialize<T>(result[index]);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool SetAll<T>(IList<Tuple<string, T>> items)
        {
            Dictionary<RedisKey, RedisValue> values = items.ToDictionary<Tuple<string, T>, RedisKey, RedisValue>(item => item.Item1, item => Serialize(item.Item2));

            return db.StringSet(values.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items)
        {
            Dictionary<RedisKey, RedisValue> values = items.ToDictionary<Tuple<string, T>, RedisKey, RedisValue>(item => item.Item1, item => Serialize(item.Item2));

            return db.StringSetAsync(values.ToArray());
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="memberName"></param>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public bool SetAdd(string memberName, string key)
        //{
        //    return db.SetAdd(memberName, key);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="memberName"></param>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public Task<bool> SetAddAsync(string memberName, string key)
        //{
        //    return db.SetAddAsync(memberName, key);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="memberName"></param>
        ///// <returns></returns>
        //public string[] SetMember(string memberName)
        //{
        //    return db.SetMembers(memberName).Select(x => x.ToString()).ToArray();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="memberName"></param>
        ///// <returns></returns>
        //public async Task<string[]> SetMemberAsync(string memberName)
        //{
        //    return (await db.SetMembersAsync(memberName)).Select(x => x.ToString()).ToArray();
        //}

        //public IEnumerable<string> SearchKeys(string pattern)
        //{
        //    var keys = new HashSet<RedisKey>();

        //    var endPoints = db.Multiplexer.GetEndPoints();

        //    foreach (var endpoint in endPoints)
        //    {
        //        var dbKeys = db.Multiplexer.GetServer(endpoint).Keys(database: db.Database, pattern: pattern);

        //        foreach (var dbKey in dbKeys)
        //        {
        //            if (!keys.Contains(dbKey))
        //            {
        //                keys.Add(dbKey);
        //            }
        //        }
        //    }

        //    return keys.Select(x => (string)x);
        //}

        //public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        //{
        //    return Task.Factory.StartNew(() => SearchKeys(pattern));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, serializer.Serialize(message), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync<T>(string channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, serializer.Serialize(message), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        public void Subscribe<T>(string channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) => handler(serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task SubscribeAsync<T>(string channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        public void Unsubscribe<T>(string channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task UnsubscribeAsync<T>(string channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync(flags);
        }

        #region IDispose

        private bool isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (connectionMultiplexer != null)
                    {
                        connectionMultiplexer.Dispose();
                    }
                    connectionMultiplexer = null;
                }
            }
            isDisposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ~RedisCacheClient()
        {
            Dispose(false);
        }

        #endregion
    }
}

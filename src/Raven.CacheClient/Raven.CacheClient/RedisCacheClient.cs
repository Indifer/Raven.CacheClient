using Raven.CacheClient.Configuration;
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
    /// 
    /// </summary>
    public class RedisCacheClient : ICacheClient
    {
        private readonly ConnectionMultiplexer connectionMultiplexer;
        private readonly IDatabase db;
        private readonly IDataSerializer serializer;

        public RedisCacheClient(IDataSerializer serializer, IRedisCachingConfiguration configuration = null)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (configuration == null)
            {
                configuration = RedisCachingSectionHandler.GetConfig();
            }

            if (configuration == null)
            {
                throw new ConfigurationErrorsException("Unable to locate <redisCacheClient> section into your configuration file. Take a look https://github.com/imperugo/StackExchange.Redis.Extensions");
            }

            var options = new ConfigurationOptions
            {
                Ssl = configuration.Ssl,
                AllowAdmin = configuration.AllowAdmin,
                Password = configuration.Password
            };

            foreach (RedisHost redisHost in configuration.RedisHosts)
            {
                options.EndPoints.Add(redisHost.Host, redisHost.CachePort);
            }

            this.connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            db = connectionMultiplexer.GetDatabase(configuration.Database);
            this.serializer = serializer;
        }
                
        public RedisCacheClient(IDataSerializer serializer, string connectionString, int database = 0)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.serializer = serializer;
            this.connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            db = connectionMultiplexer.GetDatabase(database);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            connectionMultiplexer.Dispose();
        }

        public IDatabase Database
        {
            get { return db; }
        }

        public IDataSerializer Serializer
        {
            get { return this.serializer; }
        }
        

        public bool Exists(string key)
        {
            return db.KeyExists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return db.KeyExistsAsync(key);
        }

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

        public T Get<T>(string key)
        {
            var valueBytes = db.StringGet(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return serializer.Deserialize<T>(valueBytes);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var valueBytes = await db.StringGetAsync(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return serializer.Deserialize<T>(valueBytes);
        }

        public bool Add<T>(string key, T value)
        {
            var entryBytes = serializer.Serialize(value);

            return db.StringSet(key, entryBytes);
        }

        public Task<bool> AddAsync<T>(string key, T value)
        {
            var entryBytes = serializer.Serialize(value);

            return db.StringSetAsync(key, entryBytes);
        }

        public bool Replace<T>(string key, T value)
        {
            return Add(key, value);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            return AddAsync(key, value);
        }

        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return db.StringSet(key, entryBytes, expiration);
        }

        public Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return db.StringSetAsync(key, entryBytes, expiration);
        }

        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return Add(key, value, expiresAt);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return AddAsync(key, value, expiresAt);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = serializer.Serialize(value);

            return db.StringSet(key, entryBytes, expiresIn);
        }

        public Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = serializer.Serialize(value);

            return db.StringSetAsync(key, entryBytes, expiresIn);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add(key, value, expiresIn);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            return AddAsync(key, value, expiresIn);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = db.StringGet(redisKeys);
            return redisKeys.ToDictionary(key => (string)key, key =>
            {
                {
                    var index = Array.IndexOf(redisKeys, key);
                    var value = result[index];
                    return value == RedisValue.Null ? default(T) : serializer.Deserialize<T>(result[index]);
                }
            });
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = await db.StringGetAsync(redisKeys);
            return redisKeys.ToDictionary(key => (string)key, key =>
            {
                {
                    var index = Array.IndexOf(redisKeys, key);
                    var value = result[index];
                    return value == RedisValue.Null ? default(T) : serializer.Deserialize<T>(result[index]);
                }
            });
        }

        public bool AddAll<T>(IList<Tuple<string, T>> items)
        {
            Dictionary<RedisKey, RedisValue> values = items.ToDictionary<Tuple<string, T>, RedisKey, RedisValue>(item => item.Item1, item => this.Serializer.Serialize(item.Item2));

            return db.StringSet(values.ToArray());
        }

        public Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
        {
            Dictionary<RedisKey, RedisValue> values = items.ToDictionary<Tuple<string, T>, RedisKey, RedisValue>(item => item.Item1, item => this.Serializer.Serialize(item.Item2));

            return db.StringSetAsync(values.ToArray());
        }

        public bool SetAdd(string memberName, string key)
        {
            return db.SetAdd(memberName, key);
        }

        public Task<bool> SetAddAsync(string memberName, string key)
        {
            return db.SetAddAsync(memberName, key);
        }

        public string[] SetMember(string memberName)
        {
            return db.SetMembers(memberName).Select(x => x.ToString()).ToArray();
        }

        public async Task<string[]> SetMemberAsync(string memberName)
        {
            return (await db.SetMembersAsync(memberName)).Select(x => x.ToString()).ToArray();
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            var keys = new HashSet<RedisKey>();

            var endPoints = db.Multiplexer.GetEndPoints();
            
            foreach (var endpoint in endPoints)
            {
                var dbKeys = db.Multiplexer.GetServer(endpoint).Keys(database: db.Database, pattern: pattern);

                foreach (var dbKey in dbKeys)
                {
                    if (!keys.Contains(dbKey))
                    {
                        keys.Add(dbKey);
                    }
                }
            }

            return keys.Select(x => (string)x);
        }

        public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            return Task.Factory.StartNew(() => SearchKeys(pattern));
        }
    }
}

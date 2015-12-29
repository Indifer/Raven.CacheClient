using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raven.Serializer;
using StackExchange.Redis;

namespace Raven.CacheClient
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        IDatabase Database { get; }

        /// <summary>
        /// 
        /// </summary>
        IDataSerializer Serializer { get; }

        /// <summary>
		/// Verify that the specified cache key exists
		/// </summary>
		/// <param name="key">The cache key.</param>
		/// <returns>True if the key is present into Redis. Othwerwise False</returns>
		bool Exists(string key);

        /// <summary>
        /// Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the key is present into Redis. Othwerwise False</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key has removed. Othwerwise False</returns>
        bool Remove(string key);

        /// <summary>
        /// Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key has removed. Othwerwise False</returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        void RemoveAll(IEnumerable<string> keys);

        /// <summary>
        /// Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        Task RemoveAllAsync(IEnumerable<string> keys);

        /// <summary>
        /// Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>Null if not present, otherwise the instance of T.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>Null if not present, otherwise the instance of T.</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>True if the object has been added. Otherwise false</returns>
        bool Add<T>(string key, T value);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>True if the object has been added. Otherwise false</returns>
        Task<bool> AddAsync<T>(string key, T value);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        bool Replace<T>(string key, T value);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        Task<bool> ReplaceAsync<T>(string key, T value);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        bool Add<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        bool Replace<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        bool Add<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        bool Replace<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        /// True if the object has been added. Otherwise false
        /// </returns>
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Get the objects with the specified keys from Redis database with one roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns>
        /// Empty list if there are no results, otherwise the instance of T.
        /// If a cache key is not present on Redis the specified object into the returned Dictionary will be null
        /// </returns>
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        /// <summary>
        /// Get the objects with the specified keys from Redis database with a single roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns>
        /// Empty list if there are no results, otherwise the instance of T.
        /// If a cache key is not present on Redis the specified object into the returned Dictionary will be null
        /// </returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        /// <summary>
        /// Add the objects with the specified keys to Redis database with a single roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="items">The items.</param>
        bool AddAll<T>(IList<Tuple<string, T>> items);

        /// <summary>
        /// Add the objects with the specified keys to Redis database with a single roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="items">The items.</param>
        Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items);

        /// <summary>
        /// Run SADD command <see cref="http://redis.io/commands/sadd"/>
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="key">The key.</param>
        bool SetAdd(string memberName, string key);

        /// <summary>
        /// Run SADD command <see cref="http://redis.io/commands/sadd"/>
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="key">The key.</param>
        Task<bool> SetAddAsync(string memberName, string key);

        /// <summary>
        /// Run SMEMBERS command <see cref="http://redis.io/commands/SMEMBERS"/>
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        string[] SetMember(string memberName);

        /// <summary>
        /// Run SMEMBERS command <see cref="http://redis.io/commands/SMEMBERS"/>
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        Task<string[]> SetMemberAsync(string memberName);

        /// <summary>
        /// Searches the keys from Redis database
        /// </summary>
        /// <remarks>
        /// Consider this as a command that should only be used in production environments with extreme care. It may ruin performance when it is executed against large databases
        /// </remarks>
        /// <param name="pattern">The pattern.</param>
        /// <example>
        ///		if you want to return all keys that start with "myCacheKey" uses "myCacheKey*"
        ///		if you want to return all keys that contain with "myCacheKey" uses "*myCacheKey*"
        ///		if you want to return all keys that end with "myCacheKey" uses "*myCacheKey"
        /// </example>
        /// <returns>A list of cache keys retrieved from Redis database</returns>
        IEnumerable<string> SearchKeys(string pattern);

        /// <summary>
        /// Searches the keys from Redis database
        /// </summary>
        /// <remarks>
        /// Consider this as a command that should only be used in production environments with extreme care. It may ruin performance when it is executed against large databases
        /// </remarks>
        /// <param name="pattern">The pattern.</param>
        /// <example>
        ///		if you want to return all keys that start with "myCacheKey" uses "myCacheKey*"
        ///		if you want to return all keys that contain with "myCacheKey" uses "*myCacheKey*"
        ///		if you want to return all keys that end with "myCacheKey" uses "*myCacheKey"
        /// </example>
        /// <returns>A list of cache keys retrieved from Redis database</returns>
        Task<IEnumerable<string>> SearchKeysAsync(string pattern);
    }
}

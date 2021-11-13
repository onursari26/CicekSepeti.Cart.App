using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CicekSepeti.Core.Cache
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redisConnection;
        public RedisService(IConfiguration configuration)
        {
            ConfigurationOptions config = new()
            {
                EndPoints =
                {
                    { configuration["Redis:Host"], int.Parse(configuration["Redis:Port"]) },
                },
                User = configuration["Redis:User"],
                Password = configuration["Redis:Password"]
            };

            _redisConnection = ConnectionMultiplexer.ConnectAsync(config).Result;
        }

        private IDatabase GetDatabase()
        {
            return _redisConnection.GetDatabase(0);
        }

        public async Task<bool> SetAsync(object key, object value, double keyExpire = -1)
        {
            if (key == null)
                return false;

            var redisKey = key.ToString();

            if (string.IsNullOrWhiteSpace(redisKey))
                return false;

            IDatabase redisDatabase = GetDatabase();

            return await redisDatabase.StringSetAsync(redisKey, JsonConvert.SerializeObject(value), (keyExpire != -1 ? TimeSpan.FromHours(keyExpire) : (TimeSpan?)null));
        }

        public async Task<T> GetAsync<T>(object key)
        {
            if (key == null)
                return default;

            var redisKey = key.ToString();

            if (string.IsNullOrWhiteSpace(redisKey))
                return default;

            IDatabase redisDatabase = GetDatabase();

            if (!await redisDatabase.KeyExistsAsync(redisKey))
                return default;

            var result = await redisDatabase.StringGetAsync(redisKey);

            return !result.HasValue ? default : JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<bool> RemoveAsync(object key)
        {
            if (key == null)
                return false;

            var redisKey = key.ToString();

            if (string.IsNullOrWhiteSpace(redisKey))
                return false;

            IDatabase redisDatabase = GetDatabase();

            if (!await redisDatabase.KeyExistsAsync(redisKey))
                return false;

            return await redisDatabase.KeyDeleteAsync(redisKey);
        }

    }
}

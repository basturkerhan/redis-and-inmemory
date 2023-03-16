using StackExchange.Redis;

namespace RedisInMemory.RedisExchangeAPIApp.Web.Services
{
    public class RedisService
    {
        private ConnectionMultiplexer _redis;

        public IDatabase db { get; set; }

        public RedisService(IConfiguration configuration)
        {
            var configString = $"{configuration["Redis:Host"]}:{configuration["Redis:Port"]}";
            _redis = ConnectionMultiplexer.Connect(configString);
        }

        public IDatabase GetDatabase(int db)
        {
            return _redis.GetDatabase(db);
        }


    }
}

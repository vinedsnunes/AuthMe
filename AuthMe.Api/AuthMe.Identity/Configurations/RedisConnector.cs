using StackExchange.Redis;

namespace AuthMe.Identity.Configurations
{
    public class RedisConnector
    {
        private readonly ConnectionMultiplexer _connection;
        public RedisConnector()
        {
            string redisConnectionString = "localhost:6379"; // Altere para a configuração correta do seu Redis Server
            _connection = ConnectionMultiplexer.Connect(redisConnectionString);
        }

        public IDatabase GetDatabase() { return _connection.GetDatabase(); }
    }
}

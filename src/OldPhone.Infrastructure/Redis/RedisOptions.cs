namespace OldPhone.Infrastructure.Redis
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string InstanceName { get; set; } = string.Empty;

        public int DatabaseNumber { get; set; } = 0;
    }
} 
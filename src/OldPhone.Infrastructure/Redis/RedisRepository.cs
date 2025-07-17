using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OldPhone.Core.Shared.Entities;
using OldPhone.Infrastructure.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace OldPhone.Infrastructure.Redis
{
    /// <summary>
    /// Redis-based implementation of the generic repository.
    /// </summary>
    public class RedisRepository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly ILogger<RedisRepository<T, TKey>> _logger;
        private readonly RedisOptions _options;
        private readonly IDatabase _database;
        private readonly string _keyPrefix;


        /// <summary>
        /// Initializes a new instance of the <see cref="RedisRepository{T, TKey}"/> class.
        /// </summary>
        /// <param name="redis">The Redis connection multiplexer.</param>
        public RedisRepository(
            IConnectionMultiplexer redis,
            ILogger<RedisRepository<T, TKey>> logger,
            IOptions<RedisOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _database = redis.GetDatabase(_options.DatabaseNumber);
            _keyPrefix = $"{_options.InstanceName}:{typeof(T).Name.ToLowerInvariant()}:";

        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            try
            {
                var key = $"{_keyPrefix}{id}";
                var value = await _database.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    _logger?.LogDebug("Entity of type {EntityType} with ID {Id} not found in Redis.", typeof(T).Name, id);
                    return null;
                }

                var message = JsonSerializer.Deserialize<T>(value!);
                _logger?.LogDebug("Retrieved entity of type {EntityType} with ID {Id} from Redis.", typeof(T).Name, id);

                return message;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving entity of type {EntityType} with ID {Id} from Redis.", typeof(T).Name, id);
                throw;
            }

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{_keyPrefix}*");
                var entities = new List<T>();

                foreach (var key in keys)
                {
                    var value = await _database.StringGetAsync(key);
                    if (!value.IsNullOrEmpty)
                        entities.Add(JsonSerializer.Deserialize<T>(value!)!);
                }

                _logger.LogDebug("Retrieved {Count} entities of type {EntityType} from Redis.", entities.Count, typeof(T).Name);
                return entities;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all entities of type {EntityType} from Redis.", typeof(T).Name);
                throw;
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity.CreatedAt == default)
                    entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                var value = JsonSerializer.Serialize(entity);
                await _database.StringSetAsync($"{_keyPrefix}:{entity.Id}", value);
                _logger?.LogInformation("Added entity of type {EntityType} with ID {Id} to Redis.", typeof(T).Name, entity.Id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding entity of type {EntityType} with ID {Id} to Redis.", typeof(T).Name, entity.Id);
                throw;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                var existing = await GetByIdAsync(entity.Id);
                if (existing == null)
                {
                    _logger?.LogWarning("Entity of type {EntityType} with ID {Id} not found for update in Redis.", typeof(T).Name, entity.Id);
                    throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {entity.Id} not found.");
                }

                entity.UpdatedAt = DateTime.UtcNow;

                var value = JsonSerializer.Serialize(entity);
                await _database.StringSetAsync($"{_keyPrefix}:{entity.Id}", value);

                _logger?.LogInformation("Updated entity of type {EntityType} with ID {Id} in Redis.", typeof(T).Name, entity.Id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating entity of type {EntityType} with ID {Id} in Redis.", typeof(T).Name, entity.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            try
            {
                var result = await _database.KeyDeleteAsync($"{_keyPrefix}:{id}");

                if (result)
                {
                    _logger?.LogInformation("Deleted entity of type {EntityType} with ID {Id} from Redis.", typeof(T).Name, id);
                }
                else
                {
                    _logger?.LogWarning("Entity of type {EntityType} with ID {Id} not found for deletion in Redis.", typeof(T).Name, id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting entity of type {EntityType} with ID {Id} from Redis.", typeof(T).Name, id);
                throw;
            }
        }
    }
}

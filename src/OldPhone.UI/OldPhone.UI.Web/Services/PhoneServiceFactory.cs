using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OldPhone.Core.Processor.Services;
using System.Collections.Concurrent;

namespace OldPhone.UI.Web.Services
{
    /// <summary>
    /// Factory for creating and managing phone service instances
    /// Supports multiple sessions with separate phone service instances
    /// </summary>
    public interface IPhoneServiceFactory
    {
        /// <summary>
        /// Gets or creates a phone service for a specific session
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        /// <returns>Phone service instance</returns>
        IOldPhoneKeyService GetPhoneService(string sessionId);

        /// <summary>
        /// Removes a phone service for a specific session
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        void RemovePhoneService(string sessionId);

        /// <summary>
        /// Gets all active session IDs
        /// </summary>
        /// <returns>Collection of active session IDs</returns>
        IEnumerable<string> GetActiveSessions();

        /// <summary>
        /// Gets the number of active sessions
        /// </summary>
        /// <returns>Number of active sessions</returns>
        int GetActiveSessionCount();
    }

    /// <summary>
    /// Implementation of phone service factory with session management
    /// </summary>
    public class PhoneServiceFactory : IPhoneServiceFactory, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PhoneServiceFactory> _logger;
        private readonly ConcurrentDictionary<string, IOldPhoneKeyService> _phoneServices;
        private readonly object _lockObject = new object();

        public PhoneServiceFactory(IServiceProvider serviceProvider, ILogger<PhoneServiceFactory> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _phoneServices = new ConcurrentDictionary<string, IOldPhoneKeyService>();
        }

        public IOldPhoneKeyService GetPhoneService(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
            }

            return _phoneServices.GetOrAdd(sessionId, CreatePhoneService);
        }

        public void RemovePhoneService(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            if (_phoneServices.TryRemove(sessionId, out var phoneService))
            {
                try
                {
                    phoneService?.Dispose();
                    _logger.LogInformation("Removed phone service for session: {SessionId}", sessionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing phone service for session: {SessionId}", sessionId);
                }
            }
        }

        public IEnumerable<string> GetActiveSessions()
        {
            return _phoneServices.Keys.ToList();
        }

        public int GetActiveSessionCount()
        {
            return _phoneServices.Count;
        }

        private IOldPhoneKeyService CreatePhoneService(string sessionId)
        {
            try
            {
                // Create a new scope for each phone service to ensure proper lifecycle management
                var scope = _serviceProvider.CreateScope();
                var phoneService = scope.ServiceProvider.GetRequiredService<IOldPhoneKeyService>();
                
                _logger.LogInformation("Created phone service for session: {SessionId}", sessionId);
                
                return phoneService;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating phone service for session: {SessionId}", sessionId);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                foreach (var kvp in _phoneServices)
                {
                    try
                    {
                        kvp.Value?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing phone service for session: {SessionId}", kvp.Key);
                    }
                }

                _phoneServices.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during PhoneServiceFactory disposal");
            }
        }
    }
} 
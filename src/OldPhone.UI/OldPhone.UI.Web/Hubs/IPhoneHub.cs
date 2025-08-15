using OldPhone.UI.Shared.Models.SignalR;
using ConnectionInfo = OldPhone.UI.Shared.Models.SignalR.ConnectionInfo;

namespace OldPhone.UI.Web.Hubs
{
    /// <summary>
    /// Strongly typed interface for PhoneHub client methods
    /// Provides type safety and IntelliSense for hub-to-client communication
    /// </summary>
    public interface IPhoneHub
    {
        /// <summary>
        /// Sent when a client successfully connects to the hub
        /// </summary>
        /// <param name="data">Connection information</param>
        Task Connected(ConnectionInfo data);

        /// <summary>
        /// Sent when a key is processed by the server
        /// </summary>
        /// <param name="data">Key processing result data</param>
        Task KeyProcessed(KeyProcessedData data);

        /// <summary>
        /// Sent when text is completed (send button pressed)
        /// </summary>
        /// <param name="data">Text completion data</param>
        Task TextCompleted(TextCompletedData data);

        /// <summary>
        /// Sent when text is cleared
        /// </summary>
        /// <param name="data">Text clear data</param>
        Task TextCleared(TextClearedData data);

        /// <summary>
        /// Sent when a client joins a session
        /// </summary>
        /// <param name="data">Session join data</param>
        Task ClientJoinedSession(SessionJoinData data);

        /// <summary>
        /// Sent when a client leaves a session
        /// </summary>
        /// <param name="data">Session leave data</param>
        Task ClientLeftSession(SessionLeaveData data);

        /// <summary>
        /// Sent when current state is requested
        /// </summary>
        /// <param name="data">Current state data</param>
        Task CurrentState(CurrentStateData data);

        /// <summary>
        /// Sent when server statistics are requested
        /// </summary>
        /// <param name="data">Server statistics data</param>
        Task ServerStatistics(ServerStatisticsData data);

        /// <summary>
        /// Sent when an error occurs
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        Task Error(string errorMessage);
    }
}

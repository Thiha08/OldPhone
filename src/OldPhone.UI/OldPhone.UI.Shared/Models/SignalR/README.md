# SignalR Models Organization

This directory contains the organized SignalR data models used for real-time communication between the OldPhone client applications and the SignalR hub.

## 📁 **File Structure**

```
Models/SignalR/
├── BaseSignalRModel.cs      # Base models for common properties
├── ConnectionModels.cs      # Connection-related models
├── TextProcessingModels.cs  # Text processing models
├── SessionModels.cs         # Session management models
├── StateModels.cs          # State and statistics models
└── README.md               # This documentation
```

## 🏷️ **Model Categories**

### 0. **Base Models** (`BaseSignalRModel.cs`)
- **`BaseSignalRModel`**: Base model for models that need both SessionId and Timestamp
  - `SessionId`: Session identifier for grouping related operations
  - `Timestamp`: When the event occurred
- **`BaseTimestampModel`**: Base model for models that only need Timestamp
  - `Timestamp`: When the event occurred

### 1. **Connection Models** (`ConnectionModels.cs`)
- **`ConnectionInfo`**: Information sent when a client connects to the hub
  - `ConnectionId`: Unique identifier for the client connection
  - `Timestamp`: When the connection was established (inherited from BaseTimestampModel)

### 2. **Text Processing Models** (`TextProcessingModels.cs`)
- **`KeyProcessedData`**: Data sent when a key is processed
  - `Key`: The key that was pressed
  - `CurrentText`: The resulting text after processing
  - `ProcessedBy`: Connection ID of the client that processed the key
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the key was processed (inherited from BaseSignalRModel)

- **`TextCompletedData`**: Data sent when text is completed (send button)
  - `CompletedBy`: Connection ID of the client that completed the text
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the text was completed (inherited from BaseSignalRModel)

- **`TextClearedData`**: Data sent when text is cleared
  - `ClearedBy`: Connection ID of the client that cleared the text
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the text was cleared (inherited from BaseSignalRModel)

### 3. **Session Models** (`SessionModels.cs`)
- **`SessionJoinData`**: Data sent when a client joins a session
  - `ConnectionId`: Connection ID of the joining client
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the client joined (inherited from BaseSignalRModel)

- **`SessionLeaveData`**: Data sent when a client leaves a session
  - `ConnectionId`: Connection ID of the leaving client
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the client left (inherited from BaseSignalRModel)

### 4. **State Models** (`StateModels.cs`)
- **`CurrentStateData`**: Current state information
  - `CurrentText`: Current text in the session
  - `SessionId`: Session identifier (inherited from BaseSignalRModel)
  - `Timestamp`: When the state was captured (inherited from BaseSignalRModel)

- **`ServerStatisticsData`**: Server statistics information
  - `ActiveConnections`: Number of active connections
  - `ActiveSessions`: Number of active sessions
  - `Timestamp`: When the statistics were captured (inherited from BaseTimestampModel)

## 🔄 **Migration from Old Structure**

The models were previously located in `Services/SignalR/PhoneHubModels.cs` and have been reorganized for better maintainability:

### **Before:**
```csharp
// All models in one file
namespace OldPhone.UI.Shared.Services.SignalR
{
    public record ConnectionInfo { ... }
    public record KeyProcessedData { ... }
    // ... all other models
}
```

### **After:**
```csharp
// Organized by category
namespace OldPhone.UI.Shared.Models.SignalR
{
    // Each model in its appropriate file
}
```

## 📝 **Usage**

### **Importing Models**
```csharp
using OldPhone.UI.Shared.Models.SignalR;
```

### **Creating Model Instances**
```csharp
// Models with SessionId and Timestamp (inherit from BaseSignalRModel)
var keyProcessedData = new KeyProcessedData
{
    Key = "A",
    CurrentText = "A",
    ProcessedBy = "connection-123",
    SessionId = "session-456",
    Timestamp = DateTime.UtcNow
};

// Models with only Timestamp (inherit from BaseTimestampModel)
var connectionInfo = new ConnectionInfo
{
    ConnectionId = "connection-123",
    Timestamp = DateTime.UtcNow
};

var serverStats = new ServerStatisticsData
{
    ActiveConnections = 5,
    ActiveSessions = 3,
    Timestamp = DateTime.UtcNow
};
```

### **Base Model Benefits**
- **DRY Principle**: No more duplicate SessionId and Timestamp properties
- **Consistency**: All models with the same base have consistent property names
- **Maintainability**: Changes to base properties automatically apply to all derived models
- **Type Safety**: Compile-time checking ensures proper inheritance

## 🎯 **Benefits of This Organization**

1. **Better Maintainability**: Related models are grouped together
2. **Easier Navigation**: Developers can quickly find specific model types
3. **Reduced File Size**: Each file contains a focused set of models
4. **Clear Separation of Concerns**: Models are organized by their purpose
5. **Improved Readability**: Smaller, focused files are easier to read and understand

## 🔧 **Backward Compatibility**

The old `PhoneHubModels.cs` file has been updated with a deprecation notice and will be removed in future versions. All existing code has been updated to use the new namespace and file structure. 
# NETWORKING.md – StockTV Communication (Critical)

This document explains the StockTV network communication layer. **This is mission-critical** — tournaments depend on reliable hardware synchronization.

---

## Overview: Three Layers

```
UI (StockApp.UI)
    ↓ asks for discovery
StockTVService (StockApp.Comm)
    ├── mDNS Discovery (finds displays on network)
    └── StockTV instances (each holds a NetMQ client)
        └── StockTVAppClient (NetMQ Pub-Sub)
            ↓ sends/receives
        StockTV display hardware
```

---

## Layer 1: Discovery (mDNS / Zeroconf)

### What it does
Automatically finds StockTV-compatible displays on the local network without manual IP configuration.

### How it works

**MDnsService** (`StockApp.Comm/MDns/MDnsService.cs`):
```csharp
private readonly string _protocol = "_stockTV._tcp.local.";

public void Discover()
{
    var sub = ZeroconfResolver.Resolve(_protocol, TimeSpan.FromSeconds(5), 5, 500);
    _listenSubscription = sub.Subscribe(resp => RaiseStockTVDiscoverd(new MDnsHost(resp)));
}
```

- Searches for Bonjour/mDNS service `_stockTV._tcp.local.`
- 5-second timeout, 5 retries, 500ms between retries
- Fires event `StockTVDiscovered` for each found device
- Results: `IMDnsHost` with hostname, IP, port (4747 + 4748)

**Manual Fallback:**
```csharp
public void AddManual(string hostname, string ipAddress)
{
    var host = MDnsHost.Create(hostname, ipAddress, 4747, 4748, "n.a.");
    AddNewStockTV(host);
}
```

When mDNS fails (isolated networks, misconfigured displays), manually add by IP.

### Critical Details

- **Protocol**: `_stockTV._tcp.local.` (Bonjour/mDNS standard)
- **Ports**: 4747 (command), 4748 (telemetry)
- **Timeout**: 5 seconds per discovery cycle
- **Persistence**: Subscriptions stay active until `Stop()` is called

### Failure Modes

| Issue | Symptom | Fix |
|-------|---------|-----|
| Display not found | Empty StockTV list | Check display is powered on, on same network, advertising service |
| Firewall blocks mDNS (port 5353 UDP) | Discovery times out | Allow UDP 5353 in firewall |
| Multiple networks | Finds wrong display | Use manual IP entry |
| Display IP changed | Stale connection | Re-run discovery |

---

## Layer 2: StockTVService (Orchestration)

### What it does
Manages all discovered StockTV devices, coordinates commands to multiple displays (e.g., "sync all displays to same round").

### Structure

**StockTVService** (`StockApp.Comm/NetMqStockTV/StockTVService.cs`):

```csharp
public class StockTVService : IStockTVService
{
    private readonly List<IStockTV> _stockTvList;  // All connected displays
    private readonly IMdnsService _mdnsService;     // Discovery
    private readonly object _lock = new();          // Thread-safe access
}
```

### Key Methods

**Discover displays:**
```csharp
public void Discover() => _mdnsService.Discover();
```
- Called once at startup (from `App.xaml.cs`)
- Subscribes to mDNS results, adds each found display

**Add manually:**
```csharp
public void AddManual(string hostname, string ipAddress)
```
- Fallback when mDNS fails
- Creates display without discovery

**Send to all displays:**
```csharp
private void AddNewStockTV(IMDnsHost mDns)
{
    // Create StockTV instance with NetMQ client
    IStockTV newTV = new StockTV(mDns);
    
    // Subscribe to events from this display
    newTV.StockTVResultChanged += (s, e) => RaiseStockTVResultChanged(s as IStockTV, e);
    
    // Add to collection
    _stockTvList.Add(newTV);
}
```

### Display Coordination (Director Pattern)

One display can be "director" — when director settings change, propagate to all other displays:

```csharp
StockTVDirectorChangedHandler = (s, e) =>
{
    if (s is IStockTV stockTV && e == true)
    {
        foreach (IStockTV tv in _stockTvList.Where(t => t != stockTV))
        {
            tv.Director = false;  // Only one director at a time
        }
    }
};

StockTVSettingsChangedHandler = (sender, e) =>
{
    if (sender is IStockTV s && s.Director)
    {
        object newValue = typeof(IStockTVSettings).GetProperty(e.PropertyName)
            .GetValue(s.TVSettings);
        
        foreach (IStockTV stockTV in _stockTvList.Where(t => !t.Director))
        {
            typeof(IStockTVSettings).GetProperty(e.PropertyName)
                .SetValue(stockTV.TVSettings, newValue);
        }
    }
};
```

**Important**: Director changes are **NOT automatically synced** — only when the director property itself is modified.

### Thread Safety

```csharp
private readonly object _lock = new();

lock (_lock)
{
    if (!_stockTvList.Any(s => s.IPAddress == mDns.IPAddress))
    {
        _stockTvList.Add(newTV);
        RaiseStockTVCollectionChanged(added: true);
    }
}
```

All accesses to `_stockTvList` must use lock to prevent race conditions when displays are discovered/disconnected while UI is reading.

---

## Layer 3: NetMQ Communication (Per-Display)

### What it does
Sends/receives messages to **one StockTV display** via ZeroMQ Pub-Sub pattern.

### Architecture

**StockTVAppClient** (`StockApp.Comm/NetMqStockTV/StockTVAppClient.cs`):

```csharp
private NetMQPoller _poller;           // Event loop (non-blocking)
private DealerSocket _socket;          // ZeroMQ DEALER socket
private NetMQMonitor _monitor;         // Connection state tracker
private NetMQQueue<List<NetMQFrame>> _sendQueue;    // Outbound queue
private NetMQQueue<NetMQMessage> _receiveQueue;     // Inbound queue
```

### Communication Pattern

**ZeroMQ Socket Type: DEALER**

The DEALER socket is asynchronous request-reply without enforcing strict pairing:
- Client can send multiple messages without waiting for reply
- Server replies in order (but async)
- Auto-reconnection on connection loss

### Message Flow

```
UI Layer (ViewModel)
    ↓ calls
StockTV.SendGameResult(...)
    ↓ calls
StockTVAppClient.SendToStockTV(topic, data)
    ↓ puts into queue
_sendQueue.Enqueue(frames)
    ↓ poller picks up
NetMQPoller (background thread)
    ↓ sends via socket
DealerSocket → StockTV Display (network)
    ↓ display processes & replies
Reply message
    ↓ poller receives
_receiveQueue
    ↓ dispatches event
RaiseMessageReceived(...)
    ↓ caught by
StockTV instance → UI
```

### Starting the Connection

```csharp
public void Start()
{
    _poller = new NetMQPoller();
    
    _socket = new DealerSocket();
    _socket.Options.Linger = TimeSpan.FromSeconds(10);
    _socket.Connect($"tcp://{_iPAddress}:{_port}");
    
    _monitor = new NetMQMonitor(_socket, $"inproc://clientmonitor-{_identifier}");
    _monitor.Connected += (s, e) => IsConnected = true;
    _monitor.Disconnected += (s, e) => IsConnected = false;
    _monitor.Start();
    
    _poller.Add(_socket);
    _poller.Run();  // Blocking loop
}
```

**Critical**: `_poller.Run()` blocks the calling thread. Must be called from a background thread or async context.

### Sending Messages

**Three send methods:**

```csharp
// 1. Send bytes + optional additional bytes
public void SendToStockTV(MessageTopic topic, byte[] value, byte[] additionalValue = null)
{
    var frames = new List<NetMQFrame>
    {
        new NetMQFrame(topic.ToString()),
        new NetMQFrame(value),
    };
    if (additionalValue != null)
        frames.Add(new NetMQFrame(additionalValue));
    
    _sendQueue.Enqueue(frames);
}

// 2. Send string
public void SendToStockTV(MessageTopic topic, string value)
{
    var frames = new List<NetMQFrame>
    {
        new NetMQFrame(topic.ToString()),
        new NetMQFrame(Encoding.UTF8.GetBytes(value)),
    };
    _sendQueue.Enqueue(frames);
}

// 3. Send topic only
public void SendToStockTV(MessageTopic topic)
{
    var frames = new List<NetMQFrame>
    {
        new NetMQFrame(topic.ToString()),
    };
    _sendQueue.Enqueue(frames);
}
```

All sends go to a **queue** (not direct socket write) — thread-safe without locks.

### Receiving Messages

Poller listens for replies:

```csharp
_poller.Add(_socket, (socket) =>
{
    var message = socket.ReceiveMultipartMessage();
    if (message.FrameCount >= 2)
    {
        var topic = message[0];
        var value = message[1];
        RaiseMessageReceived(topic, value);
    }
});
```

**Frame format**: Topic (string) + Value (bytes) + optional additional data.

### Connection State Tracking

NetMQMonitor fires events:

```csharp
_monitor.Connected += (s, e) => 
{
    IsConnected = true;
    RaiseConnectedChanged();
};

_monitor.Disconnected += (s, e) => 
{
    IsConnected = false;
    RaiseConnectedChanged();
};
```

UI binds to `IsConnected` — shows green/red indicator.

---

## Critical Failure Modes

| Symptom | Root Cause | Fix |
|---------|-----------|-----|
| Discovery finds 0 displays | mDNS not working, firewall | Check network, use manual IP |
| "Connected" but commands don't reach display | Display disconnected mid-game | Reconnect display, retry command |
| Display shows stale data | Message lost, no retry logic | Re-send command from UI |
| Multiple displays out of sync | Director changes not propagated | Click director again |
| Poller blocks entire app | Mistake: calling `Start()` on UI thread | Use background task/thread |
| Memory leak when display disconnected | StockTVAppClient not disposed | Ensure StockTV.Dispose() called |

---

## Disposal & Cleanup

**Critical**: Must properly dispose in reverse order.

```csharp
public void Dispose(bool disposing)
{
    if (!_disposed)
    {
        if (disposing)
        {
            Stop();  // Stops poller, closes socket
            _socket?.Dispose();
            _monitor?.Dispose();
            _sendQueue?.Dispose();
            _receiveQueue?.Dispose();
        }
        _disposed = true;
    }
}

public void Stop()
{
    _poller?.Stop();
    _poller?.Dispose();
    _poller = null;
    
    _socket?.Disconnect($"tcp://{_iPAddress}:{_port}");
    
    RaiseConnectedChanged();  // Notify UI
}
```

**When to dispose:**
- User closes app
- Tournament ends
- Display disconnected manually

**Don't forget**: `TurnierNetworkManager` holds references to StockTVService → must be disposed in `MainViewModel.Dispose()`.

---

## Configuration

### Port Numbers

- **4747** (TCP): Command channel (send game results)
- **4748** (TCP): Telemetry channel (receive status updates)

### Timeouts

- **mDNS discovery**: 5 seconds (hardcoded)
- **NetMQ socket linger**: 10 seconds (graceful close)
- **Monitor poll interval**: Implementation-dependent (typically 100ms)

### Message Topics (Enum)

Defined in `StockTVEnumerations.cs`:
```csharp
public enum MessageTopic
{
    GameResult,
    Settings,
    Alive,
    // ... more
}
```

Send messages by enum value → converted to string on wire.

---

## Testing & Debugging

### Enable Detailed Logging

In `App.xaml.cs`, configure log4net:

```csharp
LogConfigurator.Configure();
// Then set level to DEBUG
```

Watch for:
- `StockTV Discovered: ...` – mDNS working
- `MessageTopic.X received` – commands reaching display
- `Connected` / `Disconnected` – socket state changes

### Manual Testing

```csharp
var service = new StockTVService();
service.Discover();  // Wait 5 seconds

foreach (var tv in service.StockTVCollection)
{
    Console.WriteLine($"Found: {tv.HostName} ({tv.IPAddress})");
}

// Send test message
var firstTV = service.StockTVCollection.First();
firstTV.SendGameResult(gameNumber: 1, teamAPoints: 5, teamBPoints: 3);
```

### Network Sniffing

If communication fails:
```bash
# macOS / Linux
tcpdump -i en0 'port 4747 or port 4748'

# Windows (Wireshark)
# Filter: tcp.port == 4747 || tcp.port == 4748
```

Look for:
- TCP 3-way handshake on 4747
- Message frames (should see binary data)
- TCP FIN/RST (disconnections)

---

## Integration with TurnierStore

**TurnierNetworkManager** (`StockApp.UI/Services/TurnierNetworkManager.cs`):

```csharp
public class TurnierNetworkManager
{
    private readonly ITurnierStore _turnierStore;
    private readonly IStockTVService _stockTVService;
    private readonly IBroadcastService _broadCastService;
}
```

Listens to `TurnierStore` game result changes → sends to all connected StockTV displays:

```csharp
_turnierStore.TurnierChanged += (s, e) => 
{
    var results = GetLatestResults();
    foreach (var tv in _stockTVService.StockTVCollection)
    {
        tv.SendGameResult(results);
    }
};
```

**Remember**: Network errors here are silent (no UI crash). Monitor logs for failures.


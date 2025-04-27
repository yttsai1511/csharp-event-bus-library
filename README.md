# C# Event Bus Library

一個簡單、彈性且強大的 C# 事件管理工具，適用於需要動態訂閱、取消訂閱和發布事件的場景，支援多參數的事件處理。

## 功能特色

- **泛型支援**：靈活處理各類型的委託，支援多參數的 Action 委託。
- **線程安全**：多線程環境下操作安全可靠，避免資源競爭。
- **彈性設計**：動態管理事件，支援委託的訂閱、取消訂閱與事件清除。
- **快速上手**：API 設計簡單直觀，配合完整範例快速整合於實際專案中。

## 使用範例

### 1. 訂閱與發布無參數事件

訂閱一個無參數事件，用於更新計數器：

```csharp
var eventBus = new EventBus();
int counter = 0;

// 訂閱無參數事件
eventBus.Subscribe<Action>("UpdateCounter", () => counter++);

// 發布事件
eventBus.Publish("UpdateCounter");
eventBus.Publish("UpdateCounter");

// 現在 counter 的值為 2
```

---

### 2. 訂閱與發布帶參數的事件

訂閱一個帶參數的事件，用於記錄使用者操作：

```csharp
var eventBus = new EventBus();
var userActions = new List<string>();

// 訂閱帶參數事件
eventBus.Subscribe<Action<string>>("TrackUserAction", action => userActions.Add(action));

// 發布事件
eventBus.Publish("TrackUserAction", "User clicked button A");
eventBus.Publish("TrackUserAction", "User selected item B");

// 現在 userActions 包含：
// ["User clicked button A", "User selected item B"]
```

---

### 3. 動態管理事件訂閱與取消

展示如何訂閱事件、取消特定委託、取消整個事件，並清空所有事件：

```csharp
var eventBus = new EventBus();
var logMessages = new List<string>();

// 訂閱事件
Action<string> logHandler = message => logMessages.Add(message);
eventBus.Subscribe("LogEvent", logHandler);

// 發布事件
eventBus.Publish("LogEvent", "This is a log message");
// logMessages 包含 ["This is a log message"]

// 取消訂閱特定委託
eventBus.Unsubscribe("LogEvent", logHandler);

// 目標事件無委託，發布無效果
eventBus.Publish("LogEvent", "This will not be logged");

// 再次訂閱事件並發布
eventBus.Subscribe("LogEvent", logHandler);
eventBus.Publish("LogEvent", "Another log message");
// logMessages 現在包含 ["This is a log message", "Another log message"]

// 取消訂閱整個事件
eventBus.Unsubscribe("LogEvent");

// 目標事件已移除，發布無效果
eventBus.Publish("LogEvent", "This won't be logged either");

// 訂閱另一個事件
eventBus.Subscribe<Action>("SampleEvent", () => logMessages.Add("SampleEvent triggered"));

// 清空所有事件
eventBus.Clear();

// 所有事件皆已清除，發布無效果
eventBus.Publish("SampleEvent");
```

## 文件說明

### 事件 API

| 方法名稱 | 功能描述 |
| :----- | :----- |
| `Subscribe<TDelegate>` | 訂閱一個新事件與委託 |
| `Unsubscribe<TDelegate>` | 取消特定委託的訂閱 |
| `Unsubscribe` | 移除指定名稱的所有委託 |
| `Clear` | 清除所有已訂閱的事件與委託 |
| `Publish` | 發布指定的事件 |

### 主要結構

- `EventKey`：用於唯一識別每個事件（事件名稱 + 委託型別）。
- `_events`：內部的字典，用於儲存所有已訂閱的事件及委託。

## 授權

此專案基於 GPLv3 授權條款，詳情請參閱 LICENSE 文件。
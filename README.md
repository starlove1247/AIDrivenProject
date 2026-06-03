# AIDrivenProject

亞洲大學 Unity 2D 遊戲專案，實作物品欄系統、開發者 CLI 終端機、雙場景架構與場景淡入淡出切換。

---

## 環境

| 項目 | 版本 |
|------|------|
| Unity | 6000.4.1f1（Unity 6） |
| 渲染管線 | Universal Render Pipeline (URP) 2D |
| Input System | New Input System |
| UI | TextMeshPro |

---

## 場景

| 場景 | Build Index | 說明 |
|------|-------------|------|
| TitleScene | 0 | 標題畫面，點擊 Start 進入主遊戲 |
| MainScene  | 1 | 主遊戲場景，含物品欄、暫停選單、CLI |

場景切換附帶淡入淡出動畫（可透過 CLI `fade` 指令切換開關）。

---

## 功能介紹

### 物品欄系統

- 按 `I` 開關物品欄面板
- 點擊物品格顯示詳細資訊（名稱、描述、圖示）
- 點擊空白處關閉細節
- 物品以 ScriptableObject 定義，易於擴充

**內建物品**

| ID | 名稱 |
|----|------|
| `sword` | 鐵劍 |
| `potion` | 回復藥水 |
| `key` | 神秘鑰匙 |

---

### CLI 終端機

按 `` ` ``（反引號）開關終端機面板。

#### 內建指令

| 指令 | 場景 | 說明 |
|------|------|------|
| `help` | 全場景 | 列出當前場景可用指令 |
| `help <query>` | 全場景 | 模糊過濾指令清單 |
| `clear` | 全場景 | 清除終端機輸出 |
| `scene` | 全場景 | 顯示當前場景名稱 |
| `load <sceneName>` | 全場景 | 切換到指定場景 |
| `fade <on\|off>` | 全場景 | 切換場景淡入淡出效果 |
| `items` | MainScene | 顯示物品欄內容 |
| `itemlist` | MainScene | 列出 Registry 中所有可用物品 |
| `give <id>` | MainScene | 新增物品到物品欄 |
| `drop <id>` | MainScene | 從物品欄移除物品 |
| `inventory <show\|hide>` | MainScene | 控制物品欄面板顯示 |
| `pause` | MainScene | 暫停遊戲 |
| `resume` | MainScene | 繼續遊戲 |
| `title` | MainScene | 返回標題場景 |
| `start` | TitleScene | 進入主遊戲 |

#### 模糊搜尋

輸入錯誤指令時，自動以 Levenshtein 距離比對並顯示建議：

```
> halp
Unknown command: 'halp'
Did you mean: (1) help

> 1
> help
Commands:
  clear
  fade
  help
  ...
```

輸入對應編號可直接執行建議指令。

#### 其他 CLI 功能

- `↑` 方向鍵瀏覽歷史指令（最多 50 條）
- Enter / NumpadEnter 重新聚焦輸入框
- 動態捲動速度（依內容高度自適應）

---

### 暫停選單

- `ESC` 開關暫停選單（MainScene）
- 提供「繼續」與「返回標題」按鈕

---

## 架構說明

### 單例系統（DontDestroyOnLoad）

| 類別 | 路徑 | 說明 |
|------|------|------|
| `SceneLoader` | `Core/SceneLoader.cs` | 非同步場景切換，含淡入淡出 |
| `GameManager` | `Core/GameManager.cs` | ESC 暫停控制 |
| `Inventory` | `Inventory/Inventory.cs` | 物品欄狀態 |
| `CLISystem` | `CLI/CLISystem.cs` | 指令註冊與執行 |
| `ScreenFader` | `Core/ScreenFader.cs` | 全屏淡入淡出遮罩（sortingOrder=32767） |

### CLI Bootstrap

`CLIBootstrap.cs`（`[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`）在任何場景啟動前自動從 `Resources/CLIRoot.prefab` 建立 CLI 根物件，無需在每個場景手動放置。

### 場景指令隔離

`CLISystem.RegisterCommand(name, handler, scene)` 第三參數限定指令所屬場景。`help` 只顯示當前場景可用指令。

---

## 資料夾結構

```
Assets/
  Scripts/
    Core/        SceneLoader, GameManager, ItemRegistry, ScreenFader
    Inventory/   Item (SO), Inventory, InventoryUI, ItemSlotUI
    CLI/         CLISystem, CLICommands, CLIUI, CLIBootstrap
    UI/          TitleUI, PauseMenuUI
  Prefabs/
    Items/       ItemSlot
    UI/          InventoryPanel, CLIPanel, PauseMenuPanel
  Resources/     CLIRoot（Bootstrap prefab）
  ScriptableObjects/
    Items/       Sword, Potion, Key
  Scenes/
    TitleScene, MainScene
```

---

## 鍵位總覽

| 鍵 | 功能 |
|----|------|
| `` ` `` | 開關 CLI 終端機 |
| `I` | 開關物品欄 |
| `ESC` | 暫停 / 繼續（MainScene） |
| `↑` | CLI 歷史指令 |

---

## 新增 CLI 指令

在 `CLICommands.cs` 的 `Register()` 內加入：

```csharp
cli.RegisterCommand("mycommand", args => {
    // args = 指令後的參數陣列
    return "回傳值顯示在終端機";
}, "MainScene"); // 省略第三參數 = 全場景可用
```

## 新增物品

1. 右鍵 `Assets/ScriptableObjects/Items/` → Create → Inventory → Item
2. 填入 `itemId`（小寫唯一值）、`itemName`、`description`、`icon`
3. 將 Item 拖入場景 `ItemRegistry` GameObject 的 `allItems` 陣列

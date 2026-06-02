# AIDrivenProject — CLAUDE.md

## 專案概述
Unity 2D URP 專案，包含物品欄系統、CLI 系統、雙場景切換。

## 環境
- Unity 版本：見 `ProjectSettings/ProjectVersion.txt`
- 渲染管線：Universal Render Pipeline (URP) 2D
- Input System：New Input System (`InputSystem_Actions.inputactions`)
- UI：TextMeshPro (TMPro)
- **Enter Play Mode：已啟用 `DisableDomainReload`**（跳過 Domain Reload，加快進入 Play Mode）

> **注意（新增 static 欄位時必看）：** 因 Domain Reload 停用，static 變數不會在每次進入 Play Mode 時自動歸零。
> 所有 singleton 或含 static 狀態的類別，**必須加上**：
> ```csharp
> [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
> static void ResetStatic() => Instance = null; // 視情況重置所有 static 欄位
> ```
> 已套用的類別：SceneLoader、GameManager、Inventory、CLISystem、ItemRegistry、CLIUI、PauseMenuUI。

## 資料夾結構
```
Assets/
  Scripts/
    Core/        SceneLoader, GameManager, ItemRegistry
    Inventory/   Item (SO), Inventory, InventoryUI, ItemSlotUI
    CLI/         CLISystem, CLICommands, CLIUI
    UI/          TitleUI, PauseMenuUI
  Prefabs/
    UI/          InventoryPanel, CLIPanel, PauseMenu, ItemSlot
  ScriptableObjects/
    Items/       Sword, Potion, Key (Item SO 範例)
```

## 場景
| 場景 | Build Index | 說明 |
|------|-------------|------|
| TitleScene | 0 | 標題畫面，Start 按鈕進入 MainScene |
| MainScene  | 1 | 主遊戲，ESC 暫停選單，可回到 TitleScene |

## 關鍵系統

### SceneLoader（Core/SceneLoader.cs）
- 單例，`DontDestroyOnLoad`
- `LoadScene(string sceneName)` — 非同步切換場景，切換前重設 `Time.timeScale = 1`

### GameManager（Core/GameManager.cs）
- 單例，`DontDestroyOnLoad`
- ESC 鍵觸發 `TogglePause()`，控制 `Time.timeScale` 與 PauseMenuUI 顯示

### Inventory（Inventory/Inventory.cs）
- 單例，`DontDestroyOnLoad`
- `AddItem(Item)`, `RemoveItem(Item)`, `RemoveItemById(string)`, `HasItem(string)`, `FindById(string)`
- `OnInventoryChanged` Action — InventoryUI 訂閱此事件刷新

### ItemRegistry（Core/ItemRegistry.cs）
- 掛在場景內 GameObject，Inspector 填入所有 Item SO
- `ItemRegistry.Find(string id)` — CLI `give` 指令使用

### CLISystem（CLI/CLISystem.cs）
- 單例，`DontDestroyOnLoad`
- `RegisterCommand(name, handler)` — 新增指令
- `Execute(string input)` — 解析並執行

### 內建 CLI 指令
| 指令 | 說明 |
|------|------|
| `help` | 列出所有指令 |
| `items` | 顯示物品欄內容 |
| `give <id>` | 新增物品（需存在於 ItemRegistry） |
| `drop <id>` | 丟棄物品 |
| `clear` | 清除 CLI 輸出 |

### 鍵位
| 鍵 | 功能 |
|----|------|
| `` ` `` (反引號) | 開關 CLI |
| `I` | 開關物品欄 |
| `ESC` | 暫停/繼續（MainScene） |

## 新增 CLI 指令方式
在 `CLICommands.cs` 的 `Register()` 內加：
```csharp
cli.RegisterCommand("mycommand", args => {
    // args = 指令後的參數陣列
    return "回傳值會顯示在 CLI 輸出";
});
```

## 新增 Item 方式
1. 右鍵 Assets/ScriptableObjects/Items → Create → Inventory → Item
2. 填入 `itemId`（小寫唯一值）、`itemName`、`description`、`icon`
3. 將新 Item 拖入場景中 ItemRegistry GameObject 的 `allItems` 陣列

## 開發注意事項
- 所有單例使用 `DontDestroyOnLoad` — 跨場景保持狀態
- 切換場景前 SceneLoader 會強制 `Time.timeScale = 1`（防止暫停狀態殘留）
- TitleScene 不需要 GameManager/Inventory（由 MainScene 的 GameObject 提供）
- 如需在 TitleScene 也使用 CLI，需在 TitleScene 掛 SceneLoader / CLISystem

## TMP 字體設定

### 字體分配
| 用途 | Font Asset | 來源 TTF |
|------|-----------|---------|
| 一般 UI（標題、按鈕、物品欄） | `NotoSans-Medium SDF` | `Assets/fonts/NotoSans-Medium.ttf` |
| CLI 面板（輸入/輸出） | `ProggyClean SDF` | `Assets/fonts/ProggyClean.ttf` |

Font Assets 位於：`Assets/TextMesh Pro/Resources/Fonts & Materials/`  
TMP 全域預設：`TMP Settings.asset` → `m_defaultFontAsset` = NotoSans-Medium SDF

### 中文字（CJK）支援說明
> ⚠️ 專案內的 NotoSans TTF（~600KB）為 **Latin-only** 版本，不含 CJK 字形。
> 物品名稱（鐵劍、回復藥水、神秘鑰匙）等中文文字，在 Play Mode 會顯示為方塊（□）。
>
> **修復方法：** 下載 [NotoSansSC](https://fonts.google.com/noto/specimen/Noto+Sans+SC)（Simplified）或 [NotoSansTC](https://fonts.google.com/noto/specimen/Noto+Sans+TC)（Traditional，~8MB）放入 `Assets/fonts/`，
> 建立 TMP Dynamic Font Asset 後，在 `NotoSans-Medium SDF` Inspector → Fallback Font Assets 加入該字體即可。

### 新增 TMP UI 文字元件規則
1. 非 CLI 區域 → Font Asset 選 `NotoSans-Medium SDF`
2. CLIPanel 子節點 → Font Asset 選 `ProggyClean SDF`
3. 禁止直接用 `t.font = target`（呼叫 `set_font` setter 會觸發 `LoadFontAsset` 嘗試讀取 Dynamic Atlas Texture，可能拋出 Texture2D destroyed 錯誤）
4. 用 execute-dynamic-code 批次修改字體時，改用 `SerializedObject.FindProperty("m_fontAsset").objectReferenceValue` + `ApplyModifiedPropertiesWithoutUndo()`

## execute-dynamic-code 已知限制

### `Object` 模糊引用（CS0104）
在 `execute-dynamic-code` 的 snippet 中，`using UnityEngine` 已由 wrapper 自動加入。
若 snippet 也包含 `using UnityEngine`，直接寫 `Object.FindObjectsByType<T>()` 會產生：
```
CS0104: 'Object' 是 'UnityEngine.Object' 與 'object' 之間模糊的參考
```
**修正：** 使用完整限定名 `UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)`

### `GlyphRenderMode` 命名空間
`GlyphRenderMode` 實際在 `UnityEngine.TextCore.LowLevel`，非 `TMPro`。
在 snippet 加入 `using UnityEngine.TextCore.LowLevel;` 避免 auto-resolve。

## uloop 常用指令
```bash
uloop compile                          # 編譯並回報錯誤
uloop get-logs --log-type Error        # 查看錯誤
uloop screenshot                       # 截圖確認 UI
uloop control-play-mode --action start # 進入 PlayMode
uloop control-play-mode --action stop  # 停止 PlayMode
```

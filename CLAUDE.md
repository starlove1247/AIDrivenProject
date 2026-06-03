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

## 偵錯工作流程規則

### 優先靜態分析，延後 PlayMode
**進入 PlayMode 耗時顯著（Domain Reload、編譯、等待穩定）。**  
每次 Stop → 再 Play 約需 10–30 秒。應盡量減少 PlayMode 週期數。

**正確流程：**
1. **先讀場景結構**：用 `uloop get-hierarchy` 或直接讀 `Assets/Scenes/*.unity` YAML，確認 GameObject 組件、RectTransform、Inspector 值。
2. **先讀相關 Script**：確認邏輯是否有缺漏或 Unity API 誤用（如 Mask 需 Image alpha > 0、ContentSizeFitter 需配合 VerticalLayoutGroup 等）。
3. **靜態分析找到問題後修改**：編輯 Scene YAML 或 Script。
4. **最後才進 PlayMode**：僅用於最終驗證，一次解決，不反覆 Stop/Play。

> ⚠️ **禁止**：在尚未確認問題根因前就進入 PlayMode「試試看」。
> 每次 PlayMode 都應是「已知問題 → 已修改 → 驗證」的最後一步。

### Unity UI 佈局常見陷阱（靜態分析重點）
| 症狀 | 可能原因 | 靜態確認方法 |
|------|----------|------------|
| ScrollView 內容不顯示 | Viewport Image alpha = 0 → Mask 失效 | 查 Scene YAML 對應 Image `m_Color.a` |
| ContentSizeFitter 不更新 | 缺少 VerticalLayoutGroup | 查 Content 的 components 列表 |
| 文字不顯示 | Font Asset 未指定 / TMP color alpha = 0 | 查 TextMeshProUGUI `m_Color` / `m_fontAsset` |

## 任務完成規則

### 每次任務完成後必須執行
1. **更新 `PROJECT_STATUS.md`**：反映本次改動的最新狀態（已完成項目打勾、新增 Known Issues、更新 Last updated 日期）。

### PlayMode 驗證後額外執行
若本次任務有實際進入 PlayMode 測試並確認功能正常：
2. **執行 git commit**：將所有改動（含 `PROJECT_STATUS.md`）一併提交，commit message 說明功能驗證結果。

> 判斷標準：「PlayMode 驗證」= 用 `uloop control-play-mode` 進入 Play Mode，實際觸發並確認目標功能行為正確，無 Console 錯誤。

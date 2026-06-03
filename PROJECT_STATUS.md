# AIDrivenProject — Project Status

> Last updated: 2026-06-04 (CLI 跨場景 Bootstrap 系統：CLIRoot.prefab + CLIBootstrap.cs，TitleScene 背景修復)  
> Branch: master

---

## Progress Summary

| Phase | Title | Status |
|-------|-------|--------|
| 1 | 基礎架構 | ✅ Complete |
| 2 | 場景管理 | ✅ Complete |
| 3 | 物品欄系統 | ✅ Complete |
| 4 | CLI 系統 | ✅ Complete |
| 5 | 場景 UI | ✅ Complete |
| 6 | 整合與測試 | ✅ Complete |
| 7 | CLI 跨場景指令系統 | ✅ Complete |

---

## Phase 1 — 基礎架構 ✅

- [x] Git 初始化與 `.gitignore`
- [x] 資料夾結構：`Scripts/{Core,Inventory,CLI,UI}`、`Prefabs/`、`ScriptableObjects/`
- [x] 場景：`TitleScene` (index 0)、`MainScene` (index 1)

---

## Phase 2 — 場景管理 ✅

- [x] `SceneLoader.cs` — `Assets/Scripts/Core/SceneLoader.cs`
- [x] `GameManager.cs` — `Assets/Scripts/Core/GameManager.cs`

---

## Phase 3 — 物品欄系統 ⚠️

### Done
- [x] `Item.cs` — `Assets/Scripts/Inventory/Item.cs`
- [x] `Inventory.cs` — `Assets/Scripts/Inventory/Inventory.cs`
- [x] `ItemSlotUI.cs` — `Assets/Scripts/Inventory/ItemSlotUI.cs`
- [x] `InventoryUI.cs` — `Assets/Scripts/Inventory/InventoryUI.cs`
- [x] `ItemRegistry.cs` — `Assets/Scripts/Core/ItemRegistry.cs`
- [x] ScriptableObject 示範：`Sword.asset`, `Potion.asset`, `Key.asset` — `Assets/ScriptableObjects/Items/`
- [x] `ItemSlot.prefab` — `Assets/Prefabs/Items/ItemSlot.prefab`
- [x] `InventoryPanel.prefab` — `Assets/Prefabs/UI/InventoryPanel.prefab`

---

## Phase 4 — CLI 系統 ⚠️

### Done
- [x] `CLISystem.cs` — `Assets/Scripts/CLI/CLISystem.cs`（含 `UnregisterCommand()`）
- [x] `CLICommands.cs` — `Assets/Scripts/CLI/CLICommands.cs`（scene/inventory/pause/resume/title 新增）
- [x] `CLIUI.cs` — `Assets/Scripts/CLI/CLIUI.cs`
- [x] `TitleSceneCLICommands.cs` — `Assets/Scripts/CLI/TitleSceneCLICommands.cs`（start；load 已移至全域）

- [x] `CLIPanel.prefab` — `Assets/Prefabs/UI/CLIPanel.prefab`

---

## Phase 5 — 場景 UI ⚠️

### Done
- [x] `TitleUI.cs` — `Assets/Scripts/UI/TitleUI.cs`
- [x] `PauseMenuUI.cs` — `Assets/Scripts/UI/PauseMenuUI.cs`
- [x] Build Settings — TitleScene (index 0) 與 MainScene (index 1) 均已加入，`EditorBuildSettings.asset` 確認
- [x] TitleScene Canvas 設置 — 根據 git commit `8f6667b`（將 CLIUI/InventoryUI 移至 Canvas）推斷已完成
- [x] MainScene Canvas 設置 — commit `31ff8d2`（MainScene 各 Panel 場景儲存狀態改為 active）確認 Panel 存在

- [x] `PauseMenuPanel.prefab` — `Assets/Prefabs/UI/PauseMenuPanel.prefab`

---

## Phase 6 — 整合與測試 ✅

PlayMode 驗證完成（2026-06-04）：

- [x] CLI `give sword` → `Added: 鐵劍` → `items` 顯示 `[sword] 鐵劍` → `drop sword` → 移除
- [x] `inventory show` / `inventory hide` → InventoryUI 面板正確開關
- [x] `pause` / `resume` → GameManager.IsPaused 狀態正確切換
- [x] `scene` → 回傳 "MainScene"
- [x] uloop compile 無錯誤（0 errors, 0 warnings）
- [x] Console 無 Error 訊息
- [ ] Start 按鈕切換 TitleScene → MainScene（未在本次 PlayMode 驗證）
- [ ] ESC 開啟暫停選單視覺確認（未在本次 PlayMode 驗證）

> **注意：** Enter Play Mode 已啟用 `DisableDomainReload`，所有 singleton 類別均已加入 `RuntimeInitializeOnLoadMethod` 重置。

---

## Phase 7 — CLI 跨場景指令系統 ✅

**Bootstrap 架構完成。** CLIRoot 由靜態 Bootstrap 自動建立，無論從任何場景啟動皆可用。

### 實作狀態
| 檔案 | 說明 | 狀態 |
|------|------|------|
| `CLISystem.cs`（`UnregisterCommand`） | 場景指令卸載基礎 | ✅ |
| `CLICommands.cs`（scene/inventory/pause/resume/title） | 全域跨場景指令 | ✅ |
| `Scripts/CLI/CLIBootstrap.cs` | `BeforeSceneLoad` 靜態建立 CLIRoot | ✅ 新增 |
| `Scripts/CLI/TitleSceneCLICommands.cs` | TitleScene 指令：`start`（OnDestroy Unregister） | ✅ |
| `Resources/CLIRoot.prefab` | Canvas(100)+CLISystem+CLIUI+SceneLoader+CLIPanel | ✅ 新增 |
| `Scripts/Core/SceneTransition.cs` | 淡入淡出效果（CanvasGroup alpha tween） | ❌ 未實作 |

---

## Assets Inventory (Confirmed on Disk)

### Scripts
```
Assets/Scripts/Core/
  SceneLoader.cs
  GameManager.cs
  ItemRegistry.cs

Assets/Scripts/Inventory/
  Item.cs
  Inventory.cs
  InventoryUI.cs
  ItemSlotUI.cs

Assets/Scripts/CLI/
  CLISystem.cs
  CLICommands.cs
  CLIUI.cs
  CLIBootstrap.cs

Assets/Scripts/UI/
  TitleUI.cs
  PauseMenuUI.cs
```

### Prefabs
```
Assets/Prefabs/Items/
  ItemSlot.prefab        ✅

Assets/Prefabs/UI/
  InventoryPanel.prefab  ✅
  CLIPanel.prefab        ✅
  PauseMenuPanel.prefab  ✅

Assets/Resources/
  CLIRoot.prefab         ✅ (Bootstrap prefab: Canvas+CLISystem+CLIUI+SceneLoader+CLIPanel)
```

### ScriptableObjects
```
Assets/ScriptableObjects/Items/
  Sword.asset   ✅
  Potion.asset  ✅
  Key.asset     ✅
```

### Scenes
```
Assets/Scenes/
  TitleScene.unity   ✅ (Build Index 0)
  MainScene.unity    ✅ (Build Index 1)
```

---

## Known Issues / Notes

- **CLI 輸出顯示**（2026-06-04 修復）：CLIPanel OutputScrollView 的 Viewport Image alpha 原為 0，Mask stencil 不寫入導致所有輸出不可見；Content 缺少 VerticalLayoutGroup 導致 ContentSizeFitter 無法計算高度。已修復：Viewport Image alpha=1、Content 加入 VLG、CLIUI.AppendLine 改用 ForceMeshUpdate + LayoutRebuilder。
- **ItemRegistry allItems 連結**（2026-06-04 修復）：MainScene.unity 的 ItemRegistry 元件 `allItems` 三筆均為 `fileID: 0`（Inspector 未拖入），已補連結 Sword/Potion/Key SO GUID。
- **物品欄 CLI 完成**（2026-06-04）：`InventoryUI.Instance` + Show/Hide + 5 個新全域 CLI 指令（scene/inventory/pause/resume/title）+ TitleSceneCLICommands（start/load）。PlayMode 驗證通過，0 Console Error。
- **itemlist + 物品欄 UI 強化**（2026-06-04）：新增 CLI `itemlist` 指令（列出 ItemRegistry 所有物品）；`ItemSlotUI` 加 `Outline` 黃色選擇高亮；`InventoryUI` DetailPanel 執行期加入圖示 Image；EventTrigger 實現點擊空白關閉細節；CloseDetail 統一關閉邏輯。uloop compile 0 errors。
- **CLI UX 改善**（2026-06-04）：
  - 禁用水平捲動（ScrollRect `m_Horizontal=0`）
  - 新增 VerticalScrollbar（AutoHideAndExpandViewport，12px，深色背景+灰色 handle）
  - Enter / NumpadEnter 重新聚焦輸入框（面板開啟且輸入框無焦點時）
  - 上方向鍵歷史指令：最多 50 條，`↑` 逐步回溯，超出最舊或無記錄時清除輸入框
  - 動態捲動速度：以 `Mathf.Sign(scrollY) * viewportFraction / overflow` 取代固定像素靈敏度，每滾輪 tick 固定移動 25% 可視高度（`scrollViewportFraction` Inspector 可調），內容越長速度不降
- **CJK 字體**：`NotoSans-Medium SDF` 為 Latin-only，中文顯示為方塊。已加入 `NotoSansSC-Medium` 作為 fallback（commit `35e6c11`）。
- **DisableDomainReload**：啟用後 static 變數不自動歸零。所有 singleton 已加 `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` 重置。對應類別：SceneLoader、GameManager、Inventory、CLISystem、ItemRegistry、CLIUI、PauseMenuUI。
- **New Input System**：已從 Legacy 遷移（commit `0371a87`）。
- **UI Prefabs 建立**（2026-06-04）：`InventoryPanel.prefab`、`CLIPanel.prefab`、`PauseMenuPanel.prefab` 已用 `PrefabUtility.SaveAsPrefabAsset` 從場景 GameObject 建立，儲存於 `Assets/Prefabs/UI/`。
- **MainScene UI Prefab 替換**（2026-06-04）：MainScene Canvas 下三個 standalone UI GameObject（PauseMenuPanel、InventoryPanel、CLIPanel）已替換為 prefab instance（`PrefabUtility.InstantiatePrefab`）。序列化欄位自動連接，Resume 按鈕正常關閉暫停 UI（PlayMode 驗證通過）。
- **Inventory / ItemRegistry 場景化**（2026-06-04）：移除 `DontDestroyOnLoad`，加入 `OnDestroy` 清除 Instance。物品欄資料不再跨場景保存（僅 MainScene 有效）。
- **`load` 指令全域化**（2026-06-04）：從 `TitleSceneCLICommands.cs` 移至 `CLICommands.cs`，全場景可用。`TitleSceneCLICommands` 僅保留 `start`。
- **CLI 跨場景 Bootstrap**（2026-06-04）：`CLIBootstrap.cs`（`BeforeSceneLoad`）自動建立 `CLIRoot.prefab`（Canvas sortingOrder=100 + CLISystem + CLIUI + SceneLoader + CLIPanel），存於 `Assets/Resources/`。無論從 TitleScene 或 MainScene 啟動皆可用。移除 MainScene 的 CLISystem 獨立 GO 與 Canvas 上的 CLIUI/CLIPanel。PlayMode 驗證：CLIRoot 在 DontDestroyOnLoad，`give sword` / `start` 指令正常。0 Console Errors。
- **TitleScene 背景修復**（2026-06-04）：Canvas Background Image 顏色由黑改為深藍灰（0.25, 0.28, 0.38, 1），CLI 面板（深色）現在有足夠對比度可見。

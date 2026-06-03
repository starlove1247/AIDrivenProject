# AIDrivenProject — Project Status

> Last updated: 2026-06-04 (物品欄完成 + Phase 7 CLI 指令：scene/inventory/pause/resume/title/start/load)  
> Branch: master

---

## Progress Summary

| Phase | Title | Status |
|-------|-------|--------|
| 1 | 基礎架構 | ✅ Complete |
| 2 | 場景管理 | ✅ Complete |
| 3 | 物品欄系統 | ⚠️ Mostly Complete |
| 4 | CLI 系統 | ✅ Complete |
| 5 | 場景 UI | ⚠️ Mostly Complete |
| 6 | 整合與測試 | ✅ Complete |
| 7 | CLI 跨場景指令系統 | ⚠️ Mostly Complete |

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

### Pending
- [ ] `InventoryPanel` Prefab — `Assets/Prefabs/UI/` 資料夾存在但為空，Panel 目前直接建在 MainScene 內

---

## Phase 4 — CLI 系統 ⚠️

### Done
- [x] `CLISystem.cs` — `Assets/Scripts/CLI/CLISystem.cs`（含 `UnregisterCommand()`）
- [x] `CLICommands.cs` — `Assets/Scripts/CLI/CLICommands.cs`（scene/inventory/pause/resume/title 新增）
- [x] `CLIUI.cs` — `Assets/Scripts/CLI/CLIUI.cs`
- [x] `TitleSceneCLICommands.cs` — `Assets/Scripts/CLI/TitleSceneCLICommands.cs`（start/load）

### Pending
- [ ] `CLIPanel` Prefab — Panel 目前直接建在 MainScene 內，尚未獨立為 Prefab

---

## Phase 5 — 場景 UI ⚠️

### Done
- [x] `TitleUI.cs` — `Assets/Scripts/UI/TitleUI.cs`
- [x] `PauseMenuUI.cs` — `Assets/Scripts/UI/PauseMenuUI.cs`
- [x] Build Settings — TitleScene (index 0) 與 MainScene (index 1) 均已加入，`EditorBuildSettings.asset` 確認
- [x] TitleScene Canvas 設置 — 根據 git commit `8f6667b`（將 CLIUI/InventoryUI 移至 Canvas）推斷已完成
- [x] MainScene Canvas 設置 — commit `31ff8d2`（MainScene 各 Panel 場景儲存狀態改為 active）確認 Panel 存在

### Pending
- [ ] `InventoryPanel` / `CLIPanel` / `PauseMenu` 獨立 Prefab（目前嵌入場景，非獨立 Prefab）

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

## Phase 7 — CLI 跨場景指令系統 ⚠️

**核心指令已實作。** 詳見 `ROADMAP.md` Phase 7 節。

### 實作狀態
| 檔案 | 說明 | 狀態 |
|------|------|------|
| `CLISystem.cs`（`UnregisterCommand`） | 場景指令卸載基礎 | ✅ |
| `CLICommands.cs`（scene/inventory/pause/resume/title） | 全域跨場景指令 | ✅ |
| `Scripts/CLI/TitleSceneCLICommands.cs` | TitleScene 指令：`start`、`load`（OnDestroy Unregister） | ✅ |
| `Scripts/CLI/SceneCommandRegistry.cs` | 場景指令集中管理（抽象層） | ❌ 未建立 |
| `Scripts/CLI/GlobalCLICommands.cs` | 全域指令獨立檔案 | ❌ 未建立（已整合入 CLICommands.cs） |
| `Scripts/CLI/MainSceneCLICommands.cs` | MainScene 獨立指令檔案 | ❌ 未建立（已整合入 CLICommands.cs） |
| `Scripts/Core/SceneTransition.cs` | 淡入淡出效果（CanvasGroup alpha tween） | ❌ |

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

Assets/Scripts/UI/
  TitleUI.cs
  PauseMenuUI.cs
```

### Prefabs
```
Assets/Prefabs/Items/
  ItemSlot.prefab        ✅

Assets/Prefabs/UI/
  (empty — UI panels are scene-embedded, not standalone prefabs)
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
- **CLI UX 改善**（2026-06-04）：
  - 禁用水平捲動（ScrollRect `m_Horizontal=0`）
  - 新增 VerticalScrollbar（AutoHideAndExpandViewport，12px，深色背景+灰色 handle）
  - Enter / NumpadEnter 重新聚焦輸入框（面板開啟且輸入框無焦點時）
  - 上方向鍵歷史指令：最多 50 條，`↑` 逐步回溯，超出最舊或無記錄時清除輸入框
  - 動態捲動速度：以 `Mathf.Sign(scrollY) * viewportFraction / overflow` 取代固定像素靈敏度，每滾輪 tick 固定移動 25% 可視高度（`scrollViewportFraction` Inspector 可調），內容越長速度不降
- **CJK 字體**：`NotoSans-Medium SDF` 為 Latin-only，中文顯示為方塊。已加入 `NotoSansSC-Medium` 作為 fallback（commit `35e6c11`）。
- **DisableDomainReload**：啟用後 static 變數不自動歸零。所有 singleton 已加 `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` 重置。對應類別：SceneLoader、GameManager、Inventory、CLISystem、ItemRegistry、CLIUI、PauseMenuUI。
- **New Input System**：已從 Legacy 遷移（commit `0371a87`）。
- **UI Prefabs 缺失**：InventoryPanel / CLIPanel / PauseMenu 尚未獨立為 Prefab，若需跨場景複用需先建立。

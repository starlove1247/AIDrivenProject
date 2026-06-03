# AIDrivenProject — Roadmap

## 目標
2D Unity 遊戲基礎框架：物品欄、CLI 開發者工具、雙場景流程。

---

## Phase 1 — 基礎架構 ✅
- [x] Git 初始化與 .gitignore
- [x] 資料夾結構：Scripts/{Core,Inventory,CLI,UI}、Prefabs/、ScriptableObjects/
- [x] 場景：TitleScene (index 0)、MainScene (index 1)

## Phase 2 — 場景管理 ✅
- [x] `SceneLoader.cs` — 非同步場景切換單例
- [x] `GameManager.cs` — 暫停狀態管理、ESC 鍵監聽

## Phase 3 — 物品欄系統 ✅
- [x] `Item.cs` — ScriptableObject 資料型別
- [x] `Inventory.cs` — 物品欄邏輯單例
- [x] `ItemSlotUI.cs` — 單格 UI 元件
- [x] `InventoryUI.cs` — 物品欄面板（I 鍵切換）
- [x] `ItemRegistry.cs` — 全域物品查詢（CLI give 使用）
- [ ] 建立示範 ScriptableObject：Sword、Potion、Key
- [ ] 建立 ItemSlot Prefab（含 Image + Text + Button）
- [ ] 建立 InventoryPanel Prefab

## Phase 4 — CLI 系統 ✅
- [x] `CLISystem.cs` — 指令解析與分派
- [x] `CLICommands.cs` — 內建指令：help / items / give / drop / clear
- [x] `CLIUI.cs` — 輸入框 + 捲動輸出（\` 鍵切換）
- [ ] 建立 CLI Prefab（Panel + InputField + ScrollView）

## Phase 5 — 場景 UI ✅
- [x] `TitleUI.cs` — Start 按鈕切換至 MainScene
- [x] `PauseMenuUI.cs` — 繼續 / 回到標題按鈕
- [ ] TitleScene Canvas 設置（Title Text + Start Button）
- [ ] MainScene Canvas 設置（InventoryPanel + CLIPanel + PauseMenuPanel）
- [ ] 在 Build Settings 加入兩個場景

## Phase 6 — 整合與測試
- [ ] 在 TitleScene / MainScene 掛入所有必要 GameObject
- [ ] 驗證：CLI `give sword` → 物品欄顯示 → `drop sword` → 移除
- [ ] 驗證：Start 按鈕切換場景、ESC 暫停選單、回到標題
- [ ] uloop compile 無錯誤

---

## Phase 7 — CLI 跨場景指令系統

### 設計原則
- CLI 本體（`CLISystem` / `CLIUI`）為 `DontDestroyOnLoad` 單例，跨場景保持存在
- 指令分為兩類：**全域指令**（所有場景可用）、**場景指令**（僅特定場景註冊）
- 場景載入完成後由該場景的 bootstrap GameObject 呼叫 `RegisterSceneCommands()`；場景卸載時呼叫 `UnregisterSceneCommands()` 清除場景指令

### 全域指令（任何場景皆可用）
| 指令 | 說明 |
|------|------|
| `help` | 列出當前場景所有可用指令 |
| `clear` | 清除 CLI 輸出 |
| `scene` | 顯示目前場景名稱 |
| `fade <on\|off>` | 開關場景切換淡入淡出效果（預設 on） |

### TitleScene 指令
| 指令 | 說明 |
|------|------|
| `start` | 進入 MainScene（等同 Start 按鈕） |
| `load <sceneName>` | 直接載入指定場景（開發用） |

### MainScene 指令
| 指令 | 說明 |
|------|------|
| `items` | 顯示物品欄內容 |
| `give <id>` | 新增物品 |
| `drop <id>` | 丟棄物品 |
| `pause` | 開啟暫停選單 |
| `resume` | 關閉暫停選單 |
| `inventory <show\|hide>` | 強制顯示 / 隱藏物品欄面板 |
| `title` | 回到 TitleScene |

### 實作項目
- [ ] `SceneCommandRegistry.cs` — 管理場景指令的註冊 / 取消，`CLISystem` 查詢時合併全域與場景指令
- [ ] `GlobalCLICommands.cs` — 全域指令：`help`、`clear`、`scene`、`fade`
- [ ] `TitleSceneCLICommands.cs` — TitleScene 場景指令：`start`、`load`
- [ ] `MainSceneCLICommands.cs` — MainScene 場景指令：物品欄、暫停、`title`
- [ ] `SceneTransition.cs` — 淡入淡出效果（CanvasGroup alpha tween），`fade` 指令控制開關
- [ ] TitleScene / MainScene bootstrap GameObject 於 `Start()` 呼叫 `RegisterSceneCommands()`
- [ ] 驗證：TitleScene 輸入 `start` 切換至 MainScene，切換時依 fade 設定執行淡入淡出
- [ ] 驗證：MainScene 輸入 `give sword` → 物品欄顯示；`pause` → 暫停選單開啟
- [ ] 驗證：`help` 輸出隨場景不同而顯示對應指令清單

---

## 未來擴充（Backlog）
- [ ] 物品圖示系統（Sprite Atlas）
- [ ] 物品欄容量上限
- [ ] 存檔 / 讀檔（PlayerPrefs 或 JSON）
- [ ] 物品使用效果（`use <id>` CLI 指令）
- [ ] 更多場景（遊戲關卡）
- [ ] 音效系統

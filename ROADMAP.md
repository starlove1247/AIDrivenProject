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

## 未來擴充（Backlog）
- [ ] 物品圖示系統（Sprite Atlas）
- [ ] 物品欄容量上限
- [ ] 存檔 / 讀檔（PlayerPrefs 或 JSON）
- [ ] 物品使用效果（`use <id>` CLI 指令）
- [ ] 更多場景（遊戲關卡）
- [ ] 音效系統
- [ ] 動畫過場（場景切換淡入淡出）

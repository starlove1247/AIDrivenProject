# cli-test

在 PlayMode 中驗證 CLISystem + CLIUI 完整輸出流程。

## 用途

當 CLI 相關功能修改後，用本 skill 快速驗證：
- CLISystem / CLIUI singleton 存活
- OnOutput 有訂閱者
- 指令執行後文字實際寫入 outputText
- Content RectTransform 高度正確（> 0）
- Viewport Mask 正常（Image alpha = 1）
- OutputText 在 Viewport 內有合法 world bounds

## 前置條件

- Unity Editor 已開啟且處於 **PlayMode** + **MainScene**
- CLIPanel 已開啟（已按反引號或用 dynamic-code 開啟）

## 使用方式

```bash
# 1. 進入 PlayMode
uloop control-play-mode --action Play

# ⚠️ 必須等待約 5 秒讓場景完全載入，否則 singleton 可能仍為 null
# 緊接 Play 就呼叫 execute-dynamic-code 會得到 null singleton 或 NRE

# 2. 開啟 CLI 面板（dynamic-code 方式，不依賴鍵盤模擬）
uloop execute-dynamic-code --code "
CLIUI.Instance?.GetType()
    .GetMethod(\"TogglePanel\", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    ?.Invoke(CLIUI.Instance, null);
return \"CLI toggled\";
"

# 3. 執行診斷（見下方 diagnostic snippet）
uloop execute-dynamic-code --code "..."

# 4. 截圖確認
uloop screenshot --capture-mode rendering
```

## Snippet A：基礎系統診斷（複製至 execute-dynamic-code）

```csharp
// === CLI 系統完整診斷 ===
string r = "";

// 1. Singleton 狀態
r += $"CLISystem null={CLISystem.Instance == null}\n";
r += $"CLIUI null={CLIUI.Instance == null}\n";

// 2. OnOutput 訂閱者
int subs = CLISystem.Instance?.OnOutput?.GetInvocationList()?.Length ?? 0;
r += $"OnOutput subscribers={subs}\n";

// 3. 執行 help
CLISystem.Instance?.Execute("help");

// 4. OutputText 內容
var outGO = UnityEngine.GameObject.Find("OutputText");
var tmp = outGO?.GetComponent<TMPro.TextMeshProUGUI>();
r += $"OutputText text lines={tmp?.text?.Split('\n')?.Length}\n";
r += $"OutputText color alpha={tmp?.color.a}\n";

// 5. Content 高度
var contentGO = UnityEngine.GameObject.Find("Content");
var contentRT = contentGO?.GetComponent<UnityEngine.RectTransform>();
r += $"Content.height={contentRT?.rect.height}\n";

// 6. Viewport Mask 狀態
var vpGO = UnityEngine.GameObject.Find("Viewport");
var vpImg = vpGO?.GetComponent<UnityEngine.UI.Image>();
r += $"Viewport Image alpha={vpImg?.color.a}\n";

// 7. World bounds
var corners = new UnityEngine.Vector3[4];
outGO?.GetComponent<UnityEngine.RectTransform>()?.GetWorldCorners(corners);
r += $"OutputText BL={corners[0]} TR={corners[2]}\n";

return r;
```

## Snippet B：指令功能完整測試（MainScene PlayMode）

測試 give / items / inventory / pause / resume / scene 指令，確認各 singleton 行為正確。

```csharp
// === CLI 指令功能測試 ===
var cli = CLISystem.Instance;
string r = "";

// 目前場景
r += "Scene=" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "\n";
r += "ItemRegistry=" + (ItemRegistry.Instance != null) + "\n";
r += "InventoryUI=" + (InventoryUI.Instance != null) + "\n";
r += "GameManager=" + (GameManager.Instance != null) + "\n";

// scene 指令
cli.Execute("scene");

// give + items（需 ItemRegistry.allItems 已連結 SO）
cli.Execute("give sword");
cli.Execute("items");

// inventory show / hide
cli.Execute("inventory show");
r += "InventoryOpen=" + (InventoryUI.Instance?.IsOpen) + "\n";
cli.Execute("inventory hide");
r += "InventoryHidden=" + (InventoryUI.Instance?.IsOpen) + "\n";

// drop
cli.Execute("drop sword");

// pause / resume
cli.Execute("pause");
r += "IsPaused=" + GameManager.Instance?.IsPaused + "\n";
cli.Execute("resume");
r += "NotPaused=" + (!GameManager.Instance.IsPaused) + "\n";

return r;
```

**預期輸出：**
```
Scene=MainScene
ItemRegistry=True
InventoryUI=True
GameManager=True
InventoryOpen=True
InventoryHidden=False
IsPaused=True
NotPaused=True
```

---

## 預期結果（正常）

| 項目 | 預期值 |
|------|--------|
| CLISystem null | False |
| CLIUI null | False |
| OnOutput subscribers | 1 |
| OutputText text lines | ≥ 2 |
| OutputText color alpha | 1 |
| Content.height | > 0（通常 80–200） |
| Viewport Image alpha | 1 |
| OutputText TR.y | > BL.y（有高度） |

## 已知問題與修復記錄

### 問題 1：Content.height = 0（2026-06-04 修復）
**症狀**：CLI 輸出區完全空白，outputText.text 有值但不顯示。  
**原因**：`Content` 有 `ContentSizeFitter` 但缺 `VerticalLayoutGroup`，`LayoutUtility.GetPreferredHeight` 無法讀取子物件高度。  
**修復**：在 `Content` GameObject 加入 `VerticalLayoutGroup`（childControlHeight=true, childForceExpandHeight=false）。  
**相關檔案**：`Assets/Scenes/MainScene.unity`

### 問題 2：Viewport Image alpha = 0（2026-06-04 修復）
**症狀**：所有 ScrollView 內部物件不可見，即使 Content 有高度。  
**原因**：Viewport 的 `Image` 元件 `m_Color.a = 0`，Mask stencil 不寫入，所有子物件被截掉。  
**修復**：將 Viewport Image color 改為 `(1,1,1,1)`（Mask.showMaskGraphic=false 確保不影響視覺）。  
**靜態確認**：查 `MainScene.unity` Viewport GameObject 的 Image `m_Color.a` 欄位。

### 問題 4：ItemRegistry.allItems 全為 null（2026-06-04 發現）
**症狀**：`give <id>` 指令拋出 `NullReferenceException` at `ItemRegistry.cs:23`（`item.itemId.ToLower()`）。  
**原因**：MainScene.unity 的 ItemRegistry MonoBehaviour 的 `allItems` 陣列有 3 個 slot 但全為 `{fileID: 0}`（Inspector 未拖入 ScriptableObject）。`foreach (Item item in Instance.allItems)` 時 `item` 為 null。  
**靜態確認**：搜尋 `MainScene.unity` 的 `allItems:` 欄位，若子行全是 `{fileID: 0}` 即為此問題。  
**修復**：取得各 Item SO 的 GUID（`.meta` 檔），改為 `{fileID: 11400000, guid: <guid>, type: 2}`。  
**診斷 Snippet**：
```csharp
var fi = typeof(ItemRegistry).GetField("allItems",
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
var arr = fi?.GetValue(ItemRegistry.Instance) as Item[];
string r = "allItems.Length=" + arr?.Length + "\n";
for (int i = 0; arr != null && i < arr.Length; i++)
    r += "[" + i + "]=" + (arr[i] == null ? "NULL" : arr[i].itemId) + "\n";
return r;
// 正常：[0]=sword  [1]=potion  [2]=key
// 異常：[0]=NULL   [1]=NULL    [2]=NULL  → 需補 SO 連結
```
> ⚠️ `fi?.GetValue(reg)` 若 `reg` 為 null 會拋 "Non-static field requires a target"。  
> 確認 `ItemRegistry.Instance != null` 後再呼叫。

---

### 問題 3：CLIUI.AppendLine 版本
**正確實作**：
```csharp
outputText.text = string.Join("\n", _lines);
outputText.ForceMeshUpdate();
LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)outputText.transform.parent);
scrollRect.verticalNormalizedPosition = 0f;
```
- `ForceMeshUpdate()` → TMP 先計算 preferredHeight
- `ForceRebuildLayoutImmediate(parent)` → VLG + ContentSizeFitter 更新 Content 高度
- 不可用手動 `sizeDelta`（會被 ContentSizeFitter 下一幀覆蓋）

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

# 2. 開啟 CLI 面板
uloop simulate-keyboard --key Backquote --action press

# 3. 執行診斷（見下方 dynamic-code snippet）
uloop execute-dynamic-code --code "..."

# 4. 截圖確認
uloop screenshot --capture-mode rendering
```

## 診斷 Snippet（複製至 execute-dynamic-code）

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

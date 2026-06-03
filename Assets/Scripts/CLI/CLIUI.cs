using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CLIUI : MonoBehaviour
{
    public static CLIUI Instance { get; private set; }

    public bool IsOpen => panel != null && panel.activeSelf;

    [SerializeField] GameObject panel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI outputText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Key toggleKey = Key.Backquote;
    [SerializeField] float scrollViewportFraction = 0.25f;

    readonly List<string> _lines = new();
    const int MaxLines = 200;

    readonly List<string> _history = new();
    const int MaxHistory = 50;
    int _historyIndex = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => Instance = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CLISystem.Instance.OnOutput += AppendLine;
        panel.SetActive(false);
        inputField.onSubmit.AddListener(OnSubmit);
        scrollRect.scrollSensitivity = 0f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            TogglePanel();

        if (IsOpen && Keyboard.current != null &&
            (Keyboard.current[Key.Enter].wasPressedThisFrame ||
             Keyboard.current[Key.NumpadEnter].wasPressedThisFrame) && !inputField.isFocused)
        {
            inputField.ActivateInputField();
        }

        if (IsOpen && Mouse.current != null)
        {
            float scrollY = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollY) > 0.1f)
            {
                float viewportH = scrollRect.viewport.rect.height;
                float contentH = scrollRect.content.rect.height;
                float overflow = contentH - viewportH;
                if (overflow > 0f)
                {
                    float normalizedDelta = Mathf.Sign(scrollY) * (viewportH * scrollViewportFraction / overflow);
                    scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                        scrollRect.verticalNormalizedPosition + normalizedDelta);
                }
            }
        }

        if (IsOpen && Keyboard.current != null && Keyboard.current[Key.UpArrow].wasPressedThisFrame)
        {
            if (_history.Count == 0 || _historyIndex >= _history.Count - 1)
            {
                _historyIndex = -1;
                inputField.text = string.Empty;
            }
            else
            {
                _historyIndex++;
                inputField.text = _history[_history.Count - 1 - _historyIndex];
                inputField.MoveTextEnd(false);
            }
            if (!inputField.isFocused) inputField.ActivateInputField();
        }
    }

    void TogglePanel()
    {
        bool active = !panel.activeSelf;
        panel.SetActive(active);
        if (active)
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }
    }

    void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        _history.Add(text);
        if (_history.Count > MaxHistory) _history.RemoveAt(0);
        _historyIndex = -1;
        CLISystem.Instance.Execute(text);
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }

    void AppendLine(string line)
    {
        if (string.IsNullOrEmpty(line)) return;
        _lines.Add(line);
        if (_lines.Count > MaxLines) _lines.RemoveAt(0);
        outputText.text = string.Join("\n", _lines);
        outputText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)outputText.transform.parent);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void Clear()
    {
        _lines.Clear();
        outputText.text = string.Empty;
    }

    void OnDestroy()
    {
        if (CLISystem.Instance != null)
            CLISystem.Instance.OnOutput -= AppendLine;
    }
}

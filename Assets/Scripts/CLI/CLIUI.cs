using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CLIUI : MonoBehaviour
{
    public static CLIUI Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI outputText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] KeyCode toggleKey = KeyCode.BackQuote;

    readonly List<string> _lines = new();
    const int MaxLines = 200;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePanel();
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
        Canvas.ForceUpdateCanvases();
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

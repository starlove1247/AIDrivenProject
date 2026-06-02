using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] Button resumeButton;
    [SerializeField] Button titleButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.Resume());
        titleButton.onClick.AddListener(() => SceneLoader.Instance.LoadScene("TitleScene"));
        panel.SetActive(false);
    }

    public void SetVisible(bool visible) => panel.SetActive(visible);

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}

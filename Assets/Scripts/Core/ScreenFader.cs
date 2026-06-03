using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    public static bool FadeEnabled = true;

    [SerializeField] float fadeDuration = 0.35f;

    CanvasGroup _fadeGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        Instance = null;
        FadeEnabled = true;
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateFadeOverlay();
    }

    void CreateFadeOverlay()
    {
        var overlayGO = new GameObject("FadeOverlay");
        overlayGO.transform.SetParent(transform, false);

        var canvas = overlayGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        overlayGO.AddComponent<CanvasScaler>();
        overlayGO.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(overlayGO.transform, false);

        var img = imgGO.AddComponent<Image>();
        img.color = Color.black;

        var rect = imgGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        _fadeGroup = imgGO.AddComponent<CanvasGroup>();
        _fadeGroup.alpha = 0f;
        _fadeGroup.blocksRaycasts = false;
        _fadeGroup.interactable = false;
    }

    public IEnumerator FadeOut() => Fade(0f, 1f);
    public IEnumerator FadeIn() => Fade(1f, 0f);

    IEnumerator Fade(float from, float to)
    {
        _fadeGroup.blocksRaycasts = true;
        _fadeGroup.alpha = from;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            _fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        _fadeGroup.alpha = to;
        if (to <= 0f) _fadeGroup.blocksRaycasts = false;
    }
}

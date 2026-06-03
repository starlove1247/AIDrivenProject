using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => Instance = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var faderGO = new GameObject("ScreenFader");
        DontDestroyOnLoad(faderGO);
        faderGO.AddComponent<ScreenFader>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (!ScreenFader.FadeEnabled) { LoadScene(sceneName); return; }
        StartCoroutine(LoadAsyncWithFade(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        Time.timeScale = 1f;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
    }

    IEnumerator LoadAsyncWithFade(string sceneName)
    {
        Time.timeScale = 1f;
        yield return StartCoroutine(ScreenFader.Instance.FadeOut());
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }
}

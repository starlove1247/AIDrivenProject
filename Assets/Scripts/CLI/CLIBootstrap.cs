using UnityEngine;

public static class CLIBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (CLISystem.Instance != null) return;
        var prefab = Resources.Load<GameObject>("CLIRoot");
        if (prefab == null) { Debug.LogError("[CLIBootstrap] CLIRoot not found in Resources/"); return; }
        Object.Instantiate(prefab);
    }
}

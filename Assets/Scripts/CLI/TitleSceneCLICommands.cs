using UnityEngine;

public class TitleSceneCLICommands : MonoBehaviour
{
    static readonly string[] _names = { "start", "load" };

    void Start()
    {
        var cli = CLISystem.Instance;
        if (cli == null) return;

        cli.RegisterCommand("start", _ =>
        {
            SceneLoader.Instance?.LoadScene("MainScene");
            return "Loading MainScene...";
        });

        cli.RegisterCommand("load", args =>
        {
            if (args.Length == 0) return "Usage: load <sceneName>";
            SceneLoader.Instance?.LoadScene(args[0]);
            return $"Loading {args[0]}...";
        });
    }

    void OnDestroy()
    {
        var cli = CLISystem.Instance;
        if (cli == null) return;
        foreach (string n in _names) cli.UnregisterCommand(n);
    }
}

using UnityEngine;

public class TitleSceneCLICommands : MonoBehaviour
{
    static readonly string[] _names = { "start" };

    void Start()
    {
        var cli = CLISystem.Instance;
        if (cli == null) return;

        cli.RegisterCommand("start", _ =>
        {
            SceneLoader.Instance?.LoadScene("MainScene");
            return "Loading MainScene...";
        });
    }

    void OnDestroy()
    {
        var cli = CLISystem.Instance;
        if (cli == null) return;
        foreach (string n in _names) cli.UnregisterCommand(n);
    }
}

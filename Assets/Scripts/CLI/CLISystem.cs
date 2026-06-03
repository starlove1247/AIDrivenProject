using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CLISystem : MonoBehaviour
{
    public static CLISystem Instance { get; private set; }

    readonly Dictionary<string, (Func<string[], string> handler, string scene)> _commands = new();

    public Action<string> OnOutput;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => Instance = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CLICommands.Register(this);
    }

    public void RegisterCommand(string name, Func<string[], string> handler, string scene = null)
    {
        _commands[name.ToLower()] = (handler, scene);
    }

    public void UnregisterCommand(string name) => _commands.Remove(name.ToLower());

    public void Execute(string input)
    {
        input = input.Trim();
        if (string.IsNullOrEmpty(input)) return;

        Output($"> {input}");

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string cmd = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (_commands.TryGetValue(cmd, out var entry))
            Output(entry.handler(args));
        else
            Output($"Unknown command: '{cmd}'. Type 'help' for list.");
    }

    public void Output(string message) => OnOutput?.Invoke(message);

    public IEnumerable<string> GetCommandNames(string currentScene = null) =>
        currentScene == null
            ? _commands.Keys
            : _commands.Where(kv => kv.Value.scene == null || kv.Value.scene == currentScene)
                       .Select(kv => kv.Key);
}

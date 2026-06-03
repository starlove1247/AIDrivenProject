using System;
using System.Collections.Generic;
using UnityEngine;

public class CLISystem : MonoBehaviour
{
    public static CLISystem Instance { get; private set; }

    readonly Dictionary<string, Func<string[], string>> _commands = new();

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

    public void RegisterCommand(string name, Func<string[], string> handler)
    {
        _commands[name.ToLower()] = handler;
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

        if (_commands.TryGetValue(cmd, out var handler))
            Output(handler(args));
        else
            Output($"Unknown command: '{cmd}'. Type 'help' for list.");
    }

    public void Output(string message) => OnOutput?.Invoke(message);

    public IEnumerable<string> GetCommandNames() => _commands.Keys;
}

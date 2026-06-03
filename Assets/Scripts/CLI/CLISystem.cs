using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CLISystem : MonoBehaviour
{
    public static CLISystem Instance { get; private set; }

    readonly Dictionary<string, (Func<string[], string> handler, string scene)> _commands = new();
    readonly List<string> _lastSuggestions = new();

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

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string cmd = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (_lastSuggestions.Count > 0 && args.Length == 0 &&
            int.TryParse(cmd, out int idx) && idx >= 1 && idx <= _lastSuggestions.Count)
        {
            cmd = _lastSuggestions[idx - 1];
            _lastSuggestions.Clear();
            Output($"> {cmd}");
            if (_commands.TryGetValue(cmd, out var sel))
                Output(sel.handler(Array.Empty<string>()));
            return;
        }

        _lastSuggestions.Clear();
        Output($"> {input}");

        if (_commands.TryGetValue(cmd, out var entry))
            Output(entry.handler(args));
        else
        {
            string scene = SceneManager.GetActiveScene().name;
            var suggestions = FuzzySearch(cmd, scene).Take(3).ToList();
            if (suggestions.Count > 0)
            {
                _lastSuggestions.AddRange(suggestions);
                var numbered = suggestions.Select((s, i) => $"({i + 1}) {s}");
                Output($"Unknown command: '{cmd}'\nDid you mean: {string.Join(", ", numbered)}");
            }
            else
                Output($"Unknown command: '{cmd}'. Type 'help' for list.");
        }
    }

    public void Output(string message) => OnOutput?.Invoke(message);

    public IEnumerable<string> GetCommandNames(string currentScene = null) =>
        currentScene == null
            ? _commands.Keys
            : _commands.Where(kv => kv.Value.scene == null || kv.Value.scene == currentScene)
                       .Select(kv => kv.Key);

    public IEnumerable<string> FuzzySearch(string query, string currentScene = null)
    {
        if (string.IsNullOrEmpty(query)) return GetCommandNames(currentScene).OrderBy(n => n);

        query = query.ToLower();
        int threshold = query.Length <= 2 ? 1 : query.Length <= 4 ? 2 : Math.Min(query.Length / 2, 3);

        return GetCommandNames(currentScene)
            .Select(name =>
            {
                if (name.StartsWith(query)) return (name, 0);
                if (name.Contains(query))   return (name, 1);
                int dist = Levenshtein(query, name);
                if (dist <= threshold)      return (name, dist + 1);
                return (name, int.MaxValue);
            })
            .Where(t => t.Item2 < int.MaxValue)
            .OrderBy(t => t.Item2).ThenBy(t => t.Item1)
            .Select(t => t.Item1);
    }

    static int Levenshtein(string a, string b)
    {
        int m = a.Length, n = b.Length;
        int[] prev = new int[n + 1], curr = new int[n + 1];
        for (int j = 0; j <= n; j++) prev[j] = j;
        for (int i = 1; i <= m; i++)
        {
            curr[0] = i;
            for (int j = 1; j <= n; j++)
                curr[j] = a[i - 1] == b[j - 1]
                    ? prev[j - 1]
                    : 1 + Math.Min(prev[j - 1], Math.Min(prev[j], curr[j - 1]));
            (prev, curr) = (curr, prev);
        }
        return prev[n];
    }
}

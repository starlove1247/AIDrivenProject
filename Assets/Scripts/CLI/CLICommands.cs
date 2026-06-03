using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CLICommands
{
    public static void Register(CLISystem cli)
    {
        cli.RegisterCommand("help", args =>
        {
            string scene = SceneManager.GetActiveScene().name;
            IEnumerable<string> names;
            string header;
            if (args.Length > 0)
            {
                string q = args[0].ToLower();
                names = cli.FuzzySearch(q, scene);
                header = $"Commands (matching '{q}'):\n";
            }
            else
            {
                names = cli.GetCommandNames(scene).OrderBy(n => n);
                header = "Commands:\n";
            }
            var sb = new StringBuilder(header);
            foreach (string name in names)
                sb.AppendLine($"  {name}");
            string result = sb.ToString().TrimEnd();
            return result == header.TrimEnd() ? "No matching commands." : result;
        });

        cli.RegisterCommand("clear", args =>
        {
            CLIUI.Instance?.Clear();
            return string.Empty;
        });

        cli.RegisterCommand("items", args =>
        {
            var inv = Inventory.Instance;
            if (inv == null) return "Inventory not found.";
            if (inv.Items.Count == 0) return "Inventory is empty.";
            return string.Join("\n", inv.Items.Select(i => $"  [{i.itemId}] {i.itemName}"));
        }, "MainScene");

        cli.RegisterCommand("give", args =>
        {
            if (args.Length == 0) return "Usage: give <item_id>";
            string id = args[0].ToLower();
            Item item = ItemRegistry.Find(id);
            if (item == null) return $"Item '{id}' not found in registry.";
            Inventory.Instance?.AddItem(item);
            return $"Added: {item.itemName}";
        }, "MainScene");

        cli.RegisterCommand("drop", args =>
        {
            if (args.Length == 0) return "Usage: drop <item_id>";
            string id = args[0].ToLower();
            bool ok = Inventory.Instance?.RemoveItemById(id) ?? false;
            return ok ? $"Dropped item: {id}" : $"Item '{id}' not in inventory.";
        }, "MainScene");

        cli.RegisterCommand("scene", args =>
            SceneManager.GetActiveScene().name);

        cli.RegisterCommand("inventory", args =>
        {
            if (InventoryUI.Instance == null) return "InventoryUI not available in this scene.";
            if (args.Length == 0) return "Usage: inventory <show|hide>";
            switch (args[0].ToLower())
            {
                case "show": InventoryUI.Instance.Show(); return "Inventory shown.";
                case "hide": InventoryUI.Instance.Hide(); return "Inventory hidden.";
                default: return "Usage: inventory <show|hide>";
            }
        }, "MainScene");

        cli.RegisterCommand("pause", args =>
        {
            var gm = GameManager.Instance;
            if (gm == null) return "GameManager not available.";
            if (!gm.IsPaused) gm.TogglePause();
            return "Game paused.";
        }, "MainScene");

        cli.RegisterCommand("resume", args =>
        {
            var gm = GameManager.Instance;
            if (gm == null) return "GameManager not available.";
            gm.Resume();
            return "Game resumed.";
        }, "MainScene");

        cli.RegisterCommand("title", args =>
        {
            var sl = SceneLoader.Instance;
            if (sl == null) return "SceneLoader not available.";
            sl.LoadScene("TitleScene");
            return "Loading TitleScene...";
        }, "MainScene");

        cli.RegisterCommand("itemlist", args =>
        {
            if (ItemRegistry.Instance == null) return "ItemRegistry not found.";
            var items = ItemRegistry.Instance.AllItems;
            if (items.Count == 0) return "No items in registry.";
            var sb = new StringBuilder("Available items:\n");
            foreach (var item in items)
                sb.AppendLine($"  [{item.itemId}] {item.itemName} - {item.description}");
            return sb.ToString().TrimEnd();
        }, "MainScene");

        cli.RegisterCommand("load", args =>
        {
            if (args.Length == 0) return "Usage: load <sceneName>";
            var sl = SceneLoader.Instance;
            if (sl == null) return "SceneLoader not available.";
            sl.LoadScene(args[0]);
            return $"Loading {args[0]}...";
        });

        cli.RegisterCommand("fade", args =>
        {
            if (args.Length == 0)
                return $"Scene fade: {(ScreenFader.FadeEnabled ? "on" : "off")}. Usage: fade <on|off>";
            switch (args[0].ToLower())
            {
                case "on":  ScreenFader.FadeEnabled = true;  return "Scene fade: on";
                case "off": ScreenFader.FadeEnabled = false; return "Scene fade: off";
                default: return "Usage: fade <on|off>";
            }
        });
    }
}

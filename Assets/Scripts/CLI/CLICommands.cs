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
            var sb = new StringBuilder("Commands:\n");
            foreach (string name in cli.GetCommandNames().OrderBy(n => n))
                sb.AppendLine($"  {name}");
            return sb.ToString().TrimEnd();
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
        });

        cli.RegisterCommand("give", args =>
        {
            if (args.Length == 0) return "Usage: give <item_id>";
            string id = args[0].ToLower();
            Item item = ItemRegistry.Find(id);
            if (item == null) return $"Item '{id}' not found in registry.";
            Inventory.Instance?.AddItem(item);
            return $"Added: {item.itemName}";
        });

        cli.RegisterCommand("drop", args =>
        {
            if (args.Length == 0) return "Usage: drop <item_id>";
            string id = args[0].ToLower();
            bool ok = Inventory.Instance?.RemoveItemById(id) ?? false;
            return ok ? $"Dropped item: {id}" : $"Item '{id}' not in inventory.";
        });

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
        });

        cli.RegisterCommand("pause", args =>
        {
            var gm = GameManager.Instance;
            if (gm == null) return "GameManager not available.";
            if (!gm.IsPaused) gm.TogglePause();
            return "Game paused.";
        });

        cli.RegisterCommand("resume", args =>
        {
            var gm = GameManager.Instance;
            if (gm == null) return "GameManager not available.";
            gm.Resume();
            return "Game resumed.";
        });

        cli.RegisterCommand("title", args =>
        {
            var sl = SceneLoader.Instance;
            if (sl == null) return "SceneLoader not available.";
            sl.LoadScene("TitleScene");
            return "Loading TitleScene...";
        });

        cli.RegisterCommand("itemlist", args =>
        {
            if (ItemRegistry.Instance == null) return "ItemRegistry not found.";
            var items = ItemRegistry.Instance.AllItems;
            if (items.Count == 0) return "No items in registry.";
            var sb = new StringBuilder("Available items:\n");
            foreach (var item in items)
                sb.AppendLine($"  [{item.itemId}] {item.itemName} - {item.description}");
            return sb.ToString().TrimEnd();
        });

        cli.RegisterCommand("load", args =>
        {
            if (args.Length == 0) return "Usage: load <sceneName>";
            var sl = SceneLoader.Instance;
            if (sl == null) return "SceneLoader not available.";
            sl.LoadScene(args[0]);
            return $"Loading {args[0]}...";
        });
    }
}

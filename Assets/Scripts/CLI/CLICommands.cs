using System.Linq;
using System.Text;
using UnityEngine;

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
    }
}

using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WipePlayerDataParser
{
    internal class InventoryCleaner
    {
        private readonly List<string> _protectedItems = new List<string>()
        {
            "minecraft:writable_book",
            "minecraft:written_book",
            "minecraft:filled_map"
        };

        private readonly string _lightId = "minecraft:light";
        private readonly string _itemFrameId = "minecraft:item_frame";
        private readonly string _shulkerBoxIdPart = "shulker_box";

        public void CleanPlayerInventory(NbtFile playerData)
        {
            CleanInventory((NbtList)playerData.RootTag["Inventory"]);
            CleanInventory((NbtList)playerData.RootTag["EnderItems"]);
        }

        private void CleanInventory(NbtList inventory)
        {
            foreach(NbtCompound item in inventory.Reverse<NbtTag>())
            {
                var id = item.Get<NbtString>("id").Value;

                if (_protectedItems.Contains(id))
                {
                    continue;
                }
                if (id == _lightId)
                {
                    HandleLight(item);
                    continue;
                }
                if (id == _itemFrameId)
                {
                    HandleItemFrame(item, inventory);
                    continue;
                }
                if (id.Contains(_shulkerBoxIdPart))
                {
                    HandleShulkerBox(item, inventory);
                    continue;
                }
                HandleNormalItem(item, inventory);
            }
        }

        private void HandleLight(NbtCompound item)
        {
            MakrItemAsEventItem(item, "#8F8F8F", "Невидимая неделя [2021]");
        }

        private void HandleItemFrame(NbtCompound item, NbtList inventory)
        {
            if (!item.TryGet<NbtCompound>("tag", out var tag))
            {
                inventory.Remove(item);
                return;
            }
            if (!tag.TryGet<NbtCompound>("EntityTag", out var entityTag))
            {
                inventory.Remove(item);
                return;
            }
            if (!entityTag.TryGet<NbtByte>("Invisible", out var invisible))
            {
                inventory.Remove(item);
                return;
            }
            if (invisible.Value == 0)
            {
                inventory.Remove(item);
                return;
            }
            MakrItemAsEventItem(item, "#8F8F8F", "Невидимая неделя [2021]");
        }
        private static void MakrItemAsEventItem(NbtCompound item, string colorCode, string lore)
        {
            if (!item.TryGet<NbtCompound>("tag", out var tag))
            {
                item.Add(new NbtCompound("tag"));
                tag = item.Get<NbtCompound>("tag");
            }
            if (!tag.TryGet<NbtCompound>("display", out var display))
            {
                tag.Add(new NbtCompound("display"));
                display = tag.Get<NbtCompound>("display");
            }
            if (!display.TryGet<NbtList>("Lore", out var Lore))
            {
                display.Add(new NbtList("Lore"));
                Lore = display.Get<NbtList>("Lore");
            }
            Lore.Add(new NbtString($"{{\"extra\":[{{\"bold\":false,\"italic\":false,\"underlined\":false,\"strikethrough\":false,\"obfuscated\":false,\"color\":\"{colorCode}\",\"text\":\"{lore}\"}}],\"text\":\"\"}}"));
            if (!tag.TryGet<NbtCompound>("PublicBukkitValues", out var publicBukkitValues)) 
            {
                tag.Add(new NbtCompound("PublicBukkitValues"));
                publicBukkitValues = tag.Get<NbtCompound>("PublicBukkitValues");
            }
            if (!publicBukkitValues.Contains("somniumcore:rareitem"))
            {
                publicBukkitValues.Add(new NbtString("somniumcore:rareitem", $"{{{colorCode}}}{lore}"));
            }
        }

        private void HandleShulkerBox(NbtCompound item, NbtList inventory)
        {
            if (!item.Get<NbtCompound>("tag").Get<NbtCompound>("BlockEntityTag").TryGet<NbtList>("Items", out var items))
            {
                inventory.Remove(item);
                return;
            }

            CleanInventory(items);

            if (!items.Any())
            {
                inventory.Remove(item);
                return;
            }

            item.Get<NbtString>("id").Value = "minecraft:chest";
            item.Get<NbtCompound>("tag").Get<NbtCompound>("BlockEntityTag").Get<NbtString>("id").Value = "minecraft:chest";

            if (!item.Get<NbtCompound>("tag").TryGet<NbtCompound>("display", out var display))
            {
                item.Get<NbtCompound>("tag").Add(new NbtCompound("display"));
                display = item.Get<NbtCompound>("tag").Get<NbtCompound>("display");
            }
            if (!display.TryGet<NbtList>("Lore", out var Lore))
            {
                display.Add(new NbtList("Lore"));
                Lore = display.Get<NbtList>("Lore");
            }
            Lore
                .Add(new NbtString("{\"extra\":[{\"bold\":false,\"italic\":false,\"underlined\":false,\"strikethrough\":false,\"obfuscated\":false,\"color\":\"#8F8F8F\",\"text\":\"Шалкер конфискован, для получения\"}],\"text\":\"\"}"));
            Lore
                .Add(new NbtString("{\"extra\":[{\"bold\":false,\"italic\":false,\"underlined\":false,\"strikethrough\":false,\"obfuscated\":false,\"color\":\"#8F8F8F\",\"text\":\"содержимого поставьте сундук на землю.\"}],\"text\":\"\"}"));
        }

        private void HandleNormalItem(NbtCompound item, NbtList inventory)
        {
            if (!item.TryGet<NbtCompound>("tag", out var tag))
            {
                inventory.Remove(item);
                return;
            }
            if(!tag.TryGet<NbtCompound>("PublicBukkitValues", out var bukkitValues))
            {
                inventory.Remove(item);
                return;
            }
            if (!bukkitValues.Contains("somniumcore:rareitem"))
            {
                inventory.Remove(item);
                return;
            }
        }
    }
}

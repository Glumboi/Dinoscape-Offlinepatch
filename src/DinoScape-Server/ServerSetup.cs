using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace DinoScape_Server
{
    internal class ServerSetup
    {
        public class InventoryItem
        {
            public string ItemId { get; set; }
            public string ItemInstanceId { get; set; }
            public string CatalogVersion { get; set; }
            public string ItemClass { get; set; }

            public string Desc { get; set; }
            public int Type { get; set; }
            public int Rarity { get; set; }
            public int PriceEmeralds { get; set; }
            public int PriceAmber { get; set; }


            public override string ToString()
            {
                return $"eID: {ItemId}\n" +
                       $"eName: {ItemInstanceId}\n" +
                       $"eDescription: {Desc}\n" +
                       $"eType: {Type}\n" +
                       $"eRarity: {Rarity}\n" +
                       $"price_emeralds: {PriceEmeralds}\n" +
                       $"price_amber: {PriceAmber}";
            }

            public static void SaveItemFile(InventoryItem item, string folderPath)
            {
                string fileName = $"{item.ItemId}.asset.txt";
                string fullPath = Path.Combine(folderPath, fileName);

                File.WriteAllText(fullPath, item.ToString());
            }

            public static InventoryItem LoadItemFile(string path)
            {
                if (!File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");
                return ParseItemFile(path);
            }

        }

        public static InventoryItem ParseItemFile(string path)
        {
            var item = new InventoryItem();

            foreach (var line in File.ReadAllLines(path))
            {
                string l = line.Trim();

                if (l.StartsWith("eID:"))
                    item.ItemId = l.Replace("eID:", "").Trim();

                else if (l.StartsWith("eName:"))
                    item.ItemInstanceId = l.Replace("eName:", "").Trim();

                else if (l.StartsWith("eDescription:"))
                    item.Desc = l.Replace("eDescription:", "").Trim();

                else if (l.StartsWith("eType:"))
                    item.Type = int.Parse(l.Replace("eType:", "").Trim());

                else if (l.StartsWith("eRarity:"))
                    item.Rarity = int.Parse(l.Replace("eRarity:", "").Trim());

                else if (l.StartsWith("price_emeralds:"))
                    item.PriceEmeralds = int.Parse(l.Replace("price_emeralds:", "").Trim());

                else if (l.StartsWith("price_amber:"))
                    item.PriceAmber = int.Parse(l.Replace("price_amber:", "").Trim());
            }

            // optional defaults if needed
            item.CatalogVersion ??= "?";

            return item;
        }

        public static List<InventoryItem> InitPlayerInventory(string customsFolder)
        {
            List<InventoryItem> ret = new();

            if (!Directory.Exists(customsFolder))
                return ret;

            foreach (var file in Directory.GetFiles(customsFolder))
            {
                if (Path.GetExtension(file) == ".asset")
                {
                    try
                    {
                        ret.Add(ParseItemFile(file));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed parsing {file}: {ex.Message}");
                    }
                }
            }

            foreach (var dir in Directory.GetDirectories(customsFolder))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".asset")
                    {
                        try
                        {
                            ret.Add(ParseItemFile(file));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed parsing {file}: {ex.Message}");
                        }
                    }
                }
            }

            return ret;
        }
    }
}
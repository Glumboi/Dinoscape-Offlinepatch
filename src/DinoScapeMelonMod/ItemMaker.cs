using Harmony;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppV_AnimationSystem.SaveSystem;

namespace DinoScapeOffline
{
    public class ItemMaker
    {
        static Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            Texture2D result = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = source.GetPixelBilinear(
                        (float)x / width,
                        (float)y / height
                    );

                    result.SetPixel(x, y, color);
                }
            }

            result.Apply();
            return result;
        }

        public static EquipmentData LoadEquipmentFromIni(string path)
        {
            var ini = new SimpleIni(path);

            EquipmentData data = new EquipmentData();

            // Basic
            data.eID = ini.Get("Equipment", "eID");
            data.eName = ini.Get("Equipment", "eName");
            data.eDescription = ini.Get("Equipment", "eDescription") + "\n";

            data.eRarity = data.StringToRarity(ini.Get("Equipment", "eRarity"));
            data.eType = data.StringToType(ini.Get("Equipment", "eType"));

            data.justForPlayer = ini.GetBool("Equipment", "justForPlayer");
            data.hide = ini.GetBool("Equipment", "hide");

            data.price_amber = ini.GetInt("Equipment", "price_amber");
            data.price_emeralds = ini.GetInt("Equipment", "price_emeralds");

            // Extra fields (SAFE: only set if present)
            TrySetFloat(ini, "Equipment", "ImageScale", v => data.ImageScale = v);
            TrySetFloat(ini, "Equipment", "YOffset", v => data.YOffset = v);
            TrySetFloat(ini, "Equipment", "Rotate", v => data.Rotate = v);
            TrySetFloat(ini, "Equipment", "RealSpriteRotation", v => data.RealSpriteRotation = v);

            TrySetInt(ini, "Equipment", "ItemGemCost", v => data.ItemGemCost = v);

            if (HasKey(ini, "Equipment", "eSound"))
                data.eSound = ini.Get("Equipment", "eSound");

            // Texture
            string filePath = ini.Get("Texture", "file");
            int size = HasKey(ini, "Texture", "size") ? ini.GetInt("Texture", "size") : 64;

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, fileData);

                tex = ResizeTexture(tex, size, size);

                data.eTexture = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
            }

            return data;
        }

        static bool HasKey(SimpleIni ini, string section, string key)
        {
            try { ini.Get(section, key); return true; }
            catch { return false; }
        }

        static void TrySetFloat(SimpleIni ini, string section, string key, Action<float> setter)
        {
            try { setter(float.Parse(ini.Get(section, key))); }
            catch { }
        }

        static void TrySetInt(SimpleIni ini, string section, string key, Action<int> setter)
        {
            try { setter(int.Parse(ini.Get(section, key))); }
            catch { }
        }
    }
}

using DinoScapeOffline.Patches;
using Harmony;
using Il2Cpp;
using MelonLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;

namespace DinoScapeOffline
{
    public static class BuildInfo
    {
        public const string Name = "DinoScapeOffline"; // Name of the Mod.  (MUST BE SET)

        public const string
            Description = "Mod that enables an unlock all offline mode for DinoScape as a POC/preservation"; // Description for the Mod.  (Set as null if none)

        public const string Author = "Glumboi"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class AddressInjector : MelonMod
    {
        public static string NewServerAddress = "http://localhost:8000";
        const string _serverAddressFile = "newAddress.txt";

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Starting custom server injection...");
            MelonLogger.Msg($"Reading new server address from file: {_serverAddressFile}...");
            if (File.Exists(_serverAddressFile))
                NewServerAddress = File.ReadAllText(_serverAddressFile).Trim('\n', ' ');
            else
            {
                File.WriteAllText(_serverAddressFile, NewServerAddress);
            }
            MelonLogger.Msg($"Read new address: {NewServerAddress}");
        }

        public override void OnUpdate()
        {
#if _DEBUG
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                var data = ItemMaker.LoadEquipmentFromIni(@"");
                var list = new List<EquipmentData>(ContentHolder.Instance.allCustoms);
                list.Add(data);
                ContentHolder.Instance.allCustoms = list.ToArray();
                MelonLogger.Msg($"added item: {ContentHolder.Instance.allCustoms.Last().eName}");
            }
        }
#endif
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoScape_Server
{
    public class ServerConfig
    {
        public string CustomsPath { get; set; } = "";
        public string Address { get; set; } = "http://localhost";
        public string Port { get; set; } = "8000";
        public bool UnlockAll { get; set; } = true;

        public const string DEFAULT_PATH = "./serverConf.ini";
    }

    public static class ConfigLoader
    {
        public static ServerConfig LoadConfig(string path)
        {
            var cfg = new ServerConfig();

            if (!File.Exists(path))
            {
                Console.WriteLine($"Config not found: {path}, creating default config...");

                var defaultConfig =
            $@"CustomsPath=./customs
Address=http://localhost
Port=8000
UnlockAll=true";

                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");

                File.WriteAllText(path, defaultConfig);

                return cfg;
            }

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();

                // skip empty lines or comments
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#") || line.StartsWith(";")) continue;

                var parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2) continue;

                string key = parts[0];
                string value = parts[1];

                switch (key)
                {
                    case "CustomsPath":
                        cfg.CustomsPath = value;
                        break;

                    case "Address":
                        cfg.Address = value;
                        break;

                    case "Port":
                        cfg.Port = value;
                        break;

                    case "UnlockAll":
                        cfg.UnlockAll = bool.TryParse(value, out var b) && b;
                        break;
                }
            }

            return cfg;
        }
    }
}

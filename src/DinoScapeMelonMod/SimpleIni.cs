using System;
using System.Collections.Generic;
using System.IO;

namespace DinoScapeOffline
{
    public class SimpleIni
    {
        private Dictionary<string, Dictionary<string, string>> data =
            new Dictionary<string, Dictionary<string, string>>();

        public SimpleIni(string path)
        {
            string currentSection = "";

            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";"))
                    continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    // FIX: no C# 8 range syntax
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);

                    if (!data.ContainsKey(currentSection))
                        data[currentSection] = new Dictionary<string, string>();
                }
                else if (trimmed.Contains("="))
                {
                    // FIX: old Split overload
                    var parts = trimmed.Split(new char[] { '=' }, 2);

                    data[currentSection][parts[0]] = parts[1];
                }
            }
        }

        public string Get(string section, string key)
        {
            return data[section][key];
        }

        public int GetInt(string section, string key)
        {
            return int.Parse(Get(section, key));
        }

        public bool GetBool(string section, string key)
        {
            return bool.Parse(Get(section, key));
        }

    }
}
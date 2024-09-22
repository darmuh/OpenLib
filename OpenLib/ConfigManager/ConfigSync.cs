using BepInEx.Configuration;
using System.Collections.Generic;

namespace OpenLib.ConfigManager
{
    public class ConfigSync
    {
        /*
         * 
         *  No use-case for this at the moment.
         *  Too costly on performance
         * 
         *  Leaving code for future updates/iterations
         * 
        */

        //bool
        public static void UpdateFromHost(string key, bool value, ConfigFile myConfig)
        {
            //call this on clients in RPC
            if(ConfigHelper.TryFindConfigItem(key, myConfig, out ConfigEntryBase entry))
            {
                if(entry.GetType() == typeof(bool))
                {
                    ConfigEntry<bool> configEntry = entry as ConfigEntry<bool>;
                    if (configEntry.Value == value)
                        Plugin.Spam($"Config - {key} verified set to {value}");
                    else
                    {
                        configEntry.Value = value;
                        Plugin.WARNING($"Config sync has updated {key} to {value}");
                    }    
                }
            }
        }

        //string
        public static void UpdateFromHost(string key, string value, ConfigFile myConfig)
        {
            //call this on clients in RPC
            if (ConfigHelper.TryFindConfigItem(key, myConfig, out ConfigEntryBase entry))
            {
                if (entry.GetType() == typeof(string))
                {
                    ConfigEntry<string> configEntry = entry as ConfigEntry<string>;
                    if (configEntry.Value == value)
                        Plugin.Spam($"Config - {key} verified set to {value}");
                    else
                    {
                        configEntry.Value = value;
                        Plugin.WARNING($"Config sync has updated {key} to {value}");
                    }
                }
            }
        }

        //float
        public static void UpdateFromHost(string key, float value, ConfigFile myConfig)
        {
            //call this on clients in RPC
            if (ConfigHelper.TryFindConfigItem(key, myConfig, out ConfigEntryBase entry))
            {
                if (entry.GetType() == typeof(float))
                {
                    ConfigEntry<float> configEntry = entry as ConfigEntry<float>;
                    if (configEntry.Value == value)
                        Plugin.Spam($"Config - {key} verified set to {value}");
                    else
                    {
                        configEntry.Value = value;
                        Plugin.WARNING($"Config sync has updated {key} to {value}");
                    }
                }
            }
        }

        //int
        public static void UpdateFromHost(string key, int value, ConfigFile myConfig)
        {
            //call this on clients in RPC
            if (ConfigHelper.TryFindConfigItem(key, myConfig, out ConfigEntryBase entry))
            {
                if (entry.GetType() == typeof(int))
                {
                    ConfigEntry<int> configEntry = entry as ConfigEntry<int>;
                    if (configEntry.Value == value)
                        Plugin.Spam($"Config - {key} verified set to {value}");
                    else
                    {
                        configEntry.Value = value;
                        Plugin.WARNING($"Config sync has updated {key} to {value}");
                    }
                }
            }
        }


        public static List<ConfigEntryBase> PullMyConfig(ConfigFile myConfig)
        {
            //call on host player in RPC to send list to client

            return [.. myConfig.GetConfigEntries()];
        }

        public static Dictionary<string,string> ConfigStrings(List<ConfigEntryBase> myConfig)
        {
            Dictionary<string, string> items = [];

            if (myConfig.Count == 0)
                return items;

            foreach (ConfigEntryBase item in myConfig)
            {
                if(item.BoxedValue.GetType() == typeof(string))
                {
                    ConfigEntry<string> value = item as ConfigEntry<string>;
                    items.Add(item.Definition.Key, value.Value);
                }
                
            }
            return items;
        }

        public static Dictionary<string, bool> ConfigBools(List<ConfigEntryBase> myConfig)
        {
            Dictionary<string, bool> items = [];

            if (myConfig.Count == 0)
                return items;

            foreach (ConfigEntryBase item in myConfig)
            {
                if (item.BoxedValue.GetType() == typeof(bool))
                {
                    ConfigEntry<bool> value = item as ConfigEntry<bool>;
                    items.Add(item.Definition.Key, value.Value);
                }

            }
            return items;
        }

        public static Dictionary<string, int> ConfigInts(List<ConfigEntryBase> myConfig)
        {
            Dictionary<string, int> items = [];

            if (myConfig.Count == 0)
                return items;

            foreach (ConfigEntryBase item in myConfig)
            {
                if (item.BoxedValue.GetType() == typeof(int))
                {
                    ConfigEntry<int> value = item as ConfigEntry<int>;
                    items.Add(item.Definition.Key, value.Value);
                }

            }
            return items;
        }

        public static Dictionary<string, float> ConfigFloats(List<ConfigEntryBase> myConfig)
        {
            Dictionary<string, float> items = [];

            if (myConfig.Count == 0)
                return items;

            foreach (ConfigEntryBase item in myConfig)
            {
                if (item.BoxedValue.GetType() == typeof(float))
                {
                    ConfigEntry<float> value = item as ConfigEntry<float>;
                    items.Add(item.Definition.Key, value.Value);
                }

            }
            return items;
        }
    }
}

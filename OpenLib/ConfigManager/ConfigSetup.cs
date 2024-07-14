using BepInEx.Configuration;
using OpenLib.CoreMethods;
using OpenLib.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenLib.ConfigManager
{
    public static class ConfigSetup
    {
        public static List<ManagedBool> defaultManagedBools = [];
        public static MainListing defaultListing;
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<bool> DeveloperLogging { get; internal set; }

        public static void BindConfigSettings(List<ManagedBool> managedBools)
        {
            Plugin.Log.LogInfo("Binding configuration settings");

            ExtensiveLogging = MakeBool(Plugin.instance.Config, "Debug", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            DeveloperLogging = MakeBool(Plugin.instance.Config, "Debug", "DeveloperLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");

            ReadConfigAndAssignValues(Plugin.instance.Config, managedBools);
        }

        public static ConfigEntry<bool> AddManagedBool(ConfigFile ModConfig, List<ManagedBool> managedBools, string section, string configItemName, bool defaultValue, string configDescription, bool isNetworked = false, string category = "", List<string> keywordList = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true,  Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            ManagedBool managedBool = new()
            {
                defaultValue = defaultValue,
                MainAction = mainAction,
                KeywordList = keywordList,
                ConfigItemName = configItemName,
                RequiresNetworking = isNetworked,
                price = price,
                CommandType = commandType,
                clearText = clearText,
                ConfirmAction = confirmAction,
                DenyAction = denyAction,
                confirmText = confirmText,
                denyText = denyText,
                specialNum = specialNum,
                specialString = special,
                itemList = itemList,
                storeName = storeName,
                alwaysInStock = alwaysInStock,
                maxStock = maxStock,
                nodeName = nodeName,
                categoryText = category,
                configDescription = configDescription,
                reuseFunc = reuseFunc
            };

            managedBools.Add(managedBool);
            return ModConfig.Bind<bool>(section, configItemName, defaultValue, configDescription);
        }

        public static void NewManagedBool(ref List<ManagedBool> managedBools, string configItemName, bool isEnabled, string configDescription, bool isNetworked = false, string category = "", List<string> keywordList = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            ManagedBool managedBool = new()
            {
                ConfigValue = isEnabled,
                MainAction = mainAction,
                KeywordList = keywordList,
                ConfigItemName = configItemName,
                RequiresNetworking = isNetworked,
                price = price,
                CommandType = commandType,
                clearText = clearText,
                ConfirmAction = confirmAction,
                DenyAction = denyAction,
                confirmText = confirmText,
                denyText = denyText,
                specialNum = specialNum,
                specialString = special,
                itemList = itemList,
                storeName = storeName,
                alwaysInStock = alwaysInStock,
                maxStock = maxStock,
                nodeName = nodeName,
                categoryText = category,
                configDescription = configDescription,
                reuseFunc = reuseFunc
            };

            managedBools.Add(managedBool);
            return;
        }

        public static ConfigEntry<bool> MakeBool(ConfigFile ModConfig, string section, string configItemName, bool defaultValue, string configDescription)
        {
            return ModConfig.Bind<bool>(section, configItemName, defaultValue, configDescription);
        }

        public static ConfigEntry<int> MakeInt(ConfigFile ModConfig, string section, string configItemName, int defaultValue, string configDescription)
        {
            return ModConfig.Bind<int>(section, configItemName, defaultValue, configDescription);
        }

        public static ConfigEntry<string> MakeClampedString(ConfigFile ModConfig, string section, string configItemName, string defaultValue, string configDescription, AcceptableValueList<string> acceptedValues)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, acceptedValues));
        }

        public static ConfigEntry<int> MakeClampedInt(ConfigFile ModConfig, string section, string configItemName, int defaultValue, string configDescription, int minValue, int maxValue)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<int>(minValue, maxValue)));
        }

        public static ConfigEntry<float> MakeClampedFloat(ConfigFile ModConfig, string section, string configItemName, float defaultValue, string configDescription, float minValue, float maxValue)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<float>(minValue, maxValue)));
        }

        public static ConfigEntry<string> MakeString(ConfigFile ModConfig, string section, string configItemName, string defaultValue, string configDescription)
        {

            return ModConfig.Bind(section, configItemName, defaultValue, configDescription);
        }

        public static void RemoveOrphanedEntries(ConfigFile ModConfig)
        {
            Plugin.MoreLogs("removing orphaned entries (credits to Kittenji)");
            PropertyInfo orphanedEntriesProp = ModConfig.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(ModConfig, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            ModConfig.Save(); // Save the config file
        }

        public static void NetworkingCheck(bool NetworkConfigOption, ConfigFile ModConfig, List<ManagedBool> managedBools)
        {
            Plugin.Log.LogInfo("Checking if networking is disabled...");

            if (NetworkConfigOption)
                return;

            List<ConfigEntry<bool>> configBools = [];

            List<ConfigDefinition> configKeys = [.. ModConfig.Keys];
            foreach (ConfigDefinition item in configKeys)
            {
                if (ModConfig.TryGetEntry<bool>(item, out ConfigEntry<bool> entry))
                {
                    if (ManagedBoolGet.TryGetItemByName(managedBools, item.Key, out ManagedBool result))
                    {
                        if (result.RequiresNetworking)
                        {
                            configBools.Add(entry);
                            Plugin.Spam($"Adding {item.Key} to bools list to check against networking");
                        }
                        else
                            Plugin.Spam($"{item.Key} is not listed as requiring networking");
                    }
                    else
                        Plugin.Spam($"entry is not a managed bool");
                }
                Plugin.Spam($"entry is not a bool");
            }

            foreach (ConfigEntry<bool> configItem in configBools)
            {
                if (configItem.Value)
                {
                    configItem.Value = false;
                    Plugin.Log.LogWarning($"Setting {configItem.Definition.Key} to false. Networking is disabled and this setting requires networking!");
                }
            }

            ModConfig.Save(); // Save the config file
        }

        public static void ReadConfigAndAssignValues(ConfigFile ModConfig, List<ManagedBool> managedBools)
        {
            Plugin.Log.LogInfo("attempting to read config and assign values");
            //List<ConfigDefinition> configKeys = [.. ModConfig.Keys];

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list configItems");
            }

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                Plugin.Spam("checking item");
                if(pair.Value.BoxedValue.GetType() != typeof(bool))
                {
                    Plugin.Log.LogInfo($"skipping not bool item, {pair.Key.Key}");
                    continue;
                }
                if (ModConfig.TryGetEntry<bool>(pair.Key, out ConfigEntry<bool> entry))
                {
                    Plugin.Spam("entry found");
                    Plugin.Spam($"{entry.Definition.Key}");
                    if (ManagedBoolGet.TryGetItemByName(managedBools, entry.Definition.Key, out ManagedBool match))
                    {
                        match.ConfigValue = entry.Value;
                        Plugin.Spam($"Assigned ManagedBool: {match.ConfigItemName} to configValue: {entry.Value}");
                    }
                    else
                        Plugin.ERROR($"Could not find ManagedBool for {pair.Key.Key}");
                }
                else
                    Plugin.ERROR($"Unable to read configItem {pair.Key.Key}");
            }
        }
    }
}
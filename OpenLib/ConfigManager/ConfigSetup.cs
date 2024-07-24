using BepInEx.Configuration;
using OpenLib.CoreMethods;
using OpenLib.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using OpenLib.Common;
using Steamworks.Ugc;

namespace OpenLib.ConfigManager
{
    public static class ConfigSetup
    {
        public static List<ManagedConfig> defaultManaged = [];
        public static MainListing defaultListing;
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<bool> DeveloperLogging { get; internal set; }

        public static void BindConfigSettings()
        {
            Plugin.Log.LogInfo("Binding configuration settings");

            ExtensiveLogging = MakeBool(Plugin.instance.Config, "Debug", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            DeveloperLogging = MakeBool(Plugin.instance.Config, "Debug", "DeveloperLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");

            //ReadConfigAndAssignValues(Plugin.instance.Config, managedItems);
        }

        public static ManagedConfig AddManagedBool(ConfigEntry<bool> boolEntry, List<ManagedConfig> managedItems, bool isNetworked = false, string category = "", string configString = "", Func<string> mainAction = null, int commandType = 0, bool clearText = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            List<string> keywordList = CommonStringStuff.GetKeywordsPerConfigItem(configString);

            if (ManagedBoolGet.TryGetItemByName(managedItems, boolEntry.Definition.Key, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                managedItems.Add(managedBool);

                return managedBool;
            }
        }

        public static ManagedConfig AddManagedBool(ConfigEntry<bool> boolEntry, List<ManagedConfig> managedItems, bool isNetworked = false, string category = "", ConfigEntry<string> configString = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true,  Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            List<string> keywordList = [];
            bool isStringNull = true;
            if (configString != null)
            {
                isStringNull = false;
                keywordList = CommonStringStuff.GetKeywordsPerConfigItem(configString.Value);
            }

            if (ManagedBoolGet.TryGetItemByName(managedItems, boolEntry.Definition.Key, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                if (!isStringNull)
                {
                    AddManagedString(configString, ref managedItems, resultBool);
                }
                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);


                managedItems.Add(managedBool);
                
                if (!isStringNull)
                {
                    AddManagedString(configString, ref managedItems, managedBool);
                }

                return managedBool;
            }
        }

        public static ManagedConfig NewManagedBool(ref List<ManagedConfig> managedItems, string configItemName, bool isEnabled, string configDescription, bool isNetworked = false, string category = "", List<string> keywordList = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            if(ManagedBoolGet.TryGetItemByName(managedItems, configItemName, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(configItemName, isEnabled, configDescription, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);
                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(configItemName, isEnabled, configDescription, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                managedItems.Add(managedBool);
                return managedBool;
            } 
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

        public static void AddManagedString(ConfigEntry<String> configItem, ref List<ManagedConfig> managedItems, ManagedConfig relatedConfigItem)
        {
            ManagedConfig managedString = new()
            {
                ConfigItemName = configItem.Definition.Key,
                configDescription = configItem.Description.Description,
                StringValue = configItem.Value,
                relatedConfigItem = relatedConfigItem,
                ConfigType = 1
            };

            managedItems.Add(managedString);
        }

        public static void RemoveOrphanedEntries(ConfigFile ModConfig)
        {
            Plugin.MoreLogs("removing orphaned entries (credits to Kittenji)");
            PropertyInfo orphanedEntriesProp = ModConfig.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(ModConfig, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            ModConfig.Save(); // Save the config file
        }

        public static void NetworkingCheck(bool NetworkConfigOption, ConfigFile ModConfig, List<ManagedConfig> managedBools)
        {
            Plugin.Log.LogInfo("Checking if networking is disabled...");

            if (NetworkConfigOption)
                return;

            List<ConfigEntry<bool>> configBools = [];

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                if (pair.Value.BoxedValue.GetType() == typeof(bool))
                {
                    if (ModConfig.TryGetEntry<bool>(pair.Key, out ConfigEntry<bool> entry))
                    {
                        if (ManagedBoolGet.TryGetItemByName(managedBools, pair.Key.Key, 0, out ManagedConfig result))
                        {
                            if (result.ConfigType != 0)
                                Plugin.Spam("ManagedItem is type 0, bool");

                            if (result.RequiresNetworking)
                            {
                                configBools.Add(entry);
                                Plugin.Spam($"Adding {pair.Key.Key} to bools list to check against networking");
                            }
                            else
                                Plugin.Spam($"{pair.Key.Key} is not listed as requiring networking");
                        }
                        else
                            Plugin.Spam($"entry is not a managed bool");
                    }
                }
                else
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

        public static void ReadConfigAndAssignValues(ConfigFile ModConfig, List<ManagedConfig> managedBools) //good for config reload events
        {
            Plugin.Log.LogInfo("attempting to read config and assign values");
            //List<ConfigDefinition> configKeys = [.. ModConfig.Keys];

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                Plugin.Spam("checking item");
                if (pair.Value.BoxedValue.GetType() == typeof(bool))
                {
                    if (ModConfig.TryGetEntry<bool>(pair.Key, out ConfigEntry<bool> entry))
                    {
                        Plugin.Spam("bool entry found");
                        Plugin.Spam($"{entry.Definition.Key}");
                        if (ManagedBoolGet.TryGetItemByName(managedBools, entry.Definition.Key, 0, out ManagedConfig match))
                        {
                            match.BoolValue = entry.Value;
                            Plugin.Spam($"Assigned ManagedConfig: {match.ConfigItemName} to configValue: {entry.Value}");
                        }
                        else
                            Plugin.Log.LogWarning($"Could not find ManagedConfig for {pair.Key.Key}");
                    }
                }
                else if ((pair.Value.BoxedValue.GetType() == typeof(string)))
                {
                    if (ModConfig.TryGetEntry<string>(pair.Key, out ConfigEntry<string> entry))
                    {
                        Plugin.Spam("string entry found");
                        Plugin.Spam($"{entry.Definition.Key}");
                        if (ManagedBoolGet.TryGetItemByName(managedBools, entry.Definition.Key, 1, out ManagedConfig match))
                        {
                            match.StringValue = entry.Value;
                            Plugin.Spam($"Assigned ManagedConfig: {match.ConfigItemName} to configValue: {entry.Value}");
                        }
                        else
                            Plugin.Log.LogWarning($"Could not find ManagedConfig for {pair.Key.Key}");
                    }
                }
                else
                    Plugin.Log.LogWarning($"Unable to read configItem {pair.Key.Key} and match to Managed Config Item");
            }
        }
    }
}
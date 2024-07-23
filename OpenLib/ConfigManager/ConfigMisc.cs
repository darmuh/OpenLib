using BepInEx.Configuration;
using OpenLib.Events;
using System;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace OpenLib.ConfigManager
{
    public class ConfigMisc
    {
        public static bool CheckChangedConfigSetting(List<ManagedConfig> managedItems, ConfigEntryBase entryBase) //used in setting change events
        {
            foreach (ManagedConfig item in managedItems)
            {
                if(item.configDescription == entryBase.Description.Description)
                {
                    if (entryBase.BoxedValue.GetType() == typeof(bool))
                    {
                        if (entryBase.ConfigFile.TryGetEntry<bool>(entryBase.Definition, out ConfigEntry<bool> entry))
                        {
                            if (item.BoolValue != entry.Value)
                            {
                                item.ConfigChange(entry.Value);
                                Plugin.Spam($"Updating config item: {entry.Definition.Key} in managedItems");
                                return true;
                            }
                            else
                                Plugin.Spam($"item value matches config item: {entry.Definition.Key}");
                        }
                    }
                    else if (entryBase.BoxedValue.GetType() == typeof(string))
                    {
                        if (entryBase.ConfigFile.TryGetEntry<string>(entryBase.Definition, out ConfigEntry<string> entry))
                        {
                            if (item.StringValue != entry.Value)
                            {
                                item.ConfigChange(entry.Value);
                                Plugin.Spam($"Updating config item: {entry.Definition.Key} in managedItems");
                                List<string> newKeywordList = Common.CommonStringStuff.GetKeywordsPerConfigItem(entry.Value);
                                item.relatedConfigItem.KeywordList = newKeywordList;

                                return true;
                            }
                            else
                                Plugin.Spam($"item value matches config item: {entry.Definition.Key}");
                        }
                    }
                }
            }

            Plugin.Spam("could not match changed config setting to any managed config items");
            return false;
        }

        public static bool ShouldReloadConfigNow(ConfigEntry<string> entry)
        {

            if (Plugin.instance.Terminal != null)
            {
                if (!EventUsage.configsToReload.Contains(entry.ConfigFile))
                    EventUsage.configsToReload.Add(entry.ConfigFile);
                return false;
            }
            else
                return true;
            
        }
    }
}

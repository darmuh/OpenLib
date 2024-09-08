using BepInEx.Configuration;
using OpenLib.Common;
using System.Collections.Generic;

namespace OpenLib.ConfigManager
{
    public class ConfigHelper
    {

        public static List<string> GetAcceptableValues(AcceptableValueBase acceptableValueBase)
        {
            List<string> result = [];
            if(acceptableValueBase == null)
                return result;
            else
            {
                string description = acceptableValueBase.ToDescriptionString();
                //# Acceptable values: Hauntings, Intervals, Insanity, None
                description = description.Replace("# Acceptable values:", "").Replace(" ", "");
                result = CommonStringStuff.GetKeywordsPerConfigItem(description, ',');
                return result;
            }
        }

        public static List<float> GetAcceptableValueF(AcceptableValueBase acceptableValueBase)
        {
            List<float> result = [];
            if (acceptableValueBase == null)
                return result;
            else
            {
                string description = acceptableValueBase.ToDescriptionString();
                Plugin.Spam(description);
                //# Acceptable value range: From 0 to 100
                //# Acceptable value range: From 0.1 to 10
                description = description.Replace("# Acceptable value range: From ", "").Replace("to","").Replace(" ", ",");
                result = CommonStringStuff.GetFloatListFromStringList(CommonStringStuff.GetKeywordsPerConfigItem(description, ','));
                return result;
            }
        }

        public static void ChangeBool(ConfigFile ModConfig, ConfigEntryBase configItem, string newValue)
        {
            if (newValue.Length == 0)
                return;

            if (ModConfig.TryGetEntry<bool>(configItem.Definition, out ConfigEntry<bool> entry))
            {
                if (newValue == "true")
                    entry.Value = true;
                else
                    entry.Value = false;
            }
        }

        public static void ChangeString(ConfigFile ModConfig, ConfigEntryBase configItem, string newValue)
        {
            if (newValue.Length == 0)
                return;

            if (ModConfig.TryGetEntry<string>(configItem.Definition, out ConfigEntry<string> entry))
            {
                entry.Value = newValue;
            }
        }

        public static void ChangeInt(ConfigFile ModConfig, ConfigEntryBase configItem, string newValue)
        {
            if (newValue.Length == 0)
                return;

            if (ModConfig.TryGetEntry<int>(configItem.Definition, out ConfigEntry<int> entry))
            {
                entry.Value = int.Parse(newValue);
            }
        }

        public static void ChangeFloat(ConfigFile ModConfig, ConfigEntryBase configItem, string newValue)
        {
            if (newValue.Length == 0)
                return;

            if (ModConfig.TryGetEntry<float>(configItem.Definition, out ConfigEntry<float> entry))
            {
                entry.Value = float.Parse(newValue);
            }
        }
    }
}

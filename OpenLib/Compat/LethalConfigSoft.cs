using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using System;
using System.Reflection;

namespace OpenLib.Compat
{
    public class LethalConfigSoft
    {
        public static Version LethalConfigVersion;
        public static Version MinVer = new(1, 4, 3); //needed to make sure people are on the version that adds my method

        [Obsolete("Do not call this method from another mod. The config items will be added directly to OpenLib. Instead copy this method and use it in your own mod")]
        public static void QueueConfig(ConfigFile configName)
        {
            if (!IsLethalConfigUpdated())
                return;

            Plugin.Spam($"Queuing file {configName.ConfigFilePath}");
            LethalConfigManager.QueueCustomConfigFileForLateAutoGeneration(configName);

        }

        public static bool IsLethalConfigUpdated()
        {
            if (!Plugin.instance.LethalConfig)
                return false;

            if (LethalConfigVersion == null)
            {
                Plugin.ERROR("Unable to get version of LethalConfig!");
                return false;
            }

            if (LethalConfigVersion < MinVer)
            {
                Plugin.WARNING($"Cannot queue config! LethalConfig version is {LethalConfigVersion}, which is below the minimum required for this function {MinVer}");
                return false;
            }

            return true;
        }

        public static Version GetVersion(string PluginGUID)
        {
            if (Chainloader.PluginInfos.TryGetValue(PluginGUID, out var info))
            {
                return info.Metadata.Version;
            }
            else
                return null;
        }

        public static void AddButton(string section, string name, string description, string buttonText, Action methodToCall)
        {
            if (!Plugin.instance.LethalConfig)
                return;
            Assembly Caller = Assembly.GetCallingAssembly();
            Plugin.MoreLogs($"AddLoadCodeButton called from {Caller.GetName().Name}!\nName: {name}\nDescription: {description}\nButtonText: {buttonText}");


            LethalConfigManager.AddConfigItem(new GenericButtonConfigItem(section, name, description, buttonText, () =>
            {
                //code
                Plugin.Spam($"LethalConfig button [{buttonText}] has been pressed");
                methodToCall.Invoke();
            }), Caller);
        }
    }
}

using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using System;

namespace OpenLib.Compat
{
    public class LethalConfigSoft
    {
        public static void QueueConfig(ConfigFile configName)
        {
            if (!Plugin.instance.LethalConfig)
                return;

            Plugin.Spam($"Queuing file {configName.ConfigFilePath}");
            LethalConfigManager.QueueCustomConfigFileForAutoGeneration(configName);
        }

        public static void AddButton(string section, string name, string description, string buttonText, Action methodToCall)
        {
            if (!Plugin.instance.LethalConfig)
                return;

            Plugin.MoreLogs("AddLoadCodeButton called!");

            LethalConfigManager.AddConfigItem(new GenericButtonConfigItem(section, name, description, buttonText, () =>
            {
                //code
                Plugin.Spam($"LethalConfig button [{buttonText}] has been pressed");
                methodToCall.Invoke();
            }));
        }
    }
}

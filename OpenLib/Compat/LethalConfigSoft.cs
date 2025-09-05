using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using System;
using System.Reflection;
using static OpenLib.Common.Misc;

namespace OpenLib.Compat;
public class LethalConfigSoft
{
    public static Version LethalConfigVersion => GetPluginVersion("ainavt.lc.lethalconfig");
    public static Version MinVer = new(1, 4, 3); //needed to make sure people are on the version that adds my method

    [Obsolete("Do not call this method from another mod. The config items will be added directly to OpenLib. Instead copy this method and use it in your own mod")]
    public static void QueueConfig(ConfigFile configName)
    {
        if (!IsLethalConfigUpdated())
            return;

        Loggers.LogDebug($"Queuing file {configName.ConfigFilePath}");
        LethalConfigManager.QueueCustomConfigFileForLateAutoGeneration(configName);

    }

    public static bool IsLethalConfigUpdated()
    {
        if (!Plugin.instance.LethalConfig)
            return false;

        if (LethalConfigVersion == null!)
        {
            Loggers.FATAL("Unable to get version of LethalConfig!");
            return false;
        }

        if (LethalConfigVersion < MinVer)
        {
            Loggers.WARNING($"Cannot queue config! LethalConfig version is {LethalConfigVersion}, which is below the minimum required for this function {MinVer}");
            return false;
        }

        return true;
    }

    public static void AddButton(string section, string name, string description, string buttonText, Action methodToCall)
    {
        if (!Plugin.instance.LethalConfig)
            return;
        Assembly Caller = Assembly.GetCallingAssembly();
        Loggers.LogInfo($"AddLoadCodeButton called from {Caller.GetName().Name}!\nName: {name}\nDescription: {description}\nButtonText: {buttonText}");


        LethalConfigManager.AddConfigItem(new GenericButtonConfigItem(section, name, description, buttonText, () =>
        {
            //code
            Loggers.LogDebug($"LethalConfig button [{buttonText}] has been pressed");
            methodToCall.Invoke();
        }), Caller);
    }

    [Obsolete("Use OpenLib.Common.Misc.GetPluginVersion() instead!")]
    public static Version GetVersion(string PluginGUID)
    {
        return GetPluginVersion(PluginGUID);
    }
}

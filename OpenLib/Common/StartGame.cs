using BepInEx.Bootstrap;
using OpenLib.Compat;
using System.Reflection;

namespace OpenLib.Common;
public class StartGame
{
    internal static bool oneTimeOnly = false;
    internal static void CompatibilityCheck()
    {
        if (SoftCompatibility(Constants.LobbyCompat_GUID, ref Plugin.instance.LobbyCompat))
        {
            Loggers.LogDebug("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
            BMX_LobbyCompat.SetCompat(false);
        }
        if (SoftCompatibility(Constants.TF_GUID, ref Plugin.instance.TerminalFormatter))
        {
            Loggers.LogDebug("Terminal Formatter by mrov detected!");
        }
        if (SoftCompatibility(Constants.ITAPI_GUID, ref Plugin.instance.ITAPI))
        {
            Loggers.LogDebug("InteractiveTerminalAPI by WhiteSpike detected!");
        }
        if (SoftCompatibility(Constants.LethalConfig_GUID, ref Plugin.instance.LethalConfig))
        {
            Loggers.LogDebug("LethalConfig functions enabled!");
        }
        if (SoftCompatibility(Constants.OBC_GUID, ref Plugin.instance.OpenBodyCamsMod))
        {
            Loggers.LogDebug("OpenBodyCams by Zaggy1024 detected!");
        }
        if (SoftCompatibility(Constants.TWORM_GUID, ref Plugin.instance.TwoRadarMapsMod))
        {
            Loggers.LogDebug("TwoRadarMaps by Zaggy1024 detected!");
        }
        if (SoftCompatibility(Constants.MRAPI_GUID, ref Plugin.instance.ModelReplacement))
            Loggers.LogDebug("ModelReplacementAPI detected!");

        if (SoftCompatibility(Constants.TME_GUID, ref Plugin.instance.TooManyEmotes))
            Loggers.LogDebug("TooManyEmotes by FlipMods detected!");

        if (SoftCompatibility(Constants.Mirror_GUID, ref Plugin.instance.MirrorDecor))
            Loggers.LogDebug("MirrorDecor detected!");

        if (SoftCompatibility(Constants.TerminalStuff_GUID, ref Plugin.instance.TerminalStuff))
            Loggers.LogDebug("TerminalStuff detected!");
    }
    internal static void OnGameStart()
    {
        CompatibilityCheck();
        oneTimeOnly = false;
    }

    public static bool SoftCompatibility(string PluginGUID, ref bool isDetected)
    {
        if (Chainloader.PluginInfos.ContainsKey(PluginGUID))
        {
            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            isDetected = true;
            if (MyPluginInfo.PLUGIN_NAME != YourPluginName)
                Plugin.Log.LogInfo($"{PluginGUID} detected! Plugin: {YourPluginName} has set compatibility bool - {isDetected}");
            return isDetected;
        }
        return isDetected = false;
    }
}

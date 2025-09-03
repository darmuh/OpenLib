using BepInEx.Bootstrap;
using OpenLib.Compat;
using System.Reflection;

namespace OpenLib.Common
{
    public class StartGame
    {
        internal static bool oneTimeOnly = false;
        internal static void CompatibilityCheck()
        {
            if (SoftCompatibility("BMX.LobbyCompatibility", ref Plugin.instance.LobbyCompat))
            {
                Loggers.LogDebug("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
                BMX_LobbyCompat.SetCompat(false);
            }
            if (SoftCompatibility("TerminalFormatter", ref Plugin.instance.TerminalFormatter))
            {
                Loggers.LogDebug("Terminal Formatter by mrov detected!");
            }
            if (SoftCompatibility("WhiteSpike.InteractiveTerminalAPI", ref Plugin.instance.ITAPI))
            {
                Loggers.LogDebug("InteractiveTerminalAPI by WhiteSpike detected!");
            }
            if (SoftCompatibility("ainavt.lc.lethalconfig", ref Plugin.instance.LethalConfig))
            {
                LethalConfigSoft.LethalConfigVersion = Misc.GetPluginVersion("ainavt.lc.lethalconfig");
                Loggers.LogDebug("LethalConfig functions enabled!");
            }
            if (SoftCompatibility("Zaggy1024.OpenBodyCams", ref Plugin.instance.OpenBodyCamsMod))
            {
                Loggers.LogDebug("OpenBodyCams by Zaggy1024 detected!");
            }
            if (SoftCompatibility("Zaggy1024.TwoRadarMaps", ref Plugin.instance.TwoRadarMapsMod))
            {
                Loggers.LogDebug("TwoRadarMaps by Zaggy1024 detected!");
            }
            if (SoftCompatibility("meow.ModelReplacementAPI", ref Plugin.instance.ModelReplacement))
                Loggers.LogDebug("ModelReplacementAPI detected!");

            if (SoftCompatibility("FlipMods.TooManyEmotes", ref Plugin.instance.TooManyEmotes))
                Loggers.LogDebug("TooManyEmotes by FlipMods detected!");

            if (SoftCompatibility("quackandcheese.mirrordecor", ref Plugin.instance.MirrorDecor))
                Loggers.LogDebug("MirrorDecor detected!");
            if (SoftCompatibility("darmuh.TerminalStuff", ref Plugin.instance.TerminalStuff))
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
                if (Plugin.PluginInfo.PLUGIN_NAME != YourPluginName)
                    Plugin.Log.LogInfo($"{PluginGUID} detected! Plugin: {YourPluginName} has set compatibility bool - {isDetected}");
                return isDetected;
            }
            return isDetected = false;
        }
    }
}

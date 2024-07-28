using BepInEx;
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
                Plugin.Spam("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
                BMX_LobbyCompat.SetCompat(false);
            }
            if (SoftCompatibility("TerminalFormatter", ref Plugin.instance.TerminalFormatter))
            {
                Plugin.Spam("Terminal Formatter by mrov detected!");
            }
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
                Plugin.Log.LogInfo($"{PluginGUID} detected! Plugin: {YourPluginName} has set compatibility bool - {isDetected}");
                return isDetected;
            }
            return isDetected = false;
        }
    }
}

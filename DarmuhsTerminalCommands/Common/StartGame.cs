using BepInEx.Bootstrap;
using OpenLib.Compat;

namespace OpenLib.Common
{
    public class StartGame
    {
        public static bool oneTimeOnly = false;
        public static void CompatibilityCheck()
        {
            if (Chainloader.PluginInfos.ContainsKey("BMX.LobbyCompatibility"))
            {
                Plugin.MoreLogs("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
                Plugin.instance.LobbyCompat = true;
                BMX_LobbyCompat.SetCompat(false);
            }
            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
            {
                Plugin.MoreLogs("Terminal Formatter by mrov detected!");
                Plugin.instance.TerminalFormatter = true;
            }
        }
        public static void OnGameStart()
        {
            CompatibilityCheck();
            oneTimeOnly = false;
        }
    }
}

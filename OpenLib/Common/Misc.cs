using BepInEx.Configuration;
using GameNetcodeStuff;
using UnityEngine;

namespace OpenLib.Common
{
    public class Misc
    {
        public static PlayerControllerB GetPlayerFromName(string playerName)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.playerUsername.ToLower() == playerName)
                {
                    return player;
                }
            }

            return null;
        }

        public static PlayerControllerB GetPlayerUsingTerminal()
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (!player.isPlayerDead && player.currentTriggerInAnimationWith == Plugin.instance.Terminal.terminalTrigger)
                {
                    Plugin.MoreLogs($"Player: {player.playerUsername} detected using terminal.");
                    return player;
                }
            }
            return null;
        }

        public static int HostClientID()
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isHostPlayerObject)
                {
                    Plugin.MoreLogs($"Player: {player.playerUsername} is the host, client ID: {player.playerClientId}.");
                    return ((int)player.playerClientId);
                }
            }

            return -1;
        }
        public static Color HexToColor(string hex)
        {
            // Convert hex color code to Color
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }

        public static void LogColorBeforeChange(Color color, ConfigEntry<string> entry)
        {
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            Plugin.Log.LogDebug($"Previous Color noted as [{hexColor}] for configItem - {entry.Definition.Key}");
        }
    }
}

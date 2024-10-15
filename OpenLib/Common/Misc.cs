using BepInEx.Configuration;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace OpenLib.Common
{
    public class Misc
    {
        public static bool TryGetPlayerFromName(string playerName, out PlayerControllerB thePlayer)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.playerUsername.ToLower() == playerName)
                {
                    thePlayer = player;
                    return true;
                }
            }

            thePlayer = null!;
            return false;
        }

        public static bool TryGetPlayerUsingTerminal(out PlayerControllerB terminalUser)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (!player.isPlayerDead && player.currentTriggerInAnimationWith == Plugin.instance.Terminal.terminalTrigger)
                {
                    Plugin.MoreLogs($"Player: {player.playerUsername} detected using terminal.");
                    terminalUser = player;
                    return true;
                }
            }

            terminalUser = null!;
            return false;
        }

        public static bool TryGetHostClientID(out int HostClientID)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isHostPlayerObject)
                {
                    Plugin.MoreLogs($"Player: {player.playerUsername} is the host, client ID: {player.playerClientId}.");
                    HostClientID = ((int)player.playerClientId);
                    return true;
                }
            }

            HostClientID = -1;
            return false;
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


        // ----------------- Obsolete Old Methods ----------------- //

        [Obsolete("Use TryGetHostClientID instead to avoid NRE")]
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

        [Obsolete("Use TryGetPlayerFromName instead to avoid NRE")]
        public static PlayerControllerB GetPlayerFromName(string playerName)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.playerUsername.ToLower() == playerName)
                {
                    return player;
                }
            }

            return null!;
        }

        [Obsolete("Use TryGetPlayerUsingTerminal instead to avoid NRE")]
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
            return null!;
        }
    }
}

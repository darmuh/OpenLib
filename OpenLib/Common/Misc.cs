using BepInEx.Bootstrap;
using BepInEx.Configuration;
using GameNetcodeStuff;
using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace OpenLib.Common
{
    public class Misc
    {
        public static Random Random = new();
        public static bool TryGetPlayerFromName(string playerName, out PlayerControllerB thePlayer)
        {
            thePlayer = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(p => p.playerUsername.ToLower() == playerName.ToLower());
            return thePlayer != null;
        }

        public static bool TryGetPlayerUsingTerminal(out PlayerControllerB terminalUser)
        {
            terminalUser = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => !player.isPlayerDead && player.currentTriggerInAnimationWith == Plugin.instance.Terminal.terminalTrigger);

            return terminalUser != null;        }

        public static bool TryGetHostClientID(out int HostClientID)
        {
            PlayerControllerB host = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => player.isHostPlayerObject);
            if(host == null)
            {
                HostClientID = -1;
                return false;
            }

            HostClientID = (int)host.playerClientId;
            return true;
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

        public static Version GetPluginVersion(string PluginGUID)
        {
            if (Chainloader.PluginInfos.TryGetValue(PluginGUID, out var info))
            {
                return info.Metadata.Version;
            }
            else
                return null!;
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

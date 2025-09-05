using BepInEx.Bootstrap;
using BepInEx.Configuration;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace OpenLib.Common;

public class Misc
{
    public static Random Random = new();
    public static bool TryGetPlayerFromName(string playerName, out PlayerControllerB thePlayer)
    {
        thePlayer = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(p => CompareStringsInvariant(p.playerUsername, playerName));
        return thePlayer != null!;
    }

    //rather than clamping, this will take a value and return it to the opposite end of the index
    public static int CycleIndex(int value, int min, int max)
    {
        if (value < min)
            return max;

        if (value > max)
            return min;

        return value;
    }

    public static bool TryGetPlayerUsingTerminal(out PlayerControllerB terminalUser)
    {
        terminalUser = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => !player.isPlayerDead && player.currentTriggerInAnimationWith == Plugin.instance.Terminal.terminalTrigger);

        return terminalUser != null!;
    }

    public static bool TryGetHostClientID(out int HostClientID)
    {
        PlayerControllerB host = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => player.isHostPlayerObject);
        if (host == null!)
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
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;
        else
        {
            Loggers.WARNING($"Unable to get color from hex: {hex}\nReturning color - white");
            return Color.white;
        }
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

    //This is a commonly used method throughout the library
    public static bool CompareStringsInvariant(string str1, string str2, bool ignoreCase = true)
    {
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture;

        return str1.Equals(str2, comparison);
    }

    //check if a whole list is equal to the original string
    public static bool CompareStringsInvariant(List<string> stringList, bool ignoreCase = true)
    {
        if (stringList == null || stringList.Count < 2)
            return true; // Empty or single-element lists are "all same"

        string first = stringList[0];
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture;

        for (int i = 1; i < stringList.Count; i++)
        {
            if (!string.Equals(first, stringList[i], comparison))
                return false; // Immediate exit on mismatch
        }

        return true;
    }

    public static bool StringStartsWithInvariant(string fullstring, char ch, bool ignoreCase = true)
    {
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture;

        return fullstring.StartsWith($"{ch}", comparison);
    }

    public static bool StringStartsWithInvariant(string fullstring, string str, bool ignoreCase = true)
    {
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture;
        return fullstring.StartsWith(str, comparison);
    }

    public static bool StringContainsInvariant(string fullstring, string query, bool ignoreCase = true)
    {
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture;
        return fullstring.Contains(query, comparison);
    }


    // ----------------- Obsolete Old Methods ----------------- //

    [Obsolete("Use TryGetHostClientID instead to avoid NRE")]
    public static int HostClientID()
    {
        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player.isHostPlayerObject)
            {
                Loggers.LogInfo($"Player: {player.playerUsername} is the host, client ID: {player.playerClientId}.");
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
            if (CompareStringsInvariant(player.playerUsername, playerName))
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
                Loggers.LogInfo($"Player: {player.playerUsername} detected using terminal.");
                return player;
            }
        }
        return null!;
    }
}

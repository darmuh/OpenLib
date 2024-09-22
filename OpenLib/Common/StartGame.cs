﻿using BepInEx;
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
            if (SoftCompatibility("ainavt.lc.lethalconfig", ref Plugin.instance.LethalConfig))
                Plugin.Spam("LethalConfig functions enabled!");
            if (SoftCompatibility("Zaggy1024.OpenBodyCams", ref Plugin.instance.OpenBodyCamsMod))
            {
                Plugin.Spam("OpenBodyCams by Zaggy1024 detected!");
            }
            if (SoftCompatibility("Zaggy1024.TwoRadarMaps", ref Plugin.instance.TwoRadarMapsMod))
            {
                Plugin.Spam("TwoRadarMaps by Zaggy1024 detected!");
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
                if(Plugin.PluginInfo.PLUGIN_GUID != PluginGUID)
                    Plugin.Log.LogInfo($"{PluginGUID} detected! Plugin: {YourPluginName} has set compatibility bool - {isDetected}");
                return isDetected;
            }
            return isDetected = false;
        }
    }
}

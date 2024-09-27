using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using OpenLib.Events;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace OpenLib
{
    [BepInPlugin("darmuh.OpenLib", "OpenLib", (PluginInfo.PLUGIN_VERSION))]

    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.OpenLib";
            public const string PLUGIN_NAME = "OpenLib";
            public const string PLUGIN_VERSION = "0.2.1";
        }
        
        internal static ManualLogSource Log;

        //Compatibility
        public bool LobbyCompat = false;
        public bool TerminalFormatter = false;
        public bool LethalConfig = false;
        public bool OpenBodyCamsMod = false;
        public bool TwoRadarMapsMod = false;

        public static List<TerminalKeyword> keywordsAdded = [];
        public static List<TerminalNode> nodesAdded = [];
        public static List<CompatibleNoun> nounsAdded = [];

        public Terminal Terminal;
        public static List<TerminalNode> ShopNodes = [];


        private void Awake()
        {
            instance = this;
            Log = base.Logger;
            Log.LogInfo((object)$"{PluginInfo.PLUGIN_NAME} is loading with version {PluginInfo.PLUGIN_VERSION}!");
            ConfigSetup.defaultManaged = [];
            ConfigSetup.defaultListing = new();
            CommandRegistry.InitListing(ref ConfigSetup.defaultListing);
            ConfigSetup.BindConfigSettings();
            Config.ConfigReloaded += OnConfigReloaded;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            EventUsage.Subscribers();
            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} load complete!");
        }

        internal void OnConfigReloaded(object sender, EventArgs e)
        {
            Log.LogInfo("Config has been reloaded!");
            ConfigSetup.ReadConfigAndAssignValues(Plugin.instance.Config, ConfigSetup.defaultManaged);
        }

        internal static void MoreLogs(string message)
        {
            if (ConfigSetup.ExtensiveLogging.Value)
                Log.LogInfo(message);
            else
                return;
        }

        internal static void Spam(string message)
        {
            if (ConfigSetup.DeveloperLogging.Value)
                Log.LogDebug(message);
            else
                return;
        }

        internal static void ERROR(string message)
        {
            Log.LogError(message);
        }

        internal static void WARNING(string message)
        {
            Log.LogWarning(message); 
        }
    }

}
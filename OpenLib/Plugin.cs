using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using OpenLib.Events;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace OpenLib;

[BepInPlugin("darmuh.OpenLib", "OpenLib", (PluginInfo.PLUGIN_VERSION))]

public class Plugin : BaseUnityPlugin
{
    public static Plugin instance = null!;
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "darmuh.OpenLib";
        public const string PLUGIN_NAME = "OpenLib";
            public const string PLUGIN_VERSION = "0.4.0";
    }

    internal static ManualLogSource Log = null!;

    //Compatibility
    public bool TerminalStuff = false;
    public bool LobbyCompat = false;
    public bool TerminalFormatter = false;
    public bool ITAPI = false;
    public bool LethalConfig = false;
    public bool OpenBodyCamsMod = false;
    public bool TwoRadarMapsMod = false;
    public bool ModelReplacement = false;
    public bool TooManyEmotes = false;
    public bool MirrorDecor = false;

    public static List<CommandManager> AllCommands = [];
    public static List<TerminalKeyword> KeywordsAdded = [];
    public static List<TerminalNode> NodesAdded = [];
    public static List<CompatibleNoun> NounsAdded = [];
    public static List<TerminalAccessibleObject> AllTerminalCodes = [];

    public Terminal Terminal = null!;
    public static List<TerminalNode> ShopNodes = [];

    private void Awake()
    {
        instance = this;
        Log = base.Logger;
        Log.LogInfo($"{PluginInfo.PLUGIN_NAME} is loading with version {PluginInfo.PLUGIN_VERSION}!");
        ConfigSetup.defaultManaged = [];
            CommandRegistry.InitListing(ref ConfigSetup.defaultListing);
        ConfigSetup.BindConfigSettings();
        Config.ConfigReloaded += OnConfigReloaded;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        EventUsage.Subscribers();
        AllInteractiveMenus.AllMenus = [];
        Log.LogInfo($"{PluginInfo.PLUGIN_NAME} load complete!");
    }

    internal void OnConfigReloaded(object sender, EventArgs e)
    {
        Log.LogInfo("Config has been reloaded!");
        ConfigSetup.ReadConfigAndAssignValues(Plugin.instance.Config, ConfigSetup.defaultManaged);
    }
}
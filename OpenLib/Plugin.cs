using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using OpenLib.Events;
using UnityEngine;


namespace OpenLib;
#pragma warning disable 0436 //Ignore warnings about other BepinAutoPlugins from references
[BepInAutoPlugin("darmuh.OpenLib")] //Should autoset version
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin instance = null!;
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
    public bool DawnLibPresent => Chainloader.PluginInfos.ContainsKey("com.github.teamxiaolan.dawnlib");

    public static List<CommandManager> AllCommands = [];
    public static List<CommandManager> GetActiveCommands()
    {
        if (AllCommands.Count == 0)
            return [];

        return AllCommands.FindAll(x => x.IsCreated);
    }

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
        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is loading with version {MyPluginInfo.PLUGIN_VERSION}!\nThis mod has been compiled to be compatible with the experimental version of Dawnlib!");
        ConfigSetup.defaultManaged = [];
        CommandRegistry.InitListing(ref ConfigSetup.defaultListing);
        ConfigSetup.BindConfigSettings();
        Config.ConfigReloaded += OnConfigReloaded;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        EventUsage.Subscribers();
        AllInteractiveMenus.AllMenus = [];
        string bundlepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "openlib.bundle");
        if (File.Exists(bundlepath))
        {
            var bundle = AssetBundle.LoadFromFile(bundlepath);
            NetworkClassBase.Prefab = bundle.LoadAsset<GameObject>("Openlib Networker");
        }
        else
            Log.LogError($"Openlib Networker asset cannot be found! Expected path: {bundlepath}");
        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} load complete!");
    }

    internal void OnConfigReloaded(object sender, EventArgs e)
    {
        Log.LogInfo("Config has been reloaded!");
        ConfigSetup.ReadConfigAndAssignValues(Plugin.instance.Config, ConfigSetup.defaultManaged);
    }
}
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static TerminalStuff.GetFromConfig;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using static TerminalStuff.NoMoreAPI.RemoveThings;


namespace TerminalStuff
{
    [BepInPlugin("darmuh.TerminalStuff", "darmuhsTerminalStuff", (PluginInfo.PLUGIN_VERSION))]
    [BepInDependency("Rozebud.FovAdjust", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Zaggy1024.OpenBodyCams", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Zaggy1024.TwoRadarMaps", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("darmuh.suitsTerminal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TerminalFormatter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]


    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.TerminalStuff";
            public const string PLUGIN_NAME = "darmuhsTerminalStuff";
            public const string PLUGIN_VERSION = "3.2.3";
        }

        internal static ManualLogSource Log;

        //Compatibility
        public bool LobbyCompat = false;
        public bool CompatibilityAC = false;
        public bool LateGameUpgrades = false;
        public bool FovAdjust = false;
        public bool HelmetCamsMod = false;
        public bool SolosBodyCamsMod = false;
        public bool OpenBodyCamsMod = false;
        public bool TwoRadarMapsMod = false;
        public bool suitsTerminal = false;
        public bool TerminalFormatter = false;

        //public stuff for instance
        public bool radarNonPlayer = false;
        public bool isOnMirror = false;
        public bool isOnCamera = false;
        public bool isOnMap = false;
        public bool isOnOverlay = false;
        public bool isOnMiniMap = false;
        public bool isOnMiniCams = false;
        public bool activeCam = false;
        public bool splitViewCreated = false;

        //flashlight stuff
        public bool fSuccess = false;
        public bool hSuccess = false;

        //AutoComplete
        internal bool removeTab = false;

        internal Terminal Terminal;
        internal static List<TerminalNode> Allnodes = [];
        internal static ShipTeleporter NormalTP;
        internal static ShipTeleporter InverseTP;
        //internal ManualCameraRenderer MapScreen;


        public RawImage rawImage1;
        public RawImage rawImage2;
        public RenderTexture renderTexturePub;
        public Canvas terminalCanvas;
        public Vector2 originalTopSize;
        public Vector2 originalTopPosition;
        public Vector2 originalBottomSize;
        public Vector2 originalBottomPosition;
        public GameObject myNetworkPrefab;

        private void Awake()
        {
            instance = this;
            Log = base.Logger;
            Log.LogInfo((object)$"{PluginInfo.PLUGIN_NAME} is loaded with version {PluginInfo.PLUGIN_VERSION}!");
            Log.LogInfo((object)"--------[Now with more Quality of Life!]---------");
            ConfigSettings.BindConfigSettings();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            //LeaveTerminal.AddTest(); //this command is only for devtesting
            //Addkeywords used to be here
            VideoManager.Load();
            CreateKeywordLists();

            //start of networking stuff

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            //end of networking stuff
        }

        internal static void MoreLogs(string message)
        {
            if (ConfigSettings.extensiveLogging.Value)
                Log.LogInfo(message);
            else
                return;
        }

        internal static void Spam(string message)
        {
            if (ConfigSettings.developerLogging.Value)
                Log.LogInfo(message);
            else
                return;
        }

        internal static void ERROR(string message)
        {
            Log.LogError(message);
        }

        internal static void ClearLists()
        {
            //decided to let commands persist between different saves while game is still launched...
            //if someone wants to disable a command, currently they will need to relaunch

            //trying to recreate commands
            darmuhsUnlockableNodes.Clear(); //not a good idea to clear unless commands get re-created
            darmuhsStorePacks.Clear();
            DeleteAllNodes(ref TerminalEvents.darmuhsTerminalStuff);
            DeleteAllKeywords(ref TerminalEvents.darmuhsKeywords);
            ViewCommands.termViewNodes.Clear(); //clearing causes issues, so does trying to re-use nodes
            ViewCommands.termViewNodeNums.Clear();
            DynamicCommands.nodesThatAcceptAnyString.Clear();
            DynamicCommands.nodesThatAcceptNum.Clear();
            CostCommands.itemsIndexed.Clear();
        }

        internal static void AddKeywords()
        {
            DynamicCommands.GetConfigKeywordsToUse();
            AddKeywordIfEnabled(ConfigSettings.terminalQuit.Value, TerminalEvents.AddQuitKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalVideo.Value, TerminalEvents.VideoKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalLoot.Value, TerminalEvents.LootKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalCams.Value, TerminalEvents.CamsKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalClear.Value, TerminalEvents.ClearKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalHeal.Value, TerminalEvents.HealKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalDanger.Value, TerminalEvents.DangerKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalMods.Value, TerminalEvents.AddModListKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalOverlay.Value, TerminalEvents.AddOverlayView);
            AddKeywordIfEnabled(ConfigSettings.terminalMinimap.Value, TerminalEvents.AddMiniMapKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalMinicams.Value, TerminalEvents.AddMiniCams);
            AddKeywordIfEnabled(ConfigSettings.terminalMap.Value, TerminalEvents.MapKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalDoor.Value, TerminalEvents.AddDoor);
            AddKeywordIfEnabled(ConfigSettings.terminalAlwaysOn.Value, TerminalEvents.AddAlwaysOnKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalLights.Value, TerminalEvents.AddLights);
            AddKeywordIfEnabled(ConfigSettings.terminalRandomSuit.Value, TerminalEvents.AddRandomSuit);
            AddKeywordIfEnabled(ConfigSettings.terminalClockCommand.Value, TerminalEvents.AddClockKeywords);
            //internal static Action AddCommandAction(string textFail, bool clearText, string keyWord, string nodeName, Func<string> commandFunc)
            AddKeywordIfEnabled(ConfigSettings.terminalLobby.Value, TerminalEvents.LobbyKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalListItems.Value, TerminalEvents.ListItemsKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalLootDetail.Value, TerminalEvents.DetailedLootKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalPrevious.Value, TerminalEvents.PreviousKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalMirror.Value, TerminalEvents.MirrorKeywords);
            AddKeywordIfEnabled(ConfigSettings.terminalBioScan.Value, TerminalEvents.BioScanKeywords, ConfigSettings.ModNetworking.Value);
            AddKeywordIfEnabled(ConfigSettings.terminalRefund.Value, TerminalEvents.RefundKeywords, ConfigSettings.ModNetworking.Value); //unable to sync between clients without netpatch
            AddKeywordIfEnabled(ConfigSettings.terminalVitals.Value, TerminalEvents.VitalsKeywords, ConfigSettings.ModNetworking.Value);
            AddKeywordIfEnabled(ConfigSettings.terminalRouteRandom.Value, TerminalEvents.RouteRandomKeywords, ConfigSettings.ModNetworking.Value);
        }

        private static void AddKeywordIfEnabled(bool isEnabled, Action keywordAction)
        {
            if (isEnabled)
            {
                keywordAction();
            }
        }

        private static void AddKeywordIfEnabled(bool isEnabled, Action keywordAction, bool checkNetwork)
        {
            if (checkNetwork)
            {
                if (isEnabled)
                {
                    keywordAction();
                }
            }

        }
    }

}
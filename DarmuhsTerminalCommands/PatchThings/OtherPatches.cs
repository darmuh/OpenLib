using BepInEx.Bootstrap;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.AlwaysOnStuff;

namespace TerminalStuff
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    public class StartRoundPatch
    {
        public static void Postfix()
        {
            Plugin.instance.splitViewCreated = false;
            SplitViewChecks.InitSplitViewObjects(); //addSplitViewObjects
            BoolStuff.ResetEnumBools(); // resets all enum bools
            TerminalClockStuff.showTime = false; // disable clock on game restart
            ViewCommands.ResetPluginInstanceBools(); //reset view command bools
        }
    }

    //StartGame
    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    public class LandingPatch
    {
        public static void Postfix()
        {
            if (!StartOfRound.Instance.inShipPhase)
            {
                if (!TerminalEvents.clockDisabledByCommand)
                    TerminalClockStuff.showTime = true;

                /* if (ConfigSettings.alwaysOnDynamic.Value)
                    Plugin.instance.Terminal.StartCoroutine(AlwaysOnDynamic(Plugin.instance.Terminal)); */
            }
            else
            {
                TerminalClockStuff.showTime = false;
            }
                

            //Cheat credits, only uncomment when testing and needing credits
            //NetHandler.Instance.SyncCreditsServerRpc(999999, Plugin.instance.Terminal.numberOfItemsInDropship);
        }

    }

    public class SpawnPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "SpawnPlayerAnimation")]
        public class PlayerSpawnPatch : MonoBehaviour
        {
            static void Postfix()
            {
                if (ConfigSettings.alwaysOnDynamic.Value)
                    Plugin.instance.Terminal.StartCoroutine(AlwaysOnDynamic(Plugin.instance.Terminal));
            }
        }
    }

    [HarmonyPatch(typeof(ShipTeleporter), "Awake")]
    public class TeleporterInit : ShipTeleporter
    {
        static void Postfix(ShipTeleporter __instance)
        {
            if (!ConfigSettings.terminalTP.Value && !ConfigSettings.terminalITP.Value)
                return;

            CheckTeleporterTypeAndAssign(__instance);
        }

        private static void CheckTeleporterTypeAndAssign(ShipTeleporter instance)
        {
            if (instance.isInverseTeleporter)
            {
                ITPexists(instance);
            }
            else
            {
                TPexists(instance);
            }
        }

        private static void TPexists(ShipTeleporter instance)
        {
            if (!ConfigSettings.terminalTP.Value)
                return;

            Plugin.NormalTP = instance;
            Plugin.MoreLogs("NormalTP instance detected and set.");
            TerminalEvents.AddTeleportKeywords();
        }

        private static void ITPexists(ShipTeleporter instance)
        {
            if (!ConfigSettings.terminalITP.Value)
                return;

            Plugin.InverseTP = instance;
            Plugin.MoreLogs("InverseTP instance detected and set.");
            TerminalEvents.AddInverseTeleportKeywords();
        }
    }

    [HarmonyPatch(typeof(ManualCameraRenderer), "updateMapTarget")]
    public class SwitchRadarPatch
    {

        public static void Postfix(int setRadarTargetIndex)
        {
            if (StartOfRound.Instance.mapScreen == null || StartOfRound.Instance.mapScreen.radarTargets == null || StartOfRound.Instance.mapScreen.radarTargets[setRadarTargetIndex] == null)
            {
                Plugin.ERROR("Postfix failed, StartOfRound.Instance.mapScreen has null variables");
                return;
            }

            if (Plugin.instance.TwoRadarMapsMod || Plugin.instance.isOnMirror)
                return;

            Plugin.instance.radarNonPlayer = StartOfRound.Instance.mapScreen.radarTargets[setRadarTargetIndex].isNonPlayer;

            if (!ViewCommands.IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                ViewCommands.targetInt = setRadarTargetIndex;
                Plugin.MoreLogs("Updating homebrew target");
                SwitchedRadarEvent();
            }
            else if (ViewCommands.IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                if (Plugin.instance.OpenBodyCamsMod && !OpenBodyCamsCompatibility.showingBodyCam)
                    Plugin.MoreLogs("OBC Terminal Body Cam is NOT active");
                else
                    ViewCommands.GetPlayerCamsFromExternalMod();
            }

            UpdateDisplayText();
        }

        internal static void UpdateDisplayText()
        {
            Terminal getTerm = Plugin.instance.Terminal;

            if (!Plugin.instance.radarNonPlayer && StartOfRound.Instance.mapScreen.targetedPlayer == null)
                return;
            if (!Plugin.instance.activeCam || !ViewCommands.AnyActiveMonitoring())
                return;
            if (Plugin.instance.isOnMirror)
                return;

            if (getTerm != null && getTerm.currentNode != null)
            {
                ViewCommands.DisplayTextUpdater(out string displayText);
                getTerm.currentNode.displayText = displayText;
            }
        }

        private static void SwitchedRadarEvent()
        {
            //Plugin.MoreLogs($"startround: {StartOfRound.Instance.mapScreen.targetTransformIndex}\n--------------\nviewcommands {ViewCommands.targetInt}\n-------------");
            if (ViewCommands.AnyActiveMonitoring() && !ViewCommands.externalcamsmod)
            {
                Plugin.MoreLogs($"targetNum = {ViewCommands.targetInt}");
                ViewCommands.UpdateCamsTarget(ViewCommands.targetInt);
                return;
            }

        }
    }

    public class LoadGrabbablesOnShip
    {
        public static List<GrabbableObject> ItemsOnShip = [];
        public static void LoadAllItems()
        {
            ItemsOnShip.Clear();
            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var grabbableObjects = ship.GetComponentsInChildren<GrabbableObject>();
            foreach (GrabbableObject item in grabbableObjects)
            {
                ItemsOnShip.Add(item);
                Plugin.MoreLogs($"{item.itemProperties.itemName} added to list");
            }

        }

    }

    [HarmonyPatch(typeof(GameNetworkManager), "Start")]
    public class GameStartPatch
    {
        internal static bool oneTimeOnly = false;
        public static void Postfix()
        {
            CompatibilityCheck();
            oneTimeOnly = false;
        }

        private static void CompatibilityCheck()
        {
            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                Plugin.MoreLogs("Advanced Company detected, setting Advanced Company Compatibility options");
                Plugin.instance.CompatibilityAC = true;
                //if (ConfigSettings.ModNetworking.Value)
                //AdvancedCompanyCompat.AdvancedCompanyStuff();
            }
            if (Chainloader.PluginInfos.ContainsKey("Rozebud.FovAdjust"))
            {
                Plugin.MoreLogs("Rozebud's FovAdjust detected!");
                Plugin.instance.FovAdjust = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("RickArg.lethalcompany.helmetcameras"))
            {
                Plugin.MoreLogs("Helmet Cameras by Rick Arg detected!");
                Plugin.instance.HelmetCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("SolosBodycams"))
            {
                Plugin.MoreLogs("SolosBodyCams by CapyCat (Solo) detected!");
                Plugin.instance.SolosBodyCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("Zaggy1024.OpenBodyCams"))
            {
                Plugin.MoreLogs("OpenBodyCams by Zaggy1024 detected!");
                Plugin.instance.OpenBodyCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("Zaggy1024.TwoRadarMaps"))
            {
                Plugin.MoreLogs("TwoRadarMaps by Zaggy1024 detected!");
                Plugin.instance.TwoRadarMapsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades")) //other mods that simply append to the help command
            {
                Plugin.MoreLogs("Lategame Upgrades by malco detected!");
                Plugin.instance.LateGameUpgrades = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("darmuh.suitsTerminal"))
            {
                Plugin.MoreLogs("suitsTerminal detected!");
                Plugin.instance.suitsTerminal = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
            {
                Plugin.MoreLogs("Terminal Formatter by mrov detected!");
                Plugin.instance.TerminalFormatter = true;
            }
        }
    }
}

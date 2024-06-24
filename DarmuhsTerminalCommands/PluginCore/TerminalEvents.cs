using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.GetFromConfig;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using static TerminalStuff.StringStuff;

namespace TerminalStuff
{

    public static class TerminalEvents
    {
        public static Dictionary<TerminalNode, Func<string>> darmuhsTerminalStuff = [];
        internal static string TotalValueFormat = "";
        internal static string VideoErrorMessage = "";
        public static bool clockDisabledByCommand = false;

        internal static bool quitTerminalEnum = false;

        internal static TerminalNode switchNode = CreateDummyNode("switchDummy", true, "this should not display, switch command");
        //internal static TerminalNode returnCams = CreateDummyNode("returnCamsDummy", true, "this should not display, returnCams node");

        internal static Func<string> GetCommandDisplayTextSupplier(TerminalNode query)
        {
            foreach (KeyValuePair<TerminalNode, Func<string>> pairValue in darmuhsTerminalStuff)
            {
                if (pairValue.Key == query)
                {
                    return pairValue.Value;
                }
            }
            return null; // No matching command found for the given query
        }

        internal static TerminalNode GetNodeFromList(string query, Dictionary<TerminalNode, string> nodeListing)
        {
            foreach (KeyValuePair<TerminalNode, string> pairValue in nodeListing)
            {
                if (pairValue.Value == query)
                {
                    return pairValue.Key;
                }
            }
            return null; // No matching command found for the given query
        }

        internal static void StoreCommands()
        {
            if (ConfigSettings.terminalBioScan.Value && ConfigSettings.terminalBioScanPatch.Value && ConfigSettings.ModNetworking.Value)
            {
                string patchDeny = $"You have opted out of purchasing the BioScanner 2.0 Upgrade Patch.\n\n";
                MakeStoreCommand($"bioscanpatch_node", "bioscanpatch", "BioScanner 2.0 Upgrade Patch (bioscanpatch)", false, true, false, $"bioscanpatch_confirm_node", $"bioscanpatch_deny_node", "bioscanpatch.do", patchDeny, ConfigSettings.bioScanUpgradeCost.Value, CostCommands.AskBioscanUpgrade, CostCommands.PerformBioscanUpgrade, darmuhsTerminalStuff);
            }

            if (ConfigSettings.terminalVitals.Value && ConfigSettings.terminalVitalsUpgrade.Value && ConfigSettings.ModNetworking.Value)
            {
                string patchDeny = $"You have opted out of purchasing the Vitals Scanner Upgrade.\n\n";
                MakeStoreCommand($"vitalspatch_node", "vitalspatch", "Vitals Scanner Upgrade (vitalspatch)", false, true, false, $"vitalspatch_confirm_node", $"vitalspatch_deny_node", "vitalspatch.do", patchDeny, ConfigSettings.vitalsUpgradeCost.Value, CostCommands.AskVitalsUpgrade, CostCommands.PerformVitalsUpgrade, darmuhsTerminalStuff);
            }


        }

        internal static void LobbyKeywords()
        {
            MakeCommand("Lobby Name", "lobby", "lobby command\n", false, true, MoreCommands.GetLobbyName, darmuhsTerminalStuff);
        }

        internal static void ListItemsKeywords()
        {
            foreach (string keyword in itemsKW)
            {
                MakeCommand("List Items on Ship", keyword, "List Items TermEvent\n", false, true, MoreCommands.GetItemsOnShip, darmuhsTerminalStuff);
            }
        }

        internal static void DetailedLootKeywords()
        {
            //AddKeywordIfEnabled(ConfigSettings.terminalLootDetail.Value, TerminalEvents.AddCommandAction("Detailed Loot TermEvent\n", true, scrapKW, "List Scrap on Ship", AllTheLootStuff.DetailedLootCommand));
            foreach (string keyword in scrapKW)
            {
                MakeCommand("List Scrap on Ship", keyword, "Detailed Loot TermEvent\n", false, true, AllTheLootStuff.DetailedLootCommand, darmuhsTerminalStuff);
            }
        }

        internal static void BioScanKeywords()
        {
            //AddKeywordIfEnabled(ConfigSettings.terminalBioScan.Value, TerminalEvents.AddCommandAction("bioscan terminal event\n", true, "bioscan", "BioScan", CostCommands.BioscanCommand), ConfigSettings.ModNetworking.Value);
            MakeCommand("BioScan", "bioscan", "bioscan terminal event\n", false, true, CostCommands.BioscanCommand, darmuhsTerminalStuff);
        }

        internal static void MirrorKeywords()
        {
            //AddKeywordIfEnabled(ConfigSettings.terminalMirror.Value, TerminalEvents.AddCommandAction("mirror terminal event\n", true, "mirror", "Mirror", ViewCommands.MirrorEvent));
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.mirrorKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("terminalStuff Mirror", keyword, "", false, true, ViewCommands.MirrorEvent, darmuhsTerminalStuff, 6, "mirror", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }

        }

        internal static void RefundKeywords()
        {
            //AddKeywordIfEnabled(ConfigSettings.terminalRefund.Value, TerminalEvents.AddCommandAction("refund terminal event\n", true, "refund", "Refund", CostCommands.GetRefund), ConfigSettings.ModNetworking.Value); //unable to sync between clients without netpatch
            MakeCommand("Refund", "refund", "refund terminal event\n", false, true, CostCommands.GetRefund, darmuhsTerminalStuff);

        }

        internal static void PreviousKeywords()
        {
            MakeCommand("Switch to Previous", "previous", "switch back\n", false, true, ViewCommands.HandlePreviousSwitchEvent, darmuhsTerminalStuff);

        }

        //view commands
        internal static void CamsKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.camsKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("ViewInsideShipCam 1", keyword, "", false, true, ViewCommands.TermCamsEvent, darmuhsTerminalStuff, 1, "cams", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }

        }

        internal static void MapKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.mapKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("ViewInsideShipCam 1", keyword, "", false, true, ViewCommands.TermMapEvent, darmuhsTerminalStuff, 5, "map", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }

        }
        internal static void AddMiniMapKeywords()
        {
            List<string> minimapKeywords = GetKeywordsPerConfigItem(ConfigSettings.minimapKeywords.Value);
            foreach (string keyword in minimapKeywords)
            {
                MakeCommand("ViewInsideShipCam 1", keyword, "", false, true, ViewCommands.MiniMapTermEvent, darmuhsTerminalStuff, 3, "minimap", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }
        }

        internal static void AddMiniCams()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.minicamsKeywords.Value);
            foreach (string keyword in getKeywords)
            {
                MakeCommand("ViewInsideShipCam 1", keyword, "", false, true, ViewCommands.MiniCamsTermEvent, darmuhsTerminalStuff, 4, "minicams", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }
        }

        internal static void AddOverlayView()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.overlayKeywords.Value);
            foreach (string keyword in getKeywords)
            {
                MakeCommand("ViewInsideShipCam 1", keyword, "", false, true, ViewCommands.OverlayTermEvent, darmuhsTerminalStuff, 2, "overlay", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }
        }

        internal static void VideoKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.videoKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("darmuh's videoPlayer", keyword, "lol terminalEvent\n", false, true, ViewCommands.LolVideoPlayerEvent, darmuhsTerminalStuff, 0, "lol", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            }

        }

        internal static void AddDoor()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.doorKeywords.Value);
            foreach (string keyword in getKeywords)
            {
                MakeCommand("Toggle Doors", keyword, "door terminalEvent\n", false, true, ShipControls.BasicDoorCommand, darmuhsTerminalStuff);
            }
        }

        internal static void AddLights()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.lightsKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Toggle Lights", keyword, "", false, true, ShipControls.BasicLightsCommand, darmuhsTerminalStuff);
            }
        }

        internal static void AddRandomSuit()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.randomSuitKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("RandomSuit", keyword, "randomsuit terminalEvent\n", false, true, RandomSuit, darmuhsTerminalStuff);
            }
        }

        internal static void AddAlwaysOnKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.alwaysOnKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Always-On Display", keyword, "alwayson terminalEvent\n", false, true, MoreCommands.AlwaysOnDisplay, darmuhsTerminalStuff);
            }

        }

        internal static void AddModListKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.modsKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("ModList", keyword, "", false, true, MoreCommands.ModListCommand, darmuhsTerminalStuff);
            }

        }

        internal static void AddTeleportKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.tpKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeDynamicCommand("Use Teleporter", keyword, "teleporter terminalEvent\n", true, false, ShipControls.RegularTeleporterCommand, nodesThatAcceptAnyString, darmuhsTerminalStuff);
            }

        }

        internal static void AddInverseTeleportKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.itpKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Use Inverse Teleporter", keyword, "inverseteleporter terminalEvent\n", false, true, ShipControls.InverseTeleporterCommand, darmuhsTerminalStuff);
            }

        }

        internal static void AddQuitKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.quitKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Quit Terminal", keyword, "", false, true, QuitTerminalCommand, darmuhsTerminalStuff);
            }

        }

        internal static void AddClockKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.clockKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Terminal Clock", keyword, "timetoggle event\n", false, true, ClockToggle, darmuhsTerminalStuff);
            }

        }

        internal static void ClearKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.clearKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Clear Terminal Screen", keyword, "", false, true, ClearText, darmuhsTerminalStuff);
            }
        }
        internal static void DangerKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.dangerKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Check Danger Level", keyword, "danger terminalEvent\n", false, true, MoreCommands.DangerCommand, darmuhsTerminalStuff);
            }

        }
        internal static void VitalsKeywords()
        {
            MakeCommand("Check Vitals", "vitals", "vitals terminalEvent\n", false, true, CostCommands.VitalsCommand, darmuhsTerminalStuff);
        }

        internal static void RouteRandomKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.randomRouteKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Route Random by darmuh", keyword, "", false, true, LevelCommands.RouteRandomCommand, darmuhsTerminalStuff);
            }
        }

        internal static void HealKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.healKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("HealFromTerminal", keyword, "", false, true, MoreCommands.HealCommand, darmuhsTerminalStuff);
            }
        }
        internal static void LootKeywords()
        {
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.lootKeywords.Value);

            foreach (string keyword in getKeywords)
            {
                MakeCommand("Check Loot Value", keyword, "loot terminalEvent\n", false, true, AllTheLootStuff.GetLootSimple, darmuhsTerminalStuff);
            }
        }

        private static string RandomSuit()
        {
            SuitCommands.GetRandomSuit(out string suitString);
            return suitString;
        }

        private static string QuitTerminalCommand()
        {
            string text = $"{ConfigSettings.quitString.Value}\n";

            Plugin.instance.Terminal.StartCoroutine(TerminalQuitter(Plugin.instance.Terminal));
            return text;
        }

        internal static IEnumerator TerminalQuitter(Terminal terminal)
        {
            if (quitTerminalEnum)
                yield break;

            quitTerminalEnum = true;

            yield return new WaitForSeconds(0.5f);
            terminal.QuitTerminal();

            quitTerminalEnum = false;
        }

        private static string ClockToggle()
        {
            if (!TerminalClockStuff.showTime)
            {
                TerminalClockStuff.showTime = true;
                clockDisabledByCommand = false;
                string displayText = "Terminal Clock [ENABLED].\r\n";
                return displayText;
            }
            else
            {
                TerminalClockStuff.showTime = false;
                clockDisabledByCommand = true;
                string displayText = "Terminal Clock [DISABLED].\r\n";
                return displayText;
            }
        }

        private static string ClearText()
        {
            string displayText = "\n";
            Plugin.MoreLogs("display text cleared for real this time!!!");
            return displayText;
        }
    }
}


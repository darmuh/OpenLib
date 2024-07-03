using System.Collections.Generic;
using System.Text;
using TerminalStuff.NoMoreAPI;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.GetFromConfig;
using static TerminalStuff.StringStuff;
using static TerminalStuff.TerminalEvents;

namespace TerminalStuff
{

    internal class MenuBuild
    {
        internal static bool isNextEnabled = false;
        internal static int nextCount = 1; //start at same value as pages
        internal static string currentCategory = string.Empty;
        internal static List<string> comfortEnabledCommands = [];
        internal static List<string> extrasEnabledCommands = [];
        internal static List<string> controlsEnabledCommands = [];
        internal static List<string> funEnabledCommands = [];
        internal static Dictionary<int, TerminalNode> allMenuNodes = [];

        internal static void CheckAndResetMenuVariables()
        {
            if (isNextEnabled)
            {
                isNextEnabled = false;
                nextCount = 1;
                currentCategory = string.Empty;
                Plugin.MoreLogs("Reset Menu Variables");
            }
        }

        internal static void CreateMoreCommand()
        {
            string displayText = AssembleMoreMenuDisplayText();
            TerminalHook.MakeCommand("More Command by Darmuh", "more", displayText, false, true);
        }
        internal static void CreateNextCommand()
        {
            TerminalHook.MakeCommand("Next Page of Menu Item by Darmuh", "next", "next command here\r\n", false, true, NextInList, darmuhsTerminalStuff, 4, allMenuNodes);
        }

        internal static void CreateComfortCommand()
        {
            TerminalHook.MakeCommand("Comfort Commands List by Darmuh", "comfort", "comfort commands here\r\n", false, true, ComfortList, darmuhsTerminalStuff, 0, allMenuNodes);
        }

        internal static void CreateControlsCommand()
        {
            TerminalHook.MakeCommand("Controls Commands List by Darmuh", "controls", "controls commands here\r\n", false, true, ControlsList, darmuhsTerminalStuff, 1, allMenuNodes);
        }

        internal static void CreateFunListCommand()
        {
            TerminalHook.MakeCommand("Fun Commands List by Darmuh", "fun", "fun commands here\r\n", false, true, FunList, darmuhsTerminalStuff, 2, allMenuNodes);
        }

        internal static void CreateExtrasCommand()
        {
            TerminalHook.MakeCommand("Extras Commands List by Darmuh", "extras", "extras commands here\r\n", false, true, ExtrasList, darmuhsTerminalStuff, 3, allMenuNodes);
        }

        internal static string NextInList()
        {
            if (!isNextEnabled)
            {
                string displayText = "Not currently in darmuh's menu items...\r\n\r\n";
                return displayText;
            }
            else
            {
                nextCount++;
                List<string> currentList = GetListFromCat(currentCategory);
                string displayText = GetNextPage(currentList, currentCategory, 4, nextCount);
                return displayText;
            }
        }

        internal static string ComfortList()
        {
            isNextEnabled = true;
            nextCount = 1;
            currentCategory = "Comfort";
            string displayText = GetNextPage(comfortEnabledCommands, currentCategory, 4, 1);
            return displayText;
        }

        internal static string ControlsList()
        {
            isNextEnabled = true;
            nextCount = 1;
            currentCategory = "Controls";
            string displayText = GetNextPage(controlsEnabledCommands, currentCategory, 4, 1);
            return displayText;
        }

        internal static string FunList()
        {
            isNextEnabled = true;
            nextCount = 1;
            currentCategory = "Fun";
            string displayText = GetNextPage(funEnabledCommands, currentCategory, 4, 1);
            return displayText;
        }
        internal static string ExtrasList()
        {
            isNextEnabled = true;
            nextCount = 1;
            currentCategory = "Extras";
            string displayText = GetNextPage(extrasEnabledCommands, currentCategory, 4, 1);
            return displayText;
        }

        private static string AssembleMoreMenuDisplayText()
        {
            StringBuilder assembler = new();
            assembler.Append($"Welcome to darmuh's Terminal Upgrade!\r\n\tSee below Categories for new stuff :)\r\n\r\n");
            if (comfortEnabledCommands.Count > 0)
                assembler.Append("[COMFORT]\r\nImproves the terminal user experience.\r\n\r\n");
            if (extrasEnabledCommands.Count > 0)
                assembler.Append("[EXTRAS]\r\nAdds extra functionality to the ship terminal.\r\n\r\n");
            if (controlsEnabledCommands.Count > 0)
                assembler.Append("[CONTROLS]\r\nGives terminal more control of the ship's systems.\r\n\r\n");
            if (funEnabledCommands.Count > 0)
                assembler.Append("[FUN]ctionality\r\nType \"fun\" for a list of these FUNctional commands.\r\n\r\n");

            return assembler.ToString();
        }

        internal static void CreateMenus()
        {
            allMenuNodes.Clear();
            comfortEnabledCommands.Clear();
            extrasEnabledCommands.Clear();
            controlsEnabledCommands.Clear();
            funEnabledCommands.Clear();

            AddComfortCommands();
            AddExtrasCommands();
            AddControlsCommands();
            AddFunCommands();
            CreateMoreCommand();
            CreateNextCommand();

            if (extrasEnabledCommands.Count > 0)
                CreateExtrasCommand();
            else
                Plugin.MoreLogs($"extrasEnabledCommands count: {extrasEnabledCommands.Count}");

            if (controlsEnabledCommands.Count > 0)
                CreateControlsCommand();
            else
                Plugin.MoreLogs($"controlsEnabledCommands count: {controlsEnabledCommands.Count}");

            if (funEnabledCommands.Count > 0)
                CreateFunListCommand();
            else
                Plugin.MoreLogs($"funEnabledCommands count: {funEnabledCommands.Count}");

            if (comfortEnabledCommands.Count > 0)
                CreateComfortCommand();
            else
                Plugin.MoreLogs($"comfortEnabledCommands count: {comfortEnabledCommands.Count}");


        }



        // Additional methods to add category-specific commands
        static void AddComfortCommands()
        {

            if (ConfigSettings.terminalClear.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(clearKW)}\r\nClear the terminal of any existing text.\r\n");

            if (ConfigSettings.terminalAlwaysOn.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(alwaysOnKW)}\r\nToggle the Always-On Terminal Screen mode.\r\n");
            if (ConfigSettings.terminalFov.Value)
                comfortEnabledCommands.Add($"> fov <value>\r\nUpdate your in-game Field of View.\r\n");
            if (ConfigSettings.terminalHeal.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(healKW)}\r\nHeal yourself from any damage.\r\n");
            if (ConfigSettings.terminalKick.Value && (GameNetworkManager.Instance != null && GameNetworkManager.Instance.isHostingGame))
                comfortEnabledCommands.Add($"> kick\r\nKick another employee from your group.\r\n");
            if (ConfigSettings.terminalLobby.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(lobbyKW)}\r\nDisplay current lobby name.\r\n");
            if (ConfigSettings.terminalMods.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(modsKW)}\r\nDisplay your currently loaded Mods.\r\n");
            if (ConfigSettings.terminalQuit.Value)
                comfortEnabledCommands.Add($"> {GetKeywordsForMenuItem(quitKW)}\r\nLeave the terminal.\r\n");

            comfortEnabledCommands.Add("> home\r\nReturn to start screen.\r\n");

            // Add more comfort commands...
        }

        static void AddExtrasCommands()
        {
            if (ConfigSettings.terminalLink.Value)
                extrasEnabledCommands.Add($"> {ConfigSettings.linkKeyword.Value}\r\n {ConfigSettings.customLinkHint.Value}\r\n");

            if (ConfigSettings.terminalLink2.Value)
                extrasEnabledCommands.Add($"> {ConfigSettings.link2Keyword.Value}\r\n {ConfigSettings.customLink2Hint.Value}\r\n");

            if (ConfigSettings.terminalCams.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(camsKW)}\r\nToggle displaying cameras in terminal.\r\n");

            if (ConfigSettings.terminalMap.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(mapKW)}\r\nShortcut to toggle radar map on terminal.\r\n");

            if (ConfigSettings.terminalMinimap.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(mmKW)}\r\nToggle cameras and radar map via MiniMap Mode.\r\n");

            if (ConfigSettings.terminalOverlay.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(ovKW)}\r\nToggle cameras and radar map via Overlay Mode.\r\n");

            if (ConfigSettings.terminalMirror.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(mirrorKW)}\r\nToggle a camera on screen to see yourself.\r\n");

            if (ConfigSettings.terminalLoot.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(lootKW)}\r\nDisplay total value of all loot on-board.\r\n");

            if (ConfigSettings.terminalLootDetail.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(scrapKW)}\r\nDisplay a detailed list of all loot on-board.\r\n");

            if (ConfigSettings.terminalListItems.Value)
                extrasEnabledCommands.Add($"> {GetKeywordsForMenuItem(itemsKW)}\r\nDisplay a detailed list of all non-scrap items on-board that are not being held.\r\n");

            if (ConfigSettings.terminalVitals.Value && ConfigSettings.ModNetworking.Value)
                extrasEnabledCommands.Add($"> vitals\r\nDisplay vitals of employee being tracked on radar.\r\n");

            if (ConfigSettings.terminalVitalsUpgrade.Value && ConfigSettings.ModNetworking.Value)
                extrasEnabledCommands.Add($"> vitalspatch\r\nPurchase upgrade to Vitals Software Patch 2.0\r\n");

            if (ConfigSettings.terminalBioScan.Value && ConfigSettings.ModNetworking.Value)
                extrasEnabledCommands.Add($"> bioscan\r\n Use Ship BioScanner to search for non-employee lifeforms.\r\n");

            if (ConfigSettings.terminalBioScanPatch.Value && ConfigSettings.ModNetworking.Value)
                extrasEnabledCommands.Add($"> bioscanpatch\r\n Purchase upgrade to BioScanner Software Patch 2.0\r\n");

            if (ConfigSettings.terminalRefund.Value && ConfigSettings.ModNetworking.Value)
                extrasEnabledCommands.Add($"> refund \r\nCancel any purchase that has yet to be delivered from the dropship.\r\n");

            if (ConfigSettings.terminalPrevious.Value)
                extrasEnabledCommands.Add($"> previous \r\nUse this command to switch to previous radar target during any cams view.\r\n");


            // Add more extras commands...
        }

        static void AddControlsCommands()
        {
            if (ConfigSettings.terminalDanger.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(dangerKW)} \r\nDisplays the danger level once the ship has landed.\r\n");
            if (ConfigSettings.terminalLever.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(leverKW)}\r\nRemotely pull the ship lever.\r\n");
            if (ConfigSettings.terminalDoor.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(doorKW)}\r\nRemotely open/close the ship doors.\r\n");
            if (ConfigSettings.terminalLights.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(lightsKW)}\r\nRemotely toggle the ship lights.\r\n");
            if (ConfigSettings.terminalTP.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(tpKW)}\r\nRemotely push the Teleporter button.\r\n");
            if (ConfigSettings.terminalITP.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(itpKW)}\r\nRemotely push the Inverse Teleporter button.\r\n");
            if (ConfigSettings.terminalClockCommand.Value)
                controlsEnabledCommands.Add($"> {GetKeywordsForMenuItem(clockKW)}\r\nToggle Terminal Clock display on/off.\r\n");
            if (ConfigSettings.terminalRestart.Value)
                controlsEnabledCommands.Add($"> restart\r\nRestart the lobby and get a new ship. (skips firing sequence)\r\n");

            if (ConfigSettings.terminalShortcuts.Value)
            {
                controlsEnabledCommands.Add($"> bind\r\nBind a command to a key of your choosing! (saved to config)\r\n");
                controlsEnabledCommands.Add($"> unbind\r\nUnbind any shortcut key binding. (saved to config)\r\n");
            }

            // Add more controls commands...
        }

        static void AddFunCommands()
        {
            if (ConfigSettings.terminalFcolor.Value && ConfigSettings.ModNetworking.Value)
            {
                funEnabledCommands.Add($"> {fColor} <color>\r\nUpgrade your flashlight with a new color.\r\n");
                funEnabledCommands.Add($"> {fColor} list\r\nView available colors for flashlight.\r\n");
            }

            if (ConfigSettings.terminalScolor.Value && ConfigSettings.ModNetworking.Value)
            {
                funEnabledCommands.Add($"> {sColor} <all,front,middle,back> <color>\r\nChange the color of the ship's lights.\r\n");
                funEnabledCommands.Add($"> {sColor} list\r\nView available colors to change ship lights.\r\n");
            }

            if (ConfigSettings.terminalRandomSuit.Value)
                funEnabledCommands.Add($"> {GetKeywordsForMenuItem(suitKW)} \r\nPut on a random suit.\r\n");

            if (ConfigSettings.terminalGamble.Value && ConfigSettings.ModNetworking.Value)
                funEnabledCommands.Add($"> {Gamble} <percentage>\r\nGamble a percentage of your credits.\r\n");
            if (ConfigSettings.terminalVideo.Value)
                funEnabledCommands.Add($"> {GetKeywordsForMenuItem(lolKW)}\r\nPlay a silly video on the terminal.\r\n");
            if (ConfigSettings.terminalRouteRandom.Value)
                funEnabledCommands.Add($"> {GetKeywordsForMenuItem(rrKW)}\r\nRoute to a random moon for a flat rate of {ConfigSettings.routeRandomCost.Value} credits.\r\n");

            // Add more fun commands...
        }
    }
}

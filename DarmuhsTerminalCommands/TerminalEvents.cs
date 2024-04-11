using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.TerminalApi;

namespace TerminalStuff
{

    public static class TerminalEvents
    {
        public static List<CommandInfo> darmuhsTerminalStuff = new List<CommandInfo>();
        public static string TotalValueFormat = "";
        public static string VideoErrorMessage = "";
        public static bool clockDisabledByCommand = false;

        public delegate void CommandDelegate(out string displayText);

        public static Func<string> GetCommandDisplayTextSupplier(TerminalNode query)
        {
            foreach (var command in darmuhsTerminalStuff)
            {
                if (command.TriggerNode == query)
                {
                    return command.DisplayTextSupplier;
                }
            }
            return null; // No matching command found for the given query
        }

        internal static void AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
        {
            TerminalNode node = CreateTerminalNode(textFail, clearText);
            TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);

            CommandInfo commandInfo = new CommandInfo()
            {
                TriggerNode = node,
                DisplayTextSupplier = () =>
                {
                    methodName(out string displayText);
                    return displayText;
                },
                Category = category,
                Description = description
            };
            darmuhsTerminalStuff.Add(commandInfo);

            AddTerminalKeyword(termWord, commandInfo);

            node.name = nodeName;
            nodeGroup.Add(node);
        }

        internal static void AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string keyWord2, string category, string description, CommandDelegate methodName)
        {
            TerminalNode node = CreateTerminalNode(textFail, clearText);
            node.name = nodeName;
            TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);
            TerminalKeyword termWord2 = CreateTerminalKeyword(keyWord2, isVerb, node);

            CommandInfo commandInfo = new CommandInfo()
            {
                TriggerNode = node,
                DisplayTextSupplier = () =>
                {
                    methodName(out string displayText);
                    return displayText;
                },
                Category = category,
                Description = description
            };
            darmuhsTerminalStuff.Add(commandInfo);
            AddTerminalKeyword(termWord, commandInfo);
            AddTerminalKeyword(termWord2, commandInfo);

            nodeGroup.Add(node);
        }

        public static Action AddCommandAction(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
        {
            return () =>
            {
                TerminalNode node = CreateTerminalNode(textFail, clearText);
                TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);
                CommandInfo commandInfo = new CommandInfo()
                {
                    TriggerNode = node,
                    DisplayTextSupplier = () =>
                    {
                        methodName(out string displayText);
                        return displayText;
                    },
                    Category = category,
                    Description = description
                };
                darmuhsTerminalStuff.Add(commandInfo);

                AddTerminalKeyword(termWord, commandInfo);
                node.name = nodeName;
                nodeGroup.Add(node);
            };
        }

        public static Action AddCommandAction(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string keyWord2, string category, string description, CommandDelegate methodName)
        {
            return () =>
            {
                TerminalNode node = CreateTerminalNode(textFail, clearText);
                node.name = nodeName;
                TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);
                TerminalKeyword termWord2 = CreateTerminalKeyword(keyWord2, isVerb, node);
                CommandInfo commandInfo = new CommandInfo()
                {
                    TriggerNode = node,
                    DisplayTextSupplier = () =>
                    {
                        methodName(out string displayText);
                        return displayText;
                    },
                    Category = category,
                    Description = description
                };
                darmuhsTerminalStuff.Add(commandInfo);
                AddTerminalKeyword(termWord, commandInfo);
                AddTerminalKeyword(termWord2, commandInfo);
                nodeGroup.Add(node);
            };
        }

        //AddCommand(string textFail, bool clearText, string displayText, string keyWord, bool isVerb, string nodeName, string category, string description, Delegate methodName)
        public static void AddMiniMap()
        {
            AddCommand("minimap command\n", true, ViewCommands.termViewNodes, ConfigSettings.minimapKeyword.Value, false, "ViewInsideShipCam 1", "", "", ViewCommands.MiniMapTermEvent);
        }

        public static void AddMiniCams()
        {
            AddCommand("minicams command\n", true, ViewCommands.termViewNodes, ConfigSettings.minicamsKeyword.Value, false, "ViewInsideShipCam 1", "", "", ViewCommands.MiniCamsTermEvent);
        }

        public static void AddOverlayView()
        {
            AddCommand("overlay command\n", true, ViewCommands.termViewNodes, ConfigSettings.overlayKeyword.Value, false, "ViewInsideShipCam 1", "", "", ViewCommands.OverlayTermEvent);
        }
        public static void AddDoor()
        {
            AddCommand("door terminalEvent\n", true, ShipControls.shipControlNodes, ConfigSettings.doorKeyword.Value, false, "Toggle Doors", "", "", ShipControls.BasicDoorCommand);
        }

        public static void AddLights()
        {
            AddCommand("lights terminalEvent\n", true, ShipControls.shipControlNodes, ConfigSettings.lightsKeyword.Value, false, "Toggle Lights", "controls", "Toggle Shipboard Lights On/Off", ShipControls.BasicLightsCommand);
        }

        public static void AddTest()
        {
            TerminalNode test = CreateTerminalNode("test\n", true, "getscraplist");
            TerminalKeyword testKeyword = CreateTerminalKeyword("test", true, test);
            AddTerminalKeyword(testKeyword);
            Plugin.Log.LogInfo("This should only be enabled for dev testing");
        }

        public static void AddRandomSuit()
        {
            AddCommand("randomsuit terminalEvent\n", true, MoreCommands.infoOnlyNodes, ConfigSettings.randomSuitKeyword.Value, false, "RandomSuit", "", "", RandomSuit);
        }

        public static void AddAlwaysOnKeywords()
        {
            AddCommand("alwayson terminalEvent\n", true, MoreCommands.infoOnlyNodes, ConfigSettings.alwaysOnKeyword.Value, false, "Always-On Display", "", "", MoreCommands.AlwaysOnDisplay);
        }

        public static void AddModListKeywords()
        {
            AddCommand("modlist terminalEvent\n", true, MoreCommands.otherActionNodes, ConfigSettings.modsKeyword2.Value, false, "ModList", "mods", "", "", MoreCommands.ModListCommand);
        }

        public static void AddTeleportKeywords()
        {
            AddCommand("teleporter terminalEvent\n", true, ShipControls.shipControlNodes, ConfigSettings.tpKeyword2.Value, false, "Use Teleporter", ConfigSettings.tpKeyword.Value, "", "", ShipControls.RegularTeleporterCommand);
        }

        public static void AddInverseTeleportKeywords()
        {
            AddCommand("inverseteleporter terminalEvent\n", true, ShipControls.shipControlNodes, ConfigSettings.itpKeyword2.Value, false, "Use Inverse Teleporter", ConfigSettings.itpKeyword.Value, "", "", ShipControls.InverseTeleporterCommand);
        }

        public static void AddQuitKeywords()
        {
            AddCommand("leaving\n", true, MoreCommands.infoOnlyNodes, ConfigSettings.quitKeyword2.Value, false, "Quit Terminal", "quit", "", "", QuitTerminalCommand);
        }

        public static void AddClockKeywords()
        {
            AddCommand("timetoggle event\n", false, MoreCommands.infoOnlyNodes, ConfigSettings.clockKeyword2.Value, false, "Terminal Clock", "clock", "", "", ClockToggle);
        }

        public static void VideoKeywords()
        {
            AddCommand("lol terminalEvent\n", false, ViewCommands.termViewNodes, ConfigSettings.lolKeyword.Value, false, "darmuh's videoPlayer", "", "", ViewCommands.LolVideoPlayerEvent);
        }
        public static void ClearKeywords()
        {
            AddCommand("\r\n", true, MoreCommands.otherActionNodes, ConfigSettings.clearKeyword2.Value, false, "Clear Terminal Screen", "clear", "", "", ClearText);
        }
        public static void dangerKeywords()
        {
            AddCommand("danger terminalEvent\n", true, MoreCommands.otherActionNodes, ConfigSettings.dangerKeyword.Value, false, "Check Danger Level", "", "", MoreCommands.DangerCommand);
        }
        public static void vitalsKeywords()
        {
            AddCommand("vitals terminalEvent\n", true, MoreCommands.infoOnlyNodes, "vitals", false, "Check Vitals", "", "", CostCommands.VitalsCommand);
        }
        public static void healKeywords()
        {
            AddCommand("heal terminalEvent\n", true, MoreCommands.infoOnlyNodes, ConfigSettings.healKeyword2.Value, false, "HealFromTerminal", "heal", "", "", MoreCommands.HealCommand);
        }
        public static void lootKeywords()
        {
            AddCommand("loot terminalEvent\n", true, MoreCommands.otherActionNodes, ConfigSettings.lootKeyword2.Value, false, "Check Loot Value", "loot", "", "", AllTheLootStuff.GetLootSimple);
        }
        public static void camsKeywords()
        {
            AddCommand("cams terminalEvent\n", true, ViewCommands.termViewNodes, ConfigSettings.camsKeyword2.Value, false, "ViewInsideShipCam 1", "cams", "", "", ViewCommands.TermCamsEvent);
        }
        public static void mapKeywords()
        {
            AddCommand("map terminalEvent\n", true, ViewCommands.termViewNodes, ConfigSettings.mapKeyword2.Value, false, "ViewInsideShipCam 1", "map", "", "", ViewCommands.TermMapEvent);
        }

        private static void RandomSuit(out string displayText)
        {
            SuitCommands.GetRandomSuit(out string suitString);
            displayText = suitString;
        }

        private static void QuitTerminalCommand(out string displayText)
        {
            string text = $"{ConfigSettings.quitString.Value}\n";
            displayText = text;

            
            Plugin.Terminal.StartCoroutine(TerminalQuitter(Plugin.Terminal));
        }

        internal static IEnumerator TerminalQuitter(Terminal terminal)
        {
            yield return new WaitForSeconds(0.5f);
            terminal.QuitTerminal();
        }

        private static void ClockToggle(out string displayText)
        {
            

            if (!TerminalClockStuff.showTime)
            {
                TerminalClockStuff.showTime = true;
                clockDisabledByCommand = false;
                displayText = "Terminal Clock [ENABLED].\r\n";
            }
            else
            {
                TerminalClockStuff.showTime = false;
                clockDisabledByCommand = true;
                displayText = "Terminal Clock [DISABLED].\r\n";
            }
        }

        private static void ClearText(out string displayText)
        {
            displayText = "\n";
            Plugin.MoreLogs("display text cleared for real this time!!!");
        }
    }
}


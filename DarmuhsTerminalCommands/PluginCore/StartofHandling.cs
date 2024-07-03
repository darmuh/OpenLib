using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.NoMoreAPI.CommandStuff;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using static TerminalStuff.TerminalEvents;

namespace TerminalStuff
{
    internal class StartofHandling
    {
        internal static Coroutine delayedUpdater;
        internal static TerminalNode dummyNode = CreateDummyNode("handling_node", true, "");

        internal static bool textUpdater = false;

        internal static TerminalNode HandleParsed(Terminal terminal, TerminalNode currentNode, string[] words, out TerminalNode resultNode)
        {
            string firstWord = words[0].ToLower();
            HandleAnyNode(terminal, currentNode, words, firstWord, out resultNode);
            if (GetNewDisplayText(ref resultNode))
                Plugin.MoreLogs("command found in base funcstring listing...");
            return resultNode;
        }

        internal static int FindViewInt(TerminalNode givenNode)
        {
            foreach (KeyValuePair<TerminalNode, int> pairValue in ViewCommands.termViewNodes)
            {
                if (pairValue.Key == givenNode)
                {
                    int nodeNum = pairValue.Value;
                    return nodeNum;
                }
            }

            return -1;
        }

        internal static int FindViewIntByString()
        {
            string currentMode = GetViewMode();

            if (currentMode == "none")
                return -1;
            else
            {
                foreach (KeyValuePair<int, string> pairValue in ViewCommands.termViewNodeNums)
                {
                    if (pairValue.Value == currentMode)
                    {
                        int nodeNum = pairValue.Key;
                        return nodeNum;
                    }
                }

                return -1;
            }
        }

        private static string GetViewMode()
        {
            string mode;

            if (Plugin.instance.isOnCamera)
            {
                mode = "cams";
                Plugin.MoreLogs("cams mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMap)
            {
                mode = "map";
                Plugin.MoreLogs("map mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnOverlay)
            {
                mode = "overlay";
                Plugin.MoreLogs("overlay mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMiniMap)
            {
                mode = "minimap";
                Plugin.MoreLogs("minimap mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMiniCams)
            {
                mode = "minicams";
                Plugin.MoreLogs("minicams mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMirror)
            {
                mode = "mirror";
                Plugin.MoreLogs("Mirror mode detected");
                return mode;
            }
            else
            {
                Plugin.Log.LogError("Error with mode return, setting to default value");
                mode = "none";
                return mode;
            }
        }

        internal static TerminalNode FindViewNode(int givenInt)
        {
            if (givenInt < 0 || givenInt >= ViewCommands.termViewNodes.Count)
                return null;
            foreach (KeyValuePair<TerminalNode, int> pairValue in ViewCommands.termViewNodes)
            {
                if (pairValue.Value == givenInt)
                {
                    TerminalNode foundNode = pairValue.Key;
                    return foundNode;
                }
            }
            return null;
        }

        internal static void CheckNetNode(TerminalNode resultNode)
        {
            if (!ConfigSettings.networkedNodes.Value || !ConfigSettings.ModNetworking.Value)
                return;

            Plugin.MoreLogs("Networked nodes enabled, sending result to server.");
            if (resultNode != null)
            {
                if (ViewCommands.termViewNodes.ContainsKey(resultNode))
                {
                    int nodeNum = FindViewInt(resultNode);
                    NetHandler.NetNodeReset(true);
                    NetHandler.Instance.NodeLoadServerRpc(Plugin.instance.Terminal.topRightText.text, resultNode.name, resultNode.displayText, nodeNum);
                    Plugin.MoreLogs($"Valid node detected, nNS true & nodeNum: {nodeNum}");
                    return;
                }
                else
                {
                    NetHandler.NetNodeReset(true);
                    Plugin.MoreLogs("Valid node detected, nNS true");
                    NetHandler.Instance.NodeLoadServerRpc(Plugin.instance.Terminal.topRightText.text, resultNode.name, resultNode.displayText);
                    return;
                }
            }
            else
            {
                Plugin.MoreLogs("Invalid node for sync");
                return;
            }

        }

        internal static TerminalNode HandleAnyNode(Terminal terminal, TerminalNode currentNode, string[] words, string firstWord, out TerminalNode resultNode)
        {
            if (firstWord == "switch")
            {
                if (words.Length == 1)
                {
                    Plugin.MoreLogs("switch command detected");
                    resultNode = switchNode;

                    if (Plugin.instance.TwoRadarMapsMod)
                        TwoRadarMapsCompatibility.UpdateTerminalRadarTarget(terminal);
                    else
                        StartOfRound.Instance.mapScreen.SwitchRadarTargetForward(callRPC: true);

                    ViewCommands.UpdateCamsTarget(StartOfRound.Instance.mapScreen.targetTransformIndex);
                    ViewCommands.DisplayTextUpdater(out string displayText);

                    resultNode.displayText = displayText;
                    return resultNode;
                }
                else
                {
                    Plugin.MoreLogs("switch to specific player command detected");
                    resultNode = terminal.terminalNodes.specialNodes[20];

                    if (Plugin.instance.TwoRadarMapsMod)
                    {
                        int playernum = TwoRadarMapsCompatibility.CheckForPlayerNameCommand(firstWord, words[1].ToLower());
                        if (playernum != -1)
                        {
                            TwoRadarMapsCompatibility.UpdateTerminalRadarTarget(terminal, playernum);
                            ViewCommands.InitializeTextures();
                            ViewCommands.DisplayTextUpdater(out string displayText);
                            resultNode.displayText = displayText;
                            return resultNode;
                        }
                        Plugin.MoreLogs("PlayerName returned invalid number");
                        resultNode = terminal.terminalNodes.specialNodes[12];
                        return resultNode;
                    }
                    else
                    {
                        int playernum = terminal.CheckForPlayerNameCommand(firstWord, words[1].ToLower());
                        if (playernum != -1)
                        {
                            StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(playernum);
                            ViewCommands.UpdateCamsTarget(playernum);
                            ViewCommands.InitializeTextures();
                            ViewCommands.DisplayTextUpdater(out string displayText);
                            resultNode.displayText = displayText;
                            return resultNode;
                        }

                        Plugin.MoreLogs("PlayerName returned invalid number");
                        resultNode = terminal.terminalNodes.specialNodes[12];
                        return resultNode;
                    }
                }
            }
            else if (nodesThatAcceptNum.ContainsValue(firstWord))
            {
                resultNode = GetNodeFromList(firstWord, nodesThatAcceptNum);
                return resultNode;
            }
            else if (nodesThatAcceptAnyString.ContainsValue(firstWord))
            {
                resultNode = GetNodeFromList(firstWord, nodesThatAcceptAnyString);
                return resultNode;
            }
            else
            {
                resultNode = currentNode;
                return resultNode;
            }
        }

        internal static void DelayedUpdateText(Terminal terminal)
        {
            if (delayedUpdater != null)
            {
                terminal.StopCoroutine(delayedUpdater);
            }

            delayedUpdater = terminal.StartCoroutine(DelayedUpdateTextRoutine(terminal));
        }

        internal static IEnumerator DelayedUpdateTextRoutine(Terminal terminal)
        {
            if (textUpdater)
                yield break;

            textUpdater = true;

            yield return new WaitForSeconds(0.045f);
            ViewCommands.DisplayTextUpdater(out string displayText);
            ViewCommands.InitializeTextures();
            switchNode.displayText = displayText;
            terminal.LoadNewNode(switchNode);

            textUpdater = false;

        }

        internal static void FirstCheck(TerminalNode initialResult)
        {
            if (initialResult == null)
                return;

            MoreCamStuff.VideoPersist(initialResult.name);

            MoreCamStuff.CamPersistance(initialResult.name);

            if (initialResult != null && !MenuBuild.allMenuNodes.ContainsValue(initialResult))
            {
                MenuBuild.CheckAndResetMenuVariables();
            }

            return;
        }

    }
}

using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Video;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using static TerminalStuff.NoMoreAPI.CommandStuff;
using static TerminalStuff.StringStuff;
using static TerminalStuff.TerminalEvents;
using static TerminalStuff.AlwaysOnStuff;
using GameNetcodeStuff;
using UnityEngine.InputSystem;


namespace TerminalStuff
{
    public class AllMyTerminalPatches : MonoBehaviour
    {
        public static AllMyTerminalPatches Instance;

        [HarmonyPatch(typeof(Terminal), "Awake")]
        public class AwakeTermPatch : Terminal
        {
            static void Postfix(Terminal __instance)
            {
                Plugin.instance.Terminal = __instance;
                Plugin.MoreLogs($"Setting Plugin.instance.Terminal");
                Plugin.AddKeywords();
                TerminalStartPatch.firstload = false;
                Plugin.Allnodes = GetAllNodes();
            }
        }

        //Terminal disabled, disabling ESC key listener OnDisable
        [HarmonyPatch(typeof(Terminal), "OnDisable")]
        public class DisableTermPatch : Terminal
        {
            static void Postfix()
            {
                if (Plugin.instance.OpenBodyCamsMod)
                    OpenBodyCamsCompatibility.ResidualCamsCheck();

                Plugin.ClearLists();
            }
        }

        [HarmonyPatch(typeof(Terminal), "LoadNewNode")]
        public class LoadNewNodePatch : Terminal
        {
            static void Postfix()
            {
                Plugin.Spam($"LoadNewNode patch, nNS: {NetHandler.netNodeSet}");
                if (Plugin.instance.Terminal.currentNode != null)
                    Plugin.Spam($"node: {Plugin.instance.Terminal.currentNode.name}");
            }
        }

        [HarmonyPatch(typeof(Terminal), "QuitTerminal")]
        public class QuitPatch : Terminal
        {
            internal static bool videoQuitEnum = false;
            static void Postfix(Terminal __instance)
            {
                TerminalStartPatch.isTermInUse = __instance.terminalInUse;

                if (StartOfRound.Instance.localPlayerController != null)
                    ShouldLockPlayerCamera(true, StartOfRound.Instance.localPlayerController);

                //Plugin.Log.LogInfo($"terminuse set to {__instance.terminalInUse}");
                if (TerminalStartPatch.alwaysOnDisplay)
                {
                    HandleAlwaysOnQuit(__instance);
                }
                else
                {
                    HandleRegularQuit();
                }
            }

            internal static void TerminalCameraStatus(bool status)
            {
                if (status == false)
                {
                    OpenBodyCamsCompatibility.TerminalMirrorStatus(status);
                    OpenBodyCamsCompatibility.TerminalCameraStatus(status);
                }
                else if (Plugin.instance.isOnMirror)
                {
                    OpenBodyCamsCompatibility.TerminalMirrorStatus(status);
                }
                else
                    OpenBodyCamsCompatibility.TerminalCameraStatus(status);
            }

            private static void HandleRegularQuit()
            {
                if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                {
                    Plugin.MoreLogs("Leaving terminal and disabling any active monitoring");
                    TerminalCameraStatus(false);
                    SplitViewChecks.CheckForSplitView("neither");
                }
            }

            private static void HandleAlwaysOnQuit(Terminal instance)
            {
                instance.StartCoroutine(instance.waitUntilFrameEndToSetActive(active: true));
                Plugin.Spam("Screen set to active");
                if (ViewCommands.isVideoPlaying)
                {
                    instance.videoPlayer.Pause();
                    instance.StartCoroutine(WaitUntilFrameEndVideo(instance));
                }

                if (ConfigSettings.alwaysOnDynamic.Value)
                    instance.StartCoroutine(AlwaysOnDynamic(instance));
            }

            private static IEnumerator WaitUntilFrameEndVideo(Terminal instance)
            {
                if (videoQuitEnum)
                    yield break;

                videoQuitEnum = true;

                yield return new WaitForEndOfFrame();
                if (ViewCommands.isVideoPlaying)
                    instance.videoPlayer.Play();
                Plugin.MoreLogs("attemtped to resume videoplayer");

                videoQuitEnum = false;
            }

        }

        [HarmonyPatch(typeof(Terminal), "Start")]
        public class TerminalStartPatch : Terminal
        {
            internal static bool alwaysOnDisplay = false;
            internal static bool isTermInUse = false;
            internal static TerminalNode startNode = null;
            internal static TerminalNode helpNode = null;
            internal static bool firstload = false;
            internal static bool delayStartEnum = false;

            internal static void TerminalStartGroup()
            {
                Plugin.MoreLogs("Upgrading terminal with my stuff, smile.");
                OverWriteTextNodes();
                TerminalClockStuff.MakeClock();
                ViewCommands.DetermineCamsTargets();
                ShortcutBindings.InitSavedShortcuts();
                TerminalCustomization();
            }

            internal static void TerminalStartGroupDelay()
            {
                Plugin.MoreLogs("Starting TerminalDelayStartEnumerator");
                Plugin.instance.Terminal.StartCoroutine(TerminalDelayStartEnumerator());
            }

            internal static IEnumerator TerminalDelayStartEnumerator()
            {
                if (delayStartEnum)
                    yield break;

                delayStartEnum = true;

                yield return new WaitForSeconds(1);
                Plugin.MoreLogs("1 Second delay methods starting.");
                StoreCommands(); //adding after delay for storerotation mod
                SplitViewChecks.CheckForSplitView("neither");
                Plugin.MoreLogs("disabling cams views");
                ViewCommands.isVideoPlaying = false;
                StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(0); //fix vanilla bug where you need to switch map target at start
                NetHandler.UpgradeStatusCheck(); // sync upgrades status for this save
                TerminalClockStuff.StartClockCoroutine();
                AlwaysOnStart(Plugin.instance.Terminal, startNode);
                yield return new WaitForSeconds(0.1f);
                StartCheck(Plugin.instance.Terminal, startNode);
                DebugShowInfo();
                delayStartEnum = false;
            }

            private static void DebugShowInfo()
            {
                /* Plugin.MoreLogs("------------------------ all nodes in darmuhsTerminalStuff dictionary ------------------------");
                foreach(KeyValuePair<TerminalNode, Func<string>> item in darmuhsTerminalStuff)
                {
                    Plugin.MoreLogs($"{item.Key.name}");
                } */
                Plugin.Spam($"darmuhsTerminalStuff Count: {darmuhsTerminalStuff.Count}");
                /* Plugin.MoreLogs("------------------------ all keywords in darmuhsTerminalStuff dictionary ------------------------");
                foreach(TerminalKeyword keyword in darmuhsKeywords)
                {
                    Plugin.MoreLogs($"{keyword.word}");
                } */
                Plugin.Spam($"darmuhsKeywords Count: {darmuhsKeywords.Count}");
                Plugin.Spam($"allMenuNodes Count: {MenuBuild.allMenuNodes.Count}");
                Plugin.Spam($"comfortEnabledCommands Count: {MenuBuild.comfortEnabledCommands.Count}");
                Plugin.Spam($"isNextEnabled: {MenuBuild.isNextEnabled}");
                Plugin.Spam($"Terminal Keywords Count: {Plugin.instance.Terminal.terminalNodes.allKeywords.Length}");
                Plugin.Spam($"Plugin.Allnodes: {Plugin.Allnodes.Count}");
                
                Plugin.Spam("------------------------ end of darmuh's debug info ------------------------");

            }

            private static void OverWriteTextNodes()
            {
                Plugin.MoreLogs("updating displaytext for help and home");
                if (!GameStartPatch.oneTimeOnly)
                {
                    startNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1];
                    helpNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13];
                    string original = helpNode.displayText;
                    Plugin.Spam(original);
                    string replacement = original.Replace("To see the list of moons the autopilot can route to.", "List of moons the autopilot can route to.").Replace("To see the company store's selection of useful items.", "Company store's selection of useful items.").Replace("[numberOfItemsOnRoute]", ">MORE\r\nTo see a list of commands added via darmuhsTerminalStuff\r\n\r\n[numberOfItemsOnRoute]");
                    Plugin.Spam($"{replacement}");

                    Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13].displayText = replacement;
                    Plugin.Spam("~~~~~~~~~~~~~~~~~~~~~~~~~~~~ HELP MODIFIED ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                    //string maskasciiart = "     ._______.\r\n     | \\   / |\r\n  .--|.O.|.O.|______.\r\n__).-| = | = |/   \\ |\r\np__) (.'---`.)Q.|.Q.|--.\r\n      \\\\___// = | = |-.(__\r\n       `---'( .---. ) (__&lt;\r\n             \\\\.-.//\r\n              `---'\r\n\t\t\t  ";
                    string asciiArt = ConfigSettings.homeTextArt.Value;
                    asciiArt = asciiArt.Replace("[leadingSpace]", " ");
                    asciiArt = asciiArt.Replace("[leadingSpacex4]", "    ");
                    //no known compatibility issues with home screen
                    startNode.displayText = $"{ConfigSettings.homeLine1.Value}\r\n{ConfigSettings.homeLine2.Value}\r\n\r\n{ConfigSettings.homeHelpLines.Value}\r\n{asciiArt}\r\n\r\n{ConfigSettings.homeLine3.Value}\r\n\r\n";
                    GameStartPatch.oneTimeOnly = true;
                }

                StopPersistingKeywords();
                ChangeVanillaKeywords();
            }

            //change vanilla terminal stuff here
            static void Postfix(ref Terminal __instance)
            {
                isTermInUse = __instance.terminalInUse;

                TerminalStartGroup();
                TerminalStartGroupDelay();
            }

            public static void ToggleScreen(bool status)
            {
                Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.waitUntilFrameEndToSetActive(status));
                Plugin.Spam($"Screen set to {status}");
            }

            private static void AlwaysOnStart(Terminal thisterm, TerminalNode startNode)
            {

                if (ConfigSettings.alwaysOnAtStart.Value && !firstload)
                {
                    Plugin.Spam("Setting AlwaysOn Display.");
                    if (ConfigSettings.networkedNodes.Value && ConfigSettings.ModNetworking.Value)
                    {
                        Plugin.Spam("network nodes enabled, syncing alwayson status");
                        NetHandler.Instance.StartAoDServerRpc(true);
                        firstload = true;
                    }
                    else
                    {
                        alwaysOnDisplay = true;
                        ToggleScreen(true);
                        thisterm.LoadNewNode(startNode);
                        firstload = true;
                    }

                }
            }

            private static void StartCheck(Terminal thisterm, TerminalNode startNode)
            {
                if (!ConfigSettings.ModNetworking.Value || !ConfigSettings.networkedNodes.Value)
                {
                    Plugin.Spam("Networking disabled, returning...");
                    thisterm.LoadNewNode(startNode);
                    return;
                }

                if (GameNetworkManager.Instance.localPlayerController.IsHost)
                {
                    thisterm.LoadNewNode(startNode);
                    StartofHandling.CheckNetNode(startNode);
                    return;
                }
                else
                {
                    Plugin.Spam("------------ CLIENT JUST LOADED --------------");
                    Plugin.Spam("grabbing node from host");
                    Plugin.Spam("------------ CLIENT JUST LOADED --------------");

                    int hostClient = Misc.HostClientID();
                    NetHandler.Instance.GetCurrentNodeServerRpc(((int)StartOfRound.Instance.localPlayerController.playerClientId), hostClient);
                    return;
                }
            }


            private static void StopPersistingKeywords()
            {
                //deletes keywords at game start if they exist from previous plays
                if (ConfigSettings.terminalTP.Value && Plugin.NormalTP == null)
                {
                    List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.tpKeywords.Value);

                    foreach (string keyword in getKeywords)
                    {
                        CheckForAndDeleteKeyWord(keyword.ToLower());;
                    }
                }
                if (ConfigSettings.terminalITP.Value && Plugin.InverseTP == null)
                {
                    List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.itpKeywords.Value);

                    foreach (string keyword in getKeywords)
                    {
                        CheckForAndDeleteKeyWord(keyword.ToLower());
                    }
                }

            }

            private static void ChangeVanillaKeywords()
            {
                //deletes keywords at game start if they exist from previous plays

                CheckForAndDeleteKeyWord("view monitor");
                MakeCommand("ViewInsideShipCam 1", "view monitor", "view monitor", false, true, ViewCommands.TermMapEvent, darmuhsTerminalStuff, 5, "map", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
                //AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
            }
        }

        [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
        public class Terminal_Begin_Patch
        {
            internal static void StartUsingTerminalCheck(Terminal instance)
            {

                if(ConfigSettings.TerminalAutoComplete.Value)
                {
                    if(Plugin.instance.removeTab)
                    {
                        Plugin.Spam("tab is disabled to quit terminal");
                        instance.playerActions.m_Movement_OpenMenu.Disable();
                        instance.playerActions.m_Movement_OpenMenu.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/tab", overridePath = "" });
                        instance.playerActions.m_Movement_OpenMenu.Enable();
                        HUDManager.Instance.ChangeControlTip(0, "Quit terminal : [Esc]", true);
                    }

                }

                //refund init
                if (ConfigSettings.terminalRefund.Value && ConfigSettings.ModNetworking.Value)
                {
                    Plugin.MoreLogs("Syncing items between players for refund command");
                    NetHandler.Instance.SyncDropShipServerRpc();
                }

                //walkie functions
                if (ConfigSettings.walkieTerm.Value)
                {
                    Plugin.Spam("Starting TalkinTerm Coroutine");
                    instance.StartCoroutine(WalkieTerm.TalkinTerm());
                }

                if (ConfigSettings.terminalShortcuts.Value && ShortcutBindings.keyActions.Count > 0)
                {
                    Plugin.Spam("Listening for shortcuts");
                    instance.StartCoroutine(ShortcutBindings.TerminalShortCuts());
                }

                //AlwaysOn Functions
                if (!TerminalStartPatch.alwaysOnDisplay)
                {
                    Plugin.MoreLogs("disabling cams views");
                    SplitViewChecks.DisableSplitView("neither");
                    ViewCommands.isVideoPlaying = false;

                    //Always load to start if alwayson disabled
                    instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
                }
                else
                {
                    Plugin.MoreLogs("Terminal is Always On, checking for active monitoring to return to.");
                    if (Plugin.instance.isOnMirror || Plugin.instance.isOnCamera || Plugin.instance.isOnMap || Plugin.instance.isOnMiniCams || Plugin.instance.isOnMiniMap || Plugin.instance.isOnOverlay)
                    {
                        int nodeNum = StartofHandling.FindViewIntByString();
                        if (nodeNum == -1)
                            return;

                        TerminalNode returnNode = StartofHandling.FindViewNode(nodeNum);
                        instance.LoadNewNode(returnNode);
                        Plugin.MoreLogs($"[returning to camera-type node during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}");
                        return;
                    }
                    else
                    {
                        Plugin.MoreLogs($"[no matching camera-type nodes during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}");
                        instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
                        return;
                    }
                }


            }

            static void Postfix(Terminal __instance)
            {
                Plugin.MoreLogs("Start Using Terminal Postfix");

                if (__instance != null)
                {
                    TerminalStartPatch.isTermInUse = __instance.terminalInUse;
                    Plugin.MoreLogs("__instance is not null");
                }
                else
                {
                    Plugin.MoreLogs("__instance is null, trying Plugin.Terminal instead");
                    TerminalStartPatch.isTermInUse = Plugin.instance.Terminal.terminalInUse;
                }

                StartUsingTerminalCheck(Plugin.instance.Terminal);

                if (StartOfRound.Instance.localPlayerController != null)
                    ShouldLockPlayerCamera(false, StartOfRound.Instance.localPlayerController);

                if (ViewCommands.termViewNodes.ContainsKey(Plugin.instance.Terminal.currentNode))
                    return;

                StartofHandling.CheckNetNode(Plugin.instance.Terminal.currentNode);
            }
        }

        [HarmonyPatch(typeof(Terminal), "LoadTerminalImage")]
        public class FixVideoPatch : Terminal
        {
            public static bool sanityCheckLOL = false;
            static void Postfix(ref Terminal __instance, TerminalNode node)
            {

                Terminal instanceCopy = __instance;
                if (node.name == "darmuh's videoPlayer" && sanityCheckLOL)
                {
                    Plugin.Spam("testing patch");
                    if (!ViewCommands.isVideoPlaying)
                    {
                        __instance.videoPlayer.enabled = true;
                        __instance.terminalImage.enabled = true;
                        __instance.videoPlayer.loopPointReached += vp => OnVideoEnd(instanceCopy);

                        __instance.videoPlayer.Play();
                        ViewCommands.isVideoPlaying = true;
                        Plugin.MoreLogs("isVideoPlaying set to TRUE");
                        sanityCheckLOL = false;
                        return;
                    }
                }
            }

            public static void OnVideoEnd(Terminal instance)
            {
                // This method will be called when the video is done playing
                // Disable the video player and terminal image here
                if (ViewCommands.isVideoPlaying)
                {
                    instance.videoPlayer.enabled = false;
                    instance.terminalImage.enabled = false;
                    ViewCommands.isVideoPlaying = false;
                    sanityCheckLOL = false;
                    Plugin.MoreLogs("isVideoPlaying set to FALSE");
                    instance.videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    instance.videoPlayer.source = VideoSource.VideoClip;
                    instance.videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
                    instance.videoPlayer.isLooping = true;
                    instance.videoPlayer.playOnAwake = true;

                }
            }

        }

        [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
        public class Terminal_ParsePlayerSentence_Patch
        {
            static void Postfix(Terminal __instance, ref TerminalNode __result)
            {
                if (ConfigSettings.networkedNodes.Value)
                    NetHandler.NetNodeReset(false);

                Plugin.instance.Terminal = __instance;

                StartofHandling.FirstCheck(__result);

                string cleanedText = GetCleanedScreenText(__instance);
                string[] words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length < 1)
                    return;

                StartofHandling.HandleParsed(__instance, __result, words, out TerminalNode parsedNode);
                if (parsedNode != null)
                {
                    __result = parsedNode;

                    StartofHandling.CheckNetNode(__result);
                    return;
                }

            }
        }

        [HarmonyPatch(typeof(Terminal), "SetTerminalInUseClientRpc")]
        public class TerminalInUseRpcPatch
        {
            static void Postfix()
            {
                ShouldDisableTerminalLight(ConfigSettings.DisableTerminalLight.Value);
            }
        }

        [HarmonyPatch(typeof(Terminal), "LoadNewNodeIfAffordable")]
        public class AffordableNodePatch
        {
            static void Postfix()
            {
                if (!ConfigSettings.terminalRefund.Value || !ConfigSettings.ModNetworking.Value)
                    return;

                NetHandler.Instance.SyncDropShipServerRpc();
                Plugin.Spam($"items: {Plugin.instance.Terminal.orderedItemsFromTerminal.Count}");
            }
        }

    }
}
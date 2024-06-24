using System;
using System.Collections.Generic;
using static TerminalStuff.AllMyTerminalPatches;

namespace TerminalStuff
{
    internal class MoreCamStuff
    {
        internal static List<string> excludedNames =
                //stuff that should not disable cams
                [
                    "ViewInsideShipCam 1",
                    "Mirror",
                    "Toggle Doors",
                    "Toggle Lights",
                    "Always-On Display",
                    "Use Inverse Teleporter",
                    "Use Teleporter",
                    "Clear Terminal Screen",
                    "Check Danger Level",
                    "Check Vitals",
                    "HealFromTerminal",
                    "Check Loot Value",
                    "RandomSuit",
                    "Terminal Clock",
                    "Switch to Previous",
                    "SwitchRadarCamPlayer 1",
                    "SwitchedCam",
                    "switchDummy",
                    "EnteredCode",
                    "FlashedRadarBooster",
                    "SendSignalTranslator",
                    "GeneralError",
                    "ParserError1",
                    "ParserError2",
                    "ParserError3",
                    "PingedRadarBooster",
                    "SendSignalTranslator",
                    "FinishedRadarBooster",
                ];

        internal static void VideoPersist(String nodeName)
        {
            if (ViewCommands.isVideoPlaying && nodeName != "darmuh's videoPlayer")
            {
                FixVideoPatch.OnVideoEnd(Plugin.instance.Terminal);
                ViewCommands.isVideoPlaying = false;
                //Plugin.Log.LogInfo("isVideoPlaying set to FALSE");
                Plugin.MoreLogs("disabling video");
            }
        }

        internal static void CamPersistance(string nodeName)
        {
            if (!excludedNames.Contains(nodeName) && HideCams())
            {
                SplitViewChecks.DisableSplitView("neither");
                Plugin.MoreLogs("disabling ANY cams views");
            }
        }

        private static bool HideCams()
        {
            return !ConfigSettings.camsNeverHide.Value;
        }
    }
}

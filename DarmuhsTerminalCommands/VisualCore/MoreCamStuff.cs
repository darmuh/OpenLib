using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.AllMyTerminalPatches;

namespace TerminalStuff
{
    internal class MoreCamStuff
    {
        internal static List<string> excludedNames =
                //stuff that should not disable cams
                [
                    "ViewInsideShipCam 1",
                    "TerminalStuff Mirror",
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

        internal static void CamInitMirror(Camera playerCam)
        {
            playerCam.cameraType = CameraType.Game;

            Transform termTransform = Plugin.instance.Terminal.transform;
            PlayerControllerB playerUsingTerminal = Misc.GetPlayerUsingTerminal();
            if(playerUsingTerminal == null)
            {
                Plugin.ERROR("playerUsingTerminal returned NULL");
                return;
            }
            Transform playerTransform = playerUsingTerminal.transform;
            Plugin.MoreLogs("camTransform assigned to terminal");

            // Calculate the opposite direction directly in local space
            Vector3 oppositeDirection = -playerTransform.forward;

            // Calculate the new rotation to look behind
            Quaternion newRotation = Quaternion.LookRotation(oppositeDirection, playerTransform.up);

            // Define the distance to back up the camera
            float distanceBehind = 1f;

            // Set camera's rotation and position
            playerCam.transform.rotation = newRotation;
            playerCam.transform.position = playerTransform.position - oppositeDirection * distanceBehind + playerTransform.up * 2f;
            float initCamHeight = playerCam.transform.position.y;
            Plugin.MoreLogs($"initCamHeight: {initCamHeight}");

            playerCam.transform.SetParent(termTransform);
        }

        internal static void ToggleCamState(Camera playerCam, bool state)
        {
            if (playerCam == null)
                return;

            playerCam.gameObject.SetActive(state);
            Plugin.MoreLogs($"{playerCam.name} game object set to state: {state}");

            if(state)
                ViewCommands.SetBodyCamTexture(playerCam.targetTexture);
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

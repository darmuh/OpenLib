using GameNetcodeStuff;
using OpenBodyCams;
using OpenBodyCams.API;
using System.Runtime.CompilerServices;
using UnityEngine;
using static TerminalStuff.Misc;

namespace TerminalStuff
{
    internal class OpenBodyCamsCompatibility
    {
        internal static MonoBehaviour TerminalBodyCam;
        internal static MonoBehaviour TerminalMirrorCam;
        internal static bool showingBodyCam = false;

        internal static Vector2Int defaultRes = new (1000, 700);

        internal static float defNearClip;
        internal static float defFarClip;
        internal static float defaultFov;

        internal static bool usingMirror;

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void UpdateCamsTarget()
        {
            Plugin.MoreLogs("Getting ZaggyCam texture");
            if (TerminalBodyCam == null || TerminalBodyCam.gameObject == null || ((BodyCamComponent)TerminalBodyCam) == null)
                CreateTerminalBodyCam();

            TerminalCameraStatus(true);
            Plugin.MoreLogs($"Attempting to grab targetTexture");
            usingMirror = false;
            SetBodyCamTexture(((BodyCamComponent)TerminalBodyCam).GetCamera().targetTexture);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void OpenBodyCamsMirror()
        {
            Plugin.MoreLogs("Getting ZaggyCam texture");
            if (TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null)
                CreateTerminalMirror();

            TerminalMirrorStatus(true);
            Plugin.MoreLogs($"Attempting to grab targetTexture");
            usingMirror = true;
            SetBodyCamTexture(((BodyCamComponent)TerminalMirrorCam).GetCamera().targetTexture);
        }

        private static void CameraEvent(Camera cam)
        {
            Plugin.MoreLogs($"Camera created, Updating target.");
            UpdateCamsTarget();
        }

        private static void SetBodyCamTexture(RenderTexture texture)
        {
            Plugin.MoreLogs("RenderTexture Created, updating values");
            ViewCommands.camsTexture = texture;

            if (!usingMirror)
            {
                if (ViewCommands.AnyActiveMonitoring() && (Plugin.instance.Terminal.terminalInUse || AllMyTerminalPatches.TerminalStartPatch.alwaysOnDisplay))
                {
                    Plugin.MoreLogs("Active Cams mode detected on terminal");
                    TerminalCameraStatus(true);
                    ViewCommands.ReInitCurrentMode(texture);
                    ReturnToBodyCam(texture);
                    return;
                }
            }
            else
            {
                Plugin.MoreLogs("Setting mirror texture stuff");
                TerminalMirrorStatus(true);
                ChangeTerminalCameraView(usingMirror);
                return;
            }

        }

        private static void ReturnToBodyCam(RenderTexture texture)
        {
            if (ViewCommands.AnyActiveMonitoring() && (Plugin.instance.Terminal.terminalInUse || AllMyTerminalPatches.TerminalStartPatch.alwaysOnDisplay))
            {
                Plugin.MoreLogs("Active Cams mode detected on terminal");
                TerminalCameraStatus(true);
                ViewCommands.ReInitCurrentMode(texture);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalMirror()
        {
            if (!Plugin.instance.OpenBodyCamsMod)
                return;

            if (TerminalBodyCam != null && TerminalBodyCam.gameObject != null)
                Object.Destroy(TerminalBodyCam);

            if (TerminalMirrorCam != null && TerminalMirrorCam.gameObject != null)
                Object.Destroy(TerminalMirrorCam);

            Plugin.MoreLogs("CreateTerminalMirror called");
            var terminalMirrorCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, null);

            TerminalMirrorCam = terminalMirrorCam;
            terminalMirrorCam.OnRenderTextureCreated += ViewCommands.SetBodyCamTexture;
            Camera cam = terminalMirrorCam.GetCamera();
            cam.gameObject.name = "darmuh's OBC mirrorcam";
            terminalMirrorCam.Resolution = defaultRes;
            ViewCommands.SetBodyCamTexture(cam.targetTexture);

            Plugin.MoreLogs("darmuh's OBC mirrorcam updated!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalBodyCam()
        {
            if (!Plugin.instance.OpenBodyCamsMod)
                return;

            if (TerminalBodyCam != null && TerminalBodyCam.gameObject != null)
                Object.Destroy(TerminalBodyCam);

            if (TerminalMirrorCam != null && TerminalMirrorCam.gameObject != null)
                Object.Destroy(TerminalMirrorCam);

            Plugin.MoreLogs("CreateTerminalBodyCam called");

            if (Plugin.instance.TwoRadarMapsMod)
            {
                TwoRadarMapsCamCreate();
            }
            else
            {
                Plugin.MoreLogs("Creating bodycam with no obc syncing");
                var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, StartOfRound.Instance.mapScreen);

                TerminalBodyCam = terminalBodyCam;
                terminalBodyCam.Resolution = defaultRes;
                terminalBodyCam.OnRenderTextureCreated += ViewCommands.SetBodyCamTexture;
                Camera cam = terminalBodyCam.GetCamera();
                cam.gameObject.name = "darmuh's OBC bodycam";
                ViewCommands.SetBodyCamTexture(cam.targetTexture);
                GetCameraDefaults(cam.farClipPlane, cam.nearClipPlane, cam.fieldOfView);
            }

            Plugin.MoreLogs("darmuh's OBC termcam updated!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void TwoRadarMapsCamCreate()
        {
            Plugin.MoreLogs("Tying bodycam to tworadarmaps radarview");
            var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, TwoRadarMaps.Plugin.TerminalMapRenderer);

            TerminalBodyCam = terminalBodyCam;
            terminalBodyCam.Resolution = defaultRes;
            terminalBodyCam.OnRenderTextureCreated += SetBodyCamTexture;
            terminalBodyCam.OnCameraCreated += CameraEvent;
            Camera cam = terminalBodyCam.GetCamera();
            cam.gameObject.name = "darmuh's OBC bodycam";
            SetBodyCamTexture(cam.targetTexture);
            GetCameraDefaults(cam.farClipPlane, cam.nearClipPlane, cam.fieldOfView);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TerminalCameraStatus(bool enabled)
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs($"Screen Enabled: [{enabled}]");
                ((BodyCamComponent)TerminalBodyCam).ForceEnableCamera = enabled;
                showingBodyCam = enabled;
            }

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TerminalMirrorStatus(bool enabled)
        {
            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                Plugin.MoreLogs($"Screen Enabled: [{enabled}]");
                ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = enabled;
                showingBodyCam = enabled;
            }

        }

        private static void GetCameraDefaults(float farClipPlane, float nearClipPlane, float fieldOfView)
        {
            Plugin.MoreLogs("getting default values");
            defFarClip = farClipPlane;
            defNearClip = nearClipPlane;
            defaultFov = fieldOfView;
        }

        internal static void ChangeTerminalCameraView(bool isMirror)
        {
            Camera terminalCam = ((BodyCamComponent)TerminalMirrorCam).GetCamera();

            if (isMirror)
            {
                Plugin.MoreLogs("Setting OBC terminal camera to mirror mode");
                PlayerControllerB playerUsingTerminal = GetPlayerUsingTerminal();
                if (playerUsingTerminal == null)
                {
                    Plugin.ERROR("Unable to determine player using temrinal for mirror command");
                    return;
                }

                Transform termTransform = Plugin.instance.Terminal.transform;
                Transform playerTransform = playerUsingTerminal.transform;
                Plugin.MoreLogs("camTransform assigned to terminal");

                // Calculate the opposite direction directly in local space
                Vector3 oppositeDirection = -playerTransform.forward;

                // Calculate the new rotation to look behind
                Quaternion newRotation = Quaternion.LookRotation(oppositeDirection, playerTransform.up);

                // Define the distance to back up the camera
                float distanceBehind = 1f;

                // Set camera's rotation and position
                terminalCam.transform.rotation = newRotation;
                terminalCam.transform.position = playerTransform.position - oppositeDirection * distanceBehind + playerTransform.up * 2.2f;

                terminalCam.orthographic = true;
                terminalCam.orthographicSize = 3.4f;
                terminalCam.farClipPlane = 30f;
                terminalCam.nearClipPlane = 0.25f;
                terminalCam.fieldOfView = 130f;
                terminalCam.transform.SetParent(termTransform);
            }
            /*       else
                   {
                       terminalCam.orthographic = false;
                       terminalCam.nearClipPlane = defNearClip;
                       terminalCam.farClipPlane = defFarClip;
                       terminalCam.fieldOfView = defaultFov;
                       ((BodyCamComponent)TerminalBodyCam).UpdateTargetStatus();
                   } */
        }
    }
}

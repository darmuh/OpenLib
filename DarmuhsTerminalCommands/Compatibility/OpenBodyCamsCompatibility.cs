using GameNetcodeStuff;
using OpenBodyCams;
using OpenBodyCams.API;
using suitsTerminal;
using System.Collections.Generic;
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


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void UpdateCamsTarget()
        {
            Plugin.MoreLogs("OBC - Getting ZaggyCam texture");
            if (TerminalBodyCam == null || TerminalBodyCam.gameObject == null || ((BodyCamComponent)TerminalBodyCam) == null)
            {
                CreateTerminalBodyCam();
                return;
            }
            else
            {
                ToggleOpenCams(true, false);
                Plugin.MoreLogs($"OBC - camera already created, assigning targetTexture and enabling camera");
            }
                
            
        }

        private static void CameraEvent(Camera cam)
        {
            Plugin.MoreLogs($"OBC - Camera {cam.name} created.");
            //UpdateCamsTarget();
        }

        private static void SetMirrorCamTexture(RenderTexture texture)
        {
            ViewCommands.camsTexture = texture;
            Plugin.MoreLogs("OBC - Setting mirror texture stuff");
            TerminalMirrorStatus(true);
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalBodyCam()
        {
            Plugin.MoreLogs("OBC - CreateTerminalBodyCam()");

            if (!Plugin.instance.OpenBodyCamsMod)
                return;

            ToggleOpenCams(true, false);

            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs("OBC - bodycam already created and should be enabled, returning");
                return;
            }

            if (Plugin.instance.TwoRadarMapsMod)
            {
                TwoRadarMapsCamCreate();
            }
            else
            {
                Plugin.MoreLogs("OBC - Creating bodycam synced to mapScreen");
                var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, StartOfRound.Instance.mapScreen);

                TerminalBodyCam = terminalBodyCam;
                terminalBodyCam.Resolution = GetResolutionForOBC(ConfigSettings.obcResolutionBodyCam.Value);
                terminalBodyCam.OnRenderTextureCreated += ViewCommands.SetBodyCamTexture;
                terminalBodyCam.OnCameraCreated += CameraEvent;
                terminalBodyCam.OnBlankedSet += CamIsBlanked;
                terminalBodyCam.ForceEnableCamera = true;
                Camera cam = terminalBodyCam.GetCamera();
                cam.gameObject.name = "TerminalStuff OBC bodycam";
                ViewCommands.SetBodyCamTexture(cam.targetTexture);
                terminalBodyCam.SetTargetToPlayer(GetPlayerUsingTerminal());
            }

            Plugin.MoreLogs("OBC - darmuhsTerminalStuff OBC termcam updated!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ResidualCamsCheck()
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Object.Destroy(((BodyCamComponent)TerminalBodyCam));
                TerminalBodyCam = null;
                Plugin.MoreLogs("Attempting to destroy residual TerminalBodyCam");
            }

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                Object.Destroy(((BodyCamComponent)TerminalMirrorCam));
                TerminalMirrorCam = null;
                Plugin.MoreLogs("Attempting to destroy residual TerminalMirrorCam");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void TwoRadarMapsCamCreate()
        {
            if (!Plugin.instance.OpenBodyCamsMod || !Plugin.instance.TwoRadarMapsMod)
                return;

            ToggleOpenCams(true, false);

            if(TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs("OBC - bodycam already created and should be enabled, returning");
                return;
            }

            Plugin.MoreLogs("OBC - Tying bodycam to tworadarmaps radarview");
            var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, TwoRadarMaps.Plugin.TerminalMapRenderer);

            TerminalBodyCam = terminalBodyCam;
            terminalBodyCam.Resolution = GetResolutionForOBC(ConfigSettings.obcResolutionBodyCam.Value);
            terminalBodyCam.OnRenderTextureCreated += ViewCommands.SetBodyCamTexture;
            terminalBodyCam.OnCameraCreated += CameraEvent;
            terminalBodyCam.OnBlankedSet += CamIsBlanked;
            terminalBodyCam.ForceEnableCamera = true;
            Camera cam = terminalBodyCam.GetCamera();
            cam.gameObject.name = "TerminalStuff 2RadarCompat OBC bodycam";
            cam.gameObject.SetActive(true);
            ViewCommands.SetBodyCamTexture(cam.targetTexture);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TerminalCameraStatus(bool enabled)
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs($"OBC - BodyCam Screen Enabled: [{enabled}]");
                MoreCamStuff.ToggleCamState(((BodyCamComponent)TerminalBodyCam).GetCamera(), enabled);
                showingBodyCam = enabled;
            }

        }

        private static void ToggleOpenCams(bool bodyCam, bool mirrorCam)
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                ((BodyCamComponent)TerminalBodyCam).ForceEnableCamera = bodyCam;
                MoreCamStuff.ToggleCamState(((BodyCamComponent)TerminalBodyCam).GetCamera(), bodyCam);
                Plugin.MoreLogs($"OBC - BodyCam detected and set to [{bodyCam}]");
                showingBodyCam = bodyCam;
            }

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = mirrorCam;
                MoreCamStuff.ToggleCamState(((BodyCamComponent)TerminalMirrorCam).GetCamera(), mirrorCam);
                Plugin.MoreLogs($"OBC - MirrorCam detected and set to [{mirrorCam}]");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TerminalMirrorStatus(bool enabled)
        {
            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                MoreCamStuff.ToggleCamState(((BodyCamComponent)TerminalMirrorCam).GetCamera(), enabled);
                Plugin.MoreLogs($"OBC - Setting Mirror Status: [{enabled}]");
            }
                
        }


        // from suitsTerminal

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void OpenBodyCamsMirror()
        {
            Plugin.MoreLogs("OBC - Getting ZaggyCam texture OpenBodyCamsMirror()");
            if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null))
                CreateTerminalMirror();

            Plugin.MoreLogs($"OBC - Attempting to grab targetTexture");
            SetMirrorCamTexture(((BodyCamComponent)TerminalMirrorCam).GetCamera().targetTexture);
            ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = true;

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void OpenBodyCamsMirrorStatus(bool state)
        {
            Plugin.MoreLogs($"OBC - OpenBodyCamsMirrorStatus() state: {state}");
            if (state)
                OpenBodyCamsMirror();
            else
                TerminalCameraStatus(state);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalMirror()
        {
            if (!Plugin.instance.OpenBodyCamsMod)
                return;

            ToggleOpenCams(false, true);

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                Plugin.MoreLogs("OBC - MirrorCam already created and should be enabled, returning");
                return;
            }

            Plugin.MoreLogs("OBC - CreateTerminalMirror called");
            var terminalMirrorCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null);

            TerminalMirrorCam = terminalMirrorCam;
            terminalMirrorCam.OnRenderTextureCreated += SetMirrorCamTexture;
            Renderer[] termStuffToHide = GetStuffToHide();
            terminalMirrorCam.OnRenderersToHideChanged += originalRenderers => [.. termStuffToHide];
            terminalMirrorCam.OnCameraCreated += ResetTransform;
            terminalMirrorCam.OnBlankedSet += CamIsBlanked;

            terminalMirrorCam.Resolution = GetResolutionForOBC(ConfigSettings.obcResolutionMirror.Value);
            terminalMirrorCam.SetTargetToTransform(Plugin.instance.Terminal.transform);
            Camera cam = terminalMirrorCam.GetCamera();
            cam.gameObject.name = "TerminalStuff obc mirrorcam";
            SetMirrorCamTexture(cam.targetTexture);

            cam.orthographic = true;
            cam.orthographicSize = 3.4f;
            cam.usePhysicalProperties = false;
            cam.farClipPlane = 30f;
            cam.nearClipPlane = 0.05f;
            cam.fieldOfView = 130f;
            MoreCamStuff.CamInitMirror(cam);
            Plugin.MoreLogs("OBC - TerminalStuff obc mirrorcam created!");
        }

        private static Renderer[] GetStuffToHide()
        {
            Plugin.MoreLogs("OBC - Getting renderers (stuff) to hide from mirrorcam!");

            Renderer termGameObject = Plugin.instance.Terminal.gameObject.GetComponent<MeshRenderer>();
            Renderer[] allRenderers = [termGameObject];
            if (GetTermObjects(out GameObject termCable, out GameObject termKeyboard))
            {
                Renderer termCableRender = termCable.GetComponent<MeshRenderer>();
                Renderer termKeyboardRender = termKeyboard.GetComponent<MeshRenderer>();
                allRenderers = [termGameObject, termCableRender, termKeyboardRender];
            }

            return allRenderers;
        }

        private static bool GetTermObjects(out GameObject termCable, out GameObject termKeyboard)
        {
            termCable = GameObject.Find("Environment/HangarShip/Terminal/BezierCurve.001");
            termKeyboard = GameObject.Find("Environment/HangarShip/Terminal/Terminal.003");
            if (termCable && termKeyboard != null)
                return true;
            else
                return false;
        }

        private static void CamIsBlanked(bool isBlanked)
        {
            Plugin.MoreLogs($"OBC - CamIsBlanked: {isBlanked}");
            ResidualCamsCheck();
        }

        private static void ResetTransform(Camera cam)
        {
            Plugin.MoreLogs("OBC - ResetTransform Called!");
            MoreCamStuff.CamInitMirror(cam);
        }

        private static Vector2Int GetResolutionForOBC(string configItem)
        {
            Vector2Int resolution;
            List<string> resolutionStrings = StringStuff.GetKeywordsPerConfigItem(configItem);
            List<int> resolutionList = StringStuff.GetNumberListFromStringList(resolutionStrings);
            if (resolutionList.Count == 2)
            {
                resolution = new Vector2Int(resolutionList[0], resolutionList[1]);
                Plugin.Log.LogInfo($"OBC - Resolution set to {resolutionList[0]}x{resolutionList[1]}");
                return resolution;
            }
            else
            {
                resolution = new Vector2Int(1000, 700);
                Plugin.Log.LogInfo($"OBC - Unable to set resolution to values provided in config: {configItem}\nUsing default of 1000x700");
                return resolution;
            }
        }

    }
}

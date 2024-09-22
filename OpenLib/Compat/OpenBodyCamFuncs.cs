using OpenBodyCams;
using OpenBodyCams.API;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;
using static OpenLib.Common.CamStuff;
using OpenLib.Common;

namespace OpenLib.Compat
{
    public class OpenBodyCamFuncs
    {
        public static MonoBehaviour TerminalBodyCam;
        public static MonoBehaviour TerminalMirrorCam;
        public static bool ShowingBodyCam = false;
        private static Vector2Int DefaultRes = new(1000, 700);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UpdateCamsTarget(string resolution)
        {
            Plugin.MoreLogs("OBC - Getting ZaggyCam texture");
            if (TerminalBodyCam == null || TerminalBodyCam.gameObject == null || ((BodyCamComponent)TerminalBodyCam) == null)
            {
                CreateTerminalBodyCam(resolution);
                return;
            }
            else
            {
                ToggleOpenCams(true, false);
                Plugin.MoreLogs($"OBC - camera already created, assigning targetTexture and enabling camera");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool BodyCamIsUnlocked()
        {
            if (BodyCam.BodyCamsAreAvailable)
                return true;
            else
                return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Texture GetTexture(MonoBehaviour mono)
        {
            BodyCamComponent bodycam = mono as BodyCamComponent;
            if (bodycam != null)
                return bodycam.GetCamera().targetTexture;
            else
            {
                Plugin.WARNING("Unable to grab bodycamcomponent @GetTexture");
                return null;
            }
        }

        private static void CameraEvent(Camera cam)
        {
            Plugin.MoreLogs($"OBC - Camera {cam.name} created.");
            //UpdateCamsTarget();
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateTerminalBodyCam(string resolution)
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
                TwoRadarMapsCamCreate(resolution);
            }
            else
            {
                Plugin.MoreLogs("OBC - Creating bodycam synced to mapScreen");
                var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, StartOfRound.Instance.mapScreen);

                TerminalBodyCam = terminalBodyCam;
                terminalBodyCam.Resolution = GetResolutionForOBC(resolution);
                terminalBodyCam.OnRenderTextureCreated += SetBodyCamTexture;
                terminalBodyCam.OnCameraCreated += CameraEvent;
                terminalBodyCam.OnBlankedSet += CamIsBlanked;
                terminalBodyCam.ForceEnableCamera = true;
                Camera cam = terminalBodyCam.GetCamera();
                cam.gameObject.name = "TerminalStuff OBC bodycam";
                SetBodyCamTexture(cam.targetTexture);
                terminalBodyCam.SetTargetToPlayer(StartOfRound.Instance.mapScreen.targetedPlayer);
            }

            Plugin.MoreLogs("OBC - darmuhsTerminalStuff OBC termcam updated!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ResidualCamsCheck()
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
        public static void TwoRadarMapsCamCreate(string res)
        {
            if (!Plugin.instance.OpenBodyCamsMod || !Plugin.instance.TwoRadarMapsMod)
                return;

            ToggleOpenCams(true, false);

            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs("OBC - bodycam already created and should be enabled, returning");
                return;
            }

            Plugin.MoreLogs("OBC - Tying bodycam to tworadarmaps radarview");
            var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null, TwoRadarMaps.Plugin.TerminalMapRenderer);

            TerminalBodyCam = terminalBodyCam;
            terminalBodyCam.Resolution = GetResolutionForOBC(res);
            terminalBodyCam.OnRenderTextureCreated += SetBodyCamTexture;
            terminalBodyCam.OnCameraCreated += CameraEvent;
            terminalBodyCam.OnBlankedSet += CamIsBlanked;
            terminalBodyCam.ForceEnableCamera = true;
            Camera cam = terminalBodyCam.GetCamera();
            cam.gameObject.name = "TerminalStuff 2RadarCompat OBC bodycam";
            cam.gameObject.SetActive(true);
            SetBodyCamTexture(cam.targetTexture);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TerminalCameraStatus(bool enabled)
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                Plugin.MoreLogs($"OBC - BodyCam Screen Enabled: [{enabled}]");
                ToggleCamState(((BodyCamComponent)TerminalBodyCam).GetCamera(), enabled);
                ShowingBodyCam = enabled;
            }

        }

        public static void ToggleOpenCams(bool bodyCam, bool mirrorCam)
        {
            if (TerminalBodyCam != null || ((BodyCamComponent)TerminalBodyCam) != null)
            {
                ((BodyCamComponent)TerminalBodyCam).ForceEnableCamera = bodyCam;
                ToggleCamState(((BodyCamComponent)TerminalBodyCam).GetCamera(), bodyCam);
                Plugin.MoreLogs($"OBC - BodyCam detected and set to [{bodyCam}]");
                ShowingBodyCam = bodyCam;
            }

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = mirrorCam;
                ToggleCamState(((BodyCamComponent)TerminalMirrorCam).GetCamera(), mirrorCam);
                Plugin.MoreLogs($"OBC - MirrorCam detected and set to [{mirrorCam}]");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TerminalMirrorStatus(bool enabled)
        {
            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                ToggleCamState(((BodyCamComponent)TerminalMirrorCam).GetCamera(), enabled);
                Plugin.MoreLogs($"OBC - Setting Mirror Status: [{enabled}]");
            }

        }


        // from suitsTerminal

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OpenBodyCamsMirror(string res, float zoom, bool ortho)
        {
            Plugin.MoreLogs("OBC - Getting ZaggyCam texture OpenBodyCamsMirror()");
            if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null))
                CreateTerminalMirror(res, zoom, ortho);

            Plugin.MoreLogs($"OBC - Attempting to grab targetTexture");
            SetMirrorCamTexture(((BodyCamComponent)TerminalMirrorCam).GetCamera().targetTexture);
            ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = true;

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OpenBodyCamsMirrorStatus(bool state, string res, float zoom, bool ortho)
        {
            Plugin.MoreLogs($"OBC - OpenBodyCamsMirrorStatus() state: {state}");
            if (state)
                OpenBodyCamsMirror(res, zoom, ortho);
            else
                TerminalCameraStatus(state);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateTerminalMirror(string res, float zoom, bool ortho)
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

            terminalMirrorCam.Resolution = GetResolutionForOBC(res);
            terminalMirrorCam.SetTargetToTransform(MirrorObject.transform);
            Camera cam = terminalMirrorCam.GetCamera();
            cam.gameObject.name = "OpenLib OBC mirrorcam";
            SetMirrorCamTexture(cam.targetTexture);

            CamInitMirror(cam, zoom, ortho);
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
            CamInitMirror(cam, -1, false);
        }

        private static Vector2Int GetResolutionForOBC(string configItem)
        {
            Vector2Int resolution;
            List<string> resolutionStrings = Common.CommonStringStuff.GetKeywordsPerConfigItem(configItem);
            List<int> resolutionList = Common.CommonStringStuff.GetNumberListFromStringList(resolutionStrings);
            if (resolutionList.Count == 2)
            {
                resolution = new Vector2Int(resolutionList[0], resolutionList[1]);
                Plugin.Log.LogInfo($"OBC - Resolution set to {resolutionList[0]}x{resolutionList[1]}");
                return resolution;
            }
            else
            {
                resolution = DefaultRes;
                Plugin.Log.LogInfo($"OBC - Unable to set resolution to values provided in config: {configItem}\nUsing default of 1000x700");
                return resolution;
            }
        }
    }
}

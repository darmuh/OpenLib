using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


namespace OpenLib.Common
{
    public class CamStuff
    {
        public static Events.Events.CustomEvent<RenderTexture> BodyCamTextureSet = new();
        public static Events.Events.CustomEvent<RenderTexture> MirrorCamTextureSet = new();
        public static GameObject MyCameraHolder = null!;
        public static GameObject ObcCameraHolder = null!;
        public static HDAdditionalCameraData CameraData = null!;

        public static void SetBodyCamTexture(RenderTexture texture)
        {
            BodyCamTextureSet.Invoke(texture);
            Plugin.MoreLogs("Assigning bodycam texture");
        }

        public static void SetMirrorCamTexture(RenderTexture texture)
        {
            MirrorCamTextureSet.Invoke(texture);
            Plugin.MoreLogs("Assigning mirror texture");

            if (Plugin.instance.OpenBodyCamsMod)
                Compat.OpenBodyCamFuncs.TerminalMirrorStatus(true);
        }

        public static void ToggleCamState(Camera playerCam, bool state)
        {
            if (playerCam == null)
                return;

            playerCam.gameObject.SetActive(state);
            Plugin.MoreLogs($"{playerCam.gameObject.name} set to state: {state}");

            if (state)
                SetBodyCamTexture(playerCam.targetTexture);
        }

        public static void CamInitMirror(GameObject CameraHolder, Camera playerCam, float zoom, bool ortho)
        {
            if (zoom > 0)
            {
                playerCam.cameraType = CameraType.Game;
                playerCam.orthographic = ortho;
                playerCam.orthographicSize = zoom;
                playerCam.usePhysicalProperties = false;
                playerCam.farClipPlane = 30f;
                playerCam.nearClipPlane = 0.05f;
                playerCam.fieldOfView = 130f;
            }

            CameraHolder.SetActive(true);
            playerCam.transform.SetParent(CameraHolder.transform);
            Transform termTransform = Plugin.instance.Terminal.terminalImage.transform;

            Quaternion newRotation = Quaternion.LookRotation(-termTransform.transform.forward, termTransform.up);

            Plugin.MoreLogs("camTransform assigned to MirrorObject, which is assigned to termTransform");
            CameraHolder.transform.SetParent(termTransform);

            // Set camera's rotation and position
            CameraHolder.transform.rotation = newRotation;
            CameraHolder.transform.position = termTransform.position;
            Plugin.MoreLogs($"initCamHeight: {CameraHolder.transform.position.y}");


        }

        public static Camera HomebrewCam(ref RenderTexture mycamTexture, ref GameObject CamObject)
        {
            if (CamObject == null)
                CamObject = new("OpenLib Cam (Homebrew)");

            Camera playerCam;
            if (CamObject.GetComponent<Camera>() != null)
                playerCam = CamObject.GetComponent<Camera>();
            else
                playerCam = CamObject.AddComponent<Camera>();

            if (mycamTexture == null)
                mycamTexture = new(StartOfRound.Instance.localPlayerController.gameplayCamera.targetTexture);

            int cullingMaskInt = StartOfRound.Instance.localPlayerController.gameplayCamera.cullingMask & ~LayerMask.GetMask(["Ignore Raycast", "UI", "HelmetVisor"]);

            if (Plugin.instance.ModelReplacement)
                cullingMaskInt = Compat.ModelAPI.GetThirdPersonMask(cullingMaskInt);
            else
            {
                cullingMaskInt |= (1 << 23); //show this bit since every mod likes to make the player this layer

                StartOfRound.Instance.localPlayerController.thisPlayerModelArms.gameObject.layer = 5;
                //always set model arms to UI layer when using homebrew cams (except for modelreplacementAPI)

                if (CamObject.GetComponent<HDAdditionalCameraData>() == null)
                {
                    CameraData = CamObject.AddComponent<HDAdditionalCameraData>();
                    CameraData.volumeLayerMask = 1;
                    CameraData.hasPersistentHistory = true;

                    HDAdditionalCameraData original = StartOfRound.Instance.localPlayerController.gameplayCamera.GetComponent<HDAdditionalCameraData>();
                    if (original.customRenderingSettings)
                    {
                        Plugin.Spam("Using original customRenderingSettings for OpenLib cams");
                        CameraData.customRenderingSettings = true;
                        CameraData.renderingPathCustomFrameSettings = original.renderingPathCustomFrameSettings;
                        CameraData.renderingPathCustomFrameSettingsOverrideMask = original.renderingPathCustomFrameSettingsOverrideMask;

                    }
                }

            }

            playerCam.targetTexture = mycamTexture;

            playerCam.cullingMask = cullingMaskInt;

            CamObject.SetActive(false);
            Plugin.MoreLogs("playerCam instantiated");
            return playerCam;
        }

        //States
        public static void HomebrewCameraState(bool active, Camera playerCam)
        {
            if (playerCam == null)
                return;

            playerCam.gameObject.SetActive(active);

        }

        public static Camera GetCam(GameObject Container)
        {
            if (Container == null)
                return null!;

            return Container.GetComponent<Camera>();
        }
    }
}

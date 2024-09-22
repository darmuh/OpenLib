using UnityEngine;


namespace OpenLib.Common
{
    public class CamStuff
    {
        public static Events.Events.CustomEvent<RenderTexture> BodyCamTextureSet = new();
        public static Events.Events.CustomEvent<RenderTexture> MirrorCamTextureSet = new();
        public static GameObject MirrorObject;
        //public static GameObject BodyCamObject = new("BodyCamObject");

        public static void SetBodyCamTexture(RenderTexture texture)
        {
            BodyCamTextureSet.Invoke(texture);
            Plugin.MoreLogs("Assigning bodycam texture");
        }

        public static void SetMirrorCamTexture(RenderTexture texture)
        {
            MirrorCamTextureSet.Invoke(texture);
            Plugin.MoreLogs("Assigning mirror texture");

            if(Plugin.instance.OpenBodyCamsMod)
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

        public static void CamInitMirror(Camera playerCam, float zoom, bool ortho)
        {
            if(zoom > 0)
            {
                playerCam.cameraType = CameraType.Game;
                playerCam.orthographic = ortho;
                playerCam.orthographicSize = zoom;
                playerCam.usePhysicalProperties = false;
                playerCam.farClipPlane = 30f;
                playerCam.nearClipPlane = 0.05f;
                playerCam.fieldOfView = 130f;
            }

            MirrorObject.SetActive(true);
            playerCam.transform.SetParent(MirrorObject.transform);
            Transform termTransform = Plugin.instance.Terminal.terminalImage.transform;

            Quaternion newRotation = Quaternion.LookRotation(-termTransform.transform.forward, termTransform.up);

            Plugin.MoreLogs("camTransform assigned to MirrorObject, which is assigned to termTransform");
            MirrorObject.transform.SetParent(termTransform);

            // Set camera's rotation and position
            MirrorObject.transform.rotation = newRotation;
            MirrorObject.transform.position = termTransform.position;
            Plugin.MoreLogs($"initCamHeight: {MirrorObject.transform.position.y}");

            
        }
    }
}

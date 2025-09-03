using OpenBodyCams;
using OpenBodyCams.API;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static OpenLib.Common.CamStuff;
using Object = UnityEngine.Object;

namespace OpenLib.Compat;
public class OpenBodyCamFuncs
{
    public static MonoBehaviour TerminalBodyCam = null!;
    public static MonoBehaviour TerminalMirrorCam = null!;
    public static bool ShowingBodyCam = false;
    private static Vector2Int DefaultRes = new(1000, 700);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void UpdateCamsTarget(string resolution)
    {
        Loggers.LogInfo("OBC - Getting ZaggyCam texture");
        if (TerminalBodyCam == null || TerminalBodyCam.gameObject == null || ((BodyCamComponent)TerminalBodyCam) == null!)
        {
            CreateTerminalBodyCam(resolution);
            return;
        }
        else
        {
            ToggleOpenCams(true, false);
            Loggers.LogInfo($"OBC - camera already created, assigning targetTexture and enabling camera");
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
    public static Camera GetCam(MonoBehaviour mono)
    {
        BodyCamComponent? bodycam = mono as BodyCamComponent;
        if (bodycam != null!)
            return bodycam.GetCamera()!;
        else
        {
            Loggers.WARNING("Unable to grab bodycamcomponent @GetCam");
            return null!;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Texture GetTexture(MonoBehaviour mono)
    {
        BodyCamComponent? bodycam = mono as BodyCamComponent;
        if (bodycam != null!)
        {
            var cam = bodycam.GetCamera()!;
            if (cam == null!)
            {
                Loggers.WARNING("Null camera @GetTexture");
                return null!;
            }
                
            else
                return cam.targetTexture;
        }
        else
        {
            Loggers.WARNING("Unable to grab bodycamcomponent @GetTexture");
            return null!;
        }
    }

    private static void CameraEvent(Camera cam)
    {
        Loggers.LogInfo($"OBC - Camera {cam.name} created.");
        //UpdateCamsTarget();
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreateTerminalBodyCam(string resolution)
    {
        Loggers.LogInfo("OBC - CreateTerminalBodyCam()");

        if (!Plugin.instance.OpenBodyCamsMod)
            return;

        ToggleOpenCams(true, false);

        if (TerminalBodyCam != null || (TerminalBodyCam as BodyCamComponent) != null!)
        {
            Loggers.LogInfo("OBC - bodycam already created and should be enabled, returning");
            return;
        }

        if (Plugin.instance.TwoRadarMapsMod)
        {
            TwoRadarMapsCamCreate(resolution);
        }
        else
        {
            Loggers.LogInfo("OBC - Creating bodycam synced to mapScreen");
            var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null!, StartOfRound.Instance.mapScreen);

            TerminalBodyCam = terminalBodyCam;
            terminalBodyCam.Resolution = GetResolutionForOBC(resolution);
            terminalBodyCam.OnRenderTextureCreated += SetBodyCamTexture;
            terminalBodyCam.OnCameraCreated += CameraEvent;
            terminalBodyCam.OnBlankedSet += CamIsBlanked;
            terminalBodyCam.ForceEnableCamera = true;
            Camera cam = terminalBodyCam.GetCamera()!;
            if (cam == null!)
            {
                Loggers.WARNING("Camera is null at creation of CreateTerminalBodyCam!");
                return;
            }
            cam.gameObject.name = "TerminalStuff OBC bodycam";
            SetBodyCamTexture(cam.targetTexture);
            terminalBodyCam.SetTargetToPlayer(StartOfRound.Instance.mapScreen.targetedPlayer);
        }

        Loggers.LogInfo("OBC - darmuhsTerminalStuff OBC termcam updated!");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ResidualCamsCheck()
    {
        if (TerminalBodyCam != null || (TerminalBodyCam as BodyCamComponent) != null!)
        {
            Object.Destroy(((BodyCamComponent)TerminalBodyCam));
            TerminalBodyCam = null!;
            Loggers.LogInfo("Attempting to destroy residual TerminalBodyCam");
        }

        if (TerminalMirrorCam != null || (TerminalMirrorCam as BodyCamComponent) != null!)
        {
            Object.Destroy(((BodyCamComponent)TerminalMirrorCam));
            TerminalMirrorCam = null!;
            Loggers.LogInfo("Attempting to destroy residual TerminalMirrorCam");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TwoRadarMapsCamCreate(string res)
    {
        if (!Plugin.instance.OpenBodyCamsMod || !Plugin.instance.TwoRadarMapsMod)
            return;

        ToggleOpenCams(true, false);

        if (TerminalBodyCam != null || (TerminalBodyCam as BodyCamComponent) != null!)
        {
            Loggers.LogInfo("OBC - bodycam already created and should be enabled, returning");
            return;
        }

        Loggers.LogInfo("OBC - Tying bodycam to tworadarmaps radarview");
        var terminalBodyCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null!, TwoRadarMaps.Plugin.TerminalMapRenderer);

        TerminalBodyCam = terminalBodyCam;
        terminalBodyCam.Resolution = GetResolutionForOBC(res);
        terminalBodyCam.OnRenderTextureCreated += SetBodyCamTexture;
        terminalBodyCam.OnCameraCreated += CameraEvent;
        terminalBodyCam.OnBlankedSet += CamIsBlanked;
        terminalBodyCam.ForceEnableCamera = true;
        Camera? cam = terminalBodyCam.GetCamera();
        
        if (cam == null!)
        {
            Loggers.WARNING("2RadarCompat OBC: GetCamera returned NULL at creation!");
            return;
        }

        cam.gameObject.name = "TerminalStuff 2RadarCompat OBC bodycam";
        cam.gameObject.SetActive(true);
        SetBodyCamTexture(cam.targetTexture);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TerminalCameraStatus(bool enabled)
    {
        if (TerminalBodyCam == null!) return;
        var obcam = TerminalBodyCam as BodyCamComponent;
        if (obcam == null!) return;

        Loggers.LogInfo($"OBC - BodyCam Screen Enabled: [{enabled}]");
        obcam.ForceEnableCamera = enabled;
        ToggleCamState(obcam.GetCamera()!, enabled);
        ShowingBodyCam = enabled;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ToggleOpenCams(bool bodyCam, bool mirrorCam)
    {
        if (TerminalBodyCam != null!)
        {
            var bcam = TerminalBodyCam as BodyCamComponent;
            if (bcam != null!)
            {
                bcam.ForceEnableCamera = bodyCam;
                ToggleCamState(bcam.GetCamera()!, bodyCam);
                Loggers.LogInfo($"OBC - BodyCam detected and set to [{bodyCam}]");
            }
        }

        if (TerminalMirrorCam != null!)
        {
            var mcam = TerminalMirrorCam as BodyCamComponent;
            if (mcam != null!)
            {
                mcam.ForceEnableCamera = mirrorCam;
                ToggleCamState(mcam.GetCamera()!, mirrorCam);
                Loggers.LogInfo($"OBC - BodyCam detected and set to [{mirrorCam}]");
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TerminalMirrorStatus(bool enabled)
    {
        if (TerminalMirrorCam == null!) return;
            var mcam = TerminalMirrorCam as BodyCamComponent;
        if (mcam == null!)
            return;
        
        mcam.ForceEnableCamera = enabled;
        ToggleCamState(mcam.GetCamera()!, enabled);
        Loggers.LogInfo($"OBC - BodyCam detected and set to [{enabled}]");

    }

    // from suitsTerminal
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void OpenBodyCamsMirror(string res, float zoom, bool ortho, ref GameObject CamHolder)
    {
        Loggers.LogInfo("OBC - Getting ZaggyCam texture OpenBodyCamsMirror()");
        if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null!))
            CreateTerminalMirror(res, zoom, ortho, CamHolder);

        var mirror = TerminalMirrorCam as BodyCamComponent;
        if (mirror == null!)
        {
            Loggers.WARNING("OpenBodyCamsMirror: Mirror creation failed!");
            return;
        }

        Loggers.LogInfo($"OBC - Attempting to grab targetTexture");
        SetMirrorCamTexture(mirror.GetCamera()!.targetTexture);
        mirror.ForceEnableCamera = true;

    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void OpenBodyCamsMirrorStatus(bool state, string res, float zoom, bool ortho, ref GameObject CamHolder)
    {
        Loggers.LogInfo($"OBC - OpenBodyCamsMirrorStatus() state: {state}");
        if (state)
            OpenBodyCamsMirror(res, zoom, ortho, ref CamHolder);
        else
            TerminalMirrorStatus(state);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreateTerminalMirror(string res, float zoom, bool ortho, GameObject CameraHolder)
    {
        if (!Plugin.instance.OpenBodyCamsMod)
            return;

        if (CameraHolder == null!)
            CameraHolder = new("ObcCamHolder");

        ToggleOpenCams(false, true);

        if (TerminalMirrorCam != null || TerminalMirrorCam as BodyCamComponent != null!)
        {
            Loggers.LogInfo("OBC - MirrorCam already created and should be enabled, returning");
            return;
        }

        Loggers.LogInfo("OBC - CreateTerminalMirror called");
        var terminalMirrorCam = BodyCam.CreateBodyCam(Plugin.instance.Terminal.gameObject, screenMaterial: null!);

        TerminalMirrorCam = terminalMirrorCam;
        terminalMirrorCam.OnRenderTextureCreated += SetMirrorCamTexture;

        terminalMirrorCam.OnCameraCreated += ResetTransform;
        terminalMirrorCam.OnBlankedSet += CamIsBlanked;

        terminalMirrorCam.Resolution = GetResolutionForOBC(res);
        terminalMirrorCam.SetTargetToTransform(CameraHolder.transform);
        Camera cam = terminalMirrorCam.GetCamera()!;

        if (cam == null!)
        {
            Loggers.WARNING("Mirror camera is null at creation!");
            return;
        }

        cam.gameObject.name = "OpenLib OBC mirrorcam";
        SetMirrorCamTexture(cam.targetTexture);

        CamInitMirror(CameraHolder, cam, zoom, ortho);
        Loggers.LogInfo("OBC - TerminalStuff obc mirrorcam created!");
    }

    private static void CamIsBlanked(bool isBlanked)
    {
        Loggers.LogInfo($"OBC - CamIsBlanked: {isBlanked}");
        //ResidualCamsCheck();
    }

    private static void ResetTransform(Camera cam)
    {
        Loggers.LogInfo("OBC - ResetTransform Called!");
        CamInitMirror(((BodyCamComponent)TerminalMirrorCam).gameObject, cam, -1, false);
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

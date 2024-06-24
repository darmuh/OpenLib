using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TerminalStuff
{
    internal class SplitViewChecks : MonoBehaviour
    {
        internal static bool enabledSplitObjects = false;

        public static void InitSplitViewObjects()
        {
            Plugin.instance.terminalCanvas = Plugin.instance.Terminal.terminalUIScreen;

            if (Plugin.instance.terminalCanvas == null)
            {
                Plugin.Log.LogError("Canvas not found");
                return;
            }

            Plugin.MoreLogs("Canvas found");

            RawImage originalRawImage = Plugin.instance.terminalCanvas.transform.Find("MainContainer/ImageContainer/Image (1)")?.GetComponent<RawImage>();

            if (originalRawImage == null || Plugin.instance.splitViewCreated)
            {
                Plugin.MoreLogs("Original RawImage not found or split view already created");
                return;
            }

            Plugin.MoreLogs("Original RawImage found");

            // Duplicate the RawImage
            GameObject rawImageGameObject2 = Instantiate(originalRawImage.gameObject, originalRawImage.transform.parent);
            GameObject rawImageGameObject1 = Instantiate(originalRawImage.gameObject, originalRawImage.transform.parent);
            rawImageGameObject1.name = "Terminal Small Screen (Clone)";
            rawImageGameObject2.name = "Terminal Full Screen (Clone)";

            // Get the RawImage components from the duplicated GameObjects
            Plugin.instance.rawImage2 = rawImageGameObject2.GetComponent<RawImage>();
            Plugin.instance.rawImage1 = rawImageGameObject1.GetComponent<RawImage>();

            // Store the original dimensions and anchored positions
            StoreOriginalDimensionsAndPositions();

            Plugin.instance.splitViewCreated = true;
        }

        private static void StoreOriginalDimensionsAndPositions()
        {
            Plugin.instance.originalTopSize = Plugin.instance.rawImage1.rectTransform.sizeDelta;
            Plugin.instance.originalTopPosition = Plugin.instance.rawImage1.rectTransform.anchoredPosition;

            Plugin.instance.originalBottomSize = Plugin.instance.rawImage2.rectTransform.sizeDelta;
            Plugin.instance.originalBottomPosition = Plugin.instance.rawImage2.rectTransform.anchoredPosition;
        }

        public static void CheckForSplitView(string whatIsIt)
        {
            // Assuming you have a reference to the ImageContainer GameObject
            GameObject imageContainer = Plugin.instance.Terminal.terminalImageMask.gameObject;
            List<string> singleViewModes = ["cams", "map", "mirror"];

            if (enabledSplitObjects == false)
            {
                DisableCloneChildren(imageContainer);
                ResetPluginInstanceBools();
            }
            else if (whatIsIt == "neither")
            {
                enabledSplitObjects = false;
                DisableCloneChildren(imageContainer);
                ResetPluginInstanceBools();
            }
            else if (enabledSplitObjects == true && (!singleViewModes.Contains(whatIsIt)))
            {
                EnableCloneChildren(imageContainer);
                ViewCommands.SetAnyCamsTrue();
                UpdatePluginInstanceBools(whatIsIt);
            }
            else if (enabledSplitObjects == true && (singleViewModes.Contains(whatIsIt)))
            {
                EnableFullScreenAndDisableSmallScreen(imageContainer, whatIsIt);
                UpdatePluginInstanceBools(whatIsIt);
            }
            else
            {
                Plugin.MoreLogs("No matches for split view objects");
            }
        }

        private static void DisableCloneChildren(GameObject imageContainer)
        {
            if (imageContainer != null)
            {
                foreach (Transform child in imageContainer.transform)
                {
                    if (child.gameObject.name.Contains("Terminal Full Screen (Clone)") || child.gameObject.name.Contains("Terminal Small Screen (Clone)"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

        private static void EnableCloneChildren(GameObject imageContainer)
        {
            if (imageContainer != null)
            {
                foreach (Transform child in imageContainer.transform)
                {
                    if (child.gameObject.name.Contains("Terminal Full Screen (Clone)") || child.gameObject.name.Contains("Terminal Small Screen (Clone)"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }

        private static void EnableFullScreenAndDisableSmallScreen(GameObject imageContainer, string whatIsIt)
        {
            if (imageContainer != null)
            {
                foreach (Transform child in imageContainer.transform)
                {
                    if (child.gameObject.name.Equals("Terminal Full Screen (Clone)"))
                    {
                        child.gameObject.SetActive(true);
                        Plugin.MoreLogs($"Enabled full screen for {whatIsIt}");
                    }
                    if (child.gameObject.name.Equals("Terminal Small Screen (Clone)"))
                    {
                        child.gameObject.SetActive(false);
                        Plugin.MoreLogs($"Disabled small screen for {whatIsIt}");
                    }
                }
            }
        }

        internal static void ResetPluginInstanceBools()
        {
            ViewCommands.ResetPluginInstanceBools();
        }

        internal static void EnableSplitView(string whatIsIt)
        {
            enabledSplitObjects = true;
            CheckForSplitView(whatIsIt);
            UpdatePluginInstanceBools(whatIsIt);
            ShowCameraView(true);
        }

        internal static void DisableSplitView(string whatIsIt)
        {
            enabledSplitObjects = false;
            CheckForSplitView(whatIsIt);
            ShowCameraView(false);
        }

        private static void ShowCameraView(bool state)
        {
            if (ConfigSettings.camsUseDetectedMods.Value && Plugin.instance.OpenBodyCamsMod)
            {
                AllMyTerminalPatches.QuitPatch.TerminalCameraStatus(state);
            }
            else
                ViewCommands.SetCameraState(state);
        }

        private static void UpdatePluginInstanceBools(string whatIsIt)
        {
            ResetPluginInstanceBools();

            switch (whatIsIt)
            {
                case "minimap":
                    Plugin.instance.isOnMiniMap = true;
                    break;
                case "minicams":
                    Plugin.instance.isOnMiniCams = true;
                    break;
                case "overlay":
                    Plugin.instance.isOnOverlay = true;
                    break;
                case "cams":
                    Plugin.instance.isOnCamera = true;
                    break;
                case "mirror":
                    Plugin.instance.isOnMirror = true;
                    break;
                case "map":
                    Plugin.instance.isOnMap = true;
                    break;
                default:
                    Plugin.MoreLogs($"Unexpected value for whatIsIt: {whatIsIt}");
                    break;
            }
        }
    }
}

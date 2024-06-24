using GameNetcodeStuff;
using System.Text.RegularExpressions;
using UnityEngine;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.StringStuff;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace TerminalStuff
{
    internal class ColorCommands
    {
        internal static Color? CustomColor { get; private set; } // Public static variable to store the flashlight color
        internal static string flashLightColor;
        internal static bool usingHexCode = false;

        internal static void FlashLightCommandAction(out string displayText)
        {
            string playerName = GameNetworkManager.Instance.localPlayerController.playerUsername;
            ulong playerID = GameNetworkManager.Instance.localPlayerController.playerClientId;
            string colorName = flashLightColor;

            Plugin.MoreLogs($"{playerName} trying to set color {colorName} to flashlight");
            Color flashlightColor = CustomColor ?? Color.white; // Use white as a default color
            Plugin.MoreLogs($"got {colorName} - {flashlightColor}");

            NetHandler.Instance.FlashColorServerRpc(flashlightColor, playerID, playerName);
            if (Plugin.instance.fSuccess && Plugin.instance.hSuccess)
            {
                displayText = $"Flashlight Color set to {colorName}.\r\nHelmet Light Color set to {colorName}.\r\n\r\n";
                Plugin.instance.fSuccess = false;
                Plugin.instance.hSuccess = false;
                return;
            }
            else if (Plugin.instance.fSuccess && !Plugin.instance.hSuccess)
            {
                displayText = $"Flashlight Color set to {colorName}.\r\nUnable to set Helmet Light Color.\r\n\r\n";
                Plugin.instance.fSuccess = false;
                Plugin.instance.hSuccess = false;
                return;
            }
            else
            {
                displayText = "Cannot set flashlight color.\r\n\r\nEnsure you have equipped a flashlight before using this command.\r\n\r\n";
                return;
            }
        }

        internal static void SetCustomColor(string colorKeyword)
        {
            if (IsHexColorCode(colorKeyword))
            {
                // If it's a valid hex code, convert it to a Color
                usingHexCode = true;
                CustomColor = HexToColor("#" + colorKeyword);
                return;
            }
            else
            {
                CustomColor = colorKeyword.ToLower() switch
                {
                    "normal" => (Color?)Color.white,
                    "default" => (Color?)Color.white,
                    "red" => (Color?)Color.red,
                    "blue" => (Color?)Color.blue,
                    "yellow" => (Color?)Color.yellow,
                    "cyan" => (Color?)Color.cyan,
                    "magenta" => (Color?)Color.magenta,
                    "green" => (Color?)Color.green,
                    "purple" => (Color?)new Color32(144, 100, 254, 1),
                    "lime" => (Color?)new Color32(166, 254, 0, 1),
                    "pink" => (Color?)new Color32(242, 0, 254, 1),
                    "maroon" => (Color?)new Color32(114, 3, 3, 1),//new
                    "orange" => (Color?)new Color32(255, 117, 24, 1),//new
                    "sasstro" => (Color?)new Color32(212, 148, 180, 1),
                    "samstro" => (Color?)new Color32(180, 203, 240, 1),
                    _ => null,//this needs to be null for invalid results to return invalid
                };
            }
        }

        internal static bool IsHexColorCode(string input)
        {
            // Check if the input is a valid hex color code
            return Regex.IsMatch(input, "^(?:[0-9a-fA-F]{3}){1,2}$");
        }

        internal static Color HexToColor(string hex)
        {
            // Convert hex color code to Color
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }

        //dynamic commands logic

        internal static string ShipColorBase()
        {
            string[] words = GetWords();

            if (words.Length < 2)
            {
                string message = ShipColorList();
                Plugin.ERROR("not enough words for the command!");
                return message;
            }
            else if (words[1].Contains("list"))
            {
                string message = ShipColorList();
                Plugin.MoreLogs("list requested");
                return message;
            }

            bool sColorCheck = ShipColorCommon(words, out string displayText, out string targetColor, out Color newColor);
            if (!sColorCheck)
                return displayText;

            if (words[1].Contains("all"))
            {
                NetHandler.Instance.ShipColorALLServerRpc(newColor, targetColor);
                return displayText;
            }
            else if (words[1].Contains("front"))
            {
                NetHandler.Instance.ShipColorFRONTServerRpc(newColor, targetColor);
                return displayText;
            }
            else if (words[1].Contains("mid"))
            {
                NetHandler.Instance.ShipColorMIDServerRpc(newColor, targetColor);
                return displayText;
            }
            else if (words[1].Contains("back"))
            {
                NetHandler.Instance.ShipColorBACKServerRpc(newColor, targetColor);
                return displayText;
            }
            else
            {
                Plugin.ERROR("failed to grab specific part of ship lights to change");
                string listContents = ShipColorList();
                return listContents;
            }
        }

        internal static bool ShipColorCommon(string[] words, out string displayText, out string targetColor, out Color newColor)
        {
            if (words.Length < 3)
            {
                displayText = ShipColorList();
                Plugin.MoreLogs("not enough words for the command, returning list!");
                targetColor = string.Empty;
                newColor = Color.white;
                return false;
            }

            targetColor = words[2];
            Plugin.MoreLogs($"Attempting to set {words[1]} ship light colors to {targetColor}");
            SetCustomColor(targetColor);
            if (CustomColor.HasValue && targetColor != null)
            {
                newColor = CustomColor.Value;
                displayText = $"Color of {words[1]} ship lights set to {targetColor}!\r\n\r\n";
                return true;
            }
            else
            {
                targetColor = "";
                newColor = Color.white;
                displayText = $"Unable to set {words[1]} ship light color...\r\n\tInvalid color [{targetColor}] detected!\r\n\r\n";
                Plugin.ERROR("invalid color for the color command!");
                return false;
            }
        }

        internal static string ShipColorList()
        {
            string listContent = $"========= Ship Lights Color Options List =========\r\nColor Name: \"command used\"\r\n\r\nDefault: \"{sColor} all normal\" or \"{sColor} all default\"\r\nRed: \"{sColor} back red\"\r\nGreen: \"{sColor} mid green\"\r\nBlue: \"{sColor} front blue\"\r\nYellow: \"{sColor} middle yellow\"\r\nCyan: \"{sColor} all cyan\"\r\nMagenta: \"{sColor} back magenta\"\r\nPurple: \"{sColor} mid purple\"\r\nLime: \"{sColor} all lime\"\r\nPink: \"{sColor} front pink\"\r\nMaroon: \"{sColor} middle maroon\"\r\nOrange: \"{sColor} back orange\"\r\nSasstro's Color: \"{sColor} all sasstro\"\r\nSamstro's Color: \"{sColor} all samstro\"\r\nANY HEXCODE: \"{sColor} all FF00FF\"\r\n\r\n\r\n";
            return listContent;
        }

        internal static string FlashColorBase()
        {
            string[] words = GetWords();
            string message;

            if (words.Length < 2)
            {
                message = FlashColorList();
                Plugin.ERROR("getting list, not enough words for color command!");
                return message;
            }

            if (words[1].Contains("list"))
            {
                message = FlashColorList();
                Plugin.MoreLogs("displaying flashcolor list");
                return message;
            }
            if (words[1].Contains("rainbow"))
            {
                message = FlashColorRainbow();
                Plugin.MoreLogs("running rainbow command");
                return message;
            }

            string targetColor = words[1];

            Plugin.MoreLogs($"Attempting to set flashlight color to {targetColor}");
            SetCustomColor(targetColor);
            flashLightColor = targetColor;

            if (CustomColor.HasValue)
            {
                Plugin.MoreLogs($"Using flashlight color: {targetColor}");
                NetHandler.Instance.endFlashRainbow = true;
                FlashLightCommandAction(out string displayText);
                return displayText;
            }
            else
            {
                string displayText = $"Unable to set flashlight color...\r\n\tInvalid color: [{targetColor}] detected!\r\n\r\n";
                Plugin.ERROR("invalid color for the color command!");
                return displayText;
            }
        }

        internal static string FlashColorList()
        {
            string listContent = $"========= Flashlight Color Options List =========\r\nColor Name: \"command used\"\r\n\r\nDefault: \"{fColor} normal\" or \"{fColor} default\"\r\nRed: \"{fColor} red\"\r\nGreen: \"{fColor} green\"\r\nBlue: \"{fColor} blue\"\r\nYellow: \"{fColor} yellow\"\r\nCyan: \"{fColor} cyan\"\r\nMagenta: \"{fColor} magenta\"\r\nPurple: \"{fColor} purple\"\r\nLime: \"{fColor} lime\"\r\nPink: \"{fColor} pink\"\r\nMaroon: \"{fColor} maroon\"\r\nOrange: \"{fColor} orange\"\r\nSasstro's Color: \"{fColor} sasstro\"\r\nSamstro's Color: \"{fColor} samstro\"\r\n\r\nRainbow Color (animated): \"{fColor} rainbow\"\r\nANY HEXCODE: \"{fColor} FF00FF\"\r\n\r\n";
            return listContent;
        }

        internal static string FlashColorRainbow()
        {
            if (DoIhaveFlash(StartOfRound.Instance.localPlayerController))
            {
                NetHandler.Instance.CycleThroughRainbowFlash();
                string displayText = $"Flashlight color set to Rainbow Mode! (performance may vary)\r\n\r\n";
                return displayText;
            }
            else
            {
                string displayText = $"You have to be holding a flashlight to change it's color!\r\n\r\n";
                return displayText;
            }

        }

        private static bool DoIhaveFlash(PlayerControllerB player)
        {
            GrabbableObject[] objectsOfType = Object.FindObjectsOfType<GrabbableObject>();

            foreach (GrabbableObject thisFlash in objectsOfType)
            {
                if (thisFlash.playerHeldBy != null)
                {
                    if (thisFlash.playerHeldBy.playerUsername == player.playerUsername && thisFlash.gameObject.name.Contains("Flashlight"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}

using OpenLib.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenLib.Common
{
    public class TerminalStart
    {
        public static bool delayStartEnum = false;

        internal static void TerminalStartGroupDelay()
        {
            Plugin.Spam("Starting TerminalDelayStartEnumerator");
            Plugin.instance.Terminal.StartCoroutine(TerminalDelayStartEnumerator());
        }

        internal static IEnumerator TerminalDelayStartEnumerator()
        {
            if (delayStartEnum)
                yield break;

            delayStartEnum = true;

            yield return new WaitForSeconds(1);
            Plugin.MoreLogs("1 Second delay methods starting.");
            EventManager.TerminalDelayStart.Invoke();
            AddStoreItems(); //adding after delay for storerotation mod
            delayStartEnum = false;
        }

        public static void AddStoreItems()
        {
            if (Plugin.instance.TerminalFormatter)
                return;

            AddShopItemsToFurnitureList();
        }

        private static void AddShopItemsToFurnitureList()
        {
            foreach (TerminalNode shopNode in Plugin.ShopNodes)
            {

                if (!Plugin.instance.Terminal.ShipDecorSelection.Contains(shopNode))
                {
                    Plugin.instance.Terminal.ShipDecorSelection.Add(shopNode);
                    Plugin.Spam($"adding {shopNode.creatureName} to shipdecorselection");
                }
                else
                {
                    Plugin.Spam($"{shopNode.creatureName} already in shipdecorselection");
                }
            }

            Plugin.Spam("nodes have been added");
        }

        private static void OverWriteTextNodes()
        {
            Plugin.MoreLogs("updating displaytext for help and home");
            if (!StartGame.oneTimeOnly)
            {
                TerminalNode startNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1];
                TerminalNode helpNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13];
                //string original = helpNode.displayText;
                //Plugin.Spam(original);
                //string replacement = original.Replace("To see the list of moons the autopilot can route to.", "List of moons the autopilot can route to.").Replace("To see the company store's selection of useful items.", "Company store's selection of useful items.").Replace("[numberOfItemsOnRoute]", ">MORE\r\nTo see a list of commands added via darmuhsTerminalStuff\r\n\r\n[numberOfItemsOnRoute]");
                //Plugin.Spam($"{replacement}");

                //Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13].displayText = replacement;
                //Plugin.Spam("~~~~~~~~~~~~~~~~~~~~~~~~~~~~ HELP MODIFIED ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                //string maskasciiart = "     ._______.\r\n     | \\   / |\r\n  .--|.O.|.O.|______.\r\n__).-| = | = |/   \\ |\r\np__) (.'---`.)Q.|.Q.|--.\r\n      \\\\___// = | = |-.(__\r\n       `---'( .---. ) (__&lt;\r\n             \\\\.-.//\r\n              `---'\r\n\t\t\t  ";
                //string asciiArt = ConfigSettings.homeTextArt.Value;
                //asciiArt = asciiArt.Replace("[leadingSpace]", " ");
                //asciiArt = asciiArt.Replace("[leadingSpacex4]", "    ");
                //no known compatibility issues with home screen
                //startNode.displayText = $"{ConfigSettings.homeLine1.Value}\r\n{ConfigSettings.homeLine2.Value}\r\n\r\n{ConfigSettings.homeHelpLines.Value}\r\n{asciiArt}\r\n\r\n{ConfigSettings.homeLine3.Value}\r\n\r\n";
                StartGame.oneTimeOnly = true;
            }
        }
    }
}

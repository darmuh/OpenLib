using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenLib.Common
{
    public class CommonTerminal
    {
        public static TerminalNode parseNode = null!;

        public static void ToggleScreen(bool status)
        {
            Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.waitUntilFrameEndToSetActive(status));
            Plugin.Spam($"Screen set to {status}");
        }

        public static bool TryGetNodeFromList(string query, Dictionary<string, TerminalNode> nodeListing, out TerminalNode returnNode)
        {
            foreach (KeyValuePair<string, TerminalNode> pairValue in nodeListing)
            {
                if (pairValue.Key == query)
                {
                    returnNode = pairValue.Value;
                    return true;
                }
            }
            returnNode = null!;
            return false; // No matching command found for the given query
        }

        //shop

        public static void AddShopItemsToFurnitureList(List<TerminalNode> UnlockableNodes)
        {
            foreach (TerminalNode shopNode in UnlockableNodes)
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

        public static string ClearText() //function used in terminalstuff clear command
        {
            string displayText = "\n";
            Plugin.Spam("display text cleared for real this time!!!");
            return displayText;
        }

        // ----------------- Obsolete Old Methods ----------------- //

        [Obsolete("Use TryGetNodeFromList instead to avoid NRE")]
        public static TerminalNode GetNodeFromList(string query, Dictionary<string, TerminalNode> nodeListing)
        {
            foreach (KeyValuePair<string, TerminalNode> pairValue in nodeListing)
            {
                if (pairValue.Key == query)
                {
                    return pairValue.Value;
                }
            }
            return null!; // No matching command found for the given query
            //You should expect this null result if using this method!
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenLib.Common
{
    public class CommonTerminal
    {
        public static TerminalNode parseNode = null!;
        public static Color CaretOriginal;

        public static void ToggleScreen(bool status)
        {
            Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.waitUntilFrameEndToSetActive(status));
            Plugin.Spam($"Screen set to {status}");
        }

        public static void ChangeCaretColor(Color newColor, bool saveOriginal)
        {
            if(saveOriginal)
                CaretOriginal = Plugin.instance.Terminal.screenText.caretColor;

            Plugin.instance.Terminal.screenText.caretColor = newColor;
        }

        public static bool TryGetNodeFromList(string query, Dictionary<string, TerminalNode> nodeListing, out TerminalNode returnNode)
        {
            returnNode = null!;

            if (query.Length == 0)
                return false;

            string[] words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!nodeListing.Any(x => words.Contains(x.Key.ToLower())))
                return false;

            returnNode = nodeListing.FirstOrDefault(x => x.Key.ToLower() == words[0].ToLower()).Value;

            if(returnNode != null )
                return true;

            returnNode = nodeListing.FirstOrDefault(t => words.Any(x => x.ToLower() == t.Key.ToLower())).Value;

            if (returnNode != null)
                return true;

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

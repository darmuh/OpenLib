using OpenLib.Compat;
using OpenLib.CoreMethods;
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
        public static Color transparent = new(0, 0, 0, 0);

        //cached common
        public static TerminalKeyword InfoKeyword
        {
            get
            {
                if (DynamicBools.TryGetKeyword("info", out TerminalKeyword keyword))
                {
                    return keyword;
                }
                else
                {
                    Plugin.WARNING("InfoKeyword reference could not be found! [NULL]");
                    return null!;
                }
            }
        }
        public static TerminalKeyword BuyKeyword
        {
            get
            {
                if (DynamicBools.TryGetKeyword("buy", out TerminalKeyword keyword))
                {
                    return keyword;
                }
                else
                {
                    Plugin.WARNING("BuyKeyword reference could not be found! [NULL]");
                    return null!;
                }
            }
        }
        public static TerminalKeyword OtherKeyword
        {
            get
            {
                if (DynamicBools.TryGetKeyword("other", out TerminalKeyword keyword))
                {
                    return keyword;
                }
                else
                {
                    Plugin.WARNING("OtherKeyword reference could not be found! [NULL]");
                    return null!;
                }
            }
        }

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

        public static void LoadNewNode(TerminalNode node)
        {
            if (Plugin.instance.TerminalStuff)
                TerminalStuffMod.LoadAndSync(node);
            else
                Plugin.instance.Terminal.LoadNewNode(node);
        }

        public static bool TryLoadKeyword(string keyword)
        {
            if (DynamicBools.TryGetKeyword(keyword, out TerminalKeyword word))
            {
                TerminalNode node = word.specialKeywordResult;
                Plugin.Spam($"TryLoadKeyword found keyword [ {word.word} ]");
                LoadNewNode(node);
                return true;
            }

            return false;
        }

        public static bool TryGetCommand(string words, out TerminalNode returnNode)
        {
            returnNode = null!;
            
            if (words.Length == 0)
                return false;

            CommandManager special = Plugin.AllCommands.FirstOrDefault(x => x.AcceptAdditionalText && x.KeywordList.Any(s => words.ToLowerInvariant().StartsWith(s.ToLowerInvariant())));
            
            if (special != null)
            {
                returnNode = special.terminalNode;
                return returnNode != null;
            }

            CommandManager normal = Plugin.AllCommands.FirstOrDefault(x => x.KeywordList.Any(s => words.ToLowerInvariant() == s.ToLowerInvariant()));
            
            if (normal != null)
            {
                returnNode = normal.terminalNode;
                return returnNode != null;
            }

            Plugin.Spam("No matching commands in Plugin.AllCommands");
            return false;

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

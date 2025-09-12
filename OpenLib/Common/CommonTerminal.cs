using OpenLib.Compat;
using OpenLib.CoreMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenLib.Common;
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
                Loggers.WARNING("InfoKeyword reference could not be found! [NULL]");
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
                Loggers.WARNING("BuyKeyword reference could not be found! [NULL]");
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
                Loggers.WARNING("OtherKeyword reference could not be found! [NULL]");
                return null!;
            }
        }
    }
    private static TerminalNode _home = null!;
    public static TerminalNode HomePage
    {
        get
        {
            if(_home == null)
            {
                _home = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1];
            }

            return _home;
        }
    }

    public static void ToggleScreen(bool status)
    {
        Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.waitUntilFrameEndToSetActive(status));
        Loggers.LogDebug($"Screen set to {status}");
    }

    public static void ChangeCaretColor(Color newColor, bool saveOriginal)
    {
        if (saveOriginal)
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

    public static void TrySyncNodeOnly(TerminalNode node)
    {
        if (!Plugin.instance.TerminalStuff)
            return;

        TerminalStuffMod.NetSync(node);
    }

    public static bool TryLoadKeyword(string keyword)
    {
        if (DynamicBools.TryGetKeyword(keyword, out TerminalKeyword word))
        {
            TerminalNode node = word.specialKeywordResult;
            Loggers.LogDebug($"TryLoadKeyword found keyword [ {word.word} ]");
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

        List<CommandManager> enabled = Plugin.AllCommands.FindAll(x => x.IsCommandEnabled());

        // accepting input after command
        //for this one we are expecting only a partial match of the keyword to the full string of words
        var specialCommands = enabled.Where(x => x.AcceptAdditionalText);
        CommandManager special = specialCommands.FirstOrDefault(x => x.terminalKeywords.Any(s => Misc.StringStartsWithInvariant(words, s.word)));

        if (special != null!)
        {
            returnNode = special.terminalNode;
            return returnNode != null!;
        }

        //expects EXACT match
        CommandManager exactMatch = enabled.FirstOrDefault(x => x.terminalKeywords.Any(s => Misc.CompareStringsInvariant(words, s.word)));

        if (exactMatch != null)
        {
            returnNode = exactMatch.terminalNode;
            return returnNode != null!;
        }

        if (words.Length < 3)
        {
            Loggers.LogDebug($"No matching commands for [ {words} ] in all enabled commands (short query)");
            return false;
        }

        //allows for loose matching
        //for this one we are expecting only a partial match of the provided words to the terminalKeyword
        CommandManager looseMatch = enabled.FirstOrDefault(x => x.terminalKeywords.Any(s => Misc.StringStartsWithInvariant(s.word, words)));

        if (looseMatch != null)
        {
            returnNode = looseMatch.terminalNode;
            return returnNode != null;
        }

        Loggers.LogDebug($"No matching commands for [ {words} ] in all enabled commands");
        return false;

    }

    public static bool TryGetNodeFromList(string query, Dictionary<string, TerminalNode> nodeListing, out TerminalNode returnNode)
    {
        returnNode = null!;

        if (query.Length == 0)
            return false;

        string[] words = query.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (!nodeListing.Any(x => words.Contains(x.Key.ToLower())))
            return false;

        returnNode = nodeListing.FirstOrDefault(x => Misc.CompareStringsInvariant(x.Key, words[0])).Value;

        if (returnNode != null!)
            return true;

        returnNode = nodeListing.FirstOrDefault(t => words.Any(x => Misc.CompareStringsInvariant(x, t.Key))).Value;

        if (returnNode != null!)
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
                Loggers.LogDebug($"adding {shopNode.creatureName} to shipdecorselection");
            }
            else
            {
                Loggers.LogDebug($"{shopNode.creatureName} already in shipdecorselection");
            }
        }

        Loggers.LogDebug("nodes have been added");
    }

    public static string ClearText() //function used in terminalstuff clear command
    {
        string displayText = "\n";
        Loggers.LogDebug("display text cleared for real this time!!!");
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

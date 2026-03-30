// credit to iambatby's LethalLevelLoader for inspiration behind these methods
// https://github.com/IAmBatby/LethalLevelLoader/blob/main/LethalLevelLoader/Patches/TerminalManager.cs
// They have since been modified for use in this project

using OpenLib.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace OpenLib.CoreMethods;
public class BasicTerminal
{
    public static TerminalKeyword CreateNewTerminalKeyword(string name, string keyword, bool ReplaceExisting = false)
    {
        if (DynamicBools.TryGetKeyword(keyword, out TerminalKeyword existing))
        {
            if (ReplaceExisting)
                CheckForAndDeleteKeyWord(keyword);
            else
                return existing;
        }

        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
        TerminalKeyword newTerminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        newTerminalKeyword.name = name;

        newTerminalKeyword.word = keyword;
        newTerminalKeyword.isVerb = false;
        newTerminalKeyword.compatibleNouns = [];
        newTerminalKeyword.defaultVerb = null!;
        allKeywordsList.Add(newTerminalKeyword);

        // Needed for deletion tracking
        if (!Plugin.KeywordsAdded.Contains(newTerminalKeyword))
            Plugin.KeywordsAdded.Add(newTerminalKeyword);

        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        return (newTerminalKeyword);
    }

    public static TerminalNode CreateNewTerminalNode()
    {
        TerminalNode newTerminalNode = ScriptableObject.CreateInstance<TerminalNode>();
        newTerminalNode.name = "OpenLibTerminalNode";

        newTerminalNode.displayText = string.Empty;
        newTerminalNode.terminalEvent = string.Empty;
        newTerminalNode.maxCharactersToType = 25;
        newTerminalNode.buyItemIndex = -1;
        newTerminalNode.buyRerouteToMoon = -1;
        newTerminalNode.displayPlanetInfo = -1;
        newTerminalNode.shipUnlockableID = -1;
        newTerminalNode.creatureFileID = -1;
        newTerminalNode.storyLogFileID = -1;
        newTerminalNode.playSyncedClip = -1;
        newTerminalNode.terminalOptions = [];

        // Needed for deletion tracking
        if (!Plugin.NodesAdded.Contains(newTerminalNode))
            Plugin.NodesAdded.Add(newTerminalNode);

        return (newTerminalNode);
    }

    //simpler version for NodeConfirmation class
    public static CompatibleNoun CreateCompatibleNoun(string nodeName, string word, string displayText = "")
    {
        TerminalKeyword noun;
        TerminalNode result;
        if (Misc.CompareStringsInvariant(word, "deny") || Misc.CompareStringsInvariant(word, "confirm")) //catch confirmation words from being re-used
            noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);
        else if (DynamicBools.TryGetKeyword(word, out TerminalKeyword thisWord))
            noun = thisWord;
        else
            noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);


        result = CreateNewTerminalNode();
        result.name = nodeName + "_" + word;
        result.displayText = displayText;
        result.clearPreviousText = true;

        return new CompatibleNoun(noun, result);
    }

    public static CompatibleNoun CreateCompatibleNoun(string nodeName, string word, string displayText = "", int price = 0, Func<string> thisAction = null!, Dictionary<TerminalNode, Func<string>> nodeListing = null!)
    {
        TerminalKeyword noun;
        TerminalNode result;
        if (Misc.CompareStringsInvariant(word, "deny") || Misc.CompareStringsInvariant(word, "confirm")) //catch confirmation words from being re-used
            noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);
        else if (DynamicBools.TryGetKeyword(word, out TerminalKeyword thisWord))
            noun = thisWord;
        else
            noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);


        result = CreateNewTerminalNode();
        result.name = nodeName + "_" + word;
        result.displayText = displayText;
        result.clearPreviousText = true;
        result.itemCost = price;

        noun.specialKeywordResult = result;
        if (thisAction != null && nodeListing != null!)
            nodeListing.Add(result, thisAction);

        return new(noun, result);
    }

    public static void CheckForAndDeleteKeyWord(string keyWord)
    {
        Loggers.LogDebug($"Checking for {keyWord}");
        List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

        for (int i = keyWordList.Count - 1; i >= 0; i--)
        {
            if (keyWordList[i].word.Equals(keyWord))
            {
                Loggers.LogDebug($"removing {keyWordList[i].word}");
                keyWordList.RemoveAt(i);
                //Loggers.LogInfo($"Keyword: [{keyWord}] removed");
                break;
            }
        }

        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. keyWordList];
        //Loggers.LogDebug($"keyword list adjusted");
        return;
    }
}

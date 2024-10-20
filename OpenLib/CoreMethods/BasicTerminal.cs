// credit to iambatby's LethalLevelLoader for these methods
// https://github.com/IAmBatby/LethalLevelLoader/blob/main/LethalLevelLoader/Patches/TerminalManager.cs
// some minor modifications for use in this project
// if you dont know what you're doing with terminalkeywords/terminalnodes I recommend using the methods i've created in AddingThings.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenLib.CoreMethods
{
    public class BasicTerminal
    {
        public static TerminalKeyword CreateNewTerminalKeyword(string name, string keyword)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            TerminalKeyword newTerminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            newTerminalKeyword.name = name;

            newTerminalKeyword.word = keyword;
            newTerminalKeyword.compatibleNouns = [];
            newTerminalKeyword.defaultVerb = null;
            allKeywordsList.Add(newTerminalKeyword);

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

            return (newTerminalNode);
        }

        public static CompatibleNoun CreateCompatibleNoun(string nodeName, string word, string displayText = "", int price = 0, Func<string> thisAction = null, Dictionary<TerminalNode, Func<string>> nodeListing = null)
        {
            CompatibleNoun thisNoun = new();
            if(word.ToLower() == "deny" ||  word.ToLower() == "confirm") //catch confirmation words from being re-used
                thisNoun.noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);
            else if (DynamicBools.TryGetKeyword(word, out TerminalKeyword thisWord))
                thisNoun.noun = thisWord;
            else
                thisNoun.noun = CreateNewTerminalKeyword(nodeName + "_" + word, word);


            thisNoun.result = CreateNewTerminalNode();
            thisNoun.result.name = nodeName + "_" + word;
            thisNoun.result.displayText = displayText;
            thisNoun.result.clearPreviousText = true;
            thisNoun.result.itemCost = price;

            thisNoun.noun.specialKeywordResult = thisNoun.result;
            if (thisAction != null && nodeListing != null)
                nodeListing.Add(thisNoun.result, thisAction);

            return thisNoun;
        }
    }
}

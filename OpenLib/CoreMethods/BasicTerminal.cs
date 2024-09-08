// credit to iambatby's LethalLevelLoader for these methods
// https://github.com/IAmBatby/LethalLevelLoader/blob/main/LethalLevelLoader/Patches/TerminalManager.cs
// some minor modifications for use in this project

using System.Collections.Generic;
using UnityEngine;

namespace OpenLib.CoreMethods
{
    public class BasicTerminal
    {
        /*
        public static TerminalKeyword CreateNewTerminalKeyword(string name, string keyword)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            TerminalKeyword newTerminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            newTerminalKeyword.name = name;

            newTerminalKeyword.word = keyword;
            newTerminalKeyword.compatibleNouns = new CompatibleNoun[0];
            newTerminalKeyword.defaultVerb = null;

            Plugin.instance.Terminal.terminalNodes.allKeywords = Plugin.instance.Terminal.terminalNodes.allKeywords.AddItem(newTerminalKeyword).ToArray();

            return (newTerminalKeyword);
        }

        public static TerminalNode CreateNewTerminalNode()
        {
            TerminalNode newTerminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            newTerminalNode.name = "NewLethalLevelLoaderTerminalNode";

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
            newTerminalNode.terminalOptions = new CompatibleNoun[0];

            return (newTerminalNode);
        }
        */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLib.CoreMethods
{
    public class DynamicBools
    {
        public static bool UseMatchingNode(string nodeName, out TerminalNode returnNode)
        {
            TerminalNode[] allTerminalNodes = UnityEngine.Object.FindObjectsOfType<TerminalNode>();

            foreach (TerminalNode node in allTerminalNodes)
            {
                if (node.name.Equals(nodeName))
                {
                    returnNode = node;
                    Plugin.Spam($"Existing terminalNode [{nodeName}] found, using it rather than making a new one for this command");
                    return true;
                }
            }

            returnNode = null;
            return false;
        }

        public static bool TryGetKeyword(string keyWord)
        {
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] found!");
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetKeyword(string keyWord, out TerminalKeyword terminalKeyword)
        {
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    Plugin.Spam($"Keyword: [{keyWord}] found!");
                    terminalKeyword = keyword;
                    return true;
                }
            }

            terminalKeyword = null;
            return false;
        }

        public static bool TryGetAndReturnUnlockable(string unlockableName, out UnlockableItem itemOut)
        {
            List<UnlockableItem> unlockableList = [.. StartOfRound.Instance.unlockablesList.unlockables];
            foreach (UnlockableItem item in unlockableList)
            {
                if (item.unlockableName.Equals(unlockableName))
                {
                    itemOut = item;
                    return true;
                }
            }

            itemOut = null;
            return false;
        }

        public static bool TryGetAndReturnItem(string unlockableName, out Item itemOut)
        {
            List<Item> unlockableList = [.. Plugin.instance.Terminal.buyableItemsList];
            foreach (Item item in unlockableList)
            {
                if (item.itemName.Equals(unlockableName))
                {
                    itemOut = item;
                    return true;
                }
            }

            itemOut = null;
            return false;
        }

        public static bool IsCommandCreatedAlready(string keyWord, string displayText, List<TerminalKeyword> terminalKeywords)
        {
            foreach (TerminalKeyword terminalKeyword in terminalKeywords)
            {
                if (terminalKeyword.word == keyWord && terminalKeyword.specialKeywordResult.displayText == displayText)
                {
                    Plugin.Spam($"word: {keyWord} found with valid node: {terminalKeyword.specialKeywordResult.name}");
                    return true;
                }
            }

            return false;
        }

        public static bool IsCommandCreatedAlready(Dictionary<TerminalNode, Func<string>> MainCommandListing, string keyWord, Func<string> commandAction, List<TerminalKeyword> terminalKeywords)
        {
            if (MainCommandListing.Count == 0)
                return false;

            foreach (KeyValuePair<TerminalNode, Func<string>> entry in MainCommandListing)
            {
                if (entry.Value == commandAction)
                {
                    foreach (TerminalKeyword terminalKeyword in terminalKeywords)
                    {
                        if (terminalKeyword.word == keyWord && terminalKeyword.specialKeywordResult == entry.Key)
                        {
                            Plugin.Spam($"word: {keyWord} found with valid node: {terminalKeyword.specialKeywordResult.name}");
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        public static bool IsCommandCreatedAlready(Dictionary<TerminalNode, Func<string>> MainCommandListing, string keyWord, Func<string> commandAction, List<TerminalKeyword> terminalKeywords, out TerminalKeyword outKeyword)
        {
            outKeyword = null;
            if (MainCommandListing.Count == 0)
                return false;

            foreach (KeyValuePair<TerminalNode, Func<string>> entry in MainCommandListing)
            {
                if (entry.Value == commandAction)
                {
                    foreach (TerminalKeyword terminalKeyword in terminalKeywords)
                    {
                        if (terminalKeyword.word == keyWord && terminalKeyword.specialKeywordResult == entry.Key)
                        {
                            Plugin.Spam($"word: {keyWord} found with valid node: {terminalKeyword.specialKeywordResult.name}");
                            outKeyword = terminalKeyword;
                            return true;
                        }
                    }
                }
            }

            return false;

        }


        public static bool DoesNodeExist(Dictionary<TerminalNode, Func<string>> MainCommandListing, Func<string> commandAction, out TerminalNode node)
        {
            node = null;

            if (MainCommandListing.Count == 0)
                return false;

            foreach (KeyValuePair<TerminalNode, Func<string>> item in MainCommandListing)
            {
                if (item.Key == null)
                    continue;

                if (item.Value == commandAction)
                {
                    node = item.Key;
                    return true;
                }
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using static TerminalStuff.TerminalEvents;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using Object = UnityEngine.Object;
using UnityEngine;

namespace TerminalStuff.NoMoreAPI
{
    internal class CommandStuff
    {
        internal static bool GetNewDisplayText(ref TerminalNode node)
        {
            if (node == null)
                return false;

            if (node.name.Equals("0_StoreHub") && darmuhsStorePacks.Count > 0)
                GetDynamicCost();

            if (darmuhsTerminalStuff.TryGetValue(node, out Func<string> newDisplayText))
            {
                if(darmuhsStorePacks.TryGetValue(node, out string value))
                {
                    node.itemCost = 0;
                    Plugin.MoreLogs("Updating currentPackList");
                    CostCommands.currentPackList = value;
                    if(node.creatureName != string.Empty)
                        CostCommands.currentPackName = node.creatureName;
                }

                node.displayText = newDisplayText();

                return true;
            }
            else
            {
                Plugin.MoreLogs("Not in special nodeListing dictionary");
                return false;
            }

        }

        internal static void GetDynamicCost()
        {
            foreach(KeyValuePair<TerminalNode,string> item in darmuhsStorePacks)
            {
                if (!item.Key.name.Contains("_confirm"))
                {
                    int itemCost = CostCommands.GetItemListCost(item.Value);
                    item.Key.itemCost = itemCost;
                    Plugin.Spam($"Updating price for {item.Key.name} to {item.Key.itemCost}");
                }
            }
        }

        internal static bool DoesNodeExist(Func<string> action, out TerminalNode node)
        {
            node = null;

            if (darmuhsTerminalStuff.Count == 0)
                return false;

            foreach(KeyValuePair<TerminalNode, Func<string>> item in darmuhsTerminalStuff)
            {
                if(item.Value == action && item.Key != null)
                {
                    node = item.Key;
                    return true;
                }
            }

            return false;
        }

        internal static bool DoesNodeExist(string nodeName, out TerminalNode node)
        {
            node = null;

            if (darmuhsTerminalStuff.Count == 0)
                return false;

            foreach (KeyValuePair<TerminalNode, Func<string>> item in darmuhsTerminalStuff)
            {
                if (item.Key.name.ToLower() == nodeName)
                {
                    node = item.Key;
                    return true;
                }
            }

            return false;
        }

        internal static TerminalNode GetFromAllNodes(string nodeName)
        {
            List<TerminalNode> allNodes = new(Object.FindObjectsOfType<TerminalNode>(true));

            foreach (TerminalNode node in allNodes)
            {
                if (node.name == nodeName)
                {
                    Plugin.Spam($"{nodeName} found!");
                    return node;
                }
            }

            Plugin.Spam($"{nodeName} could not be found, result set to null.");

            return null;
        }

        public static List<TerminalNode> GetAllNodes()
        {
            List<TerminalNode> allPossibleNodes = [.. Resources.FindObjectsOfTypeAll<TerminalNode>()];
            return allPossibleNodes;
        }

        internal static bool IsCommandCreatedAlready(string keyWord, string displayText, List<TerminalKeyword> terminalKeywords)
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

        internal static bool IsCommandCreatedAlready(string keyWord, Func<string> commandAction, List<TerminalKeyword> terminalKeywords)
        {
            if (darmuhsTerminalStuff.Count == 0)
                return false;

            foreach(KeyValuePair<TerminalNode, Func<string>> entry in darmuhsTerminalStuff)
            {
                if(entry.Value == commandAction)
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

        internal static bool IsCommandCreatedAlready(string keyWord, Func<string> commandAction, List<TerminalKeyword> terminalKeywords, out TerminalKeyword outKeyword)
        {
            outKeyword = null;
            if (darmuhsTerminalStuff.Count == 0)
                return false;

            foreach (KeyValuePair<TerminalNode, Func<string>> entry in darmuhsTerminalStuff)
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
    }
}

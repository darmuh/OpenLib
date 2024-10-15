using OpenLib.Common;
using OpenLib.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenLib.CoreMethods
{
    public class LogicHandling
    {
        public static bool GetNewDisplayText(MainListing providedListing, ref TerminalNode node)
        {
            if (node == null)
            {
                Plugin.ERROR("NODE IS NULL @GetNewDisplayText");
                return false;
            }

            if (providedListing.Listing.Count == 0)
                return false;

            Dictionary<TerminalNode, Func<string>> CommandDictionary = providedListing.Listing;

            if (CommandDictionary.Count < 1)
                return false;

            Plugin.Spam("command dictionary is not null in provided listing");

            EventManager.GetNewDisplayText.Invoke(node);
            //create event to subscribe to and perform other actions
            //like terminalstuff doing dynamic cost analysis for the store node

            if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
            {
                CommonTerminal.parseNode = node; // set this static node for usage elsewhere
                Plugin.Spam($"Func<string> found for {node.name}");
                node.displayText = newDisplayText();
                return true;
            }
            else
            {
                Plugin.Spam("Not in special nodeListing dictionary");
                return false;
            }
        }

        public static bool GetNewDisplayText(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
        {
            if (node == null)
            {
                Plugin.ERROR("NODE IS NULL @GetNewDisplayText");
                return false;
            }

            if (providedListing.Count == 0)
                return false;

            bool funcFound = false;
            int looptimes = 0;

            foreach (MainListing listing in providedListing)
            {
                if (listing.Listing.Count < 1)
                    continue;
                Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

                looptimes++;
                Plugin.Spam($"command dictionary in this listing is not empty ({looptimes})");

                EventManager.GetNewDisplayText.Invoke(node);
                //create event to subscribe to and perform other actions
                //like terminalstuff doing dynamic cost analysis for the store node

                if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
                {
                    CommonTerminal.parseNode = node; // set this static node for usage elsewhere
                    Plugin.MoreLogs($"Func<string> found for {node.name} in one of provided listings");
                    node.displayText = newDisplayText();
                    funcFound = true;
                    break;
                }
                else
                {
                    Plugin.Spam("Not in special nodeListing dictionary");
                    continue;
                }
            }

            Plugin.MoreLogs("provided listings do not contain this node");
            return funcFound;
        }

        public static bool GetDisplayFromFaux(List<FauxKeyword> fauxWords, string words, ref TerminalNode node)
        {
            Plugin.Spam($"GetDisplayFromFaux {words}");
            foreach (FauxKeyword keyword in fauxWords)
            {
                if (keyword.ResultFunc == null || keyword.Keyword == null || keyword.MainPage == null)
                    continue;

                keyword.thisNode.displayText = "";

                if (words.StartsWith(keyword.Keyword.Substring(0, 4), true, null) && Plugin.instance.Terminal.currentNode == keyword.MainPage)
                {
                    if (keyword.ConfirmFunc != null && !keyword.GetConfirm)
                        keyword.GetConfirm = true;
                    keyword.thisNode.displayText = keyword.ResultFunc();
                    node = keyword.thisNode;
                    return true;
                }
                else if (Plugin.instance.Terminal.currentNode == keyword.thisNode && keyword.GetConfirm)
                {
                    if (words.StartsWith("c", false, null))
                    {
                        keyword.thisNode.displayText = keyword.ConfirmFunc();
                        node = keyword.thisNode;
                        keyword.GetConfirm = false;
                    }

                    else if (words.StartsWith("d", false, null))
                    {
                        keyword.thisNode.displayText = keyword.DenyFunc();
                        node = keyword.thisNode;
                        keyword.GetConfirm = false;
                    }
                    else if (words.StartsWith(keyword.Keyword, false, null))
                    {
                        keyword.thisNode.displayText = keyword.ResultFunc();
                        node = keyword.thisNode;
                    }
                    else
                        return false;

                    return true;
                }
            }

            return false;
        }

        public static bool TryGetFuncFromNode(List<MainListing> providedListing, ref TerminalNode node, out Func<string> returnFunc) //overload for multiple listings (terminalstuff)
        {
            if (node == null || providedListing.Count == 0)
            {
                Plugin.WARNING("node is null or listings do not exist");
                returnFunc = null!;
                return false;
            }

            int looptimes = 0;

            foreach (MainListing listing in providedListing)
            {
                Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

                if (CommandDictionary.Count < 1)
                    continue;

                looptimes++;
                Plugin.Spam($"command dictionary in this listing is not null ({looptimes})");

                if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
                {
                    Plugin.MoreLogs($"Func<string> found for {node.name} in one of provided listings");
                    returnFunc = newDisplayText;
                    return true;
                }
                else
                {
                    Plugin.Spam("Not in this special nodeListing dictionary");
                    continue;
                }
            }

            Plugin.MoreLogs("provided listings do not contain this node");
            returnFunc = null!;
            return false;
        }

        public static bool TryGetFromAllNodes(string nodeName, out TerminalNode outNode)
        {
            List<TerminalNode> allNodes = GetAllNodes();
            outNode = null;

            foreach (TerminalNode node in allNodes)
            {
                if (node == null)
                    continue;

                if (node.name == nodeName)
                {
                    Plugin.Spam($"{nodeName} found!");
                    outNode = node;
                    return true;
                }
            }

            Plugin.Spam($"{nodeName} could not be found, result set to null.");
            return false;
        }

        public static List<TerminalNode> GetAllNodes()
        {
            List<TerminalNode> allPossibleNodes = [.. Resources.FindObjectsOfTypeAll<TerminalNode>()];
            return allPossibleNodes;
        }

        public static void SetTerminalInput(string terminalInput)
        {
            Plugin.instance.Terminal.TextChanged(Plugin.instance.Terminal.currentText.Substring(0, Plugin.instance.Terminal.currentText.Length - Plugin.instance.Terminal.textAdded) + terminalInput);
            Plugin.instance.Terminal.screenText.text = Plugin.instance.Terminal.currentText;
            Plugin.instance.Terminal.textAdded = terminalInput.Length;
        }


        //Obsolete & Old Stuff that cannot be deleted
        [Obsolete("Use TryGetFuncFromNode instead to avoid getting NULL funcs")]
        public static Func<string> GetFuncFromNode(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
        {
            if (node == null || providedListing.Count == 0)
            {
                Plugin.WARNING("node is null or listings do not exist");
                return null!;
            }

            int looptimes = 0;

            foreach (MainListing listing in providedListing)
            {
                Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

                if (CommandDictionary.Count < 1)
                    continue;

                looptimes++;
                Plugin.Spam($"command dictionary in this listing is not null ({looptimes})");

                if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
                {
                    Plugin.MoreLogs($"Func<string> found for {node.name} in one of provided listings");
                    return newDisplayText;
                }
                else
                {
                    Plugin.Spam("Not in this special nodeListing dictionary");
                    continue;
                }
            }

            Plugin.MoreLogs("provided listings do not contain this node");
            return null;
        }

        [Obsolete("Use TryGetFromAllNodes instead to avoid getting NULL funcs")]
        public static TerminalNode GetFromAllNodes(string nodeName)
        {
            List<TerminalNode> allNodes = GetAllNodes();

            foreach (TerminalNode node in allNodes)
            {
                if (node == null)
                    continue;

                if (node.name == nodeName)
                {
                    Plugin.Spam($"{nodeName} found!");
                    return node;
                }
            }

            Plugin.Spam($"{nodeName} could not be found, result set to null.");

            return null;
        }
    }
}

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
            if (node == null || providedListing.Listing.Count == 0)
            {
                Plugin.Spam("node is null or listing is 0");
                return false;
            }

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
                Plugin.MoreLogs("Not in special nodeListing dictionary");
                return false;
            }
        }

        public static bool GetNewDisplayText(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
        {
            if (node == null || providedListing.Count == 0)
            {
                Plugin.Spam("node is null or listings do not exist");
                return false;
            }

            bool funcFound = false;
            int looptimes = 0;

            foreach(MainListing listing in providedListing)
            {
                Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

                if (CommandDictionary.Count < 1)
                    continue;
                looptimes++;
                Plugin.Spam($"command dictionary in this listing is not null ({looptimes})");

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

        public static Func<string> GetFuncFromNode(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
        {
            if (node == null || providedListing.Count == 0)
            {
                Plugin.Spam("node is null or listings do not exist");
                return null;
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


        public static TerminalNode GetFromAllNodes(string nodeName)
        {
            List<TerminalNode> allNodes = GetAllNodes();

            foreach (TerminalNode node in allNodes)
            {
                if(node == null)
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
    }
}

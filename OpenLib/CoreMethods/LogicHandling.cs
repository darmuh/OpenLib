using OpenLib.Common;
using OpenLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenLib.CoreMethods;
public class LogicHandling
{
    public static bool GetNewDisplayText(MainListing providedListing, ref TerminalNode node)
    {

        if (providedListing.Listing.Count == 0)
            return false;

        Dictionary<TerminalNode, Func<string>> CommandDictionary = providedListing.Listing;

        if (CommandDictionary.Count < 1)
            return false;

        Loggers.LogDebug("command dictionary is not null in provided listing");

        if (node == null!)
            return false;



        if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
        {
            NewDisplayTextEventInvoke(ref node);

            CommonTerminal.parseNode = node; // set this static node for usage elsewhere
            Loggers.LogDebug($"Func<string> found for {node.name}");
            node.displayText = newDisplayText();
            return true;
        }
        else
        {
            Loggers.LogDebug("Not in special nodeListing dictionary");
            return false;
        }
    }

    public static bool GetNewDisplayText2(ref TerminalNode node)
    {
        if (node == null || Plugin.AllCommands.Count == 0)
            return false;

        List<CommandManager> activeCommands = Plugin.AllCommands.FindAll(x => x.IsCreated);

        TerminalNode current = node;
        CommandManager match = activeCommands.FirstOrDefault(f => f.TerminalNode == current);
        if (match != null!)
        {
            NewDisplayTextEventInvoke(ref node);
            node.displayText = match.MainAction();
            return true;
        }

        CommandManager info = activeCommands.FirstOrDefault(i => i.InfoBase.InfoAction != null && i.InfoBase.terminalNode == current);
        if (info != null!)
        {
            NewDisplayTextEventInvoke(ref node);
            node.displayText = info.InfoBase.InfoAction();
            return true;
        }

        Loggers.LogDebug("No matches in GetNewDisplayText2");
        return false;
    }

    private static void NewDisplayTextEventInvoke(ref TerminalNode node)
    {
        if (EventManager.GetNewDisplayText.Listeners > 0) //only invoke if someone is listening to this event
            node = EventManager.GetNewDisplayText.NodeInvoke(ref node); //updated to ref node in order to change/cancel the node at parse
                                                                        //create event to subscribe to and perform other actions
                                                                        //like terminalstuff doing dynamic cost analysis for the store node
    }

    public static bool GetNewDisplayText(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
    {

        if (providedListing.Count == 0)
            return false;

        if (node == null!)
            return false;

        bool funcFound = false;
        int looptimes = 0;

        foreach (MainListing listing in providedListing)
        {
            if (listing.Listing.Count < 1)
                continue;
            Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

            looptimes++;
            Loggers.LogDebug($"command dictionary in this listing is not empty ({looptimes})");



            if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
            {
                NewDisplayTextEventInvoke(ref node);

                CommonTerminal.parseNode = node; // set this static node for usage elsewhere
                Loggers.LogInfo($"Func<string> found for {node.name} in one of provided listings");
                node.displayText = newDisplayText();
                funcFound = true;
                break;
            }
            else
            {
                Loggers.LogDebug("Not in special nodeListing dictionary");
                continue;
            }
        }

        Loggers.LogInfo("provided listings do not contain this node");
        return funcFound;
    }

    public static bool GetDisplayFromFaux(List<FauxKeyword> fauxWords, string words, ref TerminalNode node)
    {
        //Loggers.LogDebug($"GetDisplayFromFaux {words}");
        string firstWord = words.Split(' ')[0];
        foreach (FauxKeyword keyword in fauxWords)
        {
            if (keyword.ResultFunc == null || keyword.Keyword == null || keyword.MainPage == null!)
                continue;

            if(keyword.thisNode != null)
                keyword.thisNode.displayText = "";

            if (Misc.StringStartsWithInvariant(words, keyword.Keyword[..3]) && Plugin.instance.Terminal.currentNode == keyword.MainPage)
            {
                if (keyword.requireExact && Misc.CompareStringsInvariant(words, keyword.Keyword))
                    return false;

                Loggers.LogDebug("Using faux word associated with MainPage");

                if (keyword.ConfirmFunc != null && keyword.GetConfirm is false)
                    keyword.GetConfirm = true;
                if (keyword.thisNode != null)
                    keyword.thisNode.displayText = keyword.ResultFunc();
                node = keyword.thisNode!;
                return true;
            }
            else if (Plugin.instance.Terminal.currentNode == keyword.thisNode && keyword.GetConfirm is true)
            {
                Loggers.LogDebug("getting confirmation for this faux word");

                if (Misc.StringStartsWithInvariant(words, 'c'))
                {
                    if(keyword.ConfirmFunc != null)
                        keyword.thisNode.displayText = keyword.ConfirmFunc();
                    node = keyword.thisNode;
                    keyword.GetConfirm = false;
                }

                else if (Misc.StringStartsWithInvariant(words, 'd'))
                {
                    if (keyword.DenyFunc != null)
                        keyword.thisNode.displayText = keyword.DenyFunc();
                    node = keyword.thisNode;
                    keyword.GetConfirm = false;
                }
                else if (Misc.StringStartsWithInvariant(words, keyword.Keyword))
                {
                    keyword.thisNode.displayText = keyword.ResultFunc();
                    node = keyword.thisNode;
                }
                else
                    return false;

                return true;
            }
            else if (Misc.StringStartsWithInvariant(words, keyword.Keyword[..3]) && fauxWords.Any(f => f.thisNode == Plugin.instance.Terminal.currentNode && f.AllowOtherFauxWords))
            {
                if (keyword.requireExact && Misc.CompareStringsInvariant(words, keyword.Keyword))
                    return false;

                Loggers.LogDebug("Using faux word that can be called from other fauxwords");

                if (keyword.ConfirmFunc != null && keyword.GetConfirm is false)
                    keyword.GetConfirm = true;
                if (keyword.thisNode != null)
                    keyword.thisNode.displayText = keyword.ResultFunc();
                node = keyword.thisNode!;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetFuncFromNode(List<MainListing> providedListing, ref TerminalNode node, out Func<string> returnFunc) //overload for multiple listings (terminalstuff)
    {
        if (node == null || providedListing.Count == 0)
        {
            Loggers.WARNING("node is null or listings do not exist");
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
            Loggers.LogDebug($"command dictionary in this listing is not null ({looptimes})");

            if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
            {
                Loggers.LogInfo($"Func<string> found for {node.name} in one of provided listings");
                returnFunc = newDisplayText;
                return true;
            }
            else
            {
                Loggers.LogDebug("Not in this special nodeListing dictionary");
                continue;
            }
        }

        Loggers.LogInfo("provided listings do not contain this node");
        returnFunc = null!;
        return false;
    }

    public static bool TryGetFromAllNodes(string nodeName, out TerminalNode outNode)
    {
        List<TerminalNode> allNodes = GetAllNodes();
        outNode = null!;

        foreach (TerminalNode node in allNodes)
        {
            if (node == null!)
                continue;

            if (node.name == nodeName)
            {
                Loggers.LogDebug($"{nodeName} found!");
                outNode = node;
                return true;
            }
        }

        Loggers.LogDebug($"{nodeName} could not be found, result set to null.");
        return false;
    }

    public static List<TerminalNode> GetAllNodes()
    {
        List<TerminalNode> allPossibleNodes = [.. Resources.FindObjectsOfTypeAll<TerminalNode>()];
        return allPossibleNodes;
    }

    public static void SetTerminalInput(string terminalInput)
    {
        Plugin.instance.Terminal.TextChanged(Plugin.instance.Terminal.currentText[..^Plugin.instance.Terminal.textAdded] + terminalInput);
        Plugin.instance.Terminal.screenText.text = Plugin.instance.Terminal.currentText;
        Plugin.instance.Terminal.textAdded = terminalInput.Length;
    }


    //Obsolete & Old Stuff that cannot be deleted
    [Obsolete("Use TryGetFuncFromNode instead to avoid getting NULL funcs")]
    public static Func<string> GetFuncFromNode(List<MainListing> providedListing, ref TerminalNode node) //overload for multiple listings (terminalstuff)
    {
        if (node == null || providedListing.Count == 0)
        {
            Loggers.WARNING("node is null or listings do not exist");
            return null!;
        }

        int looptimes = 0;

        foreach (MainListing listing in providedListing)
        {
            Dictionary<TerminalNode, Func<string>> CommandDictionary = listing.Listing;

            if (CommandDictionary.Count < 1)
                continue;

            looptimes++;
            Loggers.LogDebug($"command dictionary in this listing is not null ({looptimes})");

            if (CommandDictionary.TryGetValue(node, out Func<string> newDisplayText))
            {
                Loggers.LogInfo($"Func<string> found for {node.name} in one of provided listings");
                return newDisplayText;
            }
            else
            {
                Loggers.LogDebug("Not in this special nodeListing dictionary");
                continue;
            }
        }

        Loggers.LogInfo("provided listings do not contain this node");
        return null!;
    }

    [Obsolete("Use TryGetFromAllNodes instead to avoid getting NULL funcs")]
    public static TerminalNode GetFromAllNodes(string nodeName)
    {
        List<TerminalNode> allNodes = GetAllNodes();

        foreach (TerminalNode node in allNodes)
        {
            if (node == null!)
                continue;

            if (node.name == nodeName)
            {
                Loggers.LogDebug($"{nodeName} found!");
                return node;
            }
        }

        Loggers.LogDebug($"{nodeName} could not be found, result set to null.");

        return null!;
    }
}

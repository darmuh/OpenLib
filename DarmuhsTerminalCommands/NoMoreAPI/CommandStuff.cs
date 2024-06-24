using System;
using System.Collections.Generic;
using static TerminalStuff.TerminalEvents;
using Object = UnityEngine.Object;

namespace TerminalStuff.NoMoreAPI
{
    internal class CommandStuff
    {
        internal static bool GetNewDisplayText(ref TerminalNode node)
        {
            if (node == null)
                return false;

            if (darmuhsTerminalStuff.TryGetValue(node, out Func<string> newDisplayText))
            {
                node.displayText = newDisplayText();
                return true;
            }
            else
            {
                Plugin.MoreLogs("Not in special nodeListing dictionary");
                return false;
            }

        }

        internal static TerminalNode GetFromAllNodes(string nodeName)
        {
            List<TerminalNode> allNodes = new(Object.FindObjectsOfType<TerminalNode>(true));

            foreach (TerminalNode node in allNodes)
            {
                if (node.name == nodeName)
                {
                    Plugin.MoreLogs($"{nodeName} found!");
                    return node;
                }
            }

            Plugin.MoreLogs($"{nodeName} could not be found, result set to null.");

            return null;
        }
    }
}

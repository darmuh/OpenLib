using System.Collections.Generic;
using UnityEngine;
using static OpenLib.CoreMethods.DynamicBools;

namespace OpenLib.CoreMethods
{
    public class CommonThings
    {
        public static void CheckForAndDeleteKeyWord(string keyWord)
        {
            Plugin.Spam($"Checking for {keyWord}");
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            for (int i = keyWordList.Count - 1; i >= 0; i--)
            {
                if (keyWordList[i].word.Equals(keyWord))
                {
                    keyWordList.RemoveAt(i);
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] removed");
                    break;
                }
            }

            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. keyWordList];
            Plugin.Spam($"keyword list adjusted, removed {keyWord}");
            return;
        }

        public static TerminalNode CreateDummyNode(string nodeName, bool clearPrevious, string displayText)
        {
            if (UseMatchingNode(nodeName, out TerminalNode terminalNode))
            {
                terminalNode.displayText = displayText;
                terminalNode.clearPreviousText = clearPrevious;
            }
            else
            {
                terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
                terminalNode.name = nodeName;
                terminalNode.displayText = displayText;
                terminalNode.clearPreviousText = clearPrevious;
            }

            return terminalNode;
        }


    }
}

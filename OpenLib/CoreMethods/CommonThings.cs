using System.Collections.Generic;

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
                    Plugin.Spam($"removing {keyWordList[i].word}");
                    keyWordList.RemoveAt(i);
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] removed");
                    break;
                }
            }

            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. keyWordList];
            //Plugin.Spam($"keyword list adjusted");
            return;
        }


    }
}

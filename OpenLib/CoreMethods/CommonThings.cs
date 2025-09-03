using System.Collections.Generic;

namespace OpenLib.CoreMethods
{
    public class CommonThings
    {
        public static void CheckForAndDeleteKeyWord(string keyWord)
        {
            Loggers.LogDebug($"Checking for {keyWord}");
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            for (int i = keyWordList.Count - 1; i >= 0; i--)
            {
                if (keyWordList[i].word.Equals(keyWord))
                {
                    Loggers.LogDebug($"removing {keyWordList[i].word}");
                    keyWordList.RemoveAt(i);
                    //Loggers.LogInfo($"Keyword: [{keyWord}] removed");
                    break;
                }
            }

            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. keyWordList];
            //Loggers.LogDebug($"keyword list adjusted");
            return;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalStuff
{
    internal class AutoComplete
    {
        internal static int AutoCompleteIndex = -1;
        internal static List<string> AutoCompleteResults = [];
        internal static List<string> GetMatchingKeywords(string input)
        {
            List<string> matching = [];
            matching.Add(input);
            Plugin.Spam($"added {input} to automcomplete list");
            foreach(TerminalKeyword word in Plugin.instance.Terminal.terminalNodes.allKeywords)
            {
                if (word.word.ToLower().Contains(input))
                {
                    matching.Add(word.word);
                    Plugin.Spam($"adding matching word: {word.word} to autocomplete list");
                }
            }

            return matching;
        }

        internal static bool CheckCurrentInput(List<string> matchList, string input)
        {
            if (!matchList.Contains(input))
            {
                AutoCompleteIndex = -1;
                return false;
            }
                
            else
                return true;
        }

        internal static string ShowMatchingKeywords(List<string> matchList, ref int currentIndex)
        {
            Plugin.Spam($"Matching Words: {matchList.Count}");
            if(matchList.Count < 1)
            {
                Plugin.Spam("matchList count is 0");
                currentIndex = 0;
                return string.Empty;
            }
            else if(matchList.Count > ConfigSettings.TerminalAutoCompleteMaxCount.Value)
            {
                Plugin.Spam("matchList count is too high");
                currentIndex = -1;
                return matchList[0].ToString();
            }

            if (currentIndex == -1)
                currentIndex = 1;
            
            if(currentIndex > matchList.Count -1)
            {
                Plugin.Spam("setting autocompleteindex to 0");
                currentIndex = -1;
                return matchList[0].ToString();
            }

            string command = matchList[currentIndex].ToString();
            currentIndex++;
            return command;

        }
    }
}

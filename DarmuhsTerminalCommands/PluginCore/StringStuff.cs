using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static TerminalStuff.MenuBuild;

namespace TerminalStuff
{
    internal class StringStuff
    {
        internal static string GetNextPage(List<string> categoryItems, string categoryTitle, int pageSize, int currentPage)
        {
            // Ensure currentPage is within valid range
            currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)categoryItems.Count / pageSize));

            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, categoryItems.Count);
            int totalItems = 0;
            int emptySpace;
            StringBuilder message = new();

            message.Append($"============ All [{categoryTitle}] Commands  ============");
            message.Append("\r\n");

            // Iterate through each item in the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                string menuItem = categoryItems[i];
                message.Append(menuItem + "\r\n");
                totalItems++;
            }

            emptySpace = pageSize - totalItems;

            for (int i = 0; i < emptySpace; i++)
            {
                message.Append("\r\n");
                //add empty space to keep menu shape
            }

            // Display pagination information
            //Page [LeftArrow] < 6/10 > [RightArrow]
            message.Append("\r\n");
            message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)categoryItems.Count / pageSize)}\r\n");

            message.Append($"Type next to see the next page of [{categoryTitle}] commands!\r\n");
            return message.ToString();
        }

        internal static List<string> GetListFromCat(string cat)
        {
            if (cat == "Comfort")
                return comfortEnabledCommands;
            else if (cat == "Fun")
                return funEnabledCommands;
            else if (cat == "Controls")
                return controlsEnabledCommands;
            else
                return extrasEnabledCommands;
        }

        internal static string[] GetWords()
        {
            string cleanedText = Plugin.instance.Terminal.screenText.text.Substring(Plugin.instance.Terminal.screenText.text.Length - Plugin.instance.Terminal.textAdded);
            string[] words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        internal static string[] GetWordsAndKeyword(List<string> configItemWords, string[] words)
        {
            List<string> filteredWords = [];
            bool keywordFound = false;

            foreach (string word in words)
            {
                Plugin.MoreLogs($"checking {word}");
                foreach (string keyword in configItemWords)
                {
                    if (keyword.Contains(word))
                    {
                        filteredWords.Add(keyword);
                        keywordFound = true;
                        Plugin.MoreLogs($"adding {keyword} to list");
                        break;
                    }
                }

                if (!keywordFound)
                {
                    filteredWords.Add(word);
                    Plugin.MoreLogs($"adding non-keyword, word: {word}");
                }

            }

            return [.. filteredWords];
        }

        internal static List<string> GetKeywordsPerConfigItem(string configItem)
        {
            List<string> keywordsInConfig = [];
            if (configItem.Length > 0)
            {
                keywordsInConfig = configItem.Split(';')
                                      .Select(item => item.TrimStart())
                                      .ToList();
                //Plugin.MoreLogs("GetKeywordsPerConfigItem split complete");
            }
                
            return keywordsInConfig;
        }

        internal static List<int> GetNumberListFromStringList(List<string> stringList)
        {
            List<int> numbersList = [];
            foreach (string item in stringList)
            {
                if (int.TryParse(item, out int number))
                {
                    numbersList.Add(number);
                }
                else
                    Plugin.ERROR($"Could not parse {item} to integer");
            }

            return numbersList;
        }

        internal static List<string> GetItemList(string rawList)
        {
            List<string> itemList = [];
            if (rawList.Length > 0)
            {
                itemList = rawList.Split(',')
                                      .Select(item => item.TrimStart())
                                      .ToList();
            }
            
            return itemList;
        }

        internal static Dictionary<string,string> GetKeywordAndItemNames(string configItem)
        {
            Dictionary<string, string> KeywordAndNames = [];
            if (configItem.Length > 0)
            {
                 KeywordAndNames = configItem
                    .Split(';')
                    .Select(item => item.Trim())
                    .Select(item => item.Split(':'))
                    .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
            }
             
            return KeywordAndNames;
        }

        internal static List<string> GetListToLower(List<string> stringList)
        {
            List<string> itemsToLower = [];

            foreach(string item in stringList)
            {
                itemsToLower.Add(item.ToLower());
            }
            return itemsToLower;
        }

        internal static string GetKeywordsForMenuItem(List<string> itemKeywords)
        {
            StringBuilder menuItem = new();
            foreach (string key in itemKeywords)
            {
                menuItem.Append($"{key}, ");
            }
            string finalList = menuItem.ToString();
            string listFixed = finalList.Remove(finalList.Length - 2);
            return listFixed;
        }
    }
}

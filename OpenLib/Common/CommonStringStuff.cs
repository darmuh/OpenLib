using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenLib.Common
{
    public class CommonStringStuff
    {
        public static string GetNextPage(List<string> categoryItems, string categoryTitle, int pageSize, int currentPage, out bool isNextEnabled) //used to return menus from terminalstuff
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

            message.Append("\r\n");
            message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)categoryItems.Count / pageSize)}\r\n");

            if (endIndex < categoryItems.Count)
            {
                message.Append($"Type next to see the next page of [{categoryTitle}] commands!\r\n");
                isNextEnabled = true;
            }
            else
                isNextEnabled = false;
                
            return message.ToString();
        }

        public static string[] GetWords() //get a word list from terminal input
        {
            string cleanedText = Plugin.instance.Terminal.screenText.text.Substring(Plugin.instance.Terminal.screenText.text.Length - Plugin.instance.Terminal.textAdded);
            string[] words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        public static string[] GetWordsAndKeyword(List<string> configItemWords, string[] words) //used with tp command in TerminalStuff
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

        public static List<string> GetKeywordsPerConfigItem(string configItem) //config item separated by semicolon only
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

        public static List<int> GetNumberListFromStringList(List<string> stringList) //return list of numbers from list of strings
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

        public static List<string> GetItemList(string rawList) //return list from raw string separated by comma
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

        public static List<string> GetListToLower(List<string> stringList) //remove punctuation from list<string>
        {
            List<string> itemsToLower = [];

            foreach(string item in stringList)
            {
                itemsToLower.Add(item.ToLower());
            }
            return itemsToLower;
        }



        public static string GetKeywordsForMenuItem(List<string> itemKeywords) //return a single string separated by commas
        {
            StringBuilder menuItem = new();
            foreach (string key in itemKeywords)
            {
                menuItem.Append($"{key}, ");
            }
            string finalList = menuItem.ToString();
            string listFixed = finalList.Remove(finalList.Length - 2);
            return listFixed; //used in terminalstuff menus setup
        }

        public static string GetCleanedScreenText(Terminal __instance) //copied from vanilla game, useful to get terminal friendly output
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);

            return RemovePunctuation(s);
        }

        public static string RemovePunctuation(string s) //copied from game files, same as above
        {
            StringBuilder stringBuilder = new();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }
    }
}

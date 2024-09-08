using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using System.Linq;
using static OpenLib.CoreMethods.DynamicBools;

namespace OpenLib.CoreMethods
{
    public class RemoveThings
    {
        public static void OnTerminalDisable()
        {
            Plugin.Spam("OnTerminalDisable called");
            DeleteAllNodes(ref Plugin.nodesAdded);
            DeleteAllKeywords(ref Plugin.keywordsAdded);
            ConfigSetup.defaultListing.DeleteAll();
        }

        public static void DeleteNounWord(ref TerminalKeyword keyWord, string terminalKeyword)
        {

            List<CompatibleNoun> keywordList = [.. keyWord.compatibleNouns];
            List<CompatibleNoun> nounsToRemove = [];
            foreach (CompatibleNoun compatibleNoun in keywordList)
            {
                if(compatibleNoun.noun.word.ToLower() == terminalKeyword.ToLower())
                {
                    nounsToRemove.Add(compatibleNoun);
                }
            }

            for (int i = nounsToRemove.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting noun: {nounsToRemove[i].noun.word} from word: {keyWord.word}");
                UnityEngine.Object.Destroy(nounsToRemove[i].noun);
            }

            keyWord.compatibleNouns = [.. keywordList];
        }

        public static void RemoveCompatibleNoun(ref TerminalKeyword mainWord, TerminalKeyword wordToRemove)
        {
            bool removedWord = false;

            if(mainWord.compatibleNouns != null)
            {
                List<CompatibleNoun> newList = [];
                foreach(CompatibleNoun noun in mainWord.compatibleNouns)
                {
                    if (noun.noun != wordToRemove)
                        newList.Add(noun);
                    else
                    {
                        removedWord = true;
                        Plugin.Spam($"Removing {wordToRemove.word}");
                    }
                }
                mainWord.compatibleNouns = [.. newList];
                Plugin.Spam($"DeleteCompatibleNoun of {wordToRemove.word} from {mainWord.word} complete, word removed: {removedWord}");
            }
        }

        public static void RemoveCompatibleNoun(ref TerminalKeyword mainWord, string nounToRemove)
        {
            bool removedWord = false;

            if (mainWord.compatibleNouns != null)
            {
                List<CompatibleNoun> newList = [];
                foreach (CompatibleNoun noun in mainWord.compatibleNouns)
                {
                    if (noun.noun.word.ToLower() != nounToRemove.ToLower())
                        newList.Add(noun);
                    else
                    {
                        //noun.noun.defaultVerb =;
                        removedWord = true;
                        Plugin.Spam($"Removing {nounToRemove}");
                    }
                }
                mainWord.compatibleNouns = [.. newList];
                Plugin.Spam($"DeleteCompatibleNoun of {nounToRemove} from {mainWord.word} complete, word removed: {removedWord}");
            }
        }

        public static void DeleteAllKeywords(ref List<TerminalKeyword> keywordList)
        {
            if (keywordList.Count == 0)
                return;

            List<TerminalKeyword> wordsToDelete = keywordList;
            List<TerminalKeyword> allKeywords = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keywordList)
            {
                if (TryGetKeyword("buy", out TerminalKeyword buyKeyword))
                {
                    DeleteNounWord(ref buyKeyword, keyword.word);
                }

                for (int i = allKeywords.Count - 1; i >= 0; i--)
                {
                    if (allKeywords[i] == keyword)
                    {
                        Plugin.Spam($"Removing {keyword.word} from all keywords list");
                        allKeywords.RemoveAt(i);
                        break;
                    }     
                }
            }

            for (int i = wordsToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting keyword Object: {wordsToDelete[i].word}");
                UnityEngine.Object.Destroy(wordsToDelete[i]);
            }

            keywordList.Clear();
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywords];
        }

        public static void DeleteAllNodes(ref Dictionary<TerminalNode, int> nodeDictionary) //viewtermnodes overload
        {
            List<TerminalNode> nodesToDelete = [];

            foreach (KeyValuePair<TerminalNode, int> item in nodeDictionary)
            {
                nodesToDelete.Add(item.Key);
            }

            nodeDictionary.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }

        public static void DeleteAllNodes(ref Dictionary<TerminalNode, Func<string>> nodeDictionary) //all nodes
        {
            if (nodeDictionary.Count == 0)
                return;

            List<TerminalNode> nodesToDelete = [];

            foreach (KeyValuePair<TerminalNode, Func<string>> item in nodeDictionary)
            {
                nodesToDelete.Add(item.Key);
            }

            nodeDictionary.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }

        public static void DeleteAllNodes(ref List<TerminalNode> nodeList) //all nodes in list
        {
            if (nodeList.Count == 0)
                return;

            List<TerminalNode> nodesToDelete = [];

            foreach (TerminalNode item in nodeList)
            {
                nodesToDelete.Add(item);
            }

            nodeList.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }

        public static void DeleteMatchingNode(string nodeName)
        {
            List<TerminalNode> allNodesList = LogicHandling.GetAllNodes();

            for (int i = allNodesList.Count - 1; i >= 0; i--)
            {
                if (allNodesList[i].name.Equals(nodeName))
                {
                    UnityEngine.Object.Destroy(allNodesList[i]);
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] removed");
                    break;
                }
            }
        }

        public static bool TryGetAndDeleteUnlockableName(string unlockableName, out int indexPos) //unused
        {
            List<UnlockableItem> unlockableList = [.. StartOfRound.Instance.unlockablesList.unlockables];

            for (int i = unlockableList.Count - 1; i >= 0; i--)
            {
                if (unlockableList[i].unlockableName.Equals(unlockableName))
                {
                    Plugin.Spam($"Unlockable: [{unlockableName}] found! Removing unlockable and noting index position");
                    StartOfRound.Instance.unlockablesList.unlockables.Remove(unlockableList[i]);
                    indexPos = i;
                    return true;
                }
            }
            indexPos = -1;
            return false;
        }
    }
}

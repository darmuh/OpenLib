using OpenLib.Common;
using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenLib.CoreMethods
{
    public class RemoveThings
    {
        public static void OnTerminalDisable()
        {
            Plugin.Spam("OnTerminalDisable called");
            DeleteAllNodes(ref Plugin.nodesAdded);
            DeleteAllNouns(ref Plugin.nounsAdded); //keywords follows this method
            DeleteCams();
            ConfigSetup.defaultListing.DeleteAll();
            DeleteAllTerminalCodes(ref Plugin.AllTerminalCodes);
        }

        private static void DeleteCams()
        {
            if (CamStuff.CameraData != null)
                GameObject.Destroy(CamStuff.CameraData);

            if (CamStuff.ObcCameraHolder != null)
                GameObject.Destroy(CamStuff.ObcCameraHolder);
            if (CamStuff.ObcCameraHolder != null)
                GameObject.Destroy(CamStuff.ObcCameraHolder);
        }

        [Obsolete("Probably dont need to do this anymore")]
        public static void DeleteNounWord(ref TerminalKeyword keyWord, string terminalKeyword)
        {
            List<CompatibleNoun> keywordList = [.. keyWord.compatibleNouns];
            List<CompatibleNoun> nounsToRemove = [];
            foreach (CompatibleNoun compatibleNoun in keywordList)
            {
                if (compatibleNoun.noun.word.ToLower() == terminalKeyword.ToLower())
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

        [Obsolete("This doesn't really work right at the moment")]
        public static void RemoveCompatibleNoun(ref TerminalKeyword mainWord, TerminalKeyword wordToRemove)
        {
            bool removedWord = false;

            if (mainWord.compatibleNouns != null)
            {
                List<CompatibleNoun> newList = [];
                foreach (CompatibleNoun noun in mainWord.compatibleNouns)
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

        [Obsolete("This doesn't really work right at the moment")]
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

        public static void DeleteAllTerminalCodes(ref List<TerminalAccessibleObject> codes)
        {
            if (codes.Count == 0)
                return;

            List<TerminalAccessibleObject> destroyList = codes;

            for (int i = destroyList.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting TerminalAccessibleCode Object: {destroyList[i]}");
                UnityEngine.Object.Destroy(destroyList[i]);
            }

            codes = [];
        }

        public static void DeleteAllNouns(ref List<CompatibleNoun> nounsToDelete)
        {
            if (nounsToDelete.Count == 0)
            {
                Plugin.Spam("no nouns detected to delete");
                DeleteAllKeywords(ref Plugin.keywordsAdded);
                return;
            }

            List<TerminalKeyword> allKeywords = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (CompatibleNoun noun in nounsToDelete)
            {
                if (allKeywords.Any(k => k.compatibleNouns != null && k.compatibleNouns.Contains(noun)))
                {
                    List<TerminalKeyword> keywords = allKeywords.FindAll(k => k.compatibleNouns != null && k.compatibleNouns.Contains(noun));

                    foreach (TerminalKeyword keyword in keywords)
                    {
                        List<CompatibleNoun> newList = [.. keyword.compatibleNouns];
                        if (newList.Remove(noun))
                        {
                            keyword.compatibleNouns = [.. newList];
                            Plugin.Spam($"{noun.noun.word} removed from word: {keyword.word}");
                        }

                        else
                            Plugin.WARNING($"Unable to remove compatible noun: {noun.noun.word} from word: {keyword.word}");
                    }
                }
                else
                    Plugin.WARNING($"Unable to find any words {noun.noun.word} is associated to");
            }

            Plugin.nounsAdded.Clear();
            Plugin.Spam("DeleteAllNouns Completed");

            DeleteAllKeywords(ref Plugin.keywordsAdded);
        }

        public static void DeleteAllKeywords(ref List<TerminalKeyword> keywordList)
        {
            if (keywordList.Count == 0)
                return;

            List<TerminalKeyword> wordsToDelete = keywordList;
            List<TerminalKeyword> allKeywords = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keywordList)
            {
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
                    Plugin.Spam($"Node: [{nodeName}] removed");
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

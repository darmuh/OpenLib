using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerminalStuff.NoMoreAPI
{
    internal class TerminalHook
    {
        internal static List<TerminalNode> darmuhsUnlockableNodes = [];
        //internal static List<UnlockableItem> darmuhsUnlockables = [];
        internal static Dictionary<TerminalNode, string> darmuhsStorePacks = [];
        internal static void AddKeywordToExistingNode(string keyWord, TerminalNode existingNode, bool addToList = false)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            List<CompatibleNoun> existingNounList = [];
            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = keyWord + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = existingNode;

            if(existingNode.terminalOptions != null)
            {
                existingNounList = [.. existingNode.terminalOptions];
                Plugin.Spam($"{existingNode.name} has existing terminalOptions");
            }

            CompatibleNoun noun = new()
            {
                noun = terminalKeyword,
                result = existingNode
            };
            existingNounList.Add(noun);
            existingNode.terminalOptions = [.. existingNounList];

            allKeywordsList.Add(terminalKeyword);
            
            if(addToList)
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            
            Plugin.Spam($"Adding {keyWord} to existing node {existingNode.name}");
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeDynamicCommand(string nodeName, string keyWord, string failtext, bool clearText, bool acceptAnything, Func<string> commandFunc, Dictionary<TerminalNode, string> specialNodeList, Dictionary<TerminalNode, Func<string>> nodeListing) //scolor,fcolor,bind,unbind,tp (nodes that accept strings)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandFunc, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if(CommandStuff.DoesNodeExist(commandFunc, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = failtext;
            terminalNode.clearPreviousText = clearText;
            terminalNode.acceptAnything = acceptAnything;
            //terminalNode.overrideOptions = true;


            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandFunc);
            specialNodeList.Add(terminalNode, keyWord.ToLower());
             
            allKeywordsList.Add(terminalKeyword);
            TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText) //more command
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, displayText, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            
            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;
            _ = new
            CompatibleNoun()
            {
                noun = terminalKeyword,
                result = terminalNode
            };

            allKeywordsList.Add(terminalKeyword);
            //TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }
        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing, int specialNumber, Dictionary<int, TerminalNode> specialNodeList) //more menu commands
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList, out TerminalKeyword outKeyword))
            {
                nodeListing.Add(outKeyword.specialKeywordResult, commandAction);
                specialNodeList.Add(specialNumber, outKeyword.specialKeywordResult);
                TerminalEvents.darmuhsKeywords.Add(outKeyword);
                return;
            }

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);
            specialNodeList.Add(specialNumber, terminalNode);

            allKeywordsList.Add(terminalKeyword);
            TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing, int specialNumber, string specialName, Dictionary<TerminalNode, int> specialNodeList, Dictionary<int, string> reverseNodeNumList) //viewNodes, mirror/cams/lol
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (CommandStuff.DoesNodeExist(commandAction, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            specialNodeList.Add(terminalNode, specialNumber);

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);
            if (!reverseNodeNumList.ContainsKey(specialNumber))
                reverseNodeNumList.Add(specialNumber, specialName);

            allKeywordsList.Add(terminalKeyword);
            TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }


        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (CommandStuff.DoesNodeExist(commandAction, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);

            allKeywordsList.Add(terminalKeyword);
            TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, bool needsConfirm, bool acceptAnything, string confirmResultName, string denyResultName, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Func<string> denyAction, Dictionary<TerminalNode, Func<string>> nodeListing) // lever/link/restart
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (CommandStuff.DoesNodeExist(commandAction, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;
            

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            if (needsConfirm && (confirmAction != null && denyAction != null))
            {
                MakeConfirmationNode(confirmResultName, denyResultName, confirmAction, denyAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                allKeywordsList.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(confirm.noun);
                TerminalEvents.darmuhsKeywords.Add(deny.noun);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                Plugin.Spam($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                allKeywordsList.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                nodeListing.Add(terminalNode, commandAction);
                Plugin.Spam($"Node/Keyword added without confirmation nodes for {keyWord}");
            }


            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, bool needsConfirm, bool acceptAnything, string confirmResultName, string denyResultName, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Func<string> denyAction, Dictionary<TerminalNode, string> specialNodeList, Dictionary<TerminalNode, Func<string>> nodeListing) //fov, gamble, kick (nodes that have strings/ints after
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (CommandStuff.DoesNodeExist(commandAction, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;
            specialNodeList.Add(terminalNode, keyWord);

            if (needsConfirm && (confirmAction != null && denyAction != null))
            {
                MakeConfirmationNode(confirmResultName, denyResultName, confirmAction, denyAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(confirm.noun);
                TerminalEvents.darmuhsKeywords.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                Plugin.Spam($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                allKeywordsList.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                nodeListing.Add(terminalNode, commandAction);
                Plugin.Spam($"Node/Keyword added without confirmation nodes for {keyWord}");
            }


            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        }

        private static void AddToBuyWord(ref TerminalKeyword buyKeyword, ref TerminalKeyword terminalKeyword, UnlockableItem item)
        {
            terminalKeyword.defaultVerb = buyKeyword;
            Plugin.Spam($"Added buy verb to {buyKeyword}");
            CompatibleNoun wordIsCompatNoun = new()
            {
                noun = terminalKeyword,
                result = item.shopSelectionNode
            };
            List<CompatibleNoun> buyKeywordList = [.. buyKeyword.compatibleNouns];
            buyKeywordList.Add(wordIsCompatNoun);
            buyKeyword.compatibleNouns = [.. buyKeywordList];

        }

        internal static void MakeStoreCommand(string nodeName, string keyWord, string storeName, bool isVerb, bool clearText, bool acceptAnything, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Dictionary<TerminalNode, Func<string>> nodeListing) //vitalspatch & bioscanpatch
        {

            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            /*
            if (CommandStuff.DoesNodeExist(commandAction, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            } */

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = storeName;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = keyWord;
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            if (confirmAction != null)
            {
                MakeConfirmationNode($"{nodeName}_confirm", $"{nodeName}_deny", confirmAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                UnlockableItem storeItem = AddUnlockable(terminalNode, true, 1, $"{storeName}");
                if(!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                    StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
                int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);
                terminalNode.creatureName = storeName;
                terminalNode.shipUnlockableID = unlockableID;
                confirm.result.shipUnlockableID = unlockableID;
                confirm.result.buyUnlockable = true;
                confirm.result.itemCost = price;

                if (TryGetKeyword("buy", out TerminalKeyword buy))
                {
                    AddToBuyWord(ref buy, ref terminalKeyword, storeItem);
                }
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(confirm.noun);
                TerminalEvents.darmuhsKeywords.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                Plugin.Spam($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                Plugin.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {keyWord}!");
            }

            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeStoreCommand(string nodeName, string keyWord, string storeName, bool isVerb, bool clearText, bool acceptAnything, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, string itemList, Dictionary<TerminalNode, Func<string>> nodeListing) //overload for purchasepacks
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (CommandStuff.IsCommandCreatedAlready(keyWord, commandAction, allKeywordsList))
                return;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (CommandStuff.DoesNodeExist(nodeName, out TerminalNode existingNode))
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, adding additional keyword {keyWord}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = storeName;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            if (confirmAction != null)
            {
                MakeConfirmationNode($"{nodeName}_confirm", $"{nodeName}_deny", confirmAction, confirmDisplayText, denyDisplayText, price, itemList, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];

                UnlockableItem storeItem = AddUnlockable(terminalNode, true, 0, $"{storeName}");
                if (!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                    StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
                int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);

                terminalNode.creatureName = storeName;
                terminalNode.shipUnlockableID = unlockableID;
                confirm.result.shipUnlockableID = unlockableID;
                confirm.result.buyUnlockable = false;
                confirm.result.itemCost = price;

                if (TryGetKeyword("buy", out TerminalKeyword buy))
                {
                    AddToBuyWord(ref buy, ref terminalKeyword, storeItem);
                }
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                TerminalEvents.darmuhsKeywords.Add(terminalKeyword);
                TerminalEvents.darmuhsKeywords.Add(confirm.noun);
                TerminalEvents.darmuhsKeywords.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                darmuhsStorePacks.Add(terminalNode, itemList);

                Plugin.Spam($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                Plugin.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {keyWord}!");
            }

            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        //store confirm
        internal static void MakeConfirmationNode(string confirmResultName, string denyResultName, Func<string> confirmAction, string confirmDisplayText, string denyDisplayText, int price, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = confirmResultName;
            confirm.result.displayText = confirmDisplayText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;
            nodeListing.Add(confirm.result, confirmAction);

            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = denyResultName;
            deny.result.clearPreviousText = true;
            deny.result.displayText = denyDisplayText;
        }

        internal static void MakeConfirmationNode(string confirmResultName, string denyResultName, Func<string> confirmAction, string confirmDisplayText, string denyDisplayText, int price, string itemList, Dictionary<TerminalNode,  Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny) //store overload
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = confirmResultName;
            confirm.result.displayText = confirmDisplayText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;
            nodeListing.Add(confirm.result, confirmAction);
            darmuhsStorePacks.Add(confirm.result, itemList);

            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = denyResultName;
            deny.result.clearPreviousText = true;
            deny.result.displayText = denyDisplayText;
        }

        internal static void MakeConfirmationNode(string confirmResultName, string denyResultName, Func<string> confirmAction, Func<string> denyAction, string confirmDisplayText, string denyDisplayText, int price, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = confirmResultName;
            confirm.result.displayText = confirmDisplayText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;
            nodeListing.Add(confirm.result, confirmAction);


            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = denyResultName;
            deny.result.displayText = denyDisplayText;
            deny.result.clearPreviousText = true;
            nodeListing.Add(deny.result, denyAction);
        }

        internal static void CheckForAndDeleteKeyWord(string keyWord)
        {
            //Plugin.MoreLogs($"Checking for {keyWord}");
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
            //Plugin.MoreLogs("keyword list adjusted");
            return;
        }

        internal static bool UseMatchingNode(string nodeName, out TerminalNode returnNode)
        {
            TerminalNode[] allTerminalNodes = UnityEngine.Object.FindObjectsOfType<TerminalNode>();

            foreach (TerminalNode node in allTerminalNodes)
            {
                if (node.name.Equals(nodeName))
                {
                    returnNode = node;
                    Plugin.Spam($"Existing terminalNode [{nodeName}] found, using it rather than making a new one for this command");
                    return true;
                }
            }

            returnNode = null;
            return false;
        }

        internal static bool TryGetKeyword(string keyWord)
        {
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] found!");
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetKeyword(string keyWord, out TerminalKeyword terminalKeyword)
        {
            List<TerminalKeyword> keyWordList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    Plugin.Spam($"Keyword: [{keyWord}] found!");
                    terminalKeyword = keyword;
                    return true;
                }
            }

            terminalKeyword = null;
            return false;
        }

        internal static TerminalNode CreateDummyNode(string nodeName, bool clearPrevious, string displayText)
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

        internal static bool TryGetAndReturnUnlockable(string unlockableName, out UnlockableItem itemOut)
        {
            List<UnlockableItem> unlockableList = [.. StartOfRound.Instance.unlockablesList.unlockables];
            foreach(UnlockableItem item in unlockableList)
            {
                if(item.unlockableName.Equals(unlockableName))
                {
                    itemOut = item;
                    return true;
                }
            }

            itemOut = null; 
            return false;
        }

        internal static bool TryGetAndReturnItem(string unlockableName, out Item itemOut)
        {
            List<Item> unlockableList = [.. Plugin.instance.Terminal.buyableItemsList];
            foreach (Item item in unlockableList)
            {
                if (item.itemName.Equals(unlockableName))
                {
                    itemOut = item;
                    return true;
                }
            }

            itemOut = null;
            return false;
        }

        internal static UnlockableItem AddUnlockable(TerminalNode shopNode, bool alwaysInStock, int maxNumber, string unlockableName)
        {
            UnlockableItem itemToReturn;
            if(TryGetAndReturnUnlockable(unlockableName, out UnlockableItem returnedItem))
            {
                Plugin.Spam($"found matching item for {unlockableName}");
                returnedItem.unlockableType = 1;
                returnedItem.shopSelectionNode = shopNode;
                returnedItem.alwaysInStock = alwaysInStock;
                returnedItem.IsPlaceable = false;
                returnedItem.spawnPrefab = false;
                returnedItem.maxNumber = maxNumber;
                itemToReturn = returnedItem;
            }
            else
            {
                UnlockableItem item = new()
                {
                    //prefabObject = new GameObject(unlockableName),
                    unlockableType = 1,
                    unlockableName = unlockableName,
                    shopSelectionNode = shopNode,
                    alwaysInStock = alwaysInStock,
                    IsPlaceable = false,
                    spawnPrefab = false,
                    maxNumber = maxNumber
                };

                itemToReturn = item;
            }
            
            //darmuhsUnlockables.Add(returnedItem);
            darmuhsUnlockableNodes.Add(shopNode);
            return itemToReturn;
        }

        internal static Item AddItem(string unlockableName, int uniqueID, int price, GameObject itemObject) //unused
        {
            Item itemToReturn;
            if(TryGetAndReturnItem(unlockableName, out Item itemOut))
            {
                itemOut.itemId = uniqueID;
                itemOut.creditsWorth = price;
                itemOut.spawnPrefab = itemObject;
                itemToReturn = itemOut;
            }
            else
            {
                Item newItem = new()
                {
                    itemName = unlockableName,
                    itemId = uniqueID,
                    creditsWorth = price,
                    spawnPrefab = itemObject
                };
                itemToReturn = newItem;
            }

            return itemToReturn;
        }

        internal static void SetTerminalInput(string terminalInput)
        {
            Plugin.instance.Terminal.TextChanged(Plugin.instance.Terminal.currentText.Substring(0, Plugin.instance.Terminal.currentText.Length - Plugin.instance.Terminal.textAdded) + terminalInput);
            Plugin.instance.Terminal.screenText.text = Plugin.instance.Terminal.currentText;
            Plugin.instance.Terminal.textAdded = terminalInput.Length;
        }
    }
}

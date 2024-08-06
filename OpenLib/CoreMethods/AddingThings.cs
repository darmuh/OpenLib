using System;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using static OpenLib.CoreMethods.DynamicBools;
using static OpenLib.CoreMethods.CommonThings;
using OpenLib.Menus;
using OpenLib.ConfigManager;
using OpenLib.Common;
using UnityEngine.InputSystem;
using Steamworks.Ugc;

namespace OpenLib.CoreMethods
{
    public class AddingThings
    {
        private static readonly char[] NewLineChars = Environment.NewLine.ToCharArray();
        public static void AddKeywordToExistingNode(string keyWord, TerminalNode existingNode, bool addToList = true)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            List<CompatibleNoun> existingNounList = [];
            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = keyWord + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = existingNode;

            if (existingNode.terminalOptions != null)
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

            if (addToList)
                Plugin.keywordsAdded.Add(terminalKeyword);

            Plugin.Spam($"Adding {keyWord} to existing node {existingNode.name}");
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        public static TerminalKeyword AddKeywordToNode(string keyWord, TerminalNode existingNode, bool addToList = true)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            List<CompatibleNoun> existingNounList = [];
            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = keyWord + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = existingNode;

            if (existingNode.terminalOptions != null)
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

            if (addToList)
                Plugin.keywordsAdded.Add(terminalKeyword);

            Plugin.Spam($"Adding {keyWord} to existing node {existingNode.name}");
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            return terminalKeyword;
        }

        public static void AddToHelpCommand(string textToAdd)
        {
            TerminalNode helpNode = Plugin.instance.Terminal.terminalNodes.specialNodes[13];

            if (helpNode.displayText.Contains(textToAdd))
            {
                Plugin.WARNING($"Help command already contains this text: {textToAdd}");
                return;
            }

            int lastCommandStart = helpNode.displayText.LastIndexOf('['); //looking for [numberOfItemsOnRoute]
            helpNode.displayText = helpNode.displayText.Insert(lastCommandStart, $"{textToAdd}\r\n\r\n");
        }

        public static void AddToExistingNodeText(string textToAdd, ref TerminalNode existingNode)
        {
            if (existingNode.displayText.Contains(textToAdd))
            {
                Plugin.Log.LogWarning($"Unable to add below text to {existingNode.name}, it already has this text in it");
                return;
            }

            Plugin.Spam($"oldtext length {existingNode.displayText.Length}");
            Plugin.Spam(existingNode.displayText);
            string newText = existingNode.displayText.TrimEnd(NewLineChars);
            newText += $"\n{textToAdd}\r\n\r\n";
            existingNode.displayText = newText;
            Plugin.Spam($"{existingNode.name} text updated!!!");
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

        public static void AddBasicCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, string category = "", string keywordDescription = "") //menus
        {
            TerminalNode otherNode = LogicHandling.GetFromAllNodes("OtherCommands");
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            if (IsCommandCreatedAlready(keyWord, displayText, allKeywordsList))
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
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            
            if (!Plugin.nodesAdded.Contains(terminalNode))
                Plugin.nodesAdded.Add(terminalNode);

            if(!Plugin.keywordsAdded.Contains(terminalKeyword))
                Plugin.keywordsAdded.Add(terminalKeyword);

            if(category.ToLower() == "other")
            {
                AddToExistingNodeText($"{keywordDescription}", ref otherNode);
                Plugin.Spam("adding node to other listing");
            }
        }

        //for use without referring to specific config items
        public static TerminalNode AddNodeManual(string nodeName, string stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, int price = 0, Func<string> ConfirmAction = null, Func<string> DenyAction = null, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "") 
        {
            TerminalNode returnNode = null;

            List<string> keywords = [];
            if (stringValue != null)
                keywords = CommonStringStuff.GetKeywordsPerConfigItem(stringValue);

            foreach (string keyWord in keywords) //added this to handle the full list in one method run to add list to menu item
            {
                List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

                if (IsCommandCreatedAlready(yourModListing.Listing, keyWord, commandAction, allKeywordsList, out TerminalKeyword outKeyword) && !reuseFunc)
                    continue;

                CheckForAndDeleteKeyWord(keyWord.ToLower());

                if (DoesNodeExist(yourModListing.Listing, commandAction, out TerminalNode existingNode) && !reuseFunc)
                {
                    AddKeywordToExistingNode(keyWord, existingNode, true);
                    Plugin.Spam($"existing node found {existingNode.name}, reusing associated func and adding additional keyword {keyWord}");
                    continue;
                }

                TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
                terminalNode.name = nodeName;
                terminalNode.displayText = nodeName;
                terminalNode.clearPreviousText = clearText;
                terminalNode.buyUnlockable = false;
                returnNode = terminalNode;

                TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
                terminalKeyword.name = nodeName + "_keyword";
                terminalKeyword.word = keyWord.ToLower();
                terminalKeyword.isVerb = false;
                terminalKeyword.specialKeywordResult = terminalNode;

                CompatibleNoun confirm = null;
                CompatibleNoun deny = null;

                if (CommandType == 1) //base requires confirmation setup
                {
                    AddConfirm(nodeName, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing.Listing, out confirm, out deny);
                    terminalNode.acceptAnything = false;
                    Plugin.Spam("command type 1 detected, adding basic confirmation");
                }
                else if (CommandType == 2) //store command
                {
                    AddStoreCommand(nodeName, storeName, ref terminalKeyword, ref terminalNode, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing, alwaysInStock, maxStock, out confirm, out deny);
                    Plugin.Spam("command type 2 detected, adding store logic");
                    terminalNode.acceptAnything = false;
                    yourModListing.shopNodes.Add(confirm.result);
                    yourModListing.shopNodes.Add(terminalNode);
                    yourModListing.shopNodes.Add(deny.result);
                    if (itemList.Length > 1)
                    {
                        confirm.result.buyUnlockable = false;
                        yourModListing.storePacks.Add(terminalNode, itemList);
                        Plugin.Spam("storepack detected, adding itemlist");
                    }
                }

                if (confirm != null && deny != null)
                {
                    allKeywordsList.Add(confirm.noun);
                    Plugin.keywordsAdded.Add(confirm.noun);

                    allKeywordsList.Add(deny.noun);
                    Plugin.keywordsAdded.Add(deny.noun);

                    terminalNode.terminalOptions = [confirm, deny];
                    terminalNode.overrideOptions = true;

                }
                else
                    Plugin.Spam($"no confirmation logic added for {keyWord}");

                yourModListing.Listing.Add(terminalNode, commandAction);
                allKeywordsList.Add(terminalKeyword);

                if (!Plugin.nodesAdded.Contains(terminalNode))
                    Plugin.nodesAdded.Add(terminalNode);

                if (!Plugin.keywordsAdded.Contains(terminalKeyword))
                    Plugin.keywordsAdded.Add(terminalKeyword);

                Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            }

            return returnNode;
        }

        //when you want to refer to config items for management but also want a terminalnode returned to you
        public static TerminalNode AddNodeManual(string nodeName, ConfigEntry<string> stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, List<ManagedConfig> managedBools, string category = "", string description = "", int price = 0, Func<string>ConfirmAction = null, Func<string>DenyAction = null, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "")
        {
            TerminalNode returnNode = null;

            List<string> keywords = [];
            bool isStringNull = true;
            if (stringValue != null)
            {
                isStringNull = false;
                keywords = CommonStringStuff.GetKeywordsPerConfigItem(stringValue.Value);
            }

            foreach (string keyWord in keywords) //added this to handle the full list in one method run to add list to menu item
            {
                List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

                if (IsCommandCreatedAlready(yourModListing.Listing, keyWord, commandAction, allKeywordsList, out TerminalKeyword outKeyword) && !reuseFunc)
                    continue;

                CheckForAndDeleteKeyWord(keyWord.ToLower());

                if (DoesNodeExist(yourModListing.Listing, commandAction, out TerminalNode existingNode) && !reuseFunc)
                {
                    AddKeywordToExistingNode(keyWord, existingNode, true);
                    Plugin.Spam($"existing node found {existingNode.name}, reusing associated func and adding additional keyword {keyWord}");
                    continue;
                }

                TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
                terminalNode.name = nodeName;
                terminalNode.displayText = nodeName;
                terminalNode.clearPreviousText = clearText;
                terminalNode.buyUnlockable = false;
                returnNode = terminalNode;

                TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
                terminalKeyword.name = nodeName + "_keyword";
                terminalKeyword.word = keyWord.ToLower();
                terminalKeyword.isVerb = false;
                terminalKeyword.specialKeywordResult = terminalNode;

                CompatibleNoun confirm = null;
                CompatibleNoun deny = null;

                if (CommandType == 1) //base requires confirmation setup
                {
                    AddConfirm(nodeName, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing.Listing, out confirm, out deny);
                    terminalNode.acceptAnything = false;
                    Plugin.Spam("command type 1 detected, adding basic confirmation");
                }
                else if (CommandType == 2) //store command
                {
                    AddStoreCommand(nodeName, storeName, ref terminalKeyword, ref terminalNode, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing, alwaysInStock, maxStock, out confirm, out deny);
                    Plugin.Spam("command type 2 detected, adding store logic");
                    terminalNode.acceptAnything = false;
                    yourModListing.shopNodes.Add(confirm.result);
                    yourModListing.shopNodes.Add(terminalNode);
                    yourModListing.shopNodes.Add(deny.result);
                    if (itemList.Length > 1)
                    {
                        confirm.result.buyUnlockable = false;
                        yourModListing.storePacks.Add(terminalNode, itemList);
                        Plugin.Spam("storepack detected, adding itemlist");
                    }
                }

                if (confirm != null && deny != null)
                {
                    allKeywordsList.Add(confirm.noun);
                    Plugin.keywordsAdded.Add(confirm.noun);

                    allKeywordsList.Add(deny.noun);
                    Plugin.keywordsAdded.Add(deny.noun);

                    terminalNode.terminalOptions = [confirm, deny];
                    terminalNode.overrideOptions = true;

                }
                else
                    Plugin.Spam($"no confirmation logic added for {keyWord}");

                yourModListing.Listing.Add(terminalNode, commandAction);
                allKeywordsList.Add(terminalKeyword);

                if (!Plugin.nodesAdded.Contains(terminalNode))
                    Plugin.nodesAdded.Add(terminalNode);

                if (!Plugin.keywordsAdded.Contains(terminalKeyword))
                    Plugin.keywordsAdded.Add(terminalKeyword);

                Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            }

            if (ManagedBoolGet.CanAddToManagedBoolList(managedBools, nodeName))
            {
                TerminalMenuItem nodeInfo = new()
                {
                    Category = category,
                    itemDescription = description,
                    ItemName = nodeName,
                    itemKeywords = keywords,
                };

                ManagedConfig fromNode = new()
                {
                    TerminalNode = returnNode,
                    menuItem = nodeInfo,
                    ConfigItemName = nodeName
                };
                managedBools.Add(fromNode);

                if (!isStringNull)
                {
                    ConfigSetup.AddManagedString(stringValue, ref managedBools, fromNode);
                }
            }

            return returnNode;
        }

        //base level node/keyword creation
        public static TerminalNode CreateNode(TerminalMenu terminalMenu, string nodeName, string keyWord, Func<string> commandAction, MainListing yourModListing, bool isNextPageCommand = false)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
            Plugin.Spam($"{nodeName}");
            Plugin.Spam(keyWord);

            bool clearText = true; //menus will always clear text for now

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = nodeName;
            terminalNode.clearPreviousText = clearText;
            terminalNode.buyUnlockable = false;

            Plugin.Spam("node created");

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = terminalNode;

            Plugin.Spam("keyword created");

            yourModListing.Listing.Add(terminalNode, commandAction);
            Plugin.Spam("func added to listing");

            if(!isNextPageCommand)
            {
                terminalMenu.terminalNodePerCategory.Add(keyWord, terminalNode);
                Plugin.Spam("added terminalNode to menus nodelisting");
            }

            allKeywordsList.Add(terminalKeyword);
            Plugin.keywordsAdded.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            Plugin.Spam("added to all keyword lists");
            return terminalNode;
        }


        public static TerminalNode CreateNode(ManagedConfig managedBool, string keyWord, MainListing yourModListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            Func<string> commandAction = managedBool.MainAction;

            string nodeName;

            if (managedBool.nodeName.Length < 2)
            {
                nodeName = managedBool.ConfigItemName;
                Plugin.Spam("managedBool nodename is blank, using configitemname");
            }
            else
            {
                nodeName = managedBool.nodeName;
                Plugin.Spam($"using nodeName: {nodeName}");
            }
                
            bool clearText = managedBool.clearText;

            if (IsCommandCreatedAlready(yourModListing.Listing, keyWord, commandAction, allKeywordsList, out TerminalKeyword outKeyword) && !managedBool.reuseFunc)
                return outKeyword.specialKeywordResult;

            CheckForAndDeleteKeyWord(keyWord.ToLower());

            if (DoesNodeExist(yourModListing.Listing, commandAction, out TerminalNode existingNode) && !managedBool.reuseFunc)
            {
                AddKeywordToExistingNode(keyWord, existingNode, true);
                Plugin.Spam($"existing node found {existingNode.name}, re-using func and adding additional keyword {keyWord}");
                return existingNode;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = nodeName;
            terminalNode.clearPreviousText = clearText;
            terminalNode.buyUnlockable = false;
            managedBool.TerminalNode = terminalNode;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = terminalNode;

            CompatibleNoun confirm = null;
            CompatibleNoun deny = null;

            if (managedBool.CommandType == 1) //base requires confirmation setup
            {
                AddConfirm(nodeName, managedBool, out confirm, out deny);
                if (managedBool.ConfirmAction != null)
                {
                    yourModListing.Listing.Add(confirm.result, managedBool.ConfirmAction);
                    Plugin.Spam("confirmResult FUNC added");
                }

                if (managedBool.DenyAction != null)
                {
                    yourModListing.Listing.Add(deny.result, managedBool.DenyAction);
                    Plugin.Spam("denyResult FUNC added");
                }

                terminalNode.acceptAnything = false;
                Plugin.Spam("command type 1 detected, adding basic confirmation");
            }
            else if (managedBool.CommandType == 2) //store command
            {
                AddStoreCommand(nodeName, ref terminalKeyword, managedBool, yourModListing, out confirm, out deny);
                Plugin.Spam("command type 2 detected, adding store logic");
                terminalNode.acceptAnything = false;
                yourModListing.shopNodes.Add(confirm.result);
                yourModListing.shopNodes.Add(terminalNode);
                yourModListing.shopNodes.Add(deny.result);
                if(managedBool.itemList.Length > 1) //unused in this method as storepacks can be changed between lobby loads
                {
                    confirm.result.buyUnlockable = false;
                    yourModListing.storePacks.Add(terminalNode, managedBool.itemList);
                    Plugin.Spam("storepack detected, adding itemlist");
                }
            }

            if (confirm != null && deny != null)
            {
                allKeywordsList.Add(confirm.noun);
                Plugin.keywordsAdded.Add(confirm.noun);

                allKeywordsList.Add(deny.noun);
                Plugin.keywordsAdded.Add(deny.noun);

                terminalNode.terminalOptions = [confirm, deny];
                terminalNode.overrideOptions = true;

            }
            else
                Plugin.Spam($"no confirmation logic added for {keyWord}");

            yourModListing.Listing.Add(terminalNode, commandAction);
            allKeywordsList.Add(terminalKeyword);
            if (!Plugin.nodesAdded.Contains(terminalNode))
                Plugin.nodesAdded.Add(terminalNode);

            if (!Plugin.keywordsAdded.Contains(terminalKeyword))
                Plugin.keywordsAdded.Add(terminalKeyword);
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
            return terminalNode;
        }

        public static void AddConfirm(string nodeName, ManagedConfig managedBool, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = nodeName + "_confirm";
            confirm.result.displayText = managedBool.confirmText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = managedBool.price;

            confirm.noun.specialKeywordResult = confirm.result;

            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = nodeName + "_deny";
            deny.result.displayText = managedBool.denyText;
            deny.result.clearPreviousText = true;

            deny.noun.specialKeywordResult = deny.result;
                
        }

        public static void AddConfirm(string nodeName, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = nodeName + "_confirm";
            confirm.result.displayText = confirmText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;

            confirm.noun.specialKeywordResult = confirm.result;
            if (ConfirmAction != null)
                nodeListing.Add(confirm.result, ConfirmAction);


            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = nodeName + "_deny";
            deny.result.displayText = denyText;
            deny.result.clearPreviousText = true;

            deny.noun.specialKeywordResult = deny.result;

            if (DenyAction != null)
                nodeListing.Add(deny.result, DenyAction);
        }

        public static void AddStoreCommand(string nodeName, ref TerminalKeyword keyword, ManagedConfig managedBool, MainListing mainListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            TerminalNode node = managedBool.TerminalNode;

            if(node == null)
            {
                Plugin.ERROR("Unable to retrieve managedBool TerminalNode");
                confirm = null;
                deny = null;
                return;
            }

            if (managedBool.ConfirmAction != null)
            {
                AddConfirm( nodeName, managedBool, out confirm, out deny);
                confirm.result.buyUnlockable = true;
                if (managedBool.ConfirmAction != null)
                {
                    mainListing.Listing.Add(confirm.result, managedBool.ConfirmAction);
                    Plugin.Spam("confirmResult FUNC added");
                }

                if (managedBool.DenyAction != null)
                {
                    mainListing.Listing.Add(deny.result, managedBool.DenyAction);
                    Plugin.Spam("denyResult FUNC added");
                }
                node.terminalOptions = [confirm, deny];

                UnlockableItem storeItem = AddUnlockable(managedBool);
                if (!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                    StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
                int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);
                Plugin.Spam($"new unlockable found at {unlockableID} out of count: {StartOfRound.Instance.unlockablesList.unlockables.Count}");

                node.creatureName = managedBool.storeName;
                node.shipUnlockableID = unlockableID;
                node.itemCost = managedBool.price;
                node.overrideOptions = true;
                confirm.result.shipUnlockableID = unlockableID;
                confirm.result.buyUnlockable = true;
                confirm.result.itemCost = managedBool.price;

                if (TryGetKeyword("buy", out TerminalKeyword buy))
                {
                    AddToBuyWord(ref buy, ref keyword, storeItem);
                }

                Plugin.ShopNodes.Add(node);
                Plugin.Spam($"Store nodes created for {managedBool.storeName}");
            }
            else
            {
                Plugin.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {managedBool.storeName}!");
                confirm = null;
                deny = null;
                return;
            }
        }

        public static void AddStoreCommand(string nodeName, string storeName, ref TerminalKeyword keyword, ref TerminalNode node, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, MainListing mainListing, bool alwaysInStock, int maxStock, out CompatibleNoun confirm, out CompatibleNoun deny)
        {

            if (node == null)
            {
                Plugin.ERROR("node is null when adding store command!!!");
                confirm = null;
                deny = null;
                return;
            }

            if (ConfirmAction != null)
            {
                AddConfirm(nodeName, price, ConfirmAction, DenyAction, confirmText, denyText, mainListing.Listing, out confirm, out deny);
                node.terminalOptions = [confirm, deny];

                UnlockableItem storeItem = AddUnlockable(nodeName, node, alwaysInStock, maxStock);
                if (!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                    StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
                int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);

                node.creatureName = storeName; //too lazy to define this at the top level
                node.shipUnlockableID = unlockableID;
                confirm.result.shipUnlockableID = unlockableID;
                confirm.result.buyUnlockable = false;
                confirm.result.itemCost = price;

                if (TryGetKeyword("buy", out TerminalKeyword buy))
                {
                    AddToBuyWord(ref buy, ref keyword, storeItem);
                }

                Plugin.ShopNodes.Add(node);
                Plugin.Spam($"Store nodes created for {nodeName}");
            }
            else
            {
                Plugin.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {nodeName}!");
                confirm = null;
                deny = null;
                return;
            }
        }

        public static UnlockableItem AddUnlockable(ManagedConfig managedBool)
        {
            if (managedBool.UnlockableItem != null)
            {
                Plugin.Spam($"{managedBool.ConfigItemName} already contained item: {managedBool.UnlockableItem.unlockableName}");
                return managedBool.UnlockableItem;
            }  

            UnlockableItem itemToReturn;

            if (TryGetAndReturnUnlockable(managedBool.storeName, out UnlockableItem returnedItem))
            {
                Plugin.Spam($"found matching item for {managedBool.storeName}");
                returnedItem.unlockableType = 1; //0 = suits, 1 = everything else that is not an actual item
                returnedItem.shopSelectionNode = managedBool.TerminalNode;
                returnedItem.alwaysInStock = managedBool.alwaysInStock;
                returnedItem.IsPlaceable = false; //these are not physical items
                returnedItem.spawnPrefab = false; //so this is set to false
                returnedItem.maxNumber = managedBool.maxStock;
                itemToReturn = returnedItem;
            }
            else
            {
                Plugin.Spam($"Creating unlockable item from managedBool: {managedBool.ConfigItemName}");

                itemToReturn = new()
                {
                    //prefabObject = new GameObject(unlockableName),
                    unlockableType = 1,
                    alreadyUnlocked = false,
                    hasBeenUnlockedByPlayer = false,
                    unlockableName = managedBool.storeName,
                    shopSelectionNode = managedBool.TerminalNode,
                    alwaysInStock = managedBool.alwaysInStock,
                    IsPlaceable = false,
                    spawnPrefab = false,
                    maxNumber = managedBool.maxStock
                };
            }

            managedBool.UnlockableItem = itemToReturn;
            return itemToReturn;
        }

        public static UnlockableItem AddUnlockable(string storeName, TerminalNode node, bool alwaysInStock, int maxStock)
        {
            UnlockableItem itemToReturn;

            if (TryGetAndReturnUnlockable(storeName, out UnlockableItem returnedItem))
            {
                Plugin.Spam($"found matching item for {storeName}");
                returnedItem.unlockableType = 1; //0 = suits, 1 = everything else that is not an actual item
                returnedItem.shopSelectionNode = node;
                returnedItem.alwaysInStock = alwaysInStock;
                returnedItem.IsPlaceable = false; //these are not physical items
                returnedItem.spawnPrefab = false; //so this is set to false
                returnedItem.maxNumber = maxStock;
                itemToReturn = returnedItem;
            }
            else
            {
                Plugin.Spam($"Creating unlockable item manually for item: {storeName}");

                itemToReturn = new()
                {
                    //prefabObject = new GameObject(unlockableName),
                    unlockableType = 1,
                    unlockableName = storeName,
                    shopSelectionNode = node,
                    alwaysInStock = alwaysInStock,
                    IsPlaceable = false,
                    spawnPrefab = false,
                    maxNumber = maxStock
                };
            }

            return itemToReturn;
        }

        public static void AddToBuyWord(ref TerminalKeyword buyKeyword, ref TerminalKeyword terminalKeyword, UnlockableItem item)
        {
            terminalKeyword.defaultVerb = buyKeyword;
            Plugin.Spam($"Added buy verb to {buyKeyword.word}");
            CompatibleNoun wordIsCompatNoun = new()
            {
                noun = terminalKeyword,
                result = item.shopSelectionNode
            };
            List<CompatibleNoun> buyKeywordList = [.. buyKeyword.compatibleNouns];
            buyKeywordList.Add(wordIsCompatNoun);
            buyKeyword.compatibleNouns = [.. buyKeywordList];

        }

        public static void AddToKeyword(ref TerminalKeyword originalKeyword, ref TerminalKeyword newWord)
        {
            if (!originalKeyword.isVerb)
            {
                Plugin.Log.LogWarning("AddToKeyword called on non-verb");
                return;
            }
                
            newWord.defaultVerb = originalKeyword;
            Plugin.Spam($"Added verb {originalKeyword.word} to {newWord.word}");

            CompatibleNoun wordIsCompatNoun = new()
            {
                noun = newWord,
                result = newWord.specialKeywordResult
            };
            List<CompatibleNoun> compatibleNouns = [.. originalKeyword.compatibleNouns];
            compatibleNouns.Add(wordIsCompatNoun);
            originalKeyword.compatibleNouns = [.. compatibleNouns];
        }

        [Obsolete("This doesn't work at the moment")]
        public static void AddNounWordSimple(string originalVerb, string nodeName, string keyWord, string displayText, bool clearText)
        {
            if (originalVerb.Length < 1)
            {
                Plugin.WARNING("originalVerb text is invalid");
                return;
            }

            if(!TryGetKeyword(originalVerb, out TerminalKeyword originalWord))
            {
                Plugin.WARNING($"Unable to find word for {originalVerb}");
                return;
            }

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;

            CompatibleNoun newNoun = new()
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };

            newNoun.noun.name = nodeName + "_Noun";
            newNoun.noun.word = keyWord.ToLower();
            newNoun.noun.isVerb = false;
            newNoun.result = terminalNode;
            newNoun.noun.specialKeywordResult = terminalNode;

            if (!Plugin.nodesAdded.Contains(terminalNode))
                Plugin.nodesAdded.Add(terminalNode);

            if (!Plugin.keywordsAdded.Contains(newNoun.noun))
                Plugin.keywordsAdded.Add(newNoun.noun);


            AddToKeyword(ref originalWord, ref newNoun.noun);
        }
    }
}

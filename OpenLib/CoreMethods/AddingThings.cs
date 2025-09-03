using BepInEx.Configuration;
using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.Menus;
using System;
using System.Collections.Generic;
using static OpenLib.CoreMethods.CommonThings;
using static OpenLib.CoreMethods.DynamicBools;

namespace OpenLib.CoreMethods;

public class AddingThings
{
    private static readonly char[] NewLineChars = Environment.NewLine.ToCharArray();
    public static void AddKeywordToExistingNode(string keyWord, TerminalNode existingNode, bool addToList = true)
    {
        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
        List<CompatibleNoun> existingNounList = [];
        TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(keyWord + "_keyword", keyWord.ToLower());
        terminalKeyword.isVerb = false;
        terminalKeyword.specialKeywordResult = existingNode;

        if (existingNode.terminalOptions != null!)
        {
            existingNounList = [.. existingNode.terminalOptions];
            Loggers.LogDebug($"{existingNode.name} has existing terminalOptions");
        }

        CompatibleNoun noun = new() //not added to noun list as no keyword associated to it
        {
            noun = terminalKeyword,
            result = existingNode
        };
        existingNounList.Add(noun);
        existingNode.terminalOptions = [.. existingNounList];

        allKeywordsList.Add(terminalKeyword);

        if (addToList)
            Plugin.KeywordsAdded.Add(terminalKeyword);

        Loggers.LogDebug($"Adding {keyWord} to existing node {existingNode.name}");
        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
    }

    public static TerminalKeyword AddKeywordToNode(string keyWord, TerminalNode existingNode, bool addToList = true)
    {
        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
        List<CompatibleNoun> existingNounList = [];
        TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(keyWord + "_keyword", keyWord.ToLower());
        terminalKeyword.isVerb = false;
        terminalKeyword.specialKeywordResult = existingNode;

        if (existingNode.terminalOptions != null!)
        {
            existingNounList = [.. existingNode.terminalOptions];
            Loggers.LogDebug($"{existingNode.name} has existing terminalOptions");
        }

        CompatibleNoun noun = new() //no associated keyword, not adding to noun list
        {
            noun = terminalKeyword,
            result = existingNode
        };
        existingNounList.Add(noun);
        existingNode.terminalOptions = [.. existingNounList];

        allKeywordsList.Add(terminalKeyword);

        if (addToList)
            Plugin.KeywordsAdded.Add(terminalKeyword);

        Loggers.LogDebug($"Adding {keyWord} to existing node {existingNode.name}");
        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        return terminalKeyword;
    }

    public static void AddToHelpCommand(string textToAdd)
    {
        TerminalNode helpNode = Plugin.instance.Terminal.terminalNodes.specialNodes[13];

        if (helpNode.displayText.Contains(textToAdd))
        {
            Loggers.WARNING($"Help command already contains this text: {textToAdd}");
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

        Loggers.LogDebug($"oldtext length {existingNode.displayText.Length}");
        Loggers.LogDebug(existingNode.displayText);
        string newText = existingNode.displayText.TrimEnd(NewLineChars);
        newText += $"\n{textToAdd}\r\n\r\n";
        existingNode.displayText = newText;
        Loggers.LogDebug($"{existingNode.name} text updated!!!");
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
            terminalNode = BasicTerminal.CreateNewTerminalNode();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearPrevious;
        }

        return terminalNode;
    }

    //[Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void AddBasicCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, string category = "", string keywordDescription = "") //menus
    {
        if (!LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode) && Misc.CompareStringsInvariant(category, "other"))
        {
            Loggers.WARNING($"Unable to add {keyWord} to {category}\n{category} TerminalNode could not be found!");
            return;
        }

        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

        if (IsCommandCreatedAlready(keyWord, displayText, allKeywordsList))
            return;

        CheckForAndDeleteKeyWord(keyWord.ToLower());

        TerminalNode terminalNode = BasicTerminal.CreateNewTerminalNode();
        terminalNode.name = nodeName;
        terminalNode.displayText = displayText;
        terminalNode.clearPreviousText = clearText;

        TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(nodeName + "_keyword", keyWord.ToLower());
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

        if (!Plugin.NodesAdded.Contains(terminalNode))
            Plugin.NodesAdded.Add(terminalNode);

        if (!Plugin.KeywordsAdded.Contains(terminalKeyword))
            Plugin.KeywordsAdded.Add(terminalKeyword);

        if (Misc.CompareStringsInvariant(category, "other") && otherNode != null!)
        {
            AddToExistingNodeText($"{keywordDescription}", ref otherNode);
            Loggers.LogDebug("adding node to other listing");
        }
    }

    //for use without referring to specific config items
    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static TerminalNode AddNodeManual(string nodeName, string stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, int price = 0, Func<string> ConfirmAction = null!, Func<string> DenyAction = null!, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "")
    {
        TerminalNode returnNode = null!;
        List<string> keywords = [];
        if (stringValue != null!)
            keywords = CommonStringStuff.GetKeywordsPerConfigItem(stringValue);

        foreach (string keyword in keywords)
        {
            returnNode = BaseCommandCreation(nodeName, keyword, commandAction, clearText, CommandType, yourModListing, price, ConfirmAction, DenyAction, confirmText, denyText, alwaysInStock, maxStock, storeName, reuseFunc, itemList);
        }


        if (returnNode == null!)
            Loggers.WARNING("Returning NULL terminal node @AddNodeManual!!!");

        return returnNode!;
    }

    //when you want to refer to config items for management but also want a terminalnode returned to you
    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static TerminalNode AddNodeManual(string nodeName, ConfigEntry<string> stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, List<ManagedConfig> managedBools, string category = "", string description = "", int price = 0, Func<string> ConfirmAction = null!, Func<string> DenyAction = null!, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "")
    {
        TerminalNode returnNode = null!;
        List<string> keywords = [];
        bool isStringNull = true;
        if (stringValue != null)
        {
            isStringNull = false;
            keywords = CommonStringStuff.GetKeywordsPerConfigItem(stringValue.Value);
        }

        foreach (string keyword in keywords)
        {
            returnNode = BaseCommandCreation(nodeName, keyword, commandAction, clearText, CommandType, yourModListing, price, ConfirmAction, DenyAction, confirmText, denyText, alwaysInStock, maxStock, storeName, reuseFunc, itemList);
        }

        if (returnNode == null!)
            Loggers.WARNING("Returning NULL terminal node @AddNodeManual!!!");

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
                ConfigSetup.AddManagedString(stringValue!, ref managedBools, fromNode);
            }
        }

        return returnNode!;
    }

    //base level node/keyword creation
    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static TerminalNode CreateNode(TerminalMenu terminalMenu, string nodeName, string keyWord, Func<string> commandAction, MainListing yourModListing, bool isNextPageCommand = false)
    {
        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];
        Loggers.LogDebug($"{nodeName}");
        Loggers.LogDebug(keyWord);

        bool clearText = true; //menus will always clear text for now

        CheckForAndDeleteKeyWord(keyWord.ToLower());

        TerminalNode terminalNode = BasicTerminal.CreateNewTerminalNode();
        terminalNode.name = nodeName;
        terminalNode.displayText = nodeName;
        terminalNode.clearPreviousText = clearText;
        terminalNode.buyUnlockable = false;

        Loggers.LogDebug("node created");

        TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(nodeName + "_keyword", keyWord.ToLower());
        terminalKeyword.isVerb = false;
        terminalKeyword.specialKeywordResult = terminalNode;

        Loggers.LogDebug("keyword created");

        yourModListing.Listing.Add(terminalNode, commandAction);
        Loggers.LogDebug("func added to listing");

        if (!isNextPageCommand)
        {
            terminalMenu.terminalNodePerCategory.Add(keyWord, terminalNode);
            Loggers.LogDebug("added terminalNode to menus nodelisting");
        }

        allKeywordsList.Add(terminalKeyword);
        Plugin.KeywordsAdded.Add(terminalKeyword);
        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        Loggers.LogDebug("added to all keyword lists");
        return terminalNode;
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void InfoText(ManagedConfig managedBool, string keyWord, TerminalKeyword infoWord, MainListing yourModListing)
    {
        if (managedBool.InfoAction != null!)
        {
            TerminalNode infoNode = BasicTerminal.CreateNewTerminalNode();
            infoNode.name = "info_" + keyWord;
            infoNode.displayText = "infoAction should replace this";
            yourModListing.Listing.Add(infoNode, managedBool.InfoAction);
            infoNode.clearPreviousText = true;
            AddCompatibleNoun(ref infoWord, keyWord, infoNode);
            Loggers.LogDebug("info node created with infoAction!");
        }
        else if (managedBool.InfoText.Length > 1)
        {
            TerminalNode infoNode = BasicTerminal.CreateNewTerminalNode();
            infoNode.name = "info_" + keyWord;
            infoNode.displayText = managedBool.InfoText;
            infoNode.clearPreviousText = true;
            AddCompatibleNoun(ref infoWord, keyWord, infoNode);
            Loggers.LogDebug("info node created");
        }
    }

    public static void AddToFauxListing(FauxKeyword fauxWord, MainListing yourListing)
    {
        if (fauxWord == null!)
            return;

        if (fauxWord.Keyword == null!)
            return;

        if (fauxWord.MainPage != null!)
            yourListing.fauxKeywords.Add(fauxWord);
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static TerminalNode CreateNode(ManagedConfig managedBool, string keyWord, MainListing yourModListing)
    {
        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

        Func<string> commandAction = managedBool.MainAction!;

        string nodeName;

        if (managedBool.nodeName.Length < 2)
        {
            nodeName = managedBool.ConfigItemName;
            Loggers.LogDebug("managedBool nodename is blank, using configitemname");
        }
        else
        {
            nodeName = managedBool.nodeName;
            Loggers.LogDebug($"using nodeName: {nodeName}");
        }

        bool clearText = managedBool.clearText;

        TerminalNode terminalNode = BaseCommandCreation(nodeName, keyWord, commandAction, managedBool.clearText, managedBool.CommandType, yourModListing, managedBool.price, managedBool.ConfirmAction!, managedBool.DenyAction!, managedBool.confirmText, managedBool.denyText, managedBool.alwaysInStock, managedBool.maxStock, managedBool.storeName, managedBool.reuseFunc, managedBool.itemList);

        if (terminalNode == null!)
            Loggers.WARNING("terminalNode is NULL at CreateNode!!!");

        return terminalNode!;
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void AddConfirm(string nodeName, ManagedConfig managedBool, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
    {
        confirm = BasicTerminal.CreateCompatibleNoun(nodeName, "confirm", managedBool.confirmText, managedBool.price, managedBool.ConfirmAction!, nodeListing);
        deny = BasicTerminal.CreateCompatibleNoun(nodeName, "deny", managedBool.denyText, managedBool.price, managedBool.DenyAction!, nodeListing);

    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void AddConfirm(string nodeName, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
    {
        confirm = BasicTerminal.CreateCompatibleNoun(nodeName, "confirm", confirmText, price, ConfirmAction, nodeListing);
        deny = BasicTerminal.CreateCompatibleNoun(nodeName, "deny", denyText, price, DenyAction, nodeListing);
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void AddStoreCommand(string nodeName, ref TerminalKeyword keyword, ManagedConfig managedBool, MainListing mainListing, out CompatibleNoun confirm, out CompatibleNoun deny)
    {
        if (managedBool.TerminalNode == null!)
        {
            Loggers.ERROR("node is null when adding store command!!!");
            confirm = null!;
            deny = null!;
            return;
        }

        if (managedBool.ConfirmAction != null!)
        {
            AddConfirm(nodeName, managedBool, mainListing.Listing, out confirm, out deny);
            StoreStuff(nodeName, managedBool.storeName, ref keyword, ref managedBool.TerminalNode, managedBool.price, managedBool.alwaysInStock, managedBool.maxStock, ref confirm, ref deny);
        }
        else
        {
            Loggers.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {nodeName}!");
            confirm = null!;
            deny = null!;
            return;
        }
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void AddStoreCommand(string nodeName, string storeName, ref TerminalKeyword keyword, ref TerminalNode node, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, MainListing mainListing, bool alwaysInStock, int maxStock, out CompatibleNoun confirm, out CompatibleNoun deny)
    {

        if (node == null!)
        {
            Loggers.ERROR("node is null when adding store command!!!");
            confirm = null!;
            deny = null!;
            return;
        }

        if (ConfirmAction != null!)
        {
            AddConfirm(nodeName, price, ConfirmAction, DenyAction, confirmText, denyText, mainListing.Listing, out confirm, out deny);
            StoreStuff(nodeName, storeName, ref keyword, ref node, price, alwaysInStock, maxStock, ref confirm, ref deny);
        }
        else
        {
            Loggers.ERROR($"Shop nodes NEED confirmation, but confirmAction is null for {nodeName}!");
            confirm = null!;
            deny = null!;
            return;
        }
    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static void StoreStuff(string nodeName, string storeName, ref TerminalKeyword keyword, ref TerminalNode node, int price, bool alwaysInStock, int maxStock, ref CompatibleNoun confirm, ref CompatibleNoun deny)
    {

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
        Loggers.LogDebug($"Store nodes created for {nodeName}");
    }

    public static UnlockableItem AddUnlockable(string storeName, TerminalNode node, bool alwaysInStock, int maxStock)
    {
        UnlockableItem itemToReturn;

        if (TryGetAndReturnUnlockable(storeName, out UnlockableItem returnedItem))
        {
            Loggers.LogDebug($"found matching item for {storeName}");
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
            Loggers.LogDebug($"Creating unlockable item manually for item: {storeName}");

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
        Loggers.LogDebug($"Added buy verb to {buyKeyword.word}");
        CompatibleNoun wordIsCompatNoun = new()
        {
            noun = terminalKeyword,
            result = item.shopSelectionNode
        };
        List<CompatibleNoun> buyKeywordList = [.. buyKeyword.compatibleNouns];
        buyKeywordList.Add(wordIsCompatNoun);
        Plugin.NounsAdded.Add(wordIsCompatNoun);
        buyKeyword.compatibleNouns = [.. buyKeywordList];

    }

    [Obsolete("Use AddCompatibleNoun instead")]
    public static void AddToKeyword(ref TerminalKeyword originalKeyword, ref TerminalKeyword newWord)
    {
        if (!originalKeyword.isVerb)
        {
            Plugin.Log.LogWarning("AddToKeyword called on non-verb");
            return;
        }

        newWord.defaultVerb = originalKeyword;
        Loggers.LogDebug($"Added verb {originalKeyword.word} to {newWord.word}");

        CompatibleNoun wordIsCompatNoun = new()
        {
            noun = newWord,
            result = newWord.specialKeywordResult
        };
        List<CompatibleNoun> compatibleNouns = [.. originalKeyword.compatibleNouns];
        compatibleNouns.Add(wordIsCompatNoun);
        Plugin.NounsAdded.Add(wordIsCompatNoun);
        originalKeyword.compatibleNouns = [.. compatibleNouns];
    }

    public static void AddCompatibleNoun(ref TerminalKeyword originalWord, string word, TerminalNode resultNode)
    {
        if (!originalWord.isVerb)
        {
            Loggers.WARNING($"{originalWord.word} is NOT a verb");
            return;
        }

        List<CompatibleNoun> originalNouns = [.. originalWord.compatibleNouns];

        foreach (CompatibleNoun compatibleNoun in originalNouns)
        {
            if (Misc.CompareStringsInvariant(compatibleNoun.noun.word, word))
            {
                Loggers.WARNING($"NOUN: {compatibleNoun.noun.word} already exists for WORD: {originalWord.word}");
                return;
            }
        }

        if (TryGetKeyword(word, out TerminalKeyword terminalKeyword))
        {
            CompatibleNoun newNoun = new()
            {
                noun = terminalKeyword,
                result = resultNode
            };

            originalNouns.Add(newNoun);
            originalWord.compatibleNouns = [.. originalNouns];
            Loggers.LogDebug($"Added NOUN: {newNoun.noun.word} to WORD: {originalWord.word} compatible nouns");
            Plugin.NounsAdded.Add(newNoun);
            return;
        }
        else
        {
            Loggers.WARNING($"word: {word} does not exist, unable to add as compatible noun");
        }

    }

    [Obsolete("use CommandManager class, if equivalent method does not exist will exist in the future")]
    public static TerminalNode BaseCommandCreation(string nodeName, string keyWord, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, bool alwaysInStock, int maxStock, string storeName, bool reuseFunc, string itemList)
    {
        List<TerminalKeyword> allKeywordsList = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

        if (IsCommandCreatedAlready(yourModListing.Listing, keyWord, commandAction, allKeywordsList, out TerminalKeyword outKeyword) && !reuseFunc)
            return outKeyword.specialKeywordResult;

        CheckForAndDeleteKeyWord(keyWord.ToLower());

        if (DoesNodeExist(yourModListing.Listing, commandAction, out TerminalNode existingNode) && !reuseFunc)
        {
            AddKeywordToExistingNode(keyWord, existingNode, true);
            Loggers.LogDebug($"existing node found {existingNode.name}, reusing associated func and adding additional keyword {keyWord}");
            return existingNode;
        }

        TerminalNode terminalNode = BasicTerminal.CreateNewTerminalNode();
        terminalNode.name = nodeName;
        terminalNode.displayText = nodeName;
        terminalNode.clearPreviousText = clearText;
        terminalNode.buyUnlockable = false;

        TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(nodeName + "_keyword", keyWord.ToLower());
        terminalKeyword.isVerb = false;
        terminalKeyword.specialKeywordResult = terminalNode;

        CompatibleNoun confirm = null!;
        CompatibleNoun deny = null!;

        if (CommandType == 1) //base requires confirmation setup
        {
            AddConfirm(nodeName, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing.Listing, out confirm, out deny);
            terminalNode.acceptAnything = false;
            Loggers.LogDebug("command type 1 detected, adding basic confirmation");
        }
        else if (CommandType == 2) //store command
        {
            AddStoreCommand(nodeName, storeName, ref terminalKeyword, ref terminalNode, price, ConfirmAction, DenyAction, confirmText, denyText, yourModListing, alwaysInStock, maxStock, out confirm, out deny);
            Loggers.LogDebug("command type 2 detected, adding store logic");
            terminalNode.acceptAnything = false;
            yourModListing.shopNodes.Add(confirm.result);
            yourModListing.shopNodes.Add(terminalNode);
            yourModListing.shopNodes.Add(deny.result);
            if (itemList.Length > 1)
            {
                confirm.result.buyUnlockable = false;
                yourModListing.storePacks.Add(terminalNode, itemList);
                Loggers.LogDebug("storepack detected, adding itemlist");
            }
        }

        if (confirm != null && deny != null!)
        {
            allKeywordsList.Add(confirm.noun);
            Plugin.KeywordsAdded.Add(confirm.noun);

            allKeywordsList.Add(deny.noun);
            Plugin.KeywordsAdded.Add(deny.noun);

            terminalNode.terminalOptions = [confirm, deny];
            terminalNode.overrideOptions = true;

        }
        else
            Loggers.LogDebug($"no confirmation logic added for {keyWord}");

        yourModListing.Listing.Add(terminalNode, commandAction);
        allKeywordsList.Add(terminalKeyword);

        if (!Plugin.NodesAdded.Contains(terminalNode))
            Plugin.NodesAdded.Add(terminalNode);

        if (!Plugin.KeywordsAdded.Contains(terminalKeyword))
            Plugin.KeywordsAdded.Add(terminalKeyword);

        Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        return terminalNode;
    }

    [Obsolete("use AddCompatibleNoun")]
    public static void AddNounWordSimple(string originalVerb, string nodeName, string keyWord, string displayText, bool clearText)
    {
        //this never worked to begin with
    }
}

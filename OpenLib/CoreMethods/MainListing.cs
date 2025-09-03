using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.Menus;
using System;
using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;

namespace OpenLib.CoreMethods
{
    public class MainListing
    {
        //Main
        //public int count = 0;
        public List<TerminalNode> terminalNodes = [];
        public List<TerminalKeyword> terminalKeywords = [];
        public Dictionary<TerminalNode, Func<string>> Listing = [];
        public List<TerminalNode> shopNodes = [];
        public List<FauxKeyword> fauxKeywords = [];

        //Special stuff
        public Dictionary<TerminalNode, int> specialListNum = [];
        public Dictionary<string, TerminalNode> specialListString = [];
        public Dictionary<int, string> ListNumToString = [];
        public Dictionary<TerminalNode, string> storePacks = [];

        public void DeleteAll()
        {
            //count = 0;
            terminalKeywords.Clear();
            terminalNodes.Clear();
            Listing.Clear();
            shopNodes.Clear();
            specialListNum.Clear();
            specialListString.Clear();
            ListNumToString.Clear();
            storePacks.Clear();
            fauxKeywords.Clear();
        }
    }

    public class CommandRegistry
    {
        public static void InitListing(ref MainListing listingName)
        {
            listingName ??= new MainListing();
            listingName.terminalNodes = [];
            listingName.terminalKeywords = [];
            listingName.Listing = [];
            listingName.shopNodes = [];
            listingName.specialListNum = [];
            listingName.specialListString = [];
            listingName.ListNumToString = [];

            if (listingName == null)
                Loggers.ERROR("InitListing still null");

        }

        public static void GetCommandsToAdd(List<ManagedConfig> managedBools, MainListing listingName)
        {
            Loggers.LogInfo("GetCommandsToAdd");
            if (managedBools == null || listingName == null)
            {
                Loggers.LogDebug("params are null");
                return;
            }

            Loggers.LogDebug($"listing count: {listingName.Listing.Count}");

            foreach (ManagedConfig m in managedBools)
            {
                if (m.BoolValue)
                {
                    Loggers.LogDebug("configvalue is true");

                    TerminalMenuItem matchItem = MakeMenuItem(m);
                    if (matchItem != null)
                        m.menuItem = matchItem;

                    Loggers.LogInfo($"{m.ConfigItemName} found in managed bools and is active");
                    if (m.KeywordList != null)
                    {
                        AddCommandKeyword(m, listingName);
                        if (Misc.CompareStringsInvariant(m.categoryText, "other"))
                        {
                            if (!LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode))
                            {
                                Loggers.WARNING($"Unable to add {m.configDescription} to OtherCommands\nOtherCommands TerminalNode could not be found!");
                            }
                            else
                                AddingThings.AddToExistingNodeText($"\n{m.configDescription}", ref otherNode);
                        }
                    }
                }
                else
                {
                    Loggers.LogDebug("configvalue is false, deleting menuItem if not null");
                    m.menuItem?.Delete();
                }
            }
        }

        public static void AddCommandKeyword(ManagedConfig managedBool, MainListing listingName)
        {
            if (managedBool == null)
            {
                Loggers.ERROR("managedBool is null @AddCommandKeyword()");
                return;
            }

            if (managedBool.KeywordList.Count == 0)
            {
                Loggers.LogDebug($"KeywordList Count = 0 for {managedBool.ConfigItemName}");
                return;
            }

            Loggers.LogDebug("AddCommandKeyword starting:");

            foreach (string keyword in managedBool.KeywordList)
            {
                Loggers.LogDebug($"adding {keyword}");
                GenerateInfoText(managedBool);
                managedBool.TerminalNode = AddingThings.CreateNode(managedBool, keyword, listingName);

                if (DynamicBools.TryGetKeyword("info", out TerminalKeyword infoWord))
                    AddingThings.InfoText(managedBool, keyword, infoWord, listingName);

                if (managedBool.specialNum != -1 && !listingName.specialListNum.ContainsKey(managedBool.TerminalNode)) //viewnodes
                {
                    listingName.specialListNum.Add(managedBool.TerminalNode, managedBool.specialNum);
                    listingName.ListNumToString.Add(managedBool.specialNum, managedBool.specialString);
                    Loggers.LogInfo($"Added viewnode types to dictionaries, {managedBool.specialNum}");
                }
                else if (managedBool.specialString.Length > 1) //dynamic commands (take any input)
                {
                    listingName.specialListString.Add(keyword, managedBool.TerminalNode);
                    Loggers.LogInfo($"mapping keyword{keyword} for {managedBool.specialString} node");
                }
            }
        }

        public static void GenerateInfoText(ManagedConfig managedBool)
        {
            if (managedBool == null)
            {
                Loggers.ERROR("managedBool is null @GenerateInfoText()");
                return;
            }

            if (managedBool.KeywordList.Count == 0)
            {
                Loggers.LogDebug($"KeywordList Count = 0 for {managedBool.ConfigItemName}");
                return;
            }

            if (managedBool.InfoAction != null)
                return;

            if (managedBool.InfoText.Length > 0)
                return;

            if (managedBool.menuItem == null)
                Loggers.LogDebug("no menu items to grab description from");
            else
                managedBool.DefaultInfoText();
        }

        public static void AddSpecialListString(ref MainListing listingName, TerminalNode node, string special)
        {
            if (listingName.Listing.ContainsKey(node))
            {
                if (!listingName.specialListString.TryAdd(special, node))
                {
                    Loggers.WARNING($"Listing already contains special string key {special}");
                    return;
                }
                else
                {
                    Loggers.LogDebug($"{node.name} added to special string listing with key {special}");
                }
            }

        }
    }
}

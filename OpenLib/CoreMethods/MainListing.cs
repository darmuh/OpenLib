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
        public Dictionary<TerminalNode,Func<string>> Listing = [];
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
                Plugin.ERROR("InitListing still null");
           
        }

        public static void GetCommandsToAdd(List<ManagedConfig> managedBools, MainListing listingName)
        {
            Plugin.MoreLogs("GetCommandsToAdd");
            if(managedBools == null || listingName == null)
            {
                Plugin.Spam("params are null");
                return;
            }

            Plugin.Spam($"listing count: {listingName.Listing.Count}");

            foreach(ManagedConfig m in managedBools)
            {
                if(m.BoolValue)
                {
                    Plugin.Spam("configvalue is true");

                    TerminalMenuItem matchItem = MakeMenuItem(m);
                    if (matchItem != null)
                        m.menuItem = matchItem;

                    Plugin.MoreLogs($"{m.ConfigItemName} found in managed bools and is active");
                    if(m.KeywordList != null)
                    {
                        AddCommandKeyword(m, listingName);
                        if(m.categoryText.ToLower() == "other")
                        {
                            if (!LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode))
                            {
                                Plugin.WARNING($"Unable to add {m.configDescription} to OtherCommands\nOtherCommands TerminalNode could not be found!");
                            }
                            else
                                AddingThings.AddToExistingNodeText($"\n{m.configDescription}", ref otherNode);
                        }
                    }      
                }
                else
                {
                    Plugin.Spam("configvalue is false, deleting menuItem if not null");
                    m.menuItem?.Delete();
                }
            }
        }

        public static void AddCommandKeyword(ManagedConfig managedBool, MainListing listingName)
        {
            if(managedBool == null)
            {
                Plugin.ERROR("managedBool is null @AddCommandKeyword()");
                return;
            }

            if (managedBool.KeywordList.Count == 0)
            {
                Plugin.Spam($"KeywordList Count = 0 for {managedBool.ConfigItemName}");
                return;
            }

            Plugin.Spam("AddCommandKeyword starting:");
                
            foreach (string keyword in managedBool.KeywordList)
            {
                Plugin.Spam($"adding {keyword}");
                GenerateInfoText(managedBool);
                managedBool.TerminalNode = AddingThings.CreateNode(managedBool, keyword, listingName);
                
                if(DynamicBools.TryGetKeyword("info", out TerminalKeyword infoWord))
                    AddingThings.InfoText(managedBool, keyword, infoWord, listingName);
                
                if (managedBool.specialNum != -1 && !listingName.specialListNum.ContainsKey(managedBool.TerminalNode)) //viewnodes
                {
                    listingName.specialListNum.Add(managedBool.TerminalNode, managedBool.specialNum);
                    listingName.ListNumToString.Add(managedBool.specialNum, managedBool.specialString);
                    Plugin.MoreLogs($"Added viewnode types to dictionaries, {managedBool.specialNum}");
                }
                else if(managedBool.specialString.Length > 1) //dynamic commands (take any input)
                {
                    listingName.specialListString.Add(keyword, managedBool.TerminalNode);
                    Plugin.MoreLogs($"mapping keyword{keyword} for {managedBool.specialString} node");
                }
            }
        }

        public static void GenerateInfoText(ManagedConfig managedBool)
        {
            if (managedBool == null)
            {
                Plugin.ERROR("managedBool is null @GenerateInfoText()");
                return;
            }

            if (managedBool.KeywordList.Count == 0)
            {
                Plugin.Spam($"KeywordList Count = 0 for {managedBool.ConfigItemName}");
                return;
            }

            if (managedBool.InfoAction != null)
                return;

            if (managedBool.InfoText.Length > 0)
                return;

            if (managedBool.menuItem == null)
                Plugin.Spam("no menu items to grab description from");
            else
                managedBool.DefaultInfoText();
        }
    }
}

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
        public int count = 0;
        public List<TerminalNode> terminalNodes = [];
        public List<TerminalKeyword> terminalKeywords = [];
        public Dictionary<TerminalNode,Func<string>> Listing = [];
        public List<TerminalNode> shopNodes = [];

        //Special stuff
        public Dictionary<TerminalNode, int> specialListNum = [];
        public Dictionary<string, TerminalNode> specialListString = [];
        public Dictionary<int, string> ListNumToString = [];
        public Dictionary<TerminalNode, string> storePacks = [];

        public void DeleteAll()
        {
            count = 0;
            terminalKeywords.Clear();
            terminalNodes.Clear();
            Listing.Clear();
            shopNodes.Clear();
            specialListNum.Clear();
            specialListString.Clear();
            ListNumToString.Clear();
            storePacks.Clear();
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
            listingName.count = 0;

            if (listingName == null)
                Plugin.ERROR("InitListing still null");
           
        }

        public static void GetCommandsToAdd(List<ManagedBool> managedBools, MainListing listingName)
        {
            Plugin.MoreLogs("GetCommandsToAdd");
            if(managedBools == null || listingName == null)
            {
                Plugin.Spam("params are null");
                return;
            }

            Plugin.Spam($"listing count: {listingName.count}");

            foreach(ManagedBool m in managedBools)
            {
                if(m.ConfigValue)
                {

                    Plugin.Spam("configvalue is true");

                    TerminalMenuItem matchItem = MakeMenuItem(m);
                    if (matchItem != null)
                        m.menuItem = matchItem;

                    listingName.count++;
                    Plugin.MoreLogs($"{m.ConfigItemName} found in managed bools and is active");
                    if(m.KeywordList != null)
                        AddCommandKeyword(m, listingName);
                }
                else
                    Plugin.MoreLogs($"{m.ConfigItemName} found in managed bools and is NOT active");
            }
        }

        public static void AddCommandKeyword(ManagedBool managedBool, MainListing listingName)
        {
            if(managedBool == null)
            {
                Plugin.ERROR("managedBool is null @AddCommandKeyword()");
                return;
            }

            if (managedBool.KeywordList.Count < 1)
            {
                Plugin.ERROR($"KEYWORD LIST COUNT < 1 FOR {managedBool.ConfigItemName}");
                return;
            }

            Plugin.Spam("AddCommandKeyword starting:");
                
            foreach (string keyword in managedBool.KeywordList)
            {
                Plugin.Spam($"adding {keyword}");
                managedBool.TerminalNode = AddingThings.CreateNode(managedBool, keyword, listingName);
                
                if(!Plugin.nodesAdded.Contains(managedBool.TerminalNode))
                    Plugin.nodesAdded.Add(managedBool.TerminalNode);

                if(managedBool.specialNum != -1 && !listingName.specialListNum.ContainsKey(managedBool.TerminalNode)) //viewnodes
                {
                    listingName.specialListNum.Add(managedBool.TerminalNode, managedBool.specialNum);
                    listingName.ListNumToString.Add(managedBool.specialNum, managedBool.specialString);
                    Plugin.MoreLogs($"Added viewnode types to dictionaries, {managedBool.specialNum}");
                }
                else if(managedBool.specialString.Length > 1 && !listingName.specialListString.ContainsKey(managedBool.specialString)) //dynamic
                {
                    listingName.specialListString.Add(managedBool.specialString, managedBool.TerminalNode);
                    Plugin.MoreLogs("Adding specialString to node");
                }
            }

        }
    }
}

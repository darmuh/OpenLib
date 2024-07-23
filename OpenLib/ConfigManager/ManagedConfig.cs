﻿using OpenLib.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLib.ConfigManager
{
    public class ManagedConfig
    {
        //MAIN
        public string ConfigItemName;
        public bool RequiresNetworking;
        public int ConfigType; //0 = bool, 1 = string, 2 = int

        //ManagedString
        public string StringValue; //for string config items

        //ManagedInt
        public int IntValue;

        //ManagedBool
        public bool BoolValue = false;
        public List<string> KeywordList;
        public Func<string> MainAction;
        public int CommandType; //0 base, 1 base confirm, 2 store node
        public bool clearText;
        public bool alwaysInStock;
        public bool reuseFunc;
        public int maxStock;
        public int price;
        public int specialNum;
        public string nodeName;
        public string storeName;
        public string itemList;
        public string specialString;
        public string confirmText;
        public string denyText;
        public string categoryText;
        public string configDescription;
        public Func<string> ConfirmAction;
        public Func<string> DenyAction;

        //for menus
        public TerminalMenuItem menuItem;

        //resulting possible objects from this managedItem
        public TerminalNode TerminalNode;
        public UnlockableItem UnlockableItem;

        //related items
        public ManagedConfig relatedConfigItem;

        public void ConfigChange(bool newValue)
        {
            if (newValue)
            {
                this.BoolValue = newValue;
            }
            else
            {
                this.BoolValue = newValue;
                //this.menuItem?.Delete(); //remove menu item
            }
        }

        public void ConfigChange(string newValue)
        {
            if (newValue != this.StringValue)
            {
                this.StringValue = newValue;
                Plugin.Spam($"Updating string value for managed item {this.ConfigItemName}");
            }
        }

        public void SetManagedBoolValues(string configItemName, bool isEnabled, string descrip, bool isNetworked = false, string category = "", List<string> keywordList = null, Func<string> mainAction = null, int commandType = 0, bool clear = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmTxt = "confirm", string denyTxt = "deny", string special = "", int specialInt = -1, string nodestring = "", string items = "", int value = 0, string storeString = "", bool inStock = true, int stockMax = 0, bool reuseFnc = false)
        {
            BoolValue = isEnabled;
            MainAction = mainAction;
            KeywordList = keywordList;
            ConfigItemName = configItemName;
            RequiresNetworking = isNetworked;
            price = value;
            CommandType = commandType;
            clearText = clear;
            ConfirmAction = confirmAction;
            DenyAction = denyAction;
            confirmText = confirmTxt;
            denyText = denyTxt;
            specialNum = specialInt;
            specialString = special;
            itemList = items;
            storeName = storeString;
            alwaysInStock = inStock;
            maxStock = stockMax;
            nodeName = nodestring;
            categoryText = category;
            configDescription = descrip;
            reuseFunc = reuseFnc;
        }

    }

    public class ManagedBoolGet
    {
        public static bool TryGetItemByName(List<ManagedConfig> managedBools, string query, out ManagedConfig result)
        {
            if (managedBools.Count == 0)
            {
                Plugin.Spam("managedBools count = 0");
                result = null;
                return false;
            }

            Plugin.Spam($"TryGetItemByName: {query}");

            foreach (ManagedConfig item in managedBools)
            {
                if (item.ConfigItemName == query)
                {
                    result = item;
                    Plugin.Spam($"{query} found in managedBool, returning true");
                    return true;
                }
            }
            Plugin.Spam($"{query} not found");
            result = null;
            return false;
        }

        public static bool CanAddToManagedBoolList(List<ManagedConfig> managedBools, string nodeName)
        {
            foreach (ManagedConfig m in managedBools)
            {
                if (m.ConfigItemName == nodeName)
                {
                    Plugin.Log.LogWarning($"Tried to add {nodeName} to managedBools list when it's already in it!");
                    return false;
                }
            }
            Plugin.Spam("node is not in managedbool list and can be added!");
            return true;
        }

        public static bool TryGetItemByName(List<ManagedConfig> managedBools, string query) //no out needed overload
        {
            foreach (ManagedConfig item in managedBools)
            {
                if (item.ConfigItemName == query)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
}

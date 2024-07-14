using OpenLib.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLib.ConfigManager
{
    public class ManagedBool
    {
        //MAIN
        public bool ConfigValue = false;
        public string ConfigItemName;
        public List<string> KeywordList;
        public Func<string> MainAction;
        public int CommandType; //0 base, 1 base confirm, 2 store node

        //Optional
        public bool defaultValue { get; internal set; }
        public bool RequiresNetworking;
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

        //resulting objects from this bool
        public TerminalNode TerminalNode;
        //public TerminalKeyword TerminalKeyword; //commented out since we will have multiple of these
        public UnlockableItem UnlockableItem;

    }

    public class ManagedBoolGet
    {
        public static bool TryGetItemByName(List<ManagedBool> managedBools, string query, out ManagedBool result)
        {
            if (managedBools.Count == 0)
            {
                Plugin.Spam("managedBools count = 0");
                result = null;
                return false;
            }

            Plugin.Spam($"TryGetItemByName: {query}");

            foreach (ManagedBool item in managedBools)
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

        public static bool CanAddToManagedBoolList(List<ManagedBool> managedBools, string nodeName)
        {
            foreach (ManagedBool m in managedBools)
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

        public static bool TryGetItemByName(List<ManagedBool> managedBools, string query) //no out needed overload
        {
            foreach (ManagedBool item in managedBools)
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

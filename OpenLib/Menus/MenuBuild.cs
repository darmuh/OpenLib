using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static OpenLib.Common.CommonStringStuff;

namespace OpenLib.Menus
{
    public class MenuBuild
    {
        public static bool isNextEnabled = false;
        public static int nextCount = 1;
        public static string currentCategory = "";
        public static TerminalMenu currentMenu;
        public static List<TerminalMenu> allMenus = [];

        public static List<TerminalMenuCategory> InitCategories(Dictionary<string, string> CategoryItems)
        {
            Plugin.Spam("InitCategories START");
            List<TerminalMenuCategory> myCategories = [];

            if (CategoryItems.Count < 1)
                return myCategories;

            foreach (KeyValuePair<string, string> item in CategoryItems)
            {
                TerminalMenuCategory newCategory = new()
                {
                    CatName = item.Key,
                    CatDescription = item.Value
                };
                myCategories.Add(newCategory);
            }
            Plugin.Spam("InitCategories SUCCESS");
            return myCategories;
        }

        public static bool ShouldAddCategoryNameToMainMenu(List<TerminalMenuItem> menuItems, string categoryName)
        {
            foreach (TerminalMenuItem item in menuItems)
            {
                if (item.Category == categoryName)
                    return true;
                else
                    continue;
            }
            return false;
        }

        public static List<TerminalMenuItem> TerminalMenuItems(List<ManagedConfig> managedBools)
        {
            List<TerminalMenuItem> myMenuItems = [];

            managedBools.RemoveAll(m => m == null); //remove null entries from list

            foreach (ManagedConfig m in managedBools)
            {

                if (m.menuItem == null)
                    continue;

                if (m.KeywordList == null)
                    continue;

                if (m.KeywordList.Count > 0)
                {
                    myMenuItems.Add(m.menuItem);
                }
            }
            Plugin.Spam("\n\n\n");
            Plugin.Spam($"myMenuItems count: {myMenuItems.Count}");
            Plugin.Spam("\n\n\n");
            return myMenuItems;
        }

        public static TerminalMenu AssembleMainMenu(string menuName, string keyword, string mainMenuText, List<TerminalMenuCategory> categoryList, List<TerminalMenuItem> menuItems, bool addToOther = false, string menuDescription = "")
        {
            TerminalMenu thisMenu = new()
            {
                MenuName = menuName,
                setKeyword = keyword,
                Categories = categoryList,
                MainMenuText = mainMenuText, //Welcome to darmuh's Terminal Upgrade!\r\n\tSee below Categories for new stuff :)
                menuItems = menuItems,
                currentCategory = "",
                nextCount = 1,
                isNextEnabled = false
            };
            string displayText = AssembleMainMenuText(thisMenu);

            if (addToOther)
            {
                AddingThings.AddBasicCommand($"{thisMenu.MenuName}_main", thisMenu.setKeyword, displayText, false, true, "other", menuDescription);
            }
            else
            {
                AddingThings.AddBasicCommand($"{thisMenu.MenuName}_main", thisMenu.setKeyword, displayText, false, true);
            }

            allMenus.Add(thisMenu);
            return thisMenu;
        }

        public static bool InMainMenu(TerminalNode terminalNode, TerminalMenu terminalMenu)
        {
            if (terminalMenu == null)
            {
                Plugin.ERROR("ERROR: OpenLib menu is NULL, most likely failed to create!");
                return false;
            }

            if (terminalNode.name.Contains(terminalMenu.MenuName))
            {
                terminalMenu.isActive = true;
                terminalMenu.nextCount = 1;
                terminalMenu.currentCategory = "";
                Plugin.Spam($"In main menu of {terminalMenu.MenuName}");
                return true;
            }
            else if (terminalMenu.isNextEnabled && terminalMenu.terminalNodes.Contains(terminalNode))
            {
                Plugin.Spam("Still in menus but not main, next is enabled");
                return false;
            }
            else
            {
                terminalMenu.isActive = false;
                return false;
            }

        }

        public static string AssembleMainMenuText(TerminalMenu terminalMenu)
        {
            StringBuilder assembler = new();
            assembler.Append($"{terminalMenu.MainMenuText}\r\n\r\n");
            if (terminalMenu.Categories.Count > 0)
            {
                foreach (TerminalMenuCategory category in terminalMenu.Categories)
                {
                    assembler.Append($"[{category.CatName.ToUpper()}]\r\n{category.CatDescription}\r\n\r\n");
                }
            }

            return assembler.ToString();
        }

        public static string AssembleMainMenuText(string MainMenuText, Dictionary<string, string> Categories)
        {
            StringBuilder assembler = new();
            assembler.Append($"{MainMenuText}\r\n\r\n");
            if (Categories.Count > 0)
            {
                foreach (KeyValuePair<string, string> category in Categories)
                {
                    assembler.Append($"[{category.Key.ToUpper()}]\r\n{category.Value}\r\n\r\n");
                }
            }

            return assembler.ToString();
        }

        public static void CreateCategoryCommands(TerminalMenu terminalMenu, MainListing yourModListing)
        {
            //Plugin.Spam("CreateCategoryCommands START");
            List<Dictionary<string, List<string>>> categoryLists = [];

            foreach (TerminalMenuCategory category in terminalMenu.Categories)
            {
                Plugin.Spam("checking category in terminalMenu.categories");
                Dictionary<string, List<string>> catListing = MakeCategoryList(category, terminalMenu.menuItems);
                if (!categoryLists.Contains(catListing))
                    categoryLists.Add(catListing);
                TerminalNode menuNode = AddingThings.CreateNode(terminalMenu, $"{category.CatName}", category.CatName.ToLower(), GetFirstInList, yourModListing);
                terminalMenu.terminalNodes.Add(menuNode);
            }
            terminalMenu.categoryLists = categoryLists;
            TerminalNode nextNode = AddingThings.CreateNode(terminalMenu, "nextInMenu", "next", NextInList, yourModListing, true);
            terminalMenu.terminalNodes.Add(nextNode);
        }

        public static void UpdateCategories(TerminalMenu myMenu)
        {
            List<Dictionary<string, List<string>>> categoryLists = [];

            foreach (TerminalMenuCategory category in myMenu.Categories)
            {
                Plugin.Spam("checking category in myMenu.categories");
                Dictionary<string, List<string>> catListing = MakeCategoryList(category, myMenu.menuItems);
                if (!categoryLists.Contains(catListing))
                    categoryLists.Add(catListing);
            }
            myMenu.categoryLists = categoryLists;
        }

        public static List<string> GetCategoryList(string catName, out TerminalMenu menuName)
        {
            Plugin.Spam("2.1");
            List<string> empty = [];
            foreach (TerminalMenu terminalMenu in allMenus)
            {
                Plugin.Spam("2.2");
                if (!terminalMenu.isActive)
                    continue;
                for (int i = 0; i < terminalMenu.Categories.Count; i++)
                {
                    Plugin.Spam("2.3");
                    foreach (KeyValuePair<string, List<string>> catList in terminalMenu.categoryLists[i])
                    {
                        if (catList.Key.ToLower() == catName.ToLower())
                        {
                            Plugin.Spam("categorylist found!!!");
                            menuName = terminalMenu;
                            return catList.Value;
                        }
                    }
                }

            }

            Plugin.Spam("2.1 FAIL");
            menuName = null;
            return empty;
        }

        public static Dictionary<string, List<string>> MakeCategoryList(TerminalMenuCategory category, List<TerminalMenuItem> terminalMenuItems)
        {
            Plugin.Spam("MakeCategoryList START");
            string catName = category.CatName;
            Plugin.Spam(catName);
            List<string> catItems = [];
            Dictionary<string, List<string>> categoryList = [];
            Plugin.Spam($"count: {terminalMenuItems.Count}");

            foreach (TerminalMenuItem menuItem in terminalMenuItems)
            {
                Plugin.Spam($"checking {menuItem.ItemName}");
                if (menuItem.Category.ToLower() == catName.ToLower())
                {
                    catItems.Add($"> {GetKeywordsForMenuItem(menuItem.itemKeywords)}\r\n{menuItem.itemDescription}\r\n");
                    Plugin.Spam($"{GetKeywordsForMenuItem(menuItem.itemKeywords)} added");
                    Plugin.Spam($"{menuItem.itemDescription} added too!");
                }
            }
            Plugin.Spam("setting catName list");
            categoryList.Add(catName, catItems);
            Plugin.Spam("MakeCategoryList END");
            return categoryList;
        }

        public static string NextInList()
        {
            if (!isNextEnabled)
            {
                string displayText = "Not currently in any menus...\r\n\r\n";
                return displayText;
            }
            else
            {
                Plugin.Spam($"currentCategory = {currentCategory}");
                nextCount++;
                List<string> currentList = GetCategoryList(currentCategory, out TerminalMenu menuName);
                if (menuName == null)
                {
                    string fail = "ERROR: Unable to get current category!\r\n\r\n";
                    return fail;
                }

                menuName.isActive = true;
                menuName.nextCount = nextCount;
                menuName.currentCategory = currentCategory;
                string displayText = GetNextPage(currentList, currentCategory, 4, nextCount, out isNextEnabled);
                Plugin.Spam($"currentCategory:{currentCategory} nextCount: {nextCount} isNextEnabled: {isNextEnabled}");
                menuName.isNextEnabled = isNextEnabled;
                return displayText;
            }
        }

        public static string GetFirstInList()
        {
            Plugin.Spam("1");
            nextCount = 1;
            currentCategory = GetCategoryFromNode(CommonTerminal.parseNode); //grabbing the node currently being parsed
            Plugin.Spam("2");
            List<string> currentList = GetCategoryList(currentCategory, out TerminalMenu menuName);
            menuName.isActive = true;
            menuName.nextCount = nextCount;
            menuName.currentCategory = currentCategory;
            Plugin.Spam("3");
            string displayText = GetNextPage(currentList, currentCategory, 4, 1, out isNextEnabled);
            menuName.isNextEnabled = isNextEnabled;
            Plugin.Spam("4");
            return displayText;
        }

        public static string GetCategoryFromNode(TerminalNode givenNode)
        {
            Plugin.Spam("1.1");
            if (givenNode == null)
            {
                Plugin.ERROR("givenNode is null!!!!");
                return "";
            }
            foreach (TerminalMenu terminalMenu in allMenus)
            {
                if (!terminalMenu.terminalNodePerCategory.ContainsValue(givenNode))
                {
                    Plugin.Spam($"menu does not contain node {givenNode.name}");
                    terminalMenu.isActive = false;
                    continue;
                }

                else
                {
                    foreach (KeyValuePair<string, TerminalNode> pair in terminalMenu.terminalNodePerCategory)
                    {
                        if (pair.Value == givenNode)
                        {
                            Plugin.Spam($"FOUND NODE AND PAIR {pair.Key}");
                            terminalMenu.isActive = true;
                            return pair.Key;
                        }
                    }
                }
            }

            Plugin.ERROR("COULD NOT FIND NODE???");
            return "";
        }

        public static TerminalMenuItem MakeMenuItem(ManagedConfig managedBool)
        {
            if (managedBool.categoryText != "")
            {
                TerminalMenuItem menuItem = new()
                {
                    itemKeywords = managedBool.KeywordList,
                    itemDescription = managedBool.configDescription,
                    ItemName = managedBool.ConfigItemName,
                    Category = managedBool.categoryText,
                };

                return menuItem;
            }
            return null;
        }

        public static TerminalMenuItem MakeMenuItem(string categoryText, List<string> keywordList, string configDescription, string itemName)
        {
            if (categoryText != "")
            {
                TerminalMenuItem menuItem = new()
                {
                    itemKeywords = keywordList,
                    itemDescription = configDescription,
                    ItemName = itemName,
                    Category = categoryText,
                };

                return menuItem;
            }

            Plugin.WARNING("Empty categoryText, Unable to create TerminalMenuItem! (null return)");
            return null;
        }
    }
}

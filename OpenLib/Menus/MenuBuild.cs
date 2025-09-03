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
        public static TerminalMenu currentMenu = null!;
        public static List<TerminalMenu> allMenus = [];

        public static List<TerminalMenuCategory> InitCategories(Dictionary<string, string> CategoryItems)
        {
            Loggers.LogDebug("InitCategories START");
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
            Loggers.LogDebug("InitCategories SUCCESS");
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
            Loggers.LogDebug("\n\n\n");
            Loggers.LogDebug($"myMenuItems count: {myMenuItems.Count}");
            Loggers.LogDebug("\n\n\n");
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
                Loggers.ERROR("ERROR: OpenLib menu is NULL, most likely failed to create!");
                return false;
            }

            if (terminalNode.name.Contains(terminalMenu.MenuName))
            {
                terminalMenu.isActive = true;
                terminalMenu.nextCount = 1;
                terminalMenu.currentCategory = "";
                Loggers.LogDebug($"In main menu of {terminalMenu.MenuName}");
                return true;
            }
            else if (terminalMenu.isNextEnabled && terminalMenu.terminalNodes.Contains(terminalNode))
            {
                Loggers.LogDebug("Still in menus but not main, next is enabled");
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
            //Loggers.LogDebug("CreateCategoryCommands START");
            List<Dictionary<string, List<string>>> categoryLists = [];

            foreach (TerminalMenuCategory category in terminalMenu.Categories)
            {
                Loggers.LogDebug("checking category in terminalMenu.categories");
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

        public static void CreateCategoryFauxCommands(TerminalMenu terminalMenu, MainListing yourModListing)
        {
            //Loggers.LogDebug("CreateCategoryCommands START");
            List<Dictionary<string, List<string>>> categoryLists = [];

            foreach (TerminalMenuCategory category in terminalMenu.Categories)
            {
                Loggers.LogDebug("checking category in terminalMenu.categories");
                Dictionary<string, List<string>> catListing = MakeCategoryList(category, terminalMenu.menuItems);
                if (!categoryLists.Contains(catListing))
                    categoryLists.Add(catListing);
                FauxKeyword menuFauxNode = new("more", category.CatName, GetFirstInList)
                {
                    AllowOtherFauxWords = true,
                    requireExact = true
                };

                AddingThings.AddToFauxListing(menuFauxNode, yourModListing);
            }
            terminalMenu.categoryLists = categoryLists;

            FauxKeyword menuFauxNext = new("more", "next", NextInList)
            {
                AllowOtherFauxWords = true
            };
            AddingThings.AddToFauxListing(menuFauxNext, yourModListing);
        }

        public static void UpdateCategories(TerminalMenu myMenu)
        {
            List<Dictionary<string, List<string>>> categoryLists = [];

            foreach (TerminalMenuCategory category in myMenu.Categories)
            {
                Loggers.LogDebug("checking category in myMenu.categories");
                Dictionary<string, List<string>> catListing = MakeCategoryList(category, myMenu.menuItems);
                if (!categoryLists.Contains(catListing))
                    categoryLists.Add(catListing);
            }
            myMenu.categoryLists = categoryLists;
        }

        public static List<string> GetCategoryList(string catName, out TerminalMenu menuName)
        {
            Loggers.LogDebug("2.1");
            List<string> empty = [];
            foreach (TerminalMenu terminalMenu in allMenus)
            {
                Loggers.LogDebug("2.2");
                if (!terminalMenu.isActive)
                    continue;
                for (int i = 0; i < terminalMenu.Categories.Count; i++)
                {
                    Loggers.LogDebug("2.3");
                    foreach (KeyValuePair<string, List<string>> catList in terminalMenu.categoryLists[i])
                    {
                        if (Misc.CompareStringsInvariant(catList.Key, catName))
                        {
                            Loggers.LogDebug("categorylist found!!!");
                            menuName = terminalMenu;
                            return catList.Value;
                        }
                    }
                }

            }

            Loggers.LogDebug("2.1 FAIL");
            menuName = null;
            return empty;
        }

        public static Dictionary<string, List<string>> MakeCategoryList(TerminalMenuCategory category, List<TerminalMenuItem> terminalMenuItems)
        {
            Loggers.LogDebug("MakeCategoryList START");
            string catName = category.CatName;
            Loggers.LogDebug(catName);
            List<string> catItems = [];
            Dictionary<string, List<string>> categoryList = [];
            Loggers.LogDebug($"count: {terminalMenuItems.Count}");

            foreach (TerminalMenuItem menuItem in terminalMenuItems)
            {
                Loggers.LogDebug($"checking {menuItem.ItemName}");
                if (Misc.CompareStringsInvariant(menuItem.Category, catName))
                {
                    catItems.Add($"> {GetKeywordsForMenuItem(menuItem.itemKeywords)}\r\n{menuItem.itemDescription}\r\n");
                    Loggers.LogDebug($"{GetKeywordsForMenuItem(menuItem.itemKeywords)} added");
                    Loggers.LogDebug($"{menuItem.itemDescription} added too!");
                }
            }
            Loggers.LogDebug("setting catName list");
            categoryList.Add(catName, catItems);
            Loggers.LogDebug("MakeCategoryList END");
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
                Loggers.LogDebug($"currentCategory = {currentCategory}");
                nextCount++;
                GetCategoryFromString(currentCategory);
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
                Loggers.LogDebug($"currentCategory:{currentCategory} nextCount: {nextCount} isNextEnabled: {isNextEnabled}");
                menuName.isNextEnabled = isNextEnabled;
                return displayText;
            }
        }

        public static string GetFirstInList()
        {
            Loggers.LogDebug("1");
            nextCount = 1;
            string screen = Plugin.instance.Terminal.screenText.text[^Plugin.instance.Terminal.textAdded..];
            currentCategory = GetCategoryFromString(screen);
            Loggers.LogDebug($"currentCategory detected as: [{currentCategory}]");
            //currentCategory = GetCategoryFromNode(CommonTerminal.parseNode); //grabbing the node currently being parsed
            Loggers.LogDebug("2");
            List<string> currentList = GetCategoryList(currentCategory, out TerminalMenu menuName);
            menuName.isActive = true;
            menuName.nextCount = nextCount;
            menuName.currentCategory = currentCategory;
            Loggers.LogDebug("3");
            string displayText = GetNextPage(currentList, currentCategory, 4, 1, out isNextEnabled);
            menuName.isNextEnabled = isNextEnabled;
            Loggers.LogDebug("4");
            return displayText;
        }

        public static string GetCategoryFromString(string input)
        {
            Loggers.LogDebug($"Getting Category from string: [{input}]");
            foreach (TerminalMenu terminalMenu in allMenus)
            {
                if (terminalMenu.categoryLists.Any(c => c.Any(d => Misc.CompareStringsInvariant(d.Key, input))))
                {
                    Loggers.LogDebug($"detected menu with categoryList containing string {input}!!");
                    terminalMenu.isActive = true;
                    int dictIndex = terminalMenu.categoryLists.FindIndex(c => c.Any(d => Misc.CompareStringsInvariant(d.Key, input)));
                    return terminalMenu.categoryLists[dictIndex].First(d => Misc.CompareStringsInvariant(d.Key, input)).Key;
                }
                else
                {
                    Loggers.LogDebug($"menu does not contain string {input}");
                    terminalMenu.isActive = false;
                    continue;
                }
            }

            return "CategoryNameFailure";
        }

        public static string GetCategoryFromNode(TerminalNode givenNode)
        {
            Loggers.LogDebug("1.1");
            if (givenNode == null)
            {
                Loggers.ERROR("GetCategoryFromNode: givenNode is null!!!!");
                return "";
            }
            foreach (TerminalMenu terminalMenu in allMenus)
            {
                if (!terminalMenu.terminalNodePerCategory.ContainsValue(givenNode))
                {
                    Loggers.LogDebug($"menu does not contain node {givenNode.name}");
                    terminalMenu.isActive = false;
                    continue;
                }

                else
                {
                    foreach (KeyValuePair<string, TerminalNode> pair in terminalMenu.terminalNodePerCategory)
                    {
                        if (pair.Value == givenNode)
                        {
                            Loggers.LogDebug($"FOUND NODE AND PAIR {pair.Key}");
                            terminalMenu.isActive = true;
                            return pair.Key;
                        }
                    }
                }
            }

            Loggers.ERROR("GetCategoryFromNode FAILURE: COULD NOT FIND NODE???");
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
            return null!;
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

            Loggers.WARNING("Empty categoryText, Unable to create TerminalMenuItem! (null return)");
            return null;
        }
    }
}

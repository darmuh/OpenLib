using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;

namespace OpenLib.Menus
{
    public class TerminalMenu
    {
        //Main
        public string MenuName;
        public string MainMenuText;
        public string setKeyword;
        public List<TerminalMenuCategory> Categories = [];
        public string currentCategory;
        public bool isActive; //currently using this menu
        public bool isNextEnabled; //for cycling pages
        public int nextCount = 1; //always at least be 1
        public List<TerminalMenuItem> menuItems;

        //TerminalStuff
        public Dictionary<string,TerminalNode> terminalNodePerCategory = [];
        public List<Dictionary<string, List<string>>> categoryLists = [];
        public List<TerminalNode> terminalNodes = [];

        public void Delete()
        {
            allMenus.Remove(this);
            this.menuItems.Clear();
            this.categoryLists.Clear();
            this.Categories.Clear();
            this.terminalNodePerCategory.Clear();
            this.terminalNodes.Clear();
        }

            }

    public class TerminalMenuItem
    {
        //Main
        public string ItemName;
        public string Category;
        public List<string> itemKeywords;
        public string itemDescription;

        public void Delete()
        {
            this.itemDescription = "";
            
            this.Category = "";
            this.ItemName = "";

            if (this.itemKeywords.Count > 0)
                this.itemKeywords.Clear();
        }

    }

    public class TerminalMenuCategory
    {
        public string CatName;
        public string CatDescription;

    }
}

using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;

namespace OpenLib.Menus
{
    public class TerminalMenu
    {
        //Main
        public string MenuName { get; set; } = string.Empty;
        public string MainMenuText { get; set; } = string.Empty;
        public string SetKeyword { get; set; } = string.Empty;
        public List<TerminalMenuCategory> Categories = [];
        public string CurrentCategory { get; internal set; } = string.Empty;
        public bool IsActive { get; internal set; } = false; //currently using this menu
        public bool IsNextEnabled { get; internal set; } = false; //for cycling pages
        public int NextCount { get; internal set; } = 1; //always at least be 1
        public List<TerminalMenuItem> MenuItems { get; set; } = [];

        //TerminalStuff
        public Dictionary<string, TerminalNode> terminalNodePerCategory = [];
        public List<Dictionary<string, List<string>>> categoryLists = [];
        public List<TerminalNode> terminalNodes = [];

        public void Delete()
        {
            allMenus.Remove(this);
            MenuItems.Clear();
            categoryLists.Clear();
            Categories.Clear();
            terminalNodePerCategory.Clear();
            terminalNodes.Clear();
        }

    }

    public class TerminalMenuItem
    {
        //Main
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> ItemKeywords { get; set; } = null!;
        public string ItemDescription { get; set; } = string.Empty;

        public void Delete()
        {
            ItemDescription = "";

            Category = "";
            ItemName = "";

            if (ItemKeywords.Count > 0)
                ItemKeywords.Clear();
        }

    }

    public class TerminalMenuCategory
    {
        public string CatName { get; set; } = null!;
        public string CatDescription { get; set; } = null!;

    }
}

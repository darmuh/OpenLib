using System;
using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;

namespace OpenLib.Menus;
[Obsolete("This style of menu will be deleted in next major update to library, once all mods are confirmed not using this")]
public class TerminalMenu
{
    //Have to keep lowercase property names for existing mods
    //Also can't add getter/setters for these or will mess with existing mods
#pragma warning disable IDE1006
    public string MenuName = string.Empty;
    public string MainMenuText = string.Empty;
    public string setKeyword = string.Empty;
    public List<TerminalMenuCategory> Categories = [];
    public string currentCategory { get; internal set; } = string.Empty;
    public bool isActive { get; internal set; } = false; //currently using this menu
    public bool isNextEnabled { get; internal set; } = false; //for cycling pages
    public int nextCount { get; internal set; } = 1; //always at least be 1
    public List<TerminalMenuItem> menuItems = [];
#pragma warning restore IDE1006
    //TerminalStuff
    public Dictionary<string, TerminalNode> terminalNodePerCategory = [];
    public List<Dictionary<string, List<string>>> categoryLists = [];
    public List<TerminalNode> terminalNodes = [];

    public void Delete()
    {
        allMenus.Remove(this);
        menuItems.Clear();
        categoryLists.Clear();
        Categories.Clear();
        terminalNodePerCategory.Clear();
        terminalNodes.Clear();
    }

}

public class TerminalMenuItem
{
    //Main
    public string ItemName = string.Empty;
    public string Category = string.Empty;
    public List<string> itemKeywords = null!; //must be lowercase for terminalstuff public
    public string itemDescription = string.Empty;

    public void Delete()
    {
        itemDescription = "";

        Category = "";
        ItemName = "";

        if (itemKeywords.Count > 0)
            itemKeywords.Clear();
    }

}

public class TerminalMenuCategory
{
    public string CatName { get; set; } = null!;
    public string CatDescription { get; set; } = null!;

}

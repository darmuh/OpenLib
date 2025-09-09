using OpenLib.CoreMethods;
using OpenLib.InteractiveMenus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static OpenLib.Events.Events;

namespace OpenLib.Menus;
public class CommandsMenu
{
    //Anything public can be re-used for your own CommandsMenu
    //OpenLib does not create any CommandsMenus by itself

    //Actual BetterMenu item
    internal static CommandsMenuBase CommandMenu = null!;
    //This should be the only page with no parent menu item
    internal static CommandMenuItem<CommandsMenuBase> MainMenuItem = null!;

    //so we don't create duplicates, this is already tracked by the lib but can be done locally as well
    internal static List<CommandMenuItem<CommandsMenuBase>> AllCommandMenuItems = [];

    //Example Init/Setup
    //This should only ever be called once and can be called at awake if no runtime elements are needed
    internal static void Init()
    {
        CommandMenu = new("Commands Menu")
        {
            MainMenu = MainMenuItem, //Define your main menu page
            PageSize = 10, //Number of menu items per page
            AdjustScrollInMenu = false //Determines if menu should scroll with selection changes, recommend keeping this off for now
        };
        MainMenuItem = CreateMainMenu(CommandMenu, "My Commands Menu", () => "=== My Commands Main Menu  ===\n\n", () => "Created by darmuh");
        
        
    }

    //Example Runtime Init/Setup
    //This is an example of what the example menu would have run each time you need to refresh the listing
    //In this example it would make the most sense to run each lobby load
    //However you could manipulate the listing as often as every key press if you wanted to
    //Will still need the regular Init stuff defined beforehand
    internal static void RuntimeSetup()
    {
        var active = Plugin.GetActiveCommands();

        UpdateMenuListing(CommandMenu, MainMenuItem, active); //you're welcome for the method lol :)
        //Below is needed in order for the menu to function
        //CommandMenu.MenuNode = TerminalNode
        CreateAndSetControlsFooter(CommandMenu, AllCommandMenuItems); //set footer of all menus to controls
    }

    //Compares a given listing of commands to an existing mainmenu's nestedmenus
    //If existing nestedmenus, disables any commands that are not enabled
    public static void UpdateMenuListing(CommandsMenuBase menuBase, CommandMenuItem<CommandsMenuBase> mainMenuItem, List<CommandManager> CommandList, string defaultCategory = "Other")
    {
        if (mainMenuItem.NestedMenus.Count == 0) //this stuff only happens on first launch of the listings
        {
            MakeCommandMenuItems(menuBase, CommandList, defaultCategory, mainMenuItem); // This creates a list of menuitems from all active commands and nests them appropriately
        }
        else
        {
            List<CommandMenuItem<CommandsMenuBase>> existing = mainMenuItem.NestedMenus.ConvertAll(x => x as CommandMenuItem<CommandsMenuBase>)!;
            foreach (CommandMenuItem<CommandsMenuBase>item in existing)
            {
                //set enabled bool
                item.isEnabled = CommandList.Contains(item.Command);

                //remove from commandlist
                CommandList.Remove(item.Command);

                UpdateMenuVisibility(item);
            }

            if(CommandList.Count > 0) //Add any commands still remaining that didn't already exist
                MakeCommandMenuItems(menuBase, CommandList, defaultCategory, mainMenuItem);
        }
    }

    private static void UpdateMenuVisibility(CommandMenuItem<CommandsMenuBase> item)
    {
        if (item.isEnabled)
        {
            if (item.Parent != null) // parent already set to non-null menu item
                return;

            if (item.CachedParent == null)
            {
                Loggers.WARNING($"Unable to update {item.Name}'s parent. CachedParent is null!");
                return;
            }

            item.SetParentMenu(item.CachedParent);
        }
        else
        {
            if (item.Parent == null)
                return;

            item.CachedParent = item.Parent;
            item.SetParentMenu(null!);
        }
    }

    //Terminal Command Output Example
    internal static string EnterCommandMenu()
    {
        if (MainMenuItem == null)
            return "This menu has not been created correctly";

        CommandMenu.EnterAtPage(MainMenuItem);
        return "";
    }

    //Use this to create a main menu item
    public static CommandMenuItem<CommandsMenuBase> CreateMainMenu(CommandsMenuBase menuBase, string menuName, Func<string> header = null!, Func<string> footer = null!)
    {
        header ??= () => string.Empty;
        footer ??= () => string.Empty;

        CommandMenuItem<CommandsMenuBase>menu = new(menuBase, menuName)
        {
            Header = header,
            Footer = footer
        };

        AllCommandMenuItems.Add(menu);
        return menu;
    }

    //Use this method to take a listing of commands to create menu items from a given list
    //Requires a main menu item has already been created
    //This will also automatically create headers for each menu item
    public static List<CommandMenuItem<CommandsMenuBase>> MakeCommandMenuItems(CommandsMenuBase menuBase, List<CommandManager> commands, string DefaultCategory, CommandMenuItem<CommandsMenuBase>MainMenu)
    {
        List<CommandMenuItem<CommandsMenuBase>> commandMenuItems = [];
        Dictionary<CommandManager, string> categories = [];

        if (commands.Count < 1)
            return commandMenuItems;

        foreach (var command in commands)
        {
            if (string.IsNullOrEmpty(command.Category))
                categories.Add(command, DefaultCategory);
            else
                categories.Add(command, command.Category);
        }

        // re-use categories from allcommandmenuitems listing
        List<CommandMenuItem<CommandsMenuBase>> cats = AllCommandMenuItems.FindAll(c => c.Command == null!);
        foreach (var category in categories)
        {
            // check if category name already exists and create one if does not exist
            if (cats.FirstOrDefault(c => Common.Misc.CompareStringsInvariant(c.Name, category.Value)) is not CommandMenuItem<CommandsMenuBase> cat)
            {
                cat = new(menuBase, category.Value.ToUpperInvariant());
                commandMenuItems.Add(cat);
                cat.SetParentMenu(MainMenu);
                cat.Header = () => $"======== {category.Value.ToUpperInvariant()} COMMANDS ========\n\n";
            }

            CreateCommandMenuItems(menuBase, cat, category.Key, ref commandMenuItems);
        }

        foreach (CommandMenuItem<CommandsMenuBase> item in commandMenuItems)
        {
            if (item.Command == null && item.Parent == null)
                item.SetParentMenu(MainMenu); //This childs ONLY the categories to the main menu
        }

        AllCommandMenuItems.AddRange(commandMenuItems);
        return commandMenuItems;
    }

    public static void CreateAndSetControlsFooter(CommandsMenuBase menuBase, List<CommandMenuItem<CommandsMenuBase>> MenuItems)
    {
        SetMenuItemsFooter(MenuItems, menuBase.Controls);
    }

    public static void SetMenuItemsFooter(List<CommandMenuItem<CommandsMenuBase>> MenuItems, Func<string> footer)
    {
        foreach(CommandMenuItem<CommandsMenuBase> menuItem in MenuItems)
        {
            menuItem.Footer = footer;
        }
    }

    public static void CreateCommandMenuItems(CommandsMenuBase menuBase, CommandMenuItem<CommandsMenuBase> parent, CommandManager command, ref List<CommandMenuItem<CommandsMenuBase>> commandMenuItems)
    {
        CommandMenuItem<CommandsMenuBase> CommandName = TryCreate(menuBase, command);
        commandMenuItems.Add(CommandName);
        CommandName.Header = () => $"======== {command.Name} Info ========\n\n";
        CommandName.SetParentMenu(parent);

        CommandMenuItem<CommandsMenuBase> CommandKeywords = new(menuBase, $"{command.Name} Keywords");
        commandMenuItems.Add(CommandKeywords);
        CommandKeywords.Header = () => $"======== {command.Name} Keywords ========\n\n";
        CommandKeywords.SetParentMenu(CommandName);

        foreach (var word in command.KeywordList)
        {
            CommandMenuItem<CommandsMenuBase> keyword = new(menuBase, word);
            commandMenuItems.Add(keyword);
            keyword.SetParentMenu(CommandKeywords);
        }

        if (command.IsEnabled != null)
        {
            CommandMenuItem<CommandsMenuBase>GetInfo = new(menuBase, $"{command.Name} Information");
            commandMenuItems.Add(GetInfo);
            GetInfo.Header = () => $"====== {command.Name} Information ======\n\n";
            GetInfo.SetParentMenu(CommandName);

            CommandMenuItem<CommandsMenuBase>Information = new(menuBase, command.IsEnabled.ConfigItem.Description.Description);
            commandMenuItems.Add(Information);
            Information.SetParentMenu(GetInfo);
        }
    }

    public static CommandMenuItem<CommandsMenuBase>TryCreate(CommandsMenuBase menuBase, CommandManager command)
    {
        CommandMenuItem<CommandsMenuBase>item = AllCommandMenuItems.FirstOrDefault(c => c.Command == command);

        if (item != null)
            return item;
        else
            return new(menuBase, command.Name)
            {
                Command = command,
            };
    }

    public static bool TryGetCategoryMenuItem(string categoryName, out CommandMenuItem<CommandsMenuBase>match)
    {
        List<CommandMenuItem<CommandsMenuBase>> CategoryMenus = AllCommandMenuItems.FindAll(x => x.Parent == MainMenuItem && x.Command == null);

        match = CategoryMenus.FirstOrDefault(x => x.Name == categoryName);
        return match != null;
    }

    //Allows you to set header of anything from the main menu
    public static void SetCategoryHeader(string categoryName, Func<string> header)
    {
        if(TryGetCategoryMenuItem(categoryName, out CommandMenuItem<CommandsMenuBase>match))
            match.Header = header;
    }

    //Allows you to set header of anything from the main menu
    public static void SetCategoryFooter(string categoryName, Func<string> footer)
    {
        if (TryGetCategoryMenuItem(categoryName, out CommandMenuItem<CommandsMenuBase>match))
            match.Footer = footer;
    }
}

public class CommandsMenuBase(string name, Dictionary<Key, Action> MoreMenuActions = null!) : BetterMenu<CommandsMenuBase>(name, MoreMenuActions)
{
    public string Controls()
    {
        StringBuilder message = new();
        message.Append($"\r\n\r\nPage [{leftMenu}] < {CurrentPage}/{Mathf.CeilToInt((float)DisplayMenuItemsOfType.Count / PageSize)} > [{rightMenu}]\r\n");
        message.Append($"Back: [{leaveMenu}] Select: [{selectMenu}]\r\n");
        return message.ToString();
    }
}

//barebones menuitems for use with CommandMenu
public class CommandMenuItem<TMenu>(TMenu betterMenu, string name) : MenuItem<TMenu>(betterMenu) where TMenu : BetterMenuBase
{
    private string _name = name;
    public override string Name
    {
        get => _name;
        set => _name = value;
    }

    private bool _showEmpty = true;
    public override bool ShowIfEmptyNest
    {
        get => _showEmpty;
        set => _showEmpty = value;
    }

    private CustomEvent _selection = new();
    public override CustomEvent SelectionEvent
    {
        get => _selection;
        set => _selection = value;
    }

    private List<MenuItem> _nested = [];
    public override List<MenuItem> NestedMenus
    {
        get => _nested;
        set => _nested = value;
    }

    public bool isEnabled = true;
    public CommandManager Command = null!;
    public MenuItem CachedParent = null!; //This will not be used to show the item
    public TMenu MenuBase = betterMenu;
}

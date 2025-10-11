using OpenLib.Common;
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

        foreach (var category in categories)
        {
            // check if category name already exists and create one if does not exist
            if (MainMenu.NestedMenus.FirstOrDefault(c => Common.Misc.CompareStringsInvariant(c.Name, category.Value)) is not CommandMenuItem<CommandsMenuBase> cat)
            {
                cat = new(menuBase, category.Value.ToUpperInvariant());
                commandMenuItems.Add(cat);
                cat.SetParentMenu(MainMenu);
                cat.Header = () => CommandsMenuBase.ConvertHeader(menuBase.CategoryHeader, cat.Name);
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
        CommandName.Header = () => CommandsMenuBase.ConvertHeader(menuBase.CommandHeader, parent.Name, CommandName.Name);
        CommandName.SetParentMenu(parent);

        if (menuBase.AddKeywordsMenu)
        {
            if(!TryGetChild(CommandName, $"{command.Name} Keywords", out CommandMenuItem<CommandsMenuBase> CommandKeywords))
            {
                CommandKeywords = new(menuBase, $"{command.Name} Keywords");
                commandMenuItems.Add(CommandKeywords);
                CommandKeywords.SetParentMenu(CommandName);
            }

            CommandKeywords.Header = () => CommandsMenuBase.ConvertHeader(menuBase.KeywordsHeader, parent.Name, CommandName.Name);

            foreach (var word in command.KeywordList)
            {
                if(!TryGetChild(CommandKeywords, word, out CommandMenuItem<CommandsMenuBase> keyword))
                {
                    keyword = new(menuBase, word);
                    commandMenuItems.Add(keyword);
                    keyword.SetParentMenu(CommandKeywords);
                    keyword.Header = () => CommandsMenuBase.ConvertHeader(menuBase.KeywordsHeader, parent.Name, CommandName.Name);
                }
            }
        }

        if (command.IsEnabled != null && menuBase.AddInfoMenu)
        {
            if (!TryGetChild(CommandName, $"{command.Name} Information", out CommandMenuItem<CommandsMenuBase> GetInfo))
            {
                GetInfo = new(menuBase, $"{command.Name} Information");
                commandMenuItems.Add(GetInfo);   
                GetInfo.SetParentMenu(CommandName);
            }

            GetInfo.Header = () => CommandsMenuBase.ConvertHeader(menuBase.InfoHeader, parent.Name, CommandName.Name);

            if (!TryGetChild(GetInfo, command.IsEnabled.ConfigItem.Description.Description, out CommandMenuItem<CommandsMenuBase> Information))
            {
                Information = new(menuBase, command.IsEnabled.ConfigItem.Description.Description);
                commandMenuItems.Add(Information);
                Information.SetParentMenu(GetInfo);
            }

            Information.Header = () => CommandsMenuBase.ConvertHeader(menuBase.InfoHeader, parent.Name, CommandName.Name);
        }

        // only do below if command runs as itself
        if (command.AcceptAdditionalText && menuBase.AddRunCommand)
            return;

        if (!TryGetChild(CommandName, $"Run {command.Name}", out CommandMenuItem<CommandsMenuBase> RunCommand))
        {
            RunCommand = new(menuBase, $"Run {command.Name}")
            {
                LoadPageOnSelect = false
            };

            commandMenuItems.Add(RunCommand);
            RunCommand.SetParentMenu(CommandName);
            CustomEvent commandInvoke = new();
            commandInvoke.AddListener(() =>
            {
                menuBase.ExitPage = command;
                menuBase.ExitMenu(true);

            });
            RunCommand.SelectionEvent = commandInvoke;
        }
    }

    public static CommandMenuItem<CommandsMenuBase> TryCreate(CommandsMenuBase menuBase, CommandManager command)
    {
        CommandMenuItem<CommandsMenuBase> item = AllCommandMenuItems.FirstOrDefault(c => c.Command == command);

        if (item != null)
            return item;
        else
            return new(menuBase, command.Name)
            {
                Command = command,
            };
    }

    public static bool TryGetChild(CommandMenuItem<CommandsMenuBase> Parent, string expectedName, out CommandMenuItem<CommandsMenuBase> item)
    {
        item = AllCommandMenuItems.FirstOrDefault(c => c.Parent == Parent && c.Name == expectedName);
        return item != null;
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

public class CommandsMenuBase : BetterMenu<CommandsMenuBase>
{
    //Customize commands generation
    public bool AddKeywordsMenu = true;
    public bool AddInfoMenu = true;
    public bool AddRunCommand = true;
    
    //Customize generated headers
    public string CommandHeader = "======== {commandName} ========\n\n";
    public string KeywordsHeader = "======== {commandName} Keywords ========\n\n";
    public string CategoryHeader = "======== {catName>>} COMMANDS ========\n\n";
    public string InfoHeader = "====== {commandName} Information ======\n\n";

    //Use below constants to define expected variables
    //Variables must be encapsulated by brackets like standard C# for translation
    public const string CategoryName = "catName";
    public const string CommandName = "commandName";
    public const string ToUpper = ">>";
    public const string ToLower = "<<";
    public static readonly List<string> AcceptableVariables = [CategoryName, CommandName, ToUpper, ToLower];

    //Custom Exit Action
    public CommandManager ExitPage = null!;


    public CommandsMenuBase(string name, Dictionary<Key, Action> MoreMenuActions = null!)
        : base(name, MoreMenuActions)
    {
        ExitAction = CustomExitAction; //using custom exit action by default
        OnEnter.AddListener(() => ExitPage = null!);
    }

    public void CustomExitAction()
    {
        if (ExitPage != null)
        {
            if(LogicHandling.GetDisplayTextFromCommand(ref ExitPage.terminalNode))
                CommonTerminal.LoadNewNode(ExitPage.terminalNode);
            else
                CommonTerminal.LoadNewNode(CommonTerminal.HomePage);
        }      
        else
            CommonTerminal.LoadNewNode(CommonTerminal.HomePage);
    }

    public static string ConvertHeader(string query, string catName, string commandName = "")
    {
        if (string.IsNullOrEmpty(query))
            return query;

        List<string> originals = Misc.GetSubstrings('{', '}', query);

        if (originals.Count == 0)
            return query;

        foreach (string value in originals)
        {
            //remove brackets
            string newString = value.Replace("{", "").Replace("}", "");

            //replace CategoryName
            if(Misc.StringContainsInvariant(value, CategoryName))
                newString = newString.Replace(CategoryName, catName);

            //replace CommandName
            if (Misc.StringContainsInvariant(value, CommandName))
                newString = newString.Replace(CommandName, commandName);

            //Convert To Uppercase
            if (Misc.StringContainsInvariant(value, ToUpper))
                newString = newString.Replace(ToUpper, "").ToUpperInvariant();

            //Convert To Lowercase
            if (Misc.StringContainsInvariant(value, ToLower))
                newString = newString.Replace(ToLower, "").ToLowerInvariant();

            //Update result
            query = query.Replace(value, newString);
        }

        return query;
    }

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

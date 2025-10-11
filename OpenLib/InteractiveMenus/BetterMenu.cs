using HarmonyLib;
using OpenLib.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static OpenLib.Events.Events;
using static OpenLib.InteractiveMenus.MenusContainer;

namespace OpenLib.InteractiveMenus;
public abstract class BetterMenuBase
{
    public abstract string Name { get; set; }
    public abstract Type MenuType { get; }
    public abstract object BoxedValue { get; }
    public abstract bool InMenu { get; set; }
    public abstract CustomEvent InputEvent { get; set; }
    public abstract CustomEvent ExitTerminal { get; set; }
    public abstract MenuItem MainMenu { get; set; }
    public List<MenuItem> AllMenuItemsOfType = [];

    public List<T> GetMenuItemsOfType<T>() where T : MenuItem
    {
        return [.. AllMenuItemsOfType.OfType<T>()];
    }

    public override string ToString()
    {
        return Name;
    }
}

public class BetterMenu<T> : BetterMenuBase
{
    private string _name = "";
    public override string Name
    {
        get => _name;
        set => _name = value;
    }
    public override Type MenuType => typeof(T);
    public override object BoxedValue => this;
    private bool _inMenu = false; //set true when using this menu
    public override bool InMenu
    {
        get => _inMenu;
        set => _inMenu = value;
    }
    private MenuItem _mainMenu = null!;
    public override MenuItem MainMenu
    {
        get => _mainMenu;
        set => _mainMenu = value;
    }

    private CustomEvent _inputEvent = new();
    public override CustomEvent InputEvent
    {
        get => _inputEvent;
        set => _inputEvent = value;
    }
    private CustomEvent _exitEvent = new();
    public override CustomEvent ExitTerminal
    {
        get => _exitEvent;
        set => _exitEvent = value;
    }

    public MenuItem CurrentMenuItem = null!;
    public TerminalNode MenuNode = null!;
    public Action ExitAction = null!;
    //public TerminalNode ExitPage = null!;
    public CustomEvent OnExit = new();
    public CustomEvent OnExitComplete = new();
    public CustomEvent OnEnter = new();
    public CustomEvent OnLoad = new();
    public CustomEvent LoadPage = new();
    public CustomEvent UpMenuEvent = new();
    public CustomEvent DownMenuEvent = new();
    public CustomEvent LeftMenuEvent = new();
    public CustomEvent RightMenuEvent = new();
    public CustomEvent AcceptAnyKeyEvent = new();
    public CustomEventRef<List<MenuItem>> MenuItemList = new();

    //keys
    public Key upMenu = Key.UpArrow;
    public Key downMenu = Key.DownArrow;
    public Key leftMenu = Key.LeftArrow;
    public Key rightMenu = Key.RightArrow;
    public Key selectMenu = Key.Enter;
    public Key leaveMenu = Key.Backspace;
    public Dictionary<Key, Action> MainActions = [];
    public Dictionary<Key, Action> OtherActions = [];

    //important
    public bool IsMenuEnabled = false;

    //Set these via custom events in the menu item
    public bool AcceptAnything = false;
    public bool AdjustScrollInMenu = false;

    private int _activeIndex = 0;
    public int ActiveSelection
    {
        get => _activeIndex; set => _activeIndex = value;
    }
    private int _endIndex = 0;
    public int EndIndex
    {
        get => _endIndex; set => _endIndex = value;
    }
    public int CurrentPage = 1;
    public int PageSize = 8;
    public List<MenuItem> DisplayMenuItemsOfType = [];

    public override string ToString()
    {
        return Name;
    }

    //should only run once per game launch
    public BetterMenu(string name, Dictionary<Key, Action> MoreMenuActions = null!)
    {
        if (TerminalUpdatePatch.usePatch == false)
            TerminalUpdatePatch.usePatch = true;

        Name = name;
        LoadPage.AddListener(Load);
        InputEvent.AddListener(HandleInput);
        ExitTerminal.AddListener(ExitTerminalLeave);
        AcceptAnyKeyEvent.AddListener(DefaultAcceptAnything);
        UpdateMainActions();

        if (MoreMenuActions != null!)
            OtherActions = MoreMenuActions;

        AllMenus.Add(this);
    }

    public void ReplaceEventAction(CustomEvent eventName, CustomEvent.Event newMethod)
    {
        if (eventName.Listeners > 0)
            eventName.RemoveAllListeners();

        eventName.AddListener(newMethod);
    }

    public void AddToOtherActions(Key key, Action action)
    {
        if (OtherActions.ContainsKey(key))
            OtherActions.Remove(key);

        OtherActions.Add(key, action);
    }

    //can be called to update main actions with new keys
    public void UpdateMainActions()
    {
        MainActions.Clear();
        MainActions.Add(upMenu, UpMenu);
        MainActions.Add(downMenu, DownMenu);
        MainActions.Add(leftMenu, LeftMenu);
        MainActions.Add(rightMenu, RightMenu);
        MainActions.Add(selectMenu, SelectInMenu);
        MainActions.Add(leaveMenu, ExitInTerminal);
    }

    internal void HandleInput()
    {
        if (!InMenu || MenuNode == null!)
            return;

        if (AcceptAnything && AcceptAnyKeyEvent.HasListeners)
        {
            AcceptAnyKeyEvent.Invoke();
            return;
        }

        Key? main = MainActions.FirstOrDefault(x => Keyboard.current[x.Key].isPressed).Key;

        if (main != null!)
        {
            Action act = MainActions.FirstOrDefault(x => x.Key == main).Value;
            act?.Invoke();
        }

        if (OtherActions.Count == 0)
            return;

        Key? other = OtherActions.FirstOrDefault(x => Keyboard.current[x.Key].isPressed).Key;

        if (other != null!)
        {
            Action act = OtherActions.FirstOrDefault(x => x.Key == other).Value;
            act?.Invoke();
        }

    }

    public void Load()
    {
        MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);
        if (current == null!)
        {
            Loggers.ERROR("Unable to load current page! Nothing is active!");
            return;
        }

        if (MenuNode == null!)
        {
            Loggers.ERROR("NRE detected at MenuNode! This menu did not set it's terminal node correctly!");
            return;
        }

        current.OnPageLoad?.Invoke();
        OnLoad.Invoke();
        Plugin.instance.Terminal.StartCoroutine(DelayUpdateText());
    }

    public void DefaultAcceptAnything()
    {
        AcceptAnything = false;

        MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);
        if (current.NestedMenus.Count == 0)
        {
            ExitInTerminal();
            return;
        }
        LoadPage.Invoke();
    }

    private IEnumerator DelayUpdateText()
    {
        yield return new WaitForEndOfFrame();

        if (!AcceptAnything) //AcceptAnything set to true is expecting displaytext to be updated externally
            MenuNode.displayText = GetPageText();

        if (string.IsNullOrEmpty(MenuNode.displayText))
            ExitInTerminal();

        yield return new WaitForEndOfFrame();
        CommonTerminal.LoadNewNode(MenuNode);
        yield return new WaitForEndOfFrame();
        if (AdjustScrollInMenu)
            ScrollAdjust();
    }

    private void ScrollAdjust()
    {
        float scrollbar = ((float)EndIndex - (float)ActiveSelection) / (float)EndIndex;
        if (scrollbar >= 1f)
            Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.forceScrollbarUp());
        else if (scrollbar == 0f)
            Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.forceScrollbarDown());
        else
            Plugin.instance.Terminal.scrollBarVertical.value = scrollbar;
    }

    public string GetPageText()
    {
        StringBuilder message = new();

        CurrentMenuItem = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);
        if (CurrentMenuItem == null)
        {
            Loggers.WARNING("Unable to get current menu page!!");
            return "";
        }

        message.Append($"{CurrentMenuItem.Header.Invoke()}");

        DisplayMenuItemsOfType = AllMenuItemsOfType.FindAll(x => CurrentMenuItem.NestedMenus.Contains(x));
        DisplayMenuItemsOfType.RemoveAll(x => !x.ShowIfEmptyNest && x.NestedMenus.Count == 0);

        if (DisplayMenuItemsOfType.Count == 0)
        {
            message.Append($"\r\n\r\nThis menu listing is currently empty :(\r\n");
            if (CurrentMenuItem != null!)
                message.Append($"{CurrentMenuItem.Footer.Invoke()}");
            return message.ToString();
        }

        CurrentMenuItem.AdjustNestedMenuList.Invoke(ref DisplayMenuItemsOfType);

        CurrentPage = Misc.CycleIndex(CurrentPage, 1, Mathf.CeilToInt((float)DisplayMenuItemsOfType.Count / PageSize));
        int startIndex = (CurrentPage - 1) * PageSize;
        int endIndex = Mathf.Min(startIndex + PageSize, DisplayMenuItemsOfType.Count);
        EndIndex = endIndex;
        ActiveSelection = Misc.CycleIndex(ActiveSelection, startIndex, endIndex - 1);
        Loggers.LogDebug($"{Name} menu activeselection: {ActiveSelection}");

        DisplayMenuItemsOfType.DoIf(x => x.OnPageLoad != null!, x => x.OnPageLoad());

        for (int i = startIndex; i < endIndex; i++)
        {
            string menuItem = string.Empty;
            if (ActiveSelection == i)
                menuItem = ">>> ";

            menuItem += DisplayMenuItemsOfType[i].Prefix;
            menuItem += DisplayMenuItemsOfType[i].Name;
            menuItem += DisplayMenuItemsOfType[i].Suffix;
            message.Append(menuItem + "\n");
        }

        int emptySpace = (endIndex - startIndex - PageSize);

        if (emptySpace < 0)
        {
            for (int i = emptySpace; i < 0; i++)
                message.Append('\n');
        }

        if (CurrentMenuItem == null!)
            Loggers.WARNING("Unable to select current menu item!!");
        else
            message.Append($"{CurrentMenuItem.Footer.Invoke()}");

        return message.ToString();
    }
    public void SelectInMenu()
    {
        if (DisplayMenuItemsOfType.Count == 0)
            return;

        MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);

        if (current == null!)
        {
            Loggers.WARNING("Unable to select current menu item!!");
            return;
        }

        Loggers.LogDebug($"Selecting Nested Menu Item!");
        MenuItem selected = DisplayMenuItemsOfType[ActiveSelection];
        selected.SelectionEvent?.Invoke();

        if (selected.NestedMenus.Count > 0)
        {
            Loggers.LogDebug("Setting to nested menu item!");
            CurrentPage = 1;
            ActiveSelection = 0;
            AllMenuItemsOfType.Do(x => x.IsActive = false);
            selected.IsActive = true;

        }

        if (selected.LoadPageOnSelect)
            LoadPage.Invoke();
    }

    public void AddMenuItem(MenuItem menuItem)
    {
        if (!AllMenuItemsOfType.Contains(menuItem))
            AllMenuItemsOfType.Add(menuItem);
    }

    public void ExitInTerminal()
    {
        MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);

        if (current == null!)
            Loggers.WARNING("Unable to get current menu page!!");

        if (current == null!)
            ExitMenu(true);
        else if (current.Parent != null!)
        {
            Loggers.LogDebug("Setting to ParentMenu!");
            ActiveSelection = 0;
            CurrentPage = 1;
            current.IsActive = false;
            current.Parent.IsActive = true;
            LoadPage.Invoke();
        }
        else
            ExitMenu(true);
    }

    public void ExitTerminalLeave()
    {
        ExitMenu(false);
    }

    public void ExitMenu(bool enableInput)
    {
        Plugin.instance.Terminal.StartCoroutine(MenuClose(enableInput));
    }

    public void EnterAtPage(MenuItem menuItem)
    {
        OnEnter.Invoke();
        Plugin.instance.Terminal.StartCoroutine(MenuStart(menuItem));
    }

    public void EnterMenu()
    {
        OnEnter.Invoke();
        Plugin.instance.Terminal.StartCoroutine(MenuStart());
    }

    internal IEnumerator MenuClose(bool enableInput)
    {
        OnExit.Invoke();
        yield return new WaitForEndOfFrame();
        InMenu = false;
        AcceptAnything = false;
        AllMenuItemsOfType.Do(x => x.IsActive = false);

        CommonTerminal.ChangeCaretColor(CommonTerminal.CaretOriginal, false);

        yield return new WaitForEndOfFrame();

        if (ExitAction == null!)
            CommonTerminal.LoadNewNode(CommonTerminal.HomePage); //load home
        else
            ExitAction.Invoke();

        if (enableInput)
        {
            Plugin.instance.Terminal.screenText.ActivateInputField();
            Plugin.instance.Terminal.screenText.interactable = true;
        }

        OnExitComplete.Invoke();
        yield break;
    }

    internal IEnumerator MenuStart(MenuItem Start = null!)
    {
        if (InMenu)
            yield break;

        Start ??= MainMenu;

        Start.IsActive = true;
        AcceptAnything = false;
        yield return new WaitForEndOfFrame();
        InMenu = true;
        CommonTerminal.ChangeCaretColor(CommonTerminal.transparent, true);
        ActiveSelection = 0;
        CurrentPage = 1;
        yield return new WaitForEndOfFrame();
        Plugin.instance.Terminal.screenText.DeactivateInputField();
        Plugin.instance.Terminal.screenText.interactable = false;
        yield return new WaitForEndOfFrame();
        Start.SelectionEvent?.Invoke();

        if (Start.LoadPageOnSelect)
            LoadPage.Invoke();

        yield break;
    }


    public void UpMenu()
    {
        if (!InMenu)
            return;

        ActiveSelection--;
        UpMenuEvent.Invoke();
        LoadPage.Invoke();
    }

    public void DownMenu()
    {
        if (!InMenu)
            return;

        ActiveSelection++;
        DownMenuEvent.Invoke();
        LoadPage.Invoke();

    }

    public void LeftMenu()
    {
        if (!InMenu)
            return;

        CurrentPage--;
        LeftMenuEvent.Invoke();
        LoadPage.Invoke();
    }

    public void RightMenu()
    {
        if (!InMenu)
            return;

        CurrentPage++;
        RightMenuEvent.Invoke();
        LoadPage.Invoke();
    }
}

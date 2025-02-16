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

namespace OpenLib.InteractiveMenus
{

    public abstract class BetterMenuBase
    {
        public abstract string Name { get; set; }
        public abstract Type MenuType { get; }
        public abstract object BoxedValue { get; }
        public abstract bool InMenu { get; set; }
        public abstract CustomEvent InputEvent { get; set; }
        public abstract CustomEvent ExitTerminal { get; set; }
        public abstract MenuItem MainMenu { get; set; }
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

        public TerminalNode MenuNode;
        public TerminalNode ExitPage = null!;
        public CustomEvent OnExit = new();
        public CustomEvent OnEnter = new();
        public CustomEvent OnLoad = new();
        public CustomEvent LoadPage = new();
        public CustomEvent UpMenuEvent = new();
        public CustomEvent DownMenuEvent = new();
        public CustomEvent LeftMenuEvent = new();
        public CustomEvent RightMenuEvent = new();
        public CustomEvent AcceptAnyKeyEvent = new();

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
        
        public bool AcceptAnything = false;
        public int ActiveSelection = 0;
        public int CurrentPage = 1;
        public int PageSize = 8;
        public List<MenuItem> AllMenuItemsOfType = [];
        public List<MenuItem> DisplayMenuItemsOfType = [];

        //should only run once per game launch
        public BetterMenu(string name, Dictionary<Key, Action> MoreMenuActions = null)
        {
            if (TerminalUpdatePatch.usePatch == false)
                TerminalUpdatePatch.usePatch = true;

            Name = name;
            LoadPage.AddListener(Load);
            InputEvent.AddListener(HandleInput);
            ExitTerminal.AddListener(ExitTerminalLeave);
            AcceptAnyKeyEvent.AddListener(DefaultAcceptAnything);
            UpdateMainActions();

            if (MoreMenuActions != null)
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
            if (!InMenu || MenuNode == null)
                return;

            if (AcceptAnything && AcceptAnyKeyEvent.HasListeners)
            {
                AcceptAnyKeyEvent.Invoke();
                return;
            }

            Key? main = MainActions.FirstOrDefault(x => Keyboard.current[x.Key].isPressed).Key;

            if (main != null)
            {
                Action act = MainActions.FirstOrDefault(x => x.Key == main).Value;
                act?.Invoke();
            }

            if (OtherActions.Count == 0)
                return;

            Key? other = OtherActions.FirstOrDefault(x => Keyboard.current[x.Key].isPressed).Key;

            if (other != null)
            {
                Action act = OtherActions.FirstOrDefault(x => x.Key == other).Value;
                act?.Invoke();
            }

        }

        public void Load()
        {
            MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);
            current.OnPageLoad?.Invoke();
            OnLoad.Invoke();
            Plugin.instance.Terminal.StartCoroutine(DelayUpdateText());
        }

        public void DefaultAcceptAnything()
        {
            AcceptAnything = false;
            Load();
        }

        private IEnumerator DelayUpdateText()
        {
            yield return new WaitForEndOfFrame();

            if(!AcceptAnything) //AcceptAnything set to true is expecting displaytext to be updated externally
                MenuNode.displayText = GetPageText();

            yield return new WaitForEndOfFrame();
            CommonTerminal.LoadNewNode(MenuNode);
            yield return new WaitForEndOfFrame();

        }

        public string GetPageText()
        {
            StringBuilder message = new();
            
            MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);
            if (current == null)
            {
                Plugin.WARNING("Unable to get current menu page!!");
                return "Unable to get current page!\r\n\r\n";
            }

            if (current == null)
                Plugin.WARNING("Unable to get current menu page!!");
            else
                message.Append($"{current.Header.Invoke()}");

            DisplayMenuItemsOfType = AllMenuItemsOfType.FindAll(x => current.NestedMenus.Contains(x));
            DisplayMenuItemsOfType.RemoveAll(x => !x.ShowIfEmptyNest && x.NestedMenus.Count == 0);

            if (DisplayMenuItemsOfType.Count == 0)
            {
                message.Append($"\r\n\r\nThis menu listing is currently empty :(\r\n");
                if (current != null)
                    message.Append($"{current.Footer.Invoke()}");
                return message.ToString();
            }

            CurrentPage = Misc.CycleIndex(CurrentPage, 1, Mathf.CeilToInt((float)DisplayMenuItemsOfType.Count / PageSize));
            int startIndex = (CurrentPage - 1) * PageSize;
            int endIndex = Mathf.Min(startIndex + PageSize, DisplayMenuItemsOfType.Count);
            ActiveSelection = Misc.CycleIndex(ActiveSelection, startIndex, endIndex - 1);
            Plugin.Spam($"{Name} menu activeselection: {ActiveSelection}");

            DisplayMenuItemsOfType.DoIf(x => x.OnPageLoad != null, x => x.OnPageLoad());

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
                    message.Append("\n");
            }

            if (current == null)
                Plugin.WARNING("Unable to select current menu item!!");
            else
                message.Append($"{current.Footer.Invoke()}");

            return message.ToString();
        }
        public void SelectInMenu()
        {
            if (DisplayMenuItemsOfType.Count == 0)
                return;

            MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);

            if (current == null)
            {
                Plugin.WARNING("Unable to select current menu item!!");
                return;
            }

            Plugin.Spam($"Selecting Nested Menu Item!");
            MenuItem selected = DisplayMenuItemsOfType[ActiveSelection];
            selected.SelectionEvent?.Invoke();

            if (selected.NestedMenus.Count > 0)
            {
                Plugin.Spam("Setting to nested menu item!");
                CurrentPage = 1;
                ActiveSelection = 0;
                AllMenuItemsOfType.Do(x => x.IsActive = false);
                selected.IsActive = true;
                
            }

            if (selected.LoadPageOnSelect)
                LoadPage.Invoke();
        }

        public void ExitInTerminal()
        {
            MenuItem current = AllMenuItemsOfType.FirstOrDefault(x => x.IsActive);

            if (current == null)
                Plugin.WARNING("Unable to get current menu page!!");

            if (current == null)
                ExitMenu(true);
            else if (current.Parent != null)
            {
                Plugin.Spam("Setting to ParentMenu!");
                ActiveSelection = 0;
                CurrentPage = 0;
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
            OnExit.Invoke();
            Plugin.instance.Terminal.StartCoroutine(MenuClose(enableInput));
        }

        public void EnterMenu()
        {
            OnEnter.Invoke();
            Plugin.instance.Terminal.StartCoroutine(MenuStart());
        }

        internal IEnumerator MenuClose(bool enableInput)
        {
            yield return new WaitForEndOfFrame();
            InMenu = false;
            AcceptAnything = false;
            AllMenuItemsOfType.Do(x => x.IsActive = false);
            yield return new WaitForEndOfFrame();

            if (ExitPage == null)
                ExitPage = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1]; //home

            CommonTerminal.LoadNewNode(ExitPage);

            yield return new WaitForEndOfFrame();
            CommonTerminal.ChangeCaretColor(CommonTerminal.CaretOriginal, false);

            if (enableInput)
            {
                Plugin.instance.Terminal.screenText.ActivateInputField();
                Plugin.instance.Terminal.screenText.interactable = true;
            }

            yield break;
        }

        internal IEnumerator MenuStart()
        {
            if (InMenu)
                yield break;

            
            MainMenu.IsActive = true;
            AcceptAnything = false;
            yield return new WaitForEndOfFrame();
            InMenu = true;
            CommonTerminal.ChangeCaretColor(CommonTerminal.transparent, true);
            ActiveSelection = 0;
            CurrentPage = 0;
            yield return new WaitForEndOfFrame();
            Plugin.instance.Terminal.screenText.DeactivateInputField();
            Plugin.instance.Terminal.screenText.interactable = false;
            yield return new WaitForEndOfFrame();
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
}

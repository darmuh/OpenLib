using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using static OpenLib.Events.Events;

namespace OpenLib.CoreMethods
{
    public class InteractiveMenu
    {
        public string MenuName = "";
        
        public Action LoadPage;
        public Action EnterMenu;
        public Action LeaveMenu;
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
        public bool isMenuEnabled = false;
        public bool inMenu = false; //set true when using this menu
        public bool acceptAnything = false;
        public int activeSelection = 0;
        public int currentPage = 1;

        //should only run once per game launch
        public InteractiveMenu(string name, Action pageLoader, Action enter, Action leave, Dictionary<Key,Action> MoreMenuActions = null)
        {
            if (TerminalUpdatePatch.usePatch == false)
                TerminalUpdatePatch.usePatch = true;

            MenuName = name;
            LoadPage = pageLoader;
            EnterMenu = enter;
            LeaveMenu = leave;
            SetupMainActions();

            if (MoreMenuActions != null)
                OtherActions = MoreMenuActions;

            AllInteractiveMenus.AllMenus.Add(this);
        }

        public void AddToOtherActions(Key key, Action action)
        {
            if(OtherActions.ContainsKey(key))
                OtherActions.Remove(key);

            OtherActions.Add(key, action);
        }

        internal void SetupMainActions()
        {
            MainActions.Clear();
            MainActions.Add(upMenu, UpMenu);
            MainActions.Add(downMenu, DownMenu);
            MainActions.Add(leftMenu, LeftMenu);
            MainActions.Add(rightMenu, RightMenu);
            MainActions.Add(selectMenu, EnterMenu.Invoke);
            MainActions.Add(leaveMenu, LeaveMenu.Invoke);
        }

        internal void HandleInput()
        {
            if (!inMenu)
                return;

            if(acceptAnything && AcceptAnyKeyEvent.HasListeners)
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

        public void UpMenu()
        {
            if (!inMenu)
                return;

            if (activeSelection > 0)
                activeSelection--;

            LoadPage();
            UpMenuEvent.Invoke();
        }

        public void DownMenu()
        {
            if (!inMenu)
                return;

            activeSelection++;
            LoadPage();
            DownMenuEvent.Invoke();
        }

        public void LeftMenu()
        {
            if (!inMenu)
                return;

            if (currentPage > 1)
                currentPage--;

            LoadPage();
            LeftMenuEvent.Invoke();
        }

        public void RightMenu()
        {
            if (!inMenu)
                return;

            currentPage++;
            LoadPage();
            RightMenuEvent.Invoke();
        }

    }

    public static class AllInteractiveMenus
    {
        public static List<InteractiveMenu> AllMenus { get; internal set; }

        public static bool TryGetMenu(string menuName, out InteractiveMenu menu)
        {
            menu = AllMenus.FirstOrDefault(x=>x.MenuName == menuName);

            if (menu == null)
                return false;
            else
                return true;
        }

        public static bool AnyMenuActive()
        {
            bool external = false;
            
            if (Plugin.instance.ITAPI)
                external = Compat.InteractiveTermAPI.ApplicationInUse();

            if(external)
                return true;

            if(AllMenus.Count == 0) 
                return false;

            if (AllMenus.Any(x => x.inMenu))
                return true;

            return false;
        }
    }
}

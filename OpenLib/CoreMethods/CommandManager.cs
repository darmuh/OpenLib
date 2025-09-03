using BepInEx.Configuration;
using HarmonyLib;
using OpenLib.Common;
using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenLib.CoreMethods
{
    public class CommandManager
    {
        public string Name { get; private set; } = string.Empty;
        public ConfigWatch<bool> IsEnabled { get; private set; } = null!;
        public bool IsCreated { get; private set; } = false;
        public ConfigEntry<string> KeywordsConfig { get; internal set; } = null!;

        public List<string> KeywordList { get; internal set; } = null!;
        public Func<string> MainAction { get; internal set; } = null!;
        public bool ClearText = true;
        public bool AddAtAwake = true;
        public bool AcceptAdditionalText = false;

        public int CommandType = 0; //0 base, 1 base confirm, 2 store node

        public NodeInfo InfoBase { get; internal set; } = null!;
        public NodeConfirmation ConfirmBase { get; internal set; } = null!;
        public int VerySpecialNum = -1; //for use with terminalstuff visual commands

        //Store Things
        public NodeStore StoreBase { get; internal set; } = null!;

        //Terminal Things
        public TerminalNode TerminalNode { get; internal set; } = null!;
        public List<TerminalKeyword> terminalKeywords = [];

        //should be able to call in awake
        public CommandManager(string commandName, ConfigEntry<bool> CommandBool, ConfigEntry<string> keywords, Func<string> commandFunc, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            IsEnabled = new(CommandBool);
            KeywordsConfig = keywords;
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2);
            if (addToMain)
                Plugin.AllCommands.Add(this);

            //prevents null errors
            InfoBase = new(this);
            ConfirmBase = new(this);
            StoreBase = new(this);
        }

        //should be able to call in awake, no config watch
        public CommandManager(string commandName, ConfigEntry<string> keywords, Func<string> commandFunc, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            KeywordsConfig = keywords;
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2);
            if (addToMain)
                Plugin.AllCommands.Add(this);

            InfoBase = new(this); //prevents errors from command not being added
        }

        //should be able to call in awake
        public CommandManager(string commandName, ConfigEntry<bool> CommandBool, List<string> manualWords, Func<string> commandFunc, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            IsEnabled = new(CommandBool);
            KeywordList = manualWords;
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2);
            if (addToMain)
                Plugin.AllCommands.Add(this);
        }

        //manual, with optional config watch
        public CommandManager(string commandName, List<string> manualWords, Func<string> commandFunc, ConfigEntry<bool> CommandBool = null!, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            KeywordList = manualWords;
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2);

            if (CommandBool != null!)
            {
                IsEnabled = new(CommandBool);
            }

            if (addToMain)
                Plugin.AllCommands.Add(this);
        }

        public bool IsCommandEnabled()
        {
            if (IsEnabled == null!)
                return true;

            if (!IsEnabled.NetworkingReq || IsEnabled.networkingConfig == null!)
                return IsEnabled.ConfigItem.Value;

            if (IsEnabled.NetworkingReq)
            {
                if (IsEnabled.networkingConfig.Value)
                    return IsEnabled.ConfigItem.Value;
            }

            return false;
        }

        //call this to set up custom info text (string)
        public void SetInfoText(string infoText)
        {
            InfoBase ??= new(this);
            InfoBase.InfoText = infoText;
        }

        //call this to set up custom info text (with added logic)
        public void SetInfoAction(Func<string> action)
        {
            InfoBase ??= new(this);
            InfoBase.InfoAction = action;
        }

        public void SetupConfirmation(Func<string> confirm, Func<string> deny)
        {
            if (confirm != null)
                ConfirmBase.SetConfirmAction(confirm);

            if (deny != null)
                ConfirmBase.SetDenyAction(deny);
        }

        public void SetupConfirmation(string confirmText, string denyText)
        {
            if (!string.IsNullOrEmpty(confirmText))
                ConfirmBase.SetConfirmText(confirmText);

            if (!string.IsNullOrEmpty(denyText))
                ConfirmBase.SetDenyText(denyText);
        }

        //call this if you need to add your command to the default listing and didnt on creation for some reason
        public void AddToDefaultListing()
        {
            if (!Plugin.AllCommands.Contains(this))
                Plugin.AllCommands.Add(this);
        }

        //should only be called on terminal awake, not allowing others to call method
        internal static void AddAllCommandsToTerminal()
        {
            if (Plugin.AllCommands.Count == 0)
                return;

            Plugin.AllCommands.DoIf(x => x.AddAtAwake, x => x.RegisterCommand());
        }

        //gets default info text from related config item if info is null or text is empty
        //then create info node
        //called after command has been created with keywords
        internal void GetInfo()
        {
            if (InfoBase == null!)
            {
                InfoBase = new(this);
                InfoBase.GetDefaultInfo(this);
            }
            else
            {
                if (InfoBase.InfoAction == null && InfoBase.InfoText.Length < 1)
                    InfoBase.GetDefaultInfo(this);
            }

            InfoBase.CreateInfoNode();
        }

        public void TerminalDisabled()
        {
            terminalKeywords = [];
            TerminalNode = null!;
        }

        //register command to terminal (should only be called after terminal exists
        public void RegisterCommand()
        {
            //Have to have this overload since this is used already without the bool
            RegisterCommand(true);
        }

        public void RegisterCommand(bool replaceExistingKW)
        {
            if (!IsCommandEnabled())
                return;

            TerminalNode = BasicTerminal.CreateNewTerminalNode();
            TerminalNode.name = Name;
            TerminalNode.displayText = string.Empty;
            TerminalNode.clearPreviousText = ClearText;


            if (KeywordList.Count == 0 && KeywordsConfig != null!)
                KeywordList = CommonStringStuff.GetKeywordsPerConfigItem(KeywordsConfig.Value);

            KeywordList.Do(w => AddKeyword(w, replaceExistingKW));
            IsCreated = true;

            if (CommandType > 0) //confirm base
                ConfirmBase.CreateConfirmation();

            if (CommandType == 2) //store base
                StoreBase.AddToStore();

            GetInfo();
        }

        public void RegisterKeywords(bool check)
        {
            KeywordList.Do(w => AddKeyword(w, check));

            IsCreated = true;
        }

        //will not create store/confirmation/info items for you due to no keyword
        public void RegisterNodeOnly()
        {
            if (!IsCommandEnabled())
                return;

            TerminalNode = BasicTerminal.CreateNewTerminalNode();
            TerminalNode.name = Name;
            TerminalNode.displayText = string.Empty;
            TerminalNode.clearPreviousText = ClearText;

            IsCreated = true;
        }

        internal void AddKeyword(string keyword, bool replaceExistingKW)
        {
            Loggers.LogDebug($"adding {keyword}");
            TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(Name + "_keyword", keyword, replaceExistingKW);
            terminalKeyword.specialKeywordResult = TerminalNode;
            terminalKeywords.Add(terminalKeyword);
        }

    }


}

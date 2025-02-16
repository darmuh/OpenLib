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
        public string Name = string.Empty;
        public ConfigWatch<bool> IsEnabled;
        public ConfigEntry<string> KeywordsConfig;

        public List<string> KeywordList;
        public Func<string> MainAction;
        public bool ClearText = true;
        public bool AddAtAwake = true;
        public bool AcceptAdditionalText = false;

        public int CommandType = 0; //0 base, 1 base confirm, 2 store node

        public NodeInfo InfoBase;
        public NodeConfirmation ConfirmBase;
        public NodeSpecial SpecialBase; //might replace this with an individual property (class has one singular property at the moment)

        //Store Things
        public NodeStore StoreBase;

        //Terminal Things
        public TerminalNode terminalNode;
        public List<TerminalKeyword> terminalKeywords = [];

        //should be able to call in awake
        public CommandManager(string commandName, ConfigEntry<bool> CommandBool, ConfigEntry<string> keywords, Func<string> commandFunc, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            IsEnabled = new(CommandBool);
            KeywordList = CommonStringStuff.GetKeywordsPerConfigItem(keywords.Value);
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2); 
            if(addToMain)
                Plugin.AllCommands.Add(this);
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
        public CommandManager(string commandName, List<string> manualWords, Func<string> commandFunc, ConfigEntry<bool> CommandBool = null, int type = 0, bool addToMain = true)
        {
            Name = commandName;
            KeywordList = manualWords;
            MainAction = commandFunc;
            CommandType = Mathf.Clamp(type, 0, 2);
            
            if(CommandBool != null)
            {
                IsEnabled = new(CommandBool);
            }

            if (addToMain)
                Plugin.AllCommands.Add(this);
        }

        public bool IsCommandEnabled()
        {
            if (IsEnabled == null)
                return true;

            if (!IsEnabled.NetworkingReq || IsEnabled.networkingConfig == null)
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

        //call this if you need to add your command to the default listing and didnt on creation for some reason
        public void AddToDefaultListing()
        {
            if(!Plugin.AllCommands.Contains(this))
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
        internal void GetInfo()
        {
            if(InfoBase == null)
            {
                InfoBase = new(this);
                InfoBase.GetDefaultInfo(this);
            }
            else
            {
                if(InfoBase.InfoAction == null && InfoBase.InfoText.Length < 1)
                    InfoBase.GetDefaultInfo(this);
            }
        }

        public void TerminalDisabled()
        {
            terminalKeywords = [];
            terminalNode = null!;
        }

        //register command to terminal (should only be called after terminal exists
        public void RegisterCommand()
        {
            if (!IsCommandEnabled())
                return;

            terminalNode = BasicTerminal.CreateNewTerminalNode();
            terminalNode.name = Name;
            terminalNode.displayText = string.Empty;
            terminalNode.clearPreviousText = ClearText;
            GetInfo();

            KeywordList.Do(w => AddKeyword(w));

            if(CommandType > 0) //confirm base
            {
                ConfirmBase.CreateConfirmation();
            }

            if(CommandType == 2) //store base
            {
                if (StoreBase == null)
                {
                    Plugin.WARNING("UNABLE TO ADD STORE ITEM, StoreBase is undefined!");
                    return;
                }
                StoreBase.AddToStore();
            }

            InfoBase.CreateInfoNode();

        }

        internal void AddKeyword(string keyword)
        {
            Plugin.Spam($"adding {keyword}");
            TerminalKeyword terminalKeyword = BasicTerminal.CreateNewTerminalKeyword(Name + "_keyword", keyword, true);
            terminalKeywords.Add(terminalKeyword);
        }

    }

    
}

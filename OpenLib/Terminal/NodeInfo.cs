using HarmonyLib;
using OpenLib.Common;
using OpenLib.CoreMethods;
using System;
using UnityEngine.InputSystem;

namespace OpenLib
{
    public class NodeInfo(CommandManager command)
    {
        public string Name = command.Name;

        //used to set custom displaytext for info node
        public string InfoText = string.Empty;

        //used to run a custom command for info node
        public Func<string> InfoAction = null!;

        public TerminalNode terminalNode = null!;


        public void GetDefaultInfo(CommandManager command)
        {
            string text = "[ " + CommonStringStuff.GetKeywordsForMenuItem(command.KeywordList) + " ]\r\n";
            if(command.IsEnabled != null)
                text += command.IsEnabled.ConfigItem.Description.Description + "\r\n\r\n";
            else
                text += "No further information on this command!\r\n\r\n";
            InfoText = text;
        }

        public void CreateInfoNode()
        {
            if (CommonTerminal.InfoKeyword == null)
                return;

            TerminalKeyword info = CommonTerminal.InfoKeyword;

            terminalNode = BasicTerminal.CreateNewTerminalNode();
            terminalNode.name = "info_" + Name;
            terminalNode.displayText = InfoText;
            terminalNode.clearPreviousText = true;
            command.KeywordList.Do(Keyword => AddingThings.AddCompatibleNoun(ref info, Keyword, terminalNode));
                
            Plugin.Spam("info node created and assigned to command!");
        }
    }
}
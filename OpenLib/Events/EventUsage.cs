using BepInEx.Configuration;
using HarmonyLib;
using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using OpenLib.InteractiveMenus;
using System.Collections.Generic;
using System.Linq;

namespace OpenLib.Events;
public class EventUsage
{
    public static List<ConfigFile> configsToReload = [];

    internal static void Subscribers()
    {
        EventManager.TerminalAwake.AddListener(OnTerminalAwake);
        EventManager.TerminalStart.AddListener(OnTerminalStart);
        EventManager.TerminalQuit.AddListener(OnTerminalQuit);
        EventManager.TerminalDisable.AddListener(OnTerminalDisable);
        EventManager.TerminalLoadNewNode.AddListener(OnLoadNewNode);
        EventManager.TerminalParseSent.AddListener(OnParseSent);
        EventManager.TerminalBeginUsing.AddListener(OnUsingTerminal);
        EventManager.GameNetworkManagerStart.AddListener(StartGame.OnGameStart);
        EventManager.TeleporterAwake.AddListener(Teleporter.CheckTeleporterTypeAndAssign);
        //EventManager.PlayerSpawn.AddListener(PlayerSpawned);
        //EventManager.PlayerEmote.AddListener(OnPlayerEmote);
        EventManager.TerminalMenuKeyPressed.AddListener(OnMenuKeyPress);

        //testing
        //EventManager.OnShipLandedMiscPatch.AddListener(Examples.Examples.TestMyTAO);
    }

    private static void OnTerminalAwake(Terminal instance)
    {
        Plugin.instance.Terminal = instance;
        Loggers.LogInfo($"Setting Plugin.instance.Terminal");
        CommandRegistry.GetCommandsToAdd(ConfigSetup.defaultManaged, ConfigSetup.defaultListing);
        CommandManager.AddAllCommandsToTerminal();
    }

    private static void OnTerminalDisable()
    {
        if (Plugin.instance.OpenBodyCamsMod)
            Compat.OpenBodyCamFuncs.ResidualCamsCheck();
        RemoveThings.OnTerminalDisable();
        TerminalStart.delayStartEnum = false;
        ListManagement.ClearLists();
        Plugin.AllCommands.Do(x => x.TerminalDisabled());

        foreach (ConfigFile config in configsToReload)
        {
            Loggers.LogDebug("reloading config from list");
            config.Save();
            config.Reload();
        }

        configsToReload.Clear();
    }

    private static void OnTerminalStart()
    {
        TerminalStart.TerminalStartGroupDelay();
    }

    private static void OnTerminalQuit()
    {
        if (MenusContainer.AllMenus.Count == 0)
            return;

        BetterMenuBase anyMenu = MenusContainer.AllMenus.FirstOrDefault(x => x.InMenu);
        anyMenu?.ExitTerminal.Invoke();
    }

    internal static void OnMenuKeyPress()
    {
        if (AllInteractiveMenus.AllMenus.Count != 0)
        {
            //check for interactive menus
            InteractiveMenu anyMenu = AllInteractiveMenus.AllMenus.FirstOrDefault(x => x.inMenu && x.isMenuEnabled);
            anyMenu?.HandleInput();
        }

        if (MenusContainer.AllMenus.Count != 0)
        {
            //check for BETTER menus
            BetterMenuBase anyMenu = MenusContainer.AllMenus.FirstOrDefault(x => x.InMenu);
            anyMenu?.InputEvent.Invoke();
        }

        CommonTerminal.TrySyncNodeOnly(Plugin.instance.Terminal.currentNode);
    }

    public static void OnUsingTerminal()
    {
        Loggers.LogInfo("Start Using Terminal Postfix");
    }

    private static bool ParseFailed(TerminalNode terminalNode, Terminal self)
    {
        //ParserError1
        if (terminalNode == self.terminalNodes.specialNodes[10])
            return true;

        //ParserError2
        if (terminalNode == self.terminalNodes.specialNodes[11])
            return true;

        //ParserError3
        if (terminalNode == self.terminalNodes.specialNodes[12])
            return true;
        
        return false;
    }

    public static TerminalNode OnParseSent(ref TerminalNode node)
    {
        Loggers.LogDebug("parsing sentence");
        if (node == null!)
        {
            Loggers.WARNING("node detected as NULL, returning...");
            return node!;
        }

        // only parse if node is currently a parser error node
        if (ParseFailed(node, Plugin.instance.Terminal))
        {
            string screenText = Plugin.instance.Terminal.screenText.text[^Plugin.instance.Terminal.textAdded..];
            if (screenText.Length > 0) //prevent errors being thrown from invalid text
            {
                //staying until all mods are done with old setup, after which version will be bumped to indicate breaking change
                if (LogicHandling.GetDisplayFromFaux(ConfigSetup.defaultListing.fauxKeywords, screenText, ref node))
                {
                    Loggers.LogInfo($"faux word detected on current node!");
                }

                //staying until all mods are done with old setup, after which version will be bumped to indicate breaking change
                if (CommonTerminal.TryGetNodeFromList(screenText, ConfigSetup.defaultListing.specialListString, out TerminalNode retrieveNode))
                {
                    node = retrieveNode;
                    Loggers.LogDebug($"node found matching specialListString in text - {screenText}");
                }

                if (CommonTerminal.TryGetCommand(screenText, out TerminalNode commandNode)) //grab node matching keyword
                {
                    node = commandNode;
                    Loggers.LogDebug($"node found matching CommandManager listing in text - {screenText}");
                }
            }
        }

        //staying until all mods are done with old setup, after which version will be bumped to indicate breaking change
        if (LogicHandling.GetNewDisplayText(ConfigSetup.defaultListing, ref node))
        {
            Loggers.LogInfo($"node found: {node.name}");
        }

        if (LogicHandling.GetDisplayTextFromCommand(ref node)) //update displaytext for matching node
        {
            Loggers.LogInfo($"command found: {node.name}");
        }

        return node;
    }

    public static void OnLoadNewNode(TerminalNode node)
    {
        if (node == null!)
            return;

        Loggers.LogDebug($"{node.name} has been loaded");

        if (node.acceptAnything && node.terminalOptions.Length < 1)
        {
            node.acceptAnything = false;
            Loggers.LogDebug("fixing node property to avoid errors! (eg. LLL route locked)");
        }
    }

}

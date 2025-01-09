using BepInEx.Configuration;
using GameNetcodeStuff;
using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenLib.Events
{
    public class EventUsage
    {
        public static List<ConfigFile> configsToReload = [];

        public static void Subscribers()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.TerminalStart.AddListener(OnTerminalStart);
            EventManager.TerminalDisable.AddListener(OnTerminalDisable);
            EventManager.TerminalLoadNewNode.AddListener(OnLoadNewNode);
            EventManager.TerminalParseSent.AddListener(OnParseSent);
            EventManager.TerminalBeginUsing.AddListener(OnUsingTerminal);
            EventManager.GameNetworkManagerStart.AddListener(StartGame.OnGameStart);
            EventManager.TeleporterAwake.AddListener(Teleporter.CheckTeleporterTypeAndAssign);
            //EventManager.PlayerSpawn.AddListener(PlayerSpawned);
            //EventManager.PlayerEmote.AddListener(OnPlayerEmote);
            EventManager.TerminalKeyPressed.AddListener(OnKeyPress);

            //testing
            EventManager.OnShipLandedMiscPatch.AddListener(TestMyTAO);
        }

        public static void TestMyTAO()
        {
            TerminalCodeObject<PlayerControllerB> playerCode = new(StartOfRound.Instance.localPlayerController, StartOfRound.Instance.localPlayerController.gameObject, true);
            playerCode.OnCodeUsed.AddListener(TestCodeEvent);
            playerCode.OnCooldownComplete.AddListener(TestCoolDownEvent);
            playerCode.SetTimers(10f, 0f);
        }

        public static void TestCodeEvent(TerminalAccessibleObject Code, PlayerControllerB playerOfCode)
        {
            if (Code == null)
            {
                Plugin.WARNING("Code is NULL!");
                return;
            }
            else
                Plugin.Log.LogDebug($"{Code.objectCode} activated! Status: isPoweredOn - {Code.isPoweredOn}, Cooldown: {Code.inCooldown}, Cooldown Timer: {Code.currentCooldownTimer}");

            //playerOfCode.drunkness = Mathf.Lerp(0f, 20f, 5f);
            playerOfCode.drunkness = 20f;

        }

        public static void TestCoolDownEvent(TerminalAccessibleObject Code, PlayerControllerB playerOfCode)
        {
            if (Code == null)
            {
                Plugin.WARNING("Code is NULL!");
                return;
            }
            else
                Plugin.Log.LogDebug($"{Code.objectCode} cooldown reached! Status: isPoweredOn - {Code.isPoweredOn}, Cooldown: {Code.inCooldown}, Cooldown Timer: {Code.currentCooldownTimer}");

            //playerOfCode.drunkness = Mathf.Lerp(0f, 20f, 5f);
            playerOfCode.drunkness = 0f;
        }

        public static void OnTerminalAwake(Terminal instance)
        {
            Plugin.instance.Terminal = instance;
            Plugin.MoreLogs($"Setting Plugin.instance.Terminal");
            CommandRegistry.GetCommandsToAdd(ConfigSetup.defaultManaged, ConfigSetup.defaultListing);
        }

        public static void OnTerminalDisable()
        {
            if (Plugin.instance.OpenBodyCamsMod)
                Compat.OpenBodyCamFuncs.ResidualCamsCheck();
            RemoveThings.OnTerminalDisable();
            TerminalStart.delayStartEnum = false;
            ListManagement.ClearLists();

            foreach (ConfigFile config in configsToReload)
            {
                Plugin.Spam("reloading config from list");
                config.Save();
                config.Reload();
            }

            configsToReload.Clear();
        }

        public static void OnTerminalStart()
        {
            TerminalStart.TerminalStartGroupDelay();
        }

        public static void OnKeyPress()
        {
            if (AllInteractiveMenus.AllMenus.Count == 0)
                return;

            //check for interactive menus
            InteractiveMenu anyMenu = AllInteractiveMenus.AllMenus.FirstOrDefault(x => x.inMenu && x.isMenuEnabled);
            anyMenu?.HandleInput();
        }

        public static void OnUsingTerminal()
        {
            Plugin.MoreLogs("Start Using Terminal Postfix");
        }

        public static TerminalNode OnParseSent(ref TerminalNode node)
        {
            Plugin.Spam("parsing sentence");
            if (node == null)
            {
                Plugin.WARNING("node detected as NULL, returning...");
                return node;
            }

            string screenText = Plugin.instance.Terminal.screenText.text.Substring(Plugin.instance.Terminal.screenText.text.Length - Plugin.instance.Terminal.textAdded);
            if (screenText.Length > 0) //prevent errors being thrown from invalid text
            {
                if (LogicHandling.GetDisplayFromFaux(ConfigSetup.defaultListing.fauxKeywords, screenText, ref node))
                {
                    Plugin.MoreLogs($"faux word detected on current node!");
                }

                if (CommonTerminal.TryGetNodeFromList(screenText, ConfigSetup.defaultListing.specialListString, out TerminalNode retrieveNode))
                {
                    node = retrieveNode;
                    Plugin.Spam($"node found matching specialListString in text - {screenText}");
                }
            }

            if (LogicHandling.GetNewDisplayText(ConfigSetup.defaultListing, ref node))
            {
                Plugin.MoreLogs($"node found: {node.name}");
            }

            return node;
        }

        public static void OnLoadNewNode(TerminalNode node)
        {
            Plugin.Spam($"listing count: {ConfigSetup.defaultListing.Listing.Count}");

            if (node == null)
                return;

            Plugin.Spam($"{node.name} has been loaded");

            if (node.acceptAnything && node.terminalOptions.Length < 1)
            {
                node.acceptAnything = false;
                Plugin.Spam("fixing node property to avoid errors! (eg. LLL route locked)");
            }
        }

    }
}

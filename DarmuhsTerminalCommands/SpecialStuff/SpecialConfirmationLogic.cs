using static TerminalStuff.DynamicCommands;
using static TerminalStuff.GetFromConfig;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using static TerminalStuff.TerminalEvents;
using Object = UnityEngine.Object;

namespace TerminalStuff
{
    internal class SpecialConfirmationLogic
    {

        internal static void InitKeywords()
        {
            Plugin.MoreLogs("Adding Confirmation keywords");

            if (ConfigSettings.terminalGamble.Value && ConfigSettings.ModNetworking.Value)
                MakeCommand($"{Gamble}_node", Gamble, "gamble prompt (this shouldnt show)", false, true, true, false, $"{Gamble}_confirm_node", $"{Gamble}_deny_node", "gamble accepted", "gamble denied", 0, GambaCommands.Ask2Gamble, GambaCommands.GambleConfirm, GambaCommands.GambleDeny, nodesThatAcceptNum, darmuhsTerminalStuff);
            if (ConfigSettings.terminalLever.Value)
            {
                if (ConfigSettings.leverConfirmOverride.Value)
                {
                    foreach (string keyword in leverKW)
                    {
                        MakeCommand($"{keyword}_node", keyword, "lever prompt", false, true, ShipControls.LeverControlCommand, darmuhsTerminalStuff);
                        Plugin.Spam($"Added lever command without confirmation required - keyword {keyword}");
                    }

                }
                else
                {
                    foreach (string keyword in leverKW)
                    {
                        MakeCommand($"{keyword}_node", keyword, "lever prompt", false, true, true, false, $"{keyword}_confirm_node", $"{keyword}_deny_node", $"{keyword} confirm", $"{keyword} deny", 0, ShipControls.AskLever, ShipControls.LeverControlCommand, ShipControls.DenyLever, darmuhsTerminalStuff);
                        Plugin.Spam($"Added lever command WITH confirmation required - keyword {keyword}");
                    }
                }
            }
            if (ConfigSettings.terminalLink.Value)
            {
                MakeCommand($"{Link}_node", Link, "link prompt", false, true, true, false, $"{Link}_confirm_node", $"{Link}_deny_node", "link confirm", "link deny", 0, MoreCommands.FirstLinkAsk, MoreCommands.FirstLinkDo, MoreCommands.FirstLinkDeny, darmuhsTerminalStuff);
                Plugin.Spam("Added link command WITH confirmation required");
            }
            if (ConfigSettings.terminalLink2.Value)
            {
                MakeCommand($"{Link2}_node", Link2, "link2 prompt", false, true, true, false, $"{Link2}_confirm_node", $"{Link2}_deny_node", "link confirm", "link deny", 0, MoreCommands.SecondLinkAsk, MoreCommands.SecondLinkDo, MoreCommands.SecondLinkDeny, darmuhsTerminalStuff);
                Plugin.Spam("Added link command WITH confirmation required");
            }
            if (ConfigSettings.terminalRestart.Value)
                if (ConfigSettings.restartConfirmOverride.Value)
                {
                    MakeCommand($"{Restart}_node", Restart, "restart prompt", false, true, RestartAction, darmuhsTerminalStuff);
                    Plugin.Spam("Added lever command without confirmation required");
                }
                else
                {
                    MakeCommand($"{Restart}_node", Restart, "restart prompt", false, true, true, false, $"{Restart}_confirm_node", $"{Restart}_deny_node", "restart confirm", "restart deny", 0, RestartAsk, RestartAction, RestartDeny, darmuhsTerminalStuff);
                    Plugin.Spam("Added lever command WITH confirmation required");
                }

        }

        internal static string RestartAsk()
        {
            string displayText = "Restart Lobby?\n\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
            return displayText;
        }

        internal static string RestartDeny()
        {
            string displayText = $"Restart lobby cancelled...\n\n";
            return displayText;
        }

        internal static string RestartAction()
        {
            if (!StartOfRound.Instance.inShipPhase)
            {
                string displayText = "This can only be done in orbit...\n\n";
                return displayText;
            }
            else if (!GameNetworkManager.Instance.localPlayerController.isHostPlayerObject)
            {
                string displayText = "Only the host can do this...\r\n";
                return displayText;
            }

            else
            {
                string displayText = "Restart lobby confirmed, getting new ship...\n\n";
                StartOfRound.Instance.ResetShip();
                Object.FindObjectOfType<Terminal>().SetItemSales();
                GameNetworkManager.Instance.SaveGameValues();
                Plugin.MoreLogs("restarting lobby");
                return displayText;
            }

        }
    }
}

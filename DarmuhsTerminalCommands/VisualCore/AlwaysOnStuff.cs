using System.Collections;
using UnityEngine;
using static TerminalStuff.AllMyTerminalPatches;

namespace TerminalStuff
{
    internal class AlwaysOnStuff
    {
        internal static bool dynamicStatus = false;
        internal static IEnumerator AlwaysOnDynamic(Terminal instance)
        {
            if (dynamicStatus)
                yield break;
            Plugin.MoreLogs("Starting AlwaysOnDynamic Coroutine");
            dynamicStatus = true;

            while (DisableScreenOnDeath() && !MoreCommands.keepAlwaysOnDisabled)
            {
                if (!StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(false);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        QuitPatch.TerminalCameraStatus(false);

                    Plugin.Spam("Disabling terminal screen.");
                }
                else if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && !instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(true);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        QuitPatch.TerminalCameraStatus(true);

                    Plugin.Spam("Enabling terminal screen.");
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (DisableScreenOnDeath()) 
            {
                instance.terminalUIScreen.gameObject.SetActive(false);
                if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                {
                    QuitPatch.TerminalCameraStatus(false);
                    Plugin.Spam("Cams disabled on player death");
                }

                Plugin.Spam("Player detected dead, disabling terminal screen.");
            }

            dynamicStatus = false; //end of coroutine, opening this up again for another run
        }

        internal static bool DisableScreenOnDeath()
        {
            if (ConfigSettings.alwaysOnWhileDead.Value)
                return false;

            return StartOfRound.Instance.localPlayerController.isPlayerDead;
        }
    }
}

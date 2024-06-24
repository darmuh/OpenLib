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

            while (!StartOfRound.Instance.localPlayerController.isPlayerDead && !MoreCommands.keepAlwaysOnDisabled)
            {
                if (!StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(false);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        QuitPatch.TerminalCameraStatus(false);

                    Plugin.MoreLogs("Disabling terminal screen.");
                }
                else if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && !instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(true);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        QuitPatch.TerminalCameraStatus(true);

                    Plugin.MoreLogs("Enabling terminal screen.");
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (DisableScreenOnDeath()) //config item for this? disableScreenOnDeath?
            {
                instance.terminalUIScreen.gameObject.SetActive(false);
                if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                {
                    QuitPatch.TerminalCameraStatus(false);
                    Plugin.MoreLogs("Cams disabled on player death");
                }

                Plugin.MoreLogs("Player detected dead, disabling terminal screen.");
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

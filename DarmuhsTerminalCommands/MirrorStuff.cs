using GameNetcodeStuff;


namespace TerminalStuff
{
    internal class MirrorStuff
    {
        //old layer info
        internal static int playerModelLayer;
        internal static int playerModelArmsLayer;

        internal static void QuitTerminalMirrorStuff()
        {
            if (!ConfigSettings.terminalMirror.Value)
                return;

            ModifyPlayerLayersForMirror(StartOfRound.Instance.localPlayerController, playerModelLayer, playerModelArmsLayer);
            Plugin.MoreLogs("Setting player layers back to saved values");
        }

        internal static void StartUsingTerminalMirrorStuff()
        {
            if (!ConfigSettings.terminalMirror.Value)
                return;

            SaveOriginalLayerInformation(StartOfRound.Instance.localPlayerController);
            ModifyPlayerLayersForMirror(StartOfRound.Instance.localPlayerController, 23, 5);
            Plugin.MoreLogs("Setting player layers for mirror view");
        }

        private static void SaveOriginalLayerInformation(PlayerControllerB player)
        {
            playerModelLayer = player.thisPlayerModel.gameObject.layer;
            playerModelArmsLayer = player.thisPlayerModelArms.gameObject.layer;

            Plugin.MoreLogs($"Saved layer information: {playerModelLayer} & {playerModelArmsLayer}");
        }

        internal static void ModifyPlayerLayersForMirror(PlayerControllerB player, int playerModel, int playerModelArms)
        {
            if (!ConfigSettings.terminalMirror.Value)
                return;

            //Credits to QuackAndCheese, using this portion of their player patch from MirrorDecor

            player.thisPlayerModel.gameObject.layer = playerModel; //23
            //player.thisPlayerModelLOD1.gameObject.layer = 5;
            player.thisPlayerModelArms.gameObject.layer = playerModelArms; //5

            Plugin.MoreLogs($"Set layer information: playerModel - {playerModel} & playerModelArms - {playerModelArms}");
        }
    }
}

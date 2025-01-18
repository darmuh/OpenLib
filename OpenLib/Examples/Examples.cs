using GameNetcodeStuff;
using OpenLib.CoreMethods;

namespace OpenLib.Examples
{
    internal class Examples
    {
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
    }
}

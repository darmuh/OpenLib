using System.Collections;
using UnityEngine;

namespace OpenLib.Compat
{
    internal class TMECompat
    {
        internal static bool runOnce = false;
        internal static void EmoteDetected()
        {
            if (runOnce)
                return;

            runOnce = true;
            Plugin.Spam("EMOTE DETECTED!"); //this DOES detect tme emotes (using the server rpc, need to check on clients though)
            Plugin.instance.StartCoroutine(OverrideLayer());
        }

        internal static IEnumerator OverrideLayer()
        {
            while (TooManyEmotes.EmoteControllerPlayer.emoteControllerLocal.isPerformingEmote)
            {
                yield return new WaitForEndOfFrame();
            }

            Plugin.Spam("Emote ended");
            yield return new WaitForEndOfFrame();
            if (Plugin.instance.MirrorDecor)
                StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject.layer = 23;
            else
                StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject.layer = LayerMask.NameToLayer("Vehicle");

            runOnce = false;
        }

        internal static int CullingMaskUpdate(int currentMask)
        {
            //int UI = (1 << 5); //TME assigns this layer to hide things (UI)
            //int player = (1 << 3); //TME assigns hands to this player model
            //int enemies = (1 << 23); //TME assigns this layer to hide player model in first person pov (player)

            //When emoting TME assigns player model to layer 3 (player), when not emoting player model is assigned layer 23 (enemiesnotrendered)
            //showing layer 23 will have the consequence of showing ghost girl t-posing on occasion
            //looks like I can use layer 30 (vehicle) to show things that TME wants to hide

            if (Plugin.instance.MirrorDecor)
                currentMask |= (1 << 23); //flip layer23 bit (mirrordecor uses this one)
            else
                currentMask |= LayerMask.GetMask("Vehicle"); //show layer30 (not visible on most cams, so can use this layer for our needs)

            //Set model to be visible in Vehicle layer
            //LOD1 is LOD2, LOD2 is LOD3, thisPlayerModel is LOD1 in unityexplorer lmao
            if (Plugin.instance.MirrorDecor)
                StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject.layer = 23;
            else
                StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject.layer = LayerMask.NameToLayer("Vehicle");

            //current implementation shows both hands and body during first person pov

            Plugin.Spam($"Cam cullingmask {currentMask}");
            return currentMask;
        }
    }
}

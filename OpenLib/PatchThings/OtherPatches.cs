using GameNetcodeStuff;
using HarmonyLib;
using OpenLib.Events;
using UnityEngine;

namespace OpenLib
{
    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    public class StartRoundAwake
    {
        public static void Postfix()
        {
            EventManager.StartOfRoundAwake.Invoke();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "Start")]
    public class StartRoundPatch
    {
        public static void Postfix()
        {
            EventManager.StartOfRoundStart.Invoke();
        }
    }

    //StartGame
    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    public class LandingPatch
    {
        public static void Postfix()
        {
            EventManager.StartOfRoundStartGame.Invoke();
        }

    }

    [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
    public class ShipResetPatch
    {
        public static void Postfix()
        {
            EventManager.ShipReset.Invoke();
        }
    }

    [HarmonyPatch(typeof(RoundManager), "SetBigDoorCodes")]
    public class SetBigDoorCodes
    {
        public static void Postfix()
        {
            EventManager.SetBigDoorCodes.Invoke();
        }
    }

    //SpawnMapObjects
    [HarmonyPatch(typeof(RoundManager), "SpawnMapObjects")]
    public class SpawnMapObjects
    {
        public static void Postfix()
        {
            EventManager.SpawnMapObjects.Invoke();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "PassTimeToNextDay")]
    public class NextDayPatch
    {
        public static void Postfix()
        {
            EventManager.NextDayEvent.Invoke();
        }
    }

    //OnShipLandedMiscEvents
    [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
    public class OnShipLandedMiscPatch
    {
        public static void Postfix()
        {
            EventManager.OnShipLandedMiscPatch.Invoke();
        }
    }

    //ShipHasLeft
    [HarmonyPatch(typeof(StartOfRound), "ShipHasLeft")]
    public class ShipLeftPatch
    {
        public static void Postfix()
        {
            EventManager.ShipLeft.Invoke();
        }
    }

    //SetNewProfitQuota
    [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
    public class NewQuotaPatch
    {
        public static void Postfix()
        {
            EventManager.NewQuota.Invoke();
        }
    }

    //ChangeLevel
    [HarmonyPatch(typeof(StartOfRound), "ChangeLevel")]
    public class RouteEvent
    {
        public static void Postfix()
        {
            EventManager.StartOfRoundChangeLevel.Invoke();
        }

    }

    //AutoParentGameObject
    [HarmonyPatch(typeof(AutoParentToShip), "Awake")]
    public class AutoParentGameObjectPatch
    {
        public static void Postfix(AutoParentToShip __instance)
        {
            if (__instance.gameObject == null)
                return;

            EventManager.AutoParentEvent.Invoke(__instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "StartPerformingEmoteServerRpc")]
    public class EmotePatch
    {
        public static void Postfix()
        {
            EventManager.PlayerEmote.Invoke();
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class PlayerUpdatePatch
    {
        //InShipEvent
        public static bool usePatch = false;
        public static bool inShip = false;
        public static bool spectate_inShip = false;
        public static bool isDead = false;

        public static void Postfix(PlayerControllerB __instance)
        {
            if (!usePatch) //any mod that wishes to use this patch needs to enable this
                return;

            if (StartOfRound.Instance == null) //in case this doesnt exist yet
                return;

            if (StartOfRound.Instance.localPlayerController == null) //or this
                return;

            if (StartOfRound.Instance.localPlayerController != __instance) //stop from detecting other player's updates
                return;

            if(__instance.isInHangarShipRoom != inShip) //local player IsInShip update
            {
                inShip = __instance.isInHangarShipRoom;
                EventManager.PlayerIsInShip.Invoke();
            }

            if(__instance.isPlayerDead != isDead) //local player isPlayerDead update
            {
                isDead = __instance.isPlayerDead;
                EventManager.PlayerIsDead.Invoke();
            }

            if (__instance.isPlayerDead && __instance.spectatedPlayerScript != null) //local player is dead and spectatedPlayer is not null
            {
                if (__instance.spectatedPlayerScript.isInHangarShipRoom != spectate_inShip) //spectatedPlayer IsInShip update
                {
                    spectate_inShip = __instance.spectatedPlayerScript.isInHangarShipRoom;
                    EventManager.SpecatingPlayerIsInShip.Invoke();
                }
            }
        }
    }

    public class SpectateNextPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "SpectateNextPlayer")]
        public class PlayerSpawnPatch : MonoBehaviour
        {
            static void Postfix()
            {
                EventManager.SpecateNextPlayer.Invoke();
            }
        }
    }

    public class SpawnPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "SpawnPlayerAnimation")]
        public class PlayerSpawnPatch : MonoBehaviour
        {
            static void Postfix()
            {
                EventManager.PlayerSpawn.Invoke();
            }
        }
    }

    [HarmonyPatch(typeof(ShipTeleporter), "Awake")]
    public class TeleporterInit : ShipTeleporter
    {
        static void Postfix(ShipTeleporter __instance)
        {
            EventManager.TeleporterAwake.Invoke(__instance);
        }


    }

    [HarmonyPatch(typeof(GameNetworkManager), "Start")]
    public class GameStartPatch
    {
        public static void Postfix()
        {
            EventManager.GameNetworkManagerStart.Invoke();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "OnClientConnect")]
    public class OnClientConnectPatch
    {
        public static void Postfix()
        {
            EventManager.OnClientConnect.Invoke();
        }
    }
}

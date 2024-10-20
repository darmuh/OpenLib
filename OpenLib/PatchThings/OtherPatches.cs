using GameNetcodeStuff;
using HarmonyLib;
using OpenLib.Events;
using System;
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

    //ChangeLevel
    [HarmonyPatch(typeof(StartOfRound), "ChangeLevel")]
    public class RouteEvent
    {
        public static void Postfix()
        {
            EventManager.StartOfRoundChangeLevel.Invoke();
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

            if(__instance.isInHangarShipRoom != inShip)
            {
                inShip = __instance.isInHangarShipRoom;
                EventManager.PlayerIsInShip.Invoke();
            }

            if(__instance.isPlayerDead != isDead)
            {
                isDead = __instance.isPlayerDead;
                EventManager.PlayerIsDead.Invoke();
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

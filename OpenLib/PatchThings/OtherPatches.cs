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

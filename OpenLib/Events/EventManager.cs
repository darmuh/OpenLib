//https://github.com/AndreyMrovol/LethalMrovLib/blob/main/MrovLib/EventManager.cs
//used above from mrov as inspiration

using UnityEngine;

namespace OpenLib.Events
{
    public static class EventManager
    {

        //no param needed
        public static Events.CustomEvent TerminalDisable = new();
        public static Events.CustomEvent TerminalStart = new();
        public static Events.CustomEvent TerminalBeginUsing = new();
        public static Events.CustomEvent TerminalQuit = new();

        //uses terminal instance
        public static Events.CustomEvent<Terminal> TerminalAwake = new();

        //gets terminalNode
        public static Events.CustomEvent<TerminalNode> TerminalLoadIfAffordable = new();
        public static Events.TerminalNodeEvent TerminalParseSent = new();
        //public static Events.TerminalKeywordEvent TerminalParseWord = new();
        public static Events.CustomEvent<TerminalNode> TerminalLoadNewNode = new();
        public static Events.TerminalNodeEvent GetNewDisplayText = new();

        //Other patch events
        public static Events.CustomEvent StartOfRoundAwake = new();
        public static Events.CustomEvent StartOfRoundStart = new();
        public static Events.CustomEvent StartOfRoundStartGame = new();
        public static Events.CustomEvent StartOfRoundChangeLevel = new();
        public static Events.CustomEvent ShipReset = new();
        public static Events.CustomEvent SetBigDoorCodes = new();
        public static Events.CustomEvent SpawnMapObjects = new();
        public static Events.CustomEvent NextDayEvent = new();
        public static Events.CustomEvent OnShipLandedMiscPatch = new();
        public static Events.CustomEvent ShipLeft = new();
        public static Events.CustomEvent NewQuota = new();
        public static Events.CustomEvent PlayerSpawn = new();
        public static Events.CustomEvent PlayerEmote = new();
        public static Events.CustomEvent<ShipTeleporter> TeleporterAwake = new();
        public static Events.CustomEvent NormalTPFound = new();
        public static Events.CustomEvent InverseTPFound = new();
        public static Events.CustomEvent GameNetworkManagerStart = new();
        public static Events.CustomEvent OnClientConnect = new();
        public static Events.CustomEvent SpecateNextPlayer = new(); //SpectateNextPlayer

        //personal events
        public static Events.CustomEvent TerminalDelayStart = new();

        //TerminalUpdate Events
        public static Events.CustomEvent SetTerminalInUse = new();
        public static Events.CustomEvent TerminalKeyPressed = new ();

        //PlayerUpdate Events
        public static Events.CustomEvent PlayerIsInShip = new();
        public static Events.CustomEvent PlayerIsDead = new();
        public static Events.CustomEvent SpecatingPlayerIsInShip = new();

        //GameObject Events
        public static Events.CustomEvent<GameObject> AutoParentEvent = new();

    }
}

//https://github.com/AndreyMrovol/LethalMrovLib/blob/main/MrovLib/EventManager.cs
//used above from mrov as inspiration

using GameNetcodeStuff;

namespace OpenLib.Events
{
    public static class EventManager
    {

        //no param needed
        public static Events.CustomEvent TerminalDisable = new();
        public static Events.CustomEvent TerminalStart = new();
        public static Events.CustomEvent TerminalBeginUsing = new();
        public static Events.CustomEvent SetTerminalInUse = new();
        public static Events.CustomEvent TerminalQuit = new();

        //uses terminal instance
        public static Events.CustomEvent<Terminal> TerminalAwake = new();

        //gets terminalNode
        public static Events.CustomEvent<TerminalNode> TerminalLoadIfAffordable = new();
        public static Events.TerminalNodeEvent TerminalParseSent = new();
        public static Events.CustomEvent<TerminalNode> TerminalLoadNewNode = new();
        public static Events.CustomEvent<TerminalNode> GetNewDisplayText = new();

        //Other patch events
        public static Events.CustomEvent StartOfRoundAwake = new();
        public static Events.CustomEvent StartOfRoundStart = new();
        public static Events.CustomEvent StartOfRoundStartGame = new();
        public static Events.CustomEvent PlayerSpawn = new();
        public static Events.CustomEvent<ShipTeleporter> TeleporterAwake = new();
        public static Events.CustomEvent NormalTPFound = new();
        public static Events.CustomEvent InverseTPFound = new();
        public static Events.CustomEvent GameNetworkManagerStart = new();

        //personal events
        public static Events.CustomEvent TerminalDelayStart = new();

    }
}

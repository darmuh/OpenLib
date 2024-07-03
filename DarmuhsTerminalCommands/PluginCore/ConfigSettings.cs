using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace TerminalStuff
{
    public static class ConfigSettings
    {
        //keybinds
        public static ConfigEntry<string> walkieTermKey { get; internal set; }
        public static ConfigEntry<string> walkieTermMB { get; internal set; }
        public static ConfigEntry<string> keyActionsConfig { get; internal set; }

        //cams special
        public static ConfigEntry<bool> camsUseDetectedMods { get; internal set; }
        public static ConfigEntry<string> obcResolutionMirror {  get; internal set; }
        public static ConfigEntry<string> obcResolutionBodyCam { get; internal set; }

        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> ModNetworking { get; internal set; }
        public static ConfigEntry<bool> terminalClock { get; internal set; } //Clock object itself
        public static ConfigEntry<bool> extensiveLogging { get; internal set; }
        public static ConfigEntry<bool> developerLogging { get; internal set; }
        public static ConfigEntry<bool> walkieTerm { get; internal set; } //Use walkie at terminal function
        public static ConfigEntry<bool> terminalShortcuts { get; internal set; } //adds the bind keyword and the enumerator checking for shortcuts
        public static ConfigEntry<bool> terminalLobby { get; internal set; } //lobby name command
        public static ConfigEntry<bool> terminalCams { get; internal set; } //cams command
        public static ConfigEntry<bool> terminalQuit { get; internal set; } //quit command
        public static ConfigEntry<bool> terminalClear { get; internal set; } //clear command
        public static ConfigEntry<bool> terminalLoot { get; internal set; } //loot command
        public static ConfigEntry<bool> terminalVideo { get; internal set; } //video command
        public static ConfigEntry<bool> terminalHeal { get; internal set; } //heal command
        public static ConfigEntry<bool> terminalFov { get; internal set; } //Fov command
        public static ConfigEntry<bool> terminalGamble { get; internal set; } //Gamble command
        public static ConfigEntry<bool> terminalLever { get; internal set; } //Lever command
        public static ConfigEntry<bool> terminalDanger { get; internal set; } //Danger command
        public static ConfigEntry<bool> terminalVitals { get; internal set; } //Vitals command
        public static ConfigEntry<bool> terminalBioScan { get; internal set; } //BioScan command
        public static ConfigEntry<bool> terminalBioScanPatch { get; internal set; } //BioScan Upgrade command
        public static ConfigEntry<bool> terminalVitalsUpgrade { get; internal set; } //Vitals Upgrade command
        public static ConfigEntry<bool> terminalTP { get; internal set; } //Teleporter command
        public static ConfigEntry<bool> terminalITP { get; internal set; } //Inverse Teleporter command
        public static ConfigEntry<bool> terminalMods { get; internal set; } //Modlist command
        public static ConfigEntry<bool> terminalKick { get; internal set; } //Kick command (host only)
        public static ConfigEntry<bool> terminalFcolor { get; internal set; } //Flashlight color command
        public static ConfigEntry<bool> terminalMap { get; internal set; } //Map shortcut
        public static ConfigEntry<bool> terminalMinimap { get; internal set; } //Minimap command
        public static ConfigEntry<bool> terminalMinicams { get; internal set; } //Minicams command
        public static ConfigEntry<bool> terminalOverlay { get; internal set; } //Overlay cams command
        public static ConfigEntry<bool> terminalDoor { get; internal set; } //Door Toggle command
        public static ConfigEntry<bool> terminalLights { get; internal set; } //Light Toggle command
        public static ConfigEntry<bool> terminalScolor { get; internal set; } //Light colors command
        public static ConfigEntry<bool> terminalAlwaysOn { get; internal set; } //AlwaysOn command
        public static ConfigEntry<bool> terminalLink { get; internal set; } //Link command
        public static ConfigEntry<bool> terminalLink2 { get; internal set; } //Link2 command
        public static ConfigEntry<bool> terminalRandomSuit { get; internal set; } //RandomSuit command
        public static ConfigEntry<bool> terminalClockCommand { get; internal set; } //toggle clock command
        public static ConfigEntry<bool> terminalListItems { get; internal set; } //List Items Command
        public static ConfigEntry<bool> terminalLootDetail { get; internal set; } //List Scrap Command
        public static ConfigEntry<bool> terminalMirror { get; internal set; } //mirror command
        public static ConfigEntry<bool> terminalRefund { get; internal set; } //refund command
        public static ConfigEntry<bool> terminalRestart { get; internal set; } //restart command
        public static ConfigEntry<bool> terminalPrevious { get; internal set; } //previous switch command
        public static ConfigEntry<bool> terminalRouteRandom { get; internal set; } // route random command
        public static ConfigEntry<bool> terminalPurchasePacks { get; internal set; } // purchase packs feature



        //Strings for display messages
        public static ConfigEntry<bool> canOpenDoorInSpace { get; internal set; } //bool to allow for opening door in space
        public static ConfigEntry<string> doorOpenString { get; internal set; } //Door String
        public static ConfigEntry<string> doorCloseString { get; internal set; } //Door String
        public static ConfigEntry<string> doorSpaceString { get; internal set; } //Door String
        public static ConfigEntry<string> quitString { get; internal set; } //Quit String
        public static ConfigEntry<string> leverString { get; internal set; } //Lever String
        public static ConfigEntry<string> videoStartString { get; internal set; } //lol, start video string
        public static ConfigEntry<string> videoStopString { get; internal set; } //lol, stop video string
        public static ConfigEntry<string> tpMessageString { get; internal set; } //TP Message String
        public static ConfigEntry<string> itpMessageString { get; internal set; } //TP Message String
        public static ConfigEntry<string> vitalsPoorString { get; internal set; } //Vitals can't afford string
        public static ConfigEntry<string> vitalsUpgradePoor { get; internal set; } //Vitals Upgrade can't afford string
        public static ConfigEntry<string> healIsFullString { get; internal set; } //full health string
        public static ConfigEntry<string> healString { get; internal set; } //healing player string
        public static ConfigEntry<string> camString { get; internal set; } //Cameras on string
        public static ConfigEntry<string> camString2 { get; internal set; } //Cameras off string
        public static ConfigEntry<string> mapString { get; internal set; } //map on string
        public static ConfigEntry<string> mapString2 { get; internal set; } //map off string
        public static ConfigEntry<string> ovString { get; internal set; } //overlay on string
        public static ConfigEntry<string> ovString2 { get; internal set; } //overlay off string
        public static ConfigEntry<string> mmString { get; internal set; } //minimap on string
        public static ConfigEntry<string> mmString2 { get; internal set; } //minimap off string
        public static ConfigEntry<string> mcString { get; internal set; } //minicam on string
        public static ConfigEntry<string> mcString2 { get; internal set; } //minicam off string


        //Cost configs
        public static ConfigEntry<int> vitalsCost { get; internal set; } //Cost of Vitals Command
        public static ConfigEntry<int> vitalsUpgradeCost { get; internal set; } //Cost of Vitals Upgrade Command
        public static ConfigEntry<int> bioScanUpgradeCost { get; internal set; } //Cost of Enemy Scan Upgrade Command
        public static ConfigEntry<int> enemyScanCost { get; internal set; } //Cost of Enemy Scan Command

        //Other config items
        public static ConfigEntry<int> gambleMinimum { get; internal set; } //Minimum amount of credits needed to gamble
        public static ConfigEntry<bool> gamblePityMode { get; internal set; } //enable or disable pity for gamblers
        public static ConfigEntry<int> gamblePityCredits { get; internal set; } //Pity Credits for losers
        public static ConfigEntry<string> gamblePoorString { get; internal set; } //gamble credits too low string
        public static ConfigEntry<string> videoFolderPath { get; internal set; } //Specify a different folder with videos
        public static ConfigEntry<bool> videoSync { get; internal set; } //Should videos be synced between players (good for AOD)
        public static ConfigEntry<bool> leverConfirmOverride { get; internal set; } //disable confirmation check for lever
        public static ConfigEntry<bool> restartConfirmOverride { get; internal set; } //disable confirmation check for lever
        public static ConfigEntry<bool> camsNeverHide { get; internal set; }
        public static ConfigEntry<bool> networkedNodes { get; internal set; } //enable or disable networked terminal nodes (beta)
        public static ConfigEntry<string> defaultCamsView { get; internal set; }
        public static ConfigEntry<int> ovOpacity { get; internal set; } //Opacity Percentage for Overlay Cams View
        public static ConfigEntry<string> customLink { get; internal set; }
        public static ConfigEntry<string> customLink2 { get; internal set; }
        public static ConfigEntry<string> customLinkHint { get; internal set; }
        public static ConfigEntry<string> customLink2Hint { get; internal set; }
        public static ConfigEntry<string> homeLine1 { get; internal set; }
        public static ConfigEntry<string> homeLine2 { get; internal set; }
        public static ConfigEntry<string> homeLine3 { get; internal set; }
        public static ConfigEntry<string> homeHelpLines { get; internal set; }
        public static ConfigEntry<string> homeTextArt { get; internal set; }
        public static ConfigEntry<bool> alwaysOnAtStart { get; internal set; }
        public static ConfigEntry<bool> alwaysOnDynamic { get; internal set; }
        public static ConfigEntry<bool> alwaysOnWhileDead { get; internal set; }
        public static ConfigEntry<string> routeRandomBannedWeather { get; internal set; }
        public static ConfigEntry<int> routeRandomCost { get; internal set; }
        public static ConfigEntry<string> purchasePackCommands { get; internal set; }

        //keyword strings (terminalapi)
        public static ConfigEntry<string> alwaysOnKeywords { get; internal set; } //string to match keyword
        public static ConfigEntry<string> minimapKeywords { get; internal set; }
        public static ConfigEntry<string> minicamsKeywords { get; internal set; }
        public static ConfigEntry<string> overlayKeywords { get; internal set; }
        public static ConfigEntry<string> doorKeywords { get; internal set; }
        public static ConfigEntry<string> lightsKeywords { get; internal set; }
        public static ConfigEntry<string> modsKeywords { get; internal set; }
        public static ConfigEntry<string> tpKeywords { get; internal set; }
        public static ConfigEntry<string> itpKeywords { get; internal set; }
        public static ConfigEntry<string> quitKeywords { get; internal set; }
        public static ConfigEntry<string> videoKeywords { get; internal set; }
        public static ConfigEntry<string> clearKeywords { get; internal set; }
        public static ConfigEntry<string> dangerKeywords { get; internal set; }
        public static ConfigEntry<string> healKeywords { get; internal set; }
        public static ConfigEntry<string> lootKeywords { get; internal set; }
        public static ConfigEntry<string> camsKeywords { get; internal set; }
        public static ConfigEntry<string> mapKeywords { get; internal set; }
        public static ConfigEntry<string> mirrorKeywords { get; internal set; }
        public static ConfigEntry<string> randomSuitKeywords { get; internal set; }
        public static ConfigEntry<string> clockKeywords { get; internal set; }
        public static ConfigEntry<string> ListItemsKeywords { get; internal set; } //List Items Command
        public static ConfigEntry<string> ListScrapKeywords { get; internal set; } //List Scrap Command
        public static ConfigEntry<string> randomRouteKeywords { get; internal set; }
        public static ConfigEntry<string> lobbyKeywords { get; internal set; } //show lobby name keywords


        //terminal patcher keywords
        public static ConfigEntry<string> fcolorKeyword { get; internal set; }
        public static ConfigEntry<string> gambleKeyword { get; internal set; }
        public static ConfigEntry<string> leverKeywords { get; internal set; }
        public static ConfigEntry<string> scolorKeyword { get; internal set; }
        public static ConfigEntry<string> linkKeyword { get; internal set; }
        public static ConfigEntry<string> link2Keyword { get; internal set; }



        //terminal patcher words 
        //"lobby", "home", "more", "next", "comfort", "controls", "extras", "fun", "kick",
        // "fcolor", "fov", "gamble", "lever", "vitalspatch", "bioscan", "bioscanpatch", "scolor"

        public static void BindConfigSettings()
        {

            Plugin.Log.LogInfo("Binding configuration settings");

            //Network Configs
            networkedNodes = MakeBool("Networked Things", "networkedNodes", false, "Enable networked Always-On Display & displaying synced terminal nodes (BETA)");
            ModNetworking = MakeBool("__General", "ModNetworking", true, "Disable this if you want to disable networking and use this mod as a Client-sided mod");
            terminalClock = MakeBool("__General", "terminalClock", true, "Enable or Disable the terminalClock");
            walkieTerm = MakeBool("__General", "walkieTerm", true, "Enable or Disable the ability to use a walkie from your inventory at the terminal (vanilla method still works)");
            terminalShortcuts = MakeBool("__General", "terminalShortcuts", true, "Enable this for the ability to bind commands to any valid key (also enables the \"bind\" keyword.");
            extensiveLogging = MakeBool("__Debug", "extensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            developerLogging = MakeBool("__Debug", "developerLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");
            keyActionsConfig = MakeString("Terminal Shortcuts", "keyActionsConfig", "", "Stored keybinds, don't modify this unless you know what you're doing!");
            purchasePackCommands = MakeString("Purchase packs", "purchasePackCommands", "Essentials:pro,shov,walkie;PortalPack:teleporter,inverse", "List of purchase pack commands to create. Format is command:item1,item2,etc.;next command:item1,item2");


            //keybinds
            walkieTermKey = MakeString("__General", "walkieTermKey", "LeftAlt", "Key used to activate your walkie while at the terminal, see here for valid key names https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html");
            walkieTermMB = MakeString("__General", "walkieTermMB", "Left", "Mousebutton used to activate your walkie while at the terminal, see here for valid button names https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/api/UnityEngine.InputSystem.LowLevel.MouseButton.html");

            //Cams Mod Config
            camsUseDetectedMods = MakeBool("__General", "camsUseDetectedMods", true, "With this enabled, this mod will detect if another mod that adds player cams is enabled and use the mod's camera for all cams commands. Currently detects the following: Helmet Cameras by Rick Arg, Body Cameras by Solo, OpenBodyCams by ");

            //enable or disable
            terminalLobby = MakeBool("Comfort Commands (On/Off)", "terminalLobby", true, "Shows the current lobby name <Lobby Name>");
            terminalQuit = MakeBool("Comfort Commands (On/Off)", "terminalQuit", true, "Command to quit terminal <Quit>");
            terminalClear = MakeBool("Comfort Commands (On/Off)", "terminalClear", true, "Command to clear terminal text <Clear>");
            terminalLoot = MakeBool("Extras Commands (On/Off)", "terminalLoot", true, "Command to show total onboard loot value <Loot>");
            terminalCams = MakeBool("Extras Commands (On/Off)", "terminalCams", true, "Command to toggle displaying cameras in terminal <Cameras>");
            terminalVideo = MakeBool("Fun Commands (On/Off)", "terminalVideo", true, "Play a video from the videoFolderPath folder <video>");
            terminalHeal = MakeBool("Comfort Commands (On/Off)", "terminalHeal", true, "Command to heal yourself <Heal>");
            terminalFov = MakeBool("Comfort Commands (On/Off)", "terminalFov", true, "Command to change your FOV <Fov>");
            terminalGamble = MakeBool("Fun Commands (On/Off)", "terminalGamble", true, "Command to gamble your credits, by percentage <Gamble>");
            terminalLever = MakeBool("Controls Commands (On/Off)", "terminalLever", true, "Pull the lever from terminal <Lever>");
            terminalDanger = MakeBool("Controls Commands (On/Off)", "terminalDanger", true, "Check moon danger level <Danger>");
            terminalVitals = MakeBool("Extras Commands (On/Off)", "terminalVitals", true, "Scan player being tracked by monitor for their Health/Weight. <Vitals>");
            terminalBioScan = MakeBool("Extras Commands (On/Off)", "terminalBioScan", true, "Scan for \"non-employee\" lifeforms. <BioScan>");
            terminalBioScanPatch = MakeBool("Extras Commands (On/Off)", "terminalBioScanPatch", true, "Purchase-able upgrade patch to bioscan for more precise information. <BioScan>");
            terminalVitalsUpgrade = MakeBool("Extras Commands (On/Off)", "terminalVitalsUpgrade", true, "Purchase-able upgrade to vitals to not cost anything each scan. <Vitals>");
            terminalTP = MakeBool("Controls Commands (On/Off)", "terminalTP", true, "Command to Activate Teleporter <TP>");
            terminalITP = MakeBool("Controls Commands (On/Off)", "terminalITP", true, "Command to Activate Inverse Teleporter <ITP>");
            terminalMods = MakeBool("Comfort Commands (On/Off)", "terminalMods", true, "Command to see your active mods <Mods>");
            terminalKick = MakeBool("Comfort Commands (On/Off)", "terminalKick", false, "Enables kick command for host. <Kick>");
            terminalFcolor = MakeBool("Fun Commands (On/Off)", "terminalFcolor", true, "Command to change flashlight color. <Fcolor colorname>");
            terminalScolor = MakeBool("Fun Commands (On/Off)", "terminalScolor", true, "Command to change ship lights colors. <Scolor all,front,middle,back colorname>");
            terminalDoor = MakeBool("Controls Commands (On/Off)", "terminalDoor", true, "Command to open/close the ship door. <Door>");
            terminalLights = MakeBool("Controls Commands (On/Off)", "terminalLights", true, "Command to toggle the ship lights");
            terminalMap = MakeBool("Extras Commands (On/Off)", "terminalMap", true, "Adds 'map' shortcut to 'view monitor' command <Map>");
            terminalMinimap = MakeBool("Extras Commands (On/Off)", "terminalMinimap", true, "Command to view cams with radar at the top right. <Minimap>");
            terminalMinicams = MakeBool("Extras Commands (On/Off)", "terminalMinicams", true, "Command to view radar with cams at the top right. <Minicams>");
            terminalOverlay = MakeBool("Extras Commands (On/Off)", "terminalOverlay", true, "Command to view cams with radar overlayed on top. <Overlay>");
            terminalAlwaysOn = MakeBool("Comfort Commands (On/Off)", "terminalAlwaysOn", true, $"Command to toggle Always-On Display <Alwayson>");
            terminalLink = MakeBool("Extras Commands (On/Off)", "terminalLink", true, "Command to link to an external web-page <Link>");
            terminalLink2 = MakeBool("Extras Commands (On/Off)", "terminalLink2", false, "Command to link to a second external web-page <Link2>");
            terminalRandomSuit = MakeBool("Fun Commands (On/Off)", "terminalRandomSuit", true, "Command to switch your suit from a random one off the rack <RandomSuit>");
            terminalClockCommand = MakeBool("Controls Commands (On/Off)", "terminalClockCommand", true, "Command to toggle the terminal Clock off/on <Clock>");
            terminalListItems = MakeBool("Extras Commands (On/Off)", "terminalListItems", true, "Command to list all non-scrap & not currently held items on the ship <ItemsList>");
            terminalLootDetail = MakeBool("Extras Commands (On/Off)", "terminalLootDetail", true, "Command to display an extensive list of all scrap on the ship <LootList>");
            terminalMirror = MakeBool("Extras Commands (On/Off)", "terminalMirror", true, "Command to toggle displaying a Mirror Cam in the terminal <Mirror>");
            terminalRefund = MakeBool("Extras Commands (On/Off)", "terminalRefund", true, "Command to cancel an undelivered order and get your credits back <Refund>");
            terminalRestart = MakeBool("Controls Commands (On/Off)", "terminalRestart", true, "Command to restart the lobby (skips firing sequence) <Restart>");
            terminalPrevious = MakeBool("Extras Commands (On/Off)", "terminalPrevious", true, "Command to switch back to previous radar target <Previous>");
            terminalRouteRandom = MakeBool("Fun Commands (On/Off)", "terminalRouteRandom", true, "Command to route to a random planet, configurable <Previous>");
            terminalPurchasePacks = MakeBool("Comfort Commands (On/Off)", "terminalPurchasePacks", true, "Use [purchasePackCommands] to create purchase packs that contain multiple store items in one run of the command");
            
            routeRandomBannedWeather = MakeString("Route Random", "routeRandomBannedWeather", "Eclipsed;Flooded;Foggy", "This semi-colon separated list is all keywords that can be used in terminal to return <alwayson> command");
            routeRandomCost = MakeClampedInt("Route Random", "routeRandomCost", 100, "Flat rate for running the route random command to get a random moon...", 0, 99999);

            //String Configs
            doorOpenString = MakeString("Door", "doorOpenString", "Opening door.", "Message returned on door (open) command.");
            doorCloseString = MakeString("Door", "doorCloseString", "Closing door.", "Message returned on door (close) command.");
            doorSpaceString = MakeString("Door", "doorSpaceString", "Can't open doors in space.", "Message returned on door (inSpace) command.");
            canOpenDoorInSpace = MakeBool("Door", "canOpenDoorInSpace", false, "Set this to true to allow for pressing the button to open the door in space. (does not change whether the door can actually be opened)");
            quitString = MakeString("Quit", "quitString", "goodbye!", "Message returned on quit command.");
            leverString = MakeString("Lever", "leverString", "PULLING THE LEVER!!!", "Message returned on lever pull command.");
            videoStartString = MakeString("Video", "videoStartString", "lol.", "Message displayed when first playing a video.");
            videoStopString = MakeString("Video", "videoStopString", "No more lol.", "Message displayed if you want to end video playback early.");
            tpMessageString = MakeString("Teleporters", "tpMessageString", "Teleport Button pressed.", "Message returned when TP command is run.");
            itpMessageString = MakeString("Teleporters", "itpMessageString", "Inverse Teleport Button pressed.", "Message returned when ITP command is run.");
            vitalsPoorString = MakeString("Vitals", "vitalsPoorString", "You can't afford to run this command.", "Message returned when you don't have enough credits to run the <Vitals> command.");
            vitalsUpgradePoor = MakeString("Vitals", "vitalsUpgradePoor", "You can't afford to upgrade the Vitals Scanner.", "Message returned when you don't have enough credits to unlock the vitals scanner upgrade.");
            healIsFullString = MakeString("Heal", "healIsFullString", "You are full health!", "Message returned when heal command is run and player is already full health.");
            healString = MakeString("Heal", "healString", "The terminal healed you?!?", "Message returned when heal command is run and player is healed.");
            camString = MakeString("Cams", "camString", "(CAMS)", "Message returned when enabling Cams command (cams).");
            camString2 = MakeString("Cams", "camString2", "Cameras disabled.", "Message returned when disabling Cams command (cams).");
            mapString = MakeString("Cams", "mapString", "(MAP)", "Message returned when enabling map command (map).");
            mapString2 = MakeString("Cams", "mapString2", "Map View disabled.", "Message returned when disabling map command (map).");
            ovString = MakeString("Cams", "ovString", "(Overlay)", "Message returned when enabling Overlay command (overlay).");
            ovString2 = MakeString("Cams", "ovString2", "Overlay disabled.", "Message returned when disabling Overlay command (overlay).");
            mmString = MakeString("Cams", "mmString", "(MiniMap)", "Message returned when enabling minimap command (minimap).");
            mmString2 = MakeString("Cams", "mmString2", "MiniMap disabled.", "Message returned when disabling minimap command (minimap).");
            mcString = MakeString("Cams", "mcString", "(MiniCams)", "Message returned when enabling minicams command (minicams).");
            mcString2 = MakeString("Cams", "mcString2", "MiniCams disabled.", "Message returned when disabling minicams command (minicams).");

            customLink = MakeString("Link", "customLink", "https://thunderstore.io/c/lethal-company/p/darmuh/darmuhsTerminalStuff/", "URL to send players to when using the \"link\" command.");
            customLinkHint = MakeString("Link", "customLinkHint", "Go to a specific web page.", "Hint given to players in extras menu for \"link\" command.");
            customLink2 = MakeString("Link", "customLink2", "https://github.com/darmuh/TerminalStuff", "URL to send players to when using the second \"link\" command.");
            customLink2Hint = MakeString("Link", "customLink2Hint", "Go to a specific web page.", "Hint given to players in extras menu for \"link\" command.");

            //Cost configs
            vitalsCost = Plugin.instance.Config.Bind<int>("Vitals", "vitalsCost", 10, "Credits cost to run Vitals Command each time it's run.");
            vitalsUpgradeCost = Plugin.instance.Config.Bind<int>("Vitals", "vitalsUpgradeCost", 200, "Credits cost to upgrade Vitals command to not cost credits anymore.");
            bioScanUpgradeCost = Plugin.instance.Config.Bind<int>("BioScan", "bioScanUpgradeCost", 300, "Credits cost to upgrade Bioscan command to provide detailed information on scanned enemies.");
            enemyScanCost = Plugin.instance.Config.Bind<int>("BioScan", "enemyScanCost", 15, "Credits cost to run Bioscan command each time it's run.");

            //Other configs
            gambleMinimum = Plugin.instance.Config.Bind<int>("Gamble", "gambleMinimum", 0, "Credits needed to start gambling, 0 means you can gamble everything.");
            gamblePityMode = MakeBool("Gamble", "gamblePityMode", false, "Enable Gamble Pity Mode, which gives credits back to those who lose everything.");
            gamblePityCredits = Plugin.instance.Config.Bind<int>("Gamble", "gamblePityCredits", 10, "If Gamble Pity Mode is enabled, specify how much Pity Credits are given to losers. (Max: 60)");
            gamblePoorString = MakeString("Gamble", "gamblePoorString", "You don't meet the minimum credits requirement to gamble.", "Message returned when your credits is less than the gambleMinimum set.");
            videoFolderPath = MakeString("Video", "videoFolderPath", "darmuh-darmuhsTerminalStuff", "Folder name where videos will be pulled from, needs to be in BepInEx/plugins");
            videoSync = MakeBool("Video", "videoSync", true, "When networking is enabled, this setting will sync videos being played on the terminal for all players whose terminal screen is on.");
            leverConfirmOverride = MakeBool("Lever", "leverConfirmOverride", false, "Setting this to true will disable the confirmation check for the <lever> command.");
            restartConfirmOverride = MakeBool("Restart", "restartConfirmOverride", false, "Setting this to true will disable the confirmation check for the <restart> command.");
            obcResolutionMirror = MakeString("Cams", "obcResolutionMirror", "1000; 700", "Set the resolution of the Mirror Camera created with OpenBodyCams for darmuhsTerminalStuff");
            obcResolutionBodyCam = MakeString("Cams", "obcResolutionBodyCam", "1000; 700", "Set the resolution of the Body Camera created with OpenBodyCams for darmuhsTerminalStuff");
            camsNeverHide = MakeBool("Cams", "camsNeverHide", false, "Setting this to true will make it so no command will ever auto-hide any cams command.");
            defaultCamsView = Plugin.instance.Config.Bind("Cams", "defaultCamsView", "cams", new ConfigDescription("Set the default view switch commands will use when nothing is active.", new AcceptableValueList<string>("map", "cams", "minimap", "minicams", "overlay")));
            ovOpacity = Plugin.instance.Config.Bind("Cams", "ovOpacity", 10, new ConfigDescription("Opacity percentage for Overlay View.", new AcceptableValueRange<int>(0, 100)));
            alwaysOnAtStart = MakeBool("AlwaysOn", "alwaysOnAtStart", true, "Setting this to true will set <alwayson> to enabled at launch.");
            alwaysOnDynamic = MakeBool("AlwaysOn", "alwaysOnDynamic", true, "Setting this to true will disable the terminal screen whenever you are not on the ship when alwayson is enabled.");
            alwaysOnWhileDead = MakeBool("AlwaysOn", "alwaysOnWhileDead", false, "Set this to true if you wish to keep the screen on after death.");


            //Keyword configs (multiple per config item)
            alwaysOnKeywords = MakeString("Custom Keywords", "alwaysOnKeywords", "alwayson;always on", "This semi-colon separated list is all keywords that can be used in terminal to return <alwayson> command");
            camsKeywords = MakeString("Custom Keywords", "camsKeywords", "cameras; show cams; cams", "This semi-colon separated list is all keywords that can be used in terminal to return <cams> command");
            mapKeywords = MakeString("Custom Keywords", "mapKeywords", "show map; map; view monitor", "Additional This semi-colon separated list is all keywords that can be used in terminal to return <map> command");
            minimapKeywords = MakeString("Custom Keywords", "minimapKeywords", "minimap; show minimap", "This semi-colon separated list is all keywords that can be used in terminal to return <minimap> command.");
            minicamsKeywords = MakeString("Custom Keywords", "minicamsKeywords", "minicams; show minicams", "This semi-colon separated list is all keywords that can be used in terminal to return <minicams> command");
            overlayKeywords = MakeString("Custom Keywords", "overlayKeywords", "overlay; show overlay", "This semi-colon separated list is all keywords that can be used in terminal to return <overlay> command");
            mirrorKeywords = MakeString("Custom Keywords", "mirrorKeywords", "mirror; reflection; show mirror", "This semi-colon separated list is all keywords that can be used in terminal to return <cams> command");
            doorKeywords = MakeString("Custom Keywords", "doorKeywords", "door; toggle door", "This semi-colon separated list is all keywords that can be used in terminal to return <door> command");
            lightsKeywords = MakeString("Custom Keywords", "lightsKeywords", "lights; toggle lights", "This semi-colon separated list is all keywords that can be used in terminal to return <lights> command");
            modsKeywords = MakeString("Custom Keywords", "modsKeywords", "modlist; mods; show mods", "This semi-colon separated list is all keywords that can be used in terminal to return <mods> command");
            tpKeywords = MakeString("Custom Keywords", "tpKeywords", "tp; use teleporter; teleport", "This semi-colon separated list is all keywords that can be used in terminal to return <tp> command");
            itpKeywords = MakeString("Custom Keywords", "itpKeywords", "itp; use inverse; inverse", "This semi-colon separated list is all keywords that can be used in terminal to return <itp> command");
            quitKeywords = MakeString("Custom Keywords", "quitKeywords", "quit;exit;leave", "This semi-colon separated list is all keywords that can be used in terminal to return <quit> command");
            videoKeywords = MakeString("Custom Keywords", "videoKeywords", "lol; play video; lolxd; hahaha", "This semi-colon separated list is all keywords that can be used in terminal to return <video> command");
            clearKeywords = MakeString("Custom Keywords", "clearKeywords", "clear;wipe;clean", "This semi-colon separated list is all keywords that can be used in terminal to return <clear> command");
            dangerKeywords = MakeString("Custom Keywords", "dangerKeywords", "danger;hazard;show danger; show hazard", "This semi-colon separated list is all keywords that can be used in terminal to return <danger> command");
            healKeywords = MakeString("Custom Keywords", "healKeywords", "healme; heal me; heal; medic", "This semi-colon separated list is all keywords that can be used in terminal to return <heal> command");
            lootKeywords = MakeString("Custom Keywords", "lootKeywords", "loot; shiploot; show loot", "This semi-colon separated list is all keywords that can be used in terminal to return <loot> command");
            randomSuitKeywords = MakeString("Custom Keywords", "randomSuitKeywords", "randomsuit; random suit", "This semi-colon separated list is all keywords that can be used in terminal to return <randomsuit> command");
            clockKeywords = MakeString("Custom Keywords", "clockKeywords", "clock; show clock; time; show time", "This semi-colon separated list is all keywords that can be used in terminal to toggle Terminal Clock display");
            ListItemsKeywords = MakeString("Custom Keywords", "ListItemsKeywords", "show items; get items; listitem", "This semi-colon separated list is all keywords that can be used in terminal to return <itemlist> command");
            ListScrapKeywords = MakeString("Custom Keywords", "ListScrapKeywords", "loot detail; listloost", "This semi-colon separated list is all keywords that can be used in terminal to return <lootlist> command");
            randomRouteKeywords = MakeString("Custom Keywords", "randomRouteKeywords", "route random; random moon", "This semi-colon separated list is all keywords that can be used in terminal to return <lootlist> command");
            lobbyKeywords = MakeString("Custom Keywords", "lobbyKeywords", "lobby; show lobby; lobby name; show lobby name", "This semi-colon separated list is all keywords that can be used in terminal to return <lobby> command");


            //terminal patcher keywords
            fcolorKeyword = MakeString("Custom Keywords", "fcolorKeyword", "fcolor", "Set the keyword that can be used in terminal to return <fcolor> command");
            gambleKeyword = MakeString("Custom Keywords", "gambleKeyword", "gamble", "Set the keyword that that can be used in terminal to return <gamble> command"); ;
            leverKeywords = MakeString("Custom Keywords", "leverKeywords", "lever", "This semi-colon separated list is all keywords that can be used in terminal to return <lever> command"); ;
            scolorKeyword = MakeString("Custom Keywords", "scolorKeyword", "scolor", "Set the keyword that that can be used in terminal to return <scolor> command"); ;
            linkKeyword = MakeString("Custom Keywords", "linkKeyword", "link", "Set the keyword that that can be used in terminal to return <link> command"); ;
            link2Keyword = MakeString("Custom Keywords", "link2Keyword", "link2", "Set the keyword that that can be used in terminal to return <link2> command"); ;


            //homescreen lines
            homeLine1 = MakeString("Home Page", "homeline1", "Welcome to the FORTUNE-9 OS PLUS", "First line of the home command (startup screen)");
            homeLine2 = MakeString("Home Page", "homeline2", "\tUpgraded by Employee: <color=#e6b800>darmuh</color>", "Second line of the home command (startup screen)");
            homeLine3 = MakeString("Home Page", "homeline3", "Have a wonderful [currentDay]!", "Last line of the home command (startup screen)");
            homeHelpLines = MakeString("Home Page", "homeHelpLines", ">>Type \"Help\" for a list of commands.\r\n>>Type <color=#b300b3>\"More\"</color> for a menu of darmuh's commands.\r\n", "these two lines should generally be used to point to menus of other usable commands. Can also be expanded to more than two lines by using \"\\r\\n\" to indicate a new line");
            
            homeTextArt = MakeString("Home Page", "homeTextArt", "[leadingSpacex4][leadingSpace]<color=#e6b800>^^      .-=-=-=-.  ^^\r\n ^^        (`-=-=-=-=-`)         ^^\r\n         (`-=-=-=-=-=-=-`)  ^^         ^^\r\n   ^^   (`-=-=-=-=-=-=-=-`)   ^^          \r\n       ( `-=-=-=-(@)-=-=-` )      ^^\r\n       (`-=-=-=-=-=-=-=-=-`)  ^^          \r\n       (`-=-=-=-=-=-=-=-=-`)  ^^\r\n        (`-=-=-=-=-=-=-=-`)          ^^\r\n         (`-=-=-=-=-=-=-`)  ^^            \r\n           (`-=-=-=-=-`)\r\n            `-=-=-=-=-`</color>", "ASCII Art goes here");

            RemoveOrphanedEntries();
            NetworkingCheck();
        }

        private static ConfigEntry<bool> MakeBool(string section, string configItemName, bool defaultValue, string configDescription)
        {
            return Plugin.instance.Config.Bind<bool>(section, configItemName, defaultValue, configDescription);
        }

        private static ConfigEntry<int> MakeInt(string section, string configItemName, int defaultValue, string configDescription)
        {
            return Plugin.instance.Config.Bind<int>(section, configItemName, defaultValue, configDescription);
        }

        private static ConfigEntry<string> MakeClampedString(string section, string configItemName, string defaultValue, string configDescription, AcceptableValueList<string> acceptedValues)
        {
            return Plugin.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, acceptedValues));
        }

        private static ConfigEntry<int> MakeClampedInt(string section, string configItemName, int defaultValue, string configDescription, int minValue, int maxValue)
        {
            return Plugin.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<int>(minValue, maxValue)));
        }

        private static ConfigEntry<float> MakeClampedFloat(string section, string configItemName, float defaultValue, string configDescription, float minValue, float maxValue)
        {
            return Plugin.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<float>(minValue, maxValue)));
        }

        private static ConfigEntry<string> MakeString(string section, string configItemName, string defaultValue, string configDescription)
        {

            return Plugin.instance.Config.Bind(section, configItemName, defaultValue, configDescription);
        }

        private static void NetworkingCheck()
        {
            Plugin.Log.LogInfo("Checking if networking is disabled...");

            if (!ModNetworking.Value)
            {
                List<ConfigEntry<bool>> networkingRequiredConfigOptions = [terminalBioScan, terminalVitals, terminalBioScanPatch, terminalVitalsUpgrade, terminalRefund, terminalScolor, terminalFcolor, terminalGamble, networkedNodes, terminalRouteRandom, videoSync];

                foreach (ConfigEntry<bool> configItem in networkingRequiredConfigOptions)
                {
                    if (configItem.Value)
                    {
                        configItem.Value = false;
                        Plugin.Log.LogWarning($"Setting {configItem.Definition.Key} to false. Networking is disabled and this setting requires networking!");
                    }
                }
            }

            Plugin.Log.LogInfo("Networking check complete.");
        }

        private static void RemoveOrphanedEntries()
        {
            Plugin.MoreLogs("removing orphaned entries (credits to Kittenji)");
            PropertyInfo orphanedEntriesProp = Plugin.instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(Plugin.instance.Config, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            Plugin.instance.Config.Save(); // Save the config file
        }
    }
}
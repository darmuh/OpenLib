# OpenLib Documentation (WIP)
The below documentation has been created to assist other mod developers who may consider using OpenLib's features within their projects.
***
## CoreMethods.CommandRegistry

### InitListing
 - `InitListing(ref MainListing listingName)`
	- Use this to initialize your own MainListing, if you are not going to use the defaultListing.
	- example usage from darmuhsTerminalStuff:
	```		ConfigSettings.TerminalStuffBools = new();
            ConfigSettings.TerminalStuffMain = new();

            InitListing(ref ConfigSettings.TerminalStuffMain);
	 ```
	- Best practice is to call this at Plugin Awake

### GetCommandsToAdd
 - `GetCommandsToAdd(List<ManagedConfig> managedBools, MainListing listingName)`
	- If not using the defaultListing, you'll want to call this at Terminal Awake
	- will iterate through your ManagedConfig list looking for commands to add.
	- the ManagedConfig items with enabled values will create the command and add it to the provided MainListing parameter.
	- NOTE: you do not need to use this method if you register your commands to the defaultListing and defaultManaged

### AddCommandKeyword
 - `AddCommandKeyword(ManagedConfig managedBool, MainListing listingName)`
	- This method is called from GetCommandsToAdd.
	- Not sure if anyone will find use with this in their code, but it's accessible anyway.

### Note on defaultListing/defaultManaged
 - Unless you plan on catching your added commands and adding specific logic, you'll almost always want to use defaultListing/defaultManaged
 - Below are some examples from darmuhsTerminalStuff of commands that use the default vs terminalStuff using it's own.
	- defaultListing/defaultManaged example:
	```
			terminalAlwaysOn = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalAlwaysOn", true, $"Command to toggle Always-On Display");
            AddManagedBool(terminalAlwaysOn, defaultManaged, false, "COMFORT", alwaysOnKeywords, MoreCommands.AlwaysOnDisplay);	
	```
	- custom listing/managed:
	```
			terminalCams = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalCams", true, "Command to toggle displaying cameras in terminal");
            AddManagedBool(terminalCams, TerminalStuffBools, false, "EXTRAS", camsKeywords, ViewCommands.TermCamsEvent, 0, true, null, null, "", "", "cams", 1, "ViewInsideShipCam 1");
	```
	- TerminalStuff uses a custom listing/managed for cams views to allow for additional logic that handles syncing view nodes between players.
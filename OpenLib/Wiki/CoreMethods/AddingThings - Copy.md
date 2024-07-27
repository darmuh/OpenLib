# OpenLib Documentation (WIP)
The below documentation has been created to assist other mod developers who may consider using OpenLib's features within their projects.
***
## CoreMethods.AddingThings

### AddBasicCommand
- `AddBasicCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, string category = "", string keywordDescription = "")`
- This is probably the simplest way to add a terminal command without any additional logic
- The return type for this method is **void**
- string nodeName = The name assigned to your TerminalNode element
- string keyWord = The keyword used in the terminal to display your command.
- bool isVerb = Determine whether your word will be considered a noun or a verb in the terminal (*most often you will set this to false*)
- bool clearText = Whether or not your command should clear the screen of any text.
- (OPTIONAL) string category = You can set this to other to display it in the Other Commands output. *(currently there is not support for this method to add commands to a custom terminal menu)*
- (OPTIONAL) string keywordDescription = This is what will display in your menu listing or on the other page depending on what is set for **category**
- Example 1: 
	- `AddBasicCommand("myNodeName", "test", "This is a test command\r\n", false, true, "other", "\ntest \nThis is my test command created with OpenLib");`
	- This will add a command to the terminal with the keyword "test". 
	- When someone types the keyword into the terminal they will see "This is a test command"
	- category is set to "other" and will add the following description to the other command output "\ntest \nThis is my test command created with OpenLib"
- Example 2:
	- `AddBasicCommand("myNodeName", "test", "This is a test command\r\n", false, true);`
	- This will add the same command as above but will not add the command description to the "other" command.
	- As you can see, you do not need to define category or keywordDescription as these are optional parameters.

### AddNodeManual
 - This is probably the simplest way to add a terminal command WITH additional logic
 - `AddNodeManual(string nodeName, string stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, int price = 0, Func<string> ConfirmAction = null, Func<string> DenyAction = null, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "")`
	 - The return type for this method is **TerminalNode**
		- when using this method you can return a terminalnode for use in your code & further modification
	 - string nodeName = The name assigned to your TerminalNode element
	 - string stringValue = The keyword used in the terminal to display your command.
	 - Func<string> commandAction = this will refer to a method with the return type string. This method is what adds logic to your command.
	 - bool clearText = Whether or not your command should clear the screen of any text.
	 - int CommandType = This library supports 3 different types of commands to be created
		- 0 = a standard command, this will be your most common command type
		- 1 = a standard command that requires confirmation. This is for commands that ask the user to input confirm or deny.
		- 2 = a store command. This will create a command that will add a faux item to the store listing.
	 - MainListing yourModListing = See the MainListing class info for more details, this will add your command to the listing specified.
		- If you do not have a method to handle your own listing commands it is recommended to use the default listing: ConfigSetup.defaultListing
	 - (OPTIONAL) int price = This int value is used only in commandtype 2 to determine the cost of your store node.
	 - (OPTIONAL) Func<string> ConfirmAction = this will refer to a method with the return type string. 
		- This method is what will run for the confirm option in your commandType 1 or 2.
		- Can be set to null if you choose to use the confirmText string property and do not wish to add any further logic.
	 - (OPTIONAL) Func<string> DenyAction = this will refer to a method with the return type string.
		- This method is what will run for the deny option in your commandType 1 or 2.
		- Can be set to null if you choose to use the denyText string property and do not wish to add any further logic.
	 - (OPTIONAL) string confirmText = string property which will be used to display in the confirm result of your command if ConfirmAction is null
	 - (OPTIONAL) string denyText = string property which will be used to display in the deny result of your command if DenyAction is null
	 - (OPTIONAL) bool alwaysInStock = bool property that determines if your store item should always be in stock (only relevant for commandType 2)
	 - (OPTIONAL) int maxStock = maximum amount of your store item that you will allow for buying (only relevant for commandType 2)
		- can be set to 0 for no maximum
	 - (OPTIONAL) string storeName = the name of the item you are adding to the store that will display in the store listing
		- recommended to keep similar to your keyword, this will retain punctuation
	 - (OPTIONAL) bool reuseFunc = this determines whether you are allowing for using a commandAction that already exists in the listing.
	 - (OPTIONAL) string itemList = this will likely never be used by any other mod. This is used by darmuhsTerminalStuff to generate storepacks.
		- the string is paired with the terminal node in a dictionary that is accessed by darmuhsTerminalStuff 
 - A basic example using this method would be `AddNodeManual("random suit command", "randomsuit", RandomSuit, true, 0, ConfigSetup.defaultListing)`
	- This uses the "randomsuit" keyword and calls the node "random suit command"
	- CommandType is set to 0 so all the optional parameters are unneeded
	- clearText is set to true so existing text will be removed from the terminal
	- There will need to be a corresponding `static string RandomSuit() method` to return the displayText from the command and any additional logic
 - For more complicated examples, I suggest looking through the darmuhsTerminalStuff github.

### AddKeywordToExistingNode
 - Easiest way to add an additional keyword to an already existing TerminalNode
 - `AddKeywordToExistingNode(string keyWord, TerminalNode existingNode, bool addToList = false)`
	- Requires a TerminalNode given as the parameter. 
	- bool addToList determines whether or not the new keyword is registered to the library for deletion at lobby close. I recommend setting this to true.
	- Return type is Void

### AddToExistingNodeText
 - Easy way to add text to an already existing TerminalNode
 - `AddToExistingNodeText(string textToAdd, ref TerminalNode existingNode)`
	- Requires a TerminalNode given as a parameter, will modify this node to add your new text to the end of it.
	- This method is used for adding commands to the OtherCommands TerminalNode, for example.
	- Return type is Void

### CreateDummyNode
 - Useful when you need to reference a blank TerminalNode
 - `CreateDummyNode(string nodeName, bool clearPrevious, string displayText)`
 - Return type is TerminalNode

### Other accessible methods that you may end up using as standalones
 - `AddStoreCommand(string nodeName, string storeName, ref TerminalKeyword keyword, ref TerminalNode node, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, MainListing mainListing, bool alwaysInStock, int maxStock, out CompatibleNoun confirm, out CompatibleNoun deny)`
	- This requires a TerminalKeyword and TerminalNode to reference. Used by the other command adding methods when commandType is 2
 - `AddConfirm(string nodeName, int price, Func<string> ConfirmAction, Func<string> DenyAction, string confirmText, string denyText, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)`
	- Return Type is void, with two out parameters for the confirm and deny CompatibleNouns.
	- This method is usually only used in conjunction with the other methods that add commands to the terminal and require confirmation
 - `AddToBuyWord(ref TerminalKeyword buyKeyword, ref TerminalKeyword terminalKeyword, UnlockableItem item)`
	- Usually used by the AddStoreCommand method.
	- You will need to provide both the "buy" TerminalKeyword and your item's TerminalKeyword in parameters
 - `AddUnlockable(string storeName, TerminalNode node, bool alwaysInStock, int maxStock)`
	- This will add a faux UnlockableItem for use with the Terminal Store (not for real unlockables)
	- Make sure you are not using an already existing unlockableitem's name as it will be replaced by yours otherwise

### Overloads and other methods that are part of larger systems in this mod
 - `CreateNode(ManagedConfig managedBool, string keyWord, MainListing yourModListing)`
	- This is called at Terminal Awake in the CommandRegistry.AddCommandKeyword method.
	- You will almost never need to call this method directly if you are using ManagedConfig to create your terminal commands.
 - `AddConfirm(string nodeName, ManagedConfig managedBool, out CompatibleNoun confirm, out CompatibleNoun deny)`
	- You will likely never need to call this method directly if you are using ManagedConfig to create your terminal commands.
 - `AddStoreCommand(string nodeName, ref TerminalKeyword keyword, ManagedConfig managedBool, MainListing mainListing, out CompatibleNoun confirm, out CompatibleNoun deny)`
	- You will likely never need to call this method directly if you are using ManagedConfig to create your terminal commands.
 - `AddNodeManual(string nodeName, ConfigEntry<string> stringValue, Func<string> commandAction, bool clearText, int CommandType, MainListing yourModListing, List<ManagedConfig> managedBools, string category = "", string description = "", int price = 0, Func<string> ConfirmAction = null, Func<string> DenyAction = null, string confirmText = "", string denyText = "", bool alwaysInStock = false, int maxStock = 1, string storeName = "", bool reuseFunc = false, string itemList = "")`
	- This is useful for keywords you dont want to define at Terminal Awake but do have tied to config options.
	- The stringValue config option can have multiple keywords in it separated by semi-colons ";" to assign multiple keywords to the same TerminalNode.
	- Example from darmuhsTerminalStuff:
		- `AddNodeManual("Use Teleporter", ConfigSettings.tpKeywords, ShipControls.RegularTeleporterCommand, true, 0, defaultListing, defaultManaged, "CONTROLS", "Activate the Teleporter. Type a crewmate's name after the command to target them");`


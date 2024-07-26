# OpenLib Documentation (WIP)

The below documentation has been created to assist other mod developers who may consider using OpenLib's features within their projects.

## Create a simple Terminal Command - AddBasicCommand method (without additional logic)
 Below is the simplest way to create a new terminal command:
- `AddBasicCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, string category = "", string keywordDescription = "")`
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

## Create a Terminal Command with additional logic - AddNodeManual method
 Below is the simplest way to create a new terminal command with additional logic:
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
 
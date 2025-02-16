# Change Log

All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [0.3.2]
 - Added EnterAtPage method to BetterMenu for loading a different menu item at load-in
	- for suitsTerminal config item in 1.6.2 of suitsTerminal

## [0.3.1] hotfixes
 - BetterMenu UpdateMainActions method added/publicized for updated main keys of a menu
	- should fix configurable keys not working in latest suitsTerminal
	- does need to be called when changing any main action key to update the dictionary of actions related to the key
	- Thanks IAmSympathy for the report
 - Updated AnyMenuActive in InteractiveMenus with an obsolete tag and having it redirect the results from MenusContainer.AnyMenuActive
	- Also updated MenusContainer.AnyMenuActive to check for InteractiveMenus to keep things all working together
	- should fix darmuhsTerminalStuff shortcuts not respecting the new BetterMenus system
	- Thanks Lunxara for the report

## [0.3.0] (The refactor begins)
 - As mentioned in last version, there is some pretty old/ugly core code in this library now. Starting now I'll be working to migrate to some new classes from legacy code such as:
	- ManagedConfig/ManagedBool/ManagedString -> ConfigWatch
		- Hoping to get away from methods with 7+ params each with all different niche use-cases. Starting with my managedconfig items
		- ConfigWatch uses a generic to work with any config item and will ONLY be used to watch the config item. Nothing else.
		- Currently only really need this class to watch for networking required booleans, but who knows maybe the other config types will be needed in the future.
	- AddingThings/MainListing -> CommandManager & the various Node Classes
		- Part of detangling everything starts here
		- CommandManager can/will be created/initialized in awake, no need to define a listing per mod either
		- OpenLib listing can be opted out of ofc
		- The various "node" classes such as NodeConfirmation, NodeInfo, NodeSpecial allows for further complex command creation without adding a bunch of parameters/properties in one class.
		- Default CommandManager listing will create terminal commands conditionally based on setup of each CommandManager item at terminal awake
		- Optional constructors for commands that are defined during runtime or do not have config items for their keywords etc. are available
		- should allow same or better flexibility in the future without being a tangled mess
	- InteractiveMenu -> BetterMenu/MenusContainer/MenuItem
		- As with the theme, untangled one class into a few different ones and added a more standardized menu base for all OpenLib mods to potentially use.
		- Provides a bunch of different events based on keypress, menu page load, menu item load, etc.
		- BetterMenu uses a generic type so that each mod can define their own type of BetterMenu to inherit from.
		- suitsTerminal is the first test example which is using a BetterMenu type of SuitMenuItem (which inherits from MenuItem, this will probably be necessary for all uses of this menu system)
		- this implementation should hopefully reduce the redundant code between mods while still allowing a great deal of flexibility with each different menu type
 - While I plan to move all existing mods to these new classes, it will take me a while to migrate everything.
	- In order to not break shit between updates I am leaving the older classes in tact with just an obsolete tag noting they have been replaced.
	- Eventually all methods will be replaced, but as I've been testing with suitsTerminal there is the possibility I will need to expand on the new classes for other mods
 - Added new method to Events.cs class ``RemoveAllListeners()``
	- This method is used by the new BetterMenus class for situations where an event may want to completely replace any existing listeners for a new one.
 - Added new method to Misc.cs class ``CycleIndex``
	- This method is used by the new BetterMenus feature to cycle the index rather than clamp it
	- when cycled, this will take a value and return it to the opposite end of the index
	- in BetterMenus, that means when you try to go past the last page or last item, it will take you to the first page or first item rather than keeping you at your current spot in the index.
 - Added new methods to CommonTerminal.cs
	- LoadNewNode - will load the provided terminal node and, if terminalstuff is present, run the compatibility method to then sync that node
	- TryLoadKeyword - will try to load the terminal node matching a provided keyword
	- TryGetCommand - will try to find a matching CommandManager item in this library's listing (used to load the command manager command)
 - Also started caching some common keywords in CommonTerminal with a getter
 - Updated TerminalStuff compatibility methods
	- TryLoadHomePage now actually loads the home page rather than trying to load the start page
	- Added TryLoadStartPage to load the start page (if it's not null)
 - Added GetNewDisplayText2 to LogicHandling.cs for updating CommandManager commands' displaytext at terminal parse
 - Some other various changes as part of the beginning of the refactoring process

### If you experience issues with OpenLib version 0.3.0, please report the issues with logs and revert to version 0.2.14.	

## [0.2.14]
 - Updated for darmuhsTerminalStuff v3.8.0
 - Updated TryGetNodeFromList from foreach loop to use two separate linqs.
	- This should resolve issue noted in terminalstuff where, if two specialstrings were detected it would return the first in the list.
	- Will now detect if the keywords provided starts with the special string before then going and finding the first matching keyword in the provided string.
 - Optimized a few other methods to use linq in place of foreach loops
 - Added InteractiveTerminalAPI compatibility method for checking if a menu is open (used by terminalstuff to avoid using shortcuts while a menu is open)
 - Adjusted some things in ManagedConfig, to be honest I want to completely rewrite this code it's soooooo ugly (unfortunately it's a core function of this library)

## [0.2.13]
 - This update is being pushed for version 0.3.0 of ShipColors. There may be further updates to some of the experimental, unused features before use in other mods.
 - Added TerminalAccessibleObject support with dynamic transforms that will follow a target.
	- Still experimental, not in use in any public mod as of yet.
 - Added some new events to subscribe to:
	- AutoParentEvent (Requires GameObject parameter): This method is called whenever an object is spawned in the ship (furniture/upgrades)
	- SetBigDoorCodes - called when big doors, turrets, mines etc. are assigned codes
	- SpawnMapObjects - called when creating the other objects on the map
	- OnShipLandedMiscPatch - called a little after the ship lands
 - Added TryLoadHomePage compatibility method for when darmuhsTerminalStuff is present.
	- Still experimental, not in use in any public mod as of yet.
 - Created example class for code examples
	- This code is not used actively in the mod

## [0.2.12]
 - Added support for easily creating InteractiveMenus like the suits menu from suitsTerminal or the Moons Plus page from darmuhsTerminalStuff.
 - Added to CommonStringStuff:
	- BepinFriendlyString - for getting a string that wont throw errors in a bepinex config.
	- TryGetKey - for getting a Unity InputSystem key from a valid string
 - Added to CommonTerminal:
	- Added public color caretOriginal for other mods to set/read from when updating the terminal caret color
	- Added ChangeCaretColor for modifying the terminal caret color (and the original via bool parameter)
 - Added darmuhsTerminalStuff compatibility functions:
	- NetSync, for syncing a node to other users when the node has already been loaded
	- LoadAndSync, for loading a node and then attempting to sync the node if terminalstuff is present
 - Added check for invoking customevents to see if the event has listeners, will early return if no listeners

## [0.2.11]
 - Small change for OpenBodyCams compatibility
	- will no longer delete cameras on camblanked event

## [0.2.10]
 - Small update to OpenBodyCams compatibility
 - Added 2 new properties for FauxKeywords
	- ``AllowOtherFauxWords`` will allow other FauxKeywords to be used from the result page of the current FauxKeyword
	- ``requireExact`` will determine whether the FauxKeyword accepts partial matches or requires an exact match
 - Added new event for detecting when a key has been pressed while using the terminal ``TerminalKeyPressed``
 - Updated MenuBuild handling to allow for FauxKeyword menu categories & items.
 - Updated ``ParsePlayerSentence`` Patch to not invoke the ``TerminalParseSent`` event when the node is null.
	- This fixes an issue that was reported in darmuhsTerminalStuff with the vanilla view monitor relating to door codes.

## [0.2.9]
 - Added new events ShipReset, NextDayEvent, ShipLeft, and NewQuota
 - Added public accessible System.Random at Common.Misc.Random
 - Created Common.Misc.GetPluginVersion from method that was originally in LethalConfigCompat so it can be used with other plugins.
 - Added CheckForConfigName bool function in ConfigSetup
 - Check out the new web config management page - https://darmuh.github.io/OpenLib/OpenLib/Website/Examples/ConfigManagement.html
	- Updated to 0.1.2, now looks for a valid hexcode default value before creating color picker config input.
	- Also did some styling cleanup, though I could definitely use help to make it cleaner :)

## [0.2.8]
 - Hotfix for OpenBodyCams API update

## [0.2.7]
 - Fixed GetNewDisplayText event causing null terminal node errors also bricking the terminal if there were no listeners for the event (returning a null node).
	- Will now only affect the node if the event has a listener.
	- Thanks CoolLKKPS from github and shadow_glassen on discord for the reports!

## [0.2.6]
 - Changed GetNewDisplayText to be more useful and reference the terminal node so that you can manipulate it during the event.
	- Was needed for filtering keywords to not run when using the CruiserTerminal in darmuhsTerminalStuff
	- WARNING: This will break any mod that used the older version of this event! (darmuhsTerminalStuff versions 3.6.7 and earlier)
 - Added SpecateNextPlayer event, fired when local player switches spectated player
 - Added SpecatingPlayerIsInShip event in PlayerControllerB Update patch, fired when the currently specated player's inShip status changes

## [0.2.5]
 - Fixed confirmation keyword issue introudced in 0.2.1(?) which would result in typing confirm by itself loading a random confirmation result.
	- Thanks _stormyy for the report via darmuhsTerminalStuff!
 - Removed TooManyEmotes layer override patch, was causing issues with other camera mods when my homebrew cams were not in use.
	- Homebrew cams will now use layer23 to show the player's body
 - Fixed issue noted with FauxKeywords throwing errors when they are too short (noted in LethalConstellations thread)
 - Added new Update patches for Terminal/Player, currently only utilized by darmuhsTerminalStuff
	- These patches have a boolean that disables invoking the events by default and must be enabled by the mod expecting to use them.

## [0.2.4]
 - Added more null handling in MenuBuild.TerminalMenuItems

## [0.2.3]
 - Added GetCam function for OpenBodyCams Compat with suitsTerminal upcoming update
 - Added Homebrew camera methods (from suitsTerminal/terminalstuff)
	- Updated homebrew cam compatibility with: TooManyEmotes, MirrorDecor, and ModelReplacementAPI
 - Updated some OpenBodyCams stuff
 - Added patch for detecting emotes (works with TME)
 - Some more code cleanup

## [0.2.2]
 - General Code Cleanup
 - Added new method AddSpecialListString to identify manually commands that accept any input
	- Also modified this behavior to be a bit more forgiving with where to expect the string
 - Added new overload for SetBMXCompat which does not need Version as a param

## [0.2.1]
 - Fixed issue where new compatible nouns were not deleted on exit to main menu
 - Fixed issue with blank terminal input throwing errors
 - Updated terminalkeyword deletion to not try every single word against "buy" as a compatible noun

## [0.2.0]
 - Added nullable to project and updated some common use methods to prefer a TryGet bool in order to avoid NREs.
 - Added OpenBodyCams common methods for creating a terminal bodycam or terminal mirror for use in suitsTerminal/darmuhsTerminalStuff
 - Refactored terminal keyword/node/noun creation
 - Added FauxKeywords for creating terminal keywords that should only work in the context of a certain terminal node. (for LethalConstellations)
 - Added Config file to HTML page generation methods for config editing within an HTML page. 
	- These pages will produce a code that can be read by a single config item to update the entire config.
 - Added LLL Hotfix for route locked terminal node soft-locking the terminal and throwing errors on any input.
 - General code cleanup, too many changes to individually note.
	
## [0.1.8]
 - Added AddToHelpCommand method.
 - Adjusted some other small things

## [0.1.7]
 - Added GetKeywordsPerConfigItem overload that allows for specifying separator char.
 - Added some WIP methods in AddingThings, use at your own-risk.
 - adjusted keyword check to be case in-sensitive
 - made it so "specialString" in managedconfig items does not have to match the keyword
 - Added RemoveCompatibleNoun method in RemoveThings.cs
 - Added new patch/event StartOfRoundChangeLevel
 - Added AssembleMainMenuText overload for when you're not using ManagedConfig stuff
 - Added Plugin.WARNING shortcut for LogWarning messages

## [0.1.6]
 - Added soft compatibility method in Common.StartGame
	- Call this method with the GameNetworkManagerStart Event to set your soft compatibility bools.
 - Adjusted some methods that dont have any public use to be internal
 - Removed duplicate method in CoreMethods.CommonThings
 - Added some more public methods to Common.Misc for getting/setting color via hexcode config
 - Updated terminal menus to display category names in upper case to be more in-line with vanilla terminal menus.

## [0.1.5]
 - Adjusted some logging messages.
 - Fixed error being thrown by invalid input during TerminalParseSent event.

## [0.1.4]
 - Updated ManagedConfig items to indicate what type they are (bool or string)
	- can expand to floats/ints/etc. later if needed
 - Updated Managedbool and ManagedString methods to include the configtype at creation/modification
 - Updated TryGetItemByName to indicate ConfigType as a parameter
	- this is to ensure you are getting the config item you are looking for specifically
 - Updated NetworkingCheck method to properly iterate through a config file

## [0.1.3]
 - Removed property "count" from MainListing class.
	- This was causing an odd interaction where menus would not update their displaytext properly due to this property being equal to 0.
	- updated any usage of the count property to the Listing property's "Count"

## [0.1.2]
 - Fixed manifest link to the correct github page

## [0.1.1]
 - Added bool check ShouldAddCategoryNameToMainMenu for MenuBuild in darmuhsTerminalStuff

## [0.1.0]
 - Initial release with usage by darmuhsTerminalStuff
 - Replaced ManagedBool class with ManagedConfig, as I am also managing string config items.
 - Added some more overloads for use in terminalstuff. ReadConfigAndAssignValues should only need to be called on full config reload.
	- Singular config item change event can be subscribed to in bepinex.
	- see CheckChangedConfigSetting in ConfigMisc
		- This will update any corresponding ManagedConfig item whether it's a bool or a string.
	- ShouldReloadConfigNow is unused at this point, since the ManagedConfig class could be modified directly in the config item change event bepinex provides
 - Ported menu handling system from terminalstuff to this library. See terminalstuff for an example on usage.
 - Decided against defining EVERYTHING when creating the config option. 
	- Instead you can bind the config option, then directly after it you can define your managedconfig item with either managedbool or managedstring.
 - Cleaning as much as possible back to a blank state at TerminalDisable event (with exception to managedconfig items)
 - Added overload for getting new displaytext from multiple listings.
	- Only use-case I see for it right now is terminalstuff's node syncing feature but who knows it may be useful in the future.
	- There is an event you can subscribe to when getnewdisplaytext is called, however, I'd recommend against it as it can be called multiple times.
 - Added support for adding your terminal command to the Other command listing when category is set to "Other"
	- With this support there is also accessible methods for adding text to an existing node
	- Also added support for adding your terminal menu command to the other listing, an example of this is in terminalStuff with the "more" command
 - If for whatever reason you need a config to reload on terminal disable, you can add it to the list configsToReload in EventUsage.cs
	- Not using this at all at the moment.
  
## [0.0.1]
 - Began port of commonly used functions between my (darmuh) own personal mods. This mod was created by creating a clone of darmuhsTerminalStuff.

  </details>
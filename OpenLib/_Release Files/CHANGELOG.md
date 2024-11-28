# Change Log

All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/).

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
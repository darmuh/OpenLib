# Change Log

All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/).

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
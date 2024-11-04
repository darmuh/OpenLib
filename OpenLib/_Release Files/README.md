# OpenLib

### Open-source development library for LethalCompany

- This is a library of commonly used methods from darmuh's mods. Allowing for flexible additions to the terminal and unified game patching.
- This will remain open to the public for collaborative efforts. All I ask is that you maintain compatibility with any mod that uses this library.


### Current Features:
- ManagedConfig class will let you watch and assign values to certain config items.
	- Currently supporting ManagedBools and ManagedStrings
	- when a change is detected to the assigned value, your managedconfig item will also update.
	- use this to manage your keyword creation/removal between lobby loads.
	- also provides support for a networking configuration option via ConfigSetup.NetworkingCheck
- Terminal Keyword/Node creation with flexibility in mind.
	- Create simple commands that can use multiple keywords.
	- Create shop nodes that will be added to the furniture rotation list
	- Create complex commands that require confirmation
	- All added keywords are tracked by this library and will be deleted upon Terminal Disable (closing the lobby)
- Terminal Menu System
	- Create your own command menu listing with categories and a list of commands under each category.
	- Ported from darmuhsTerminalStuff for general use
	- Optionally add a reference to this menu in the other commands listing
- Modify already existing terminal node displayText
	- Will gracefully remove new line spaces to add your content and then add newlines below your content.
- Assign new keywords to already existing nodes.
- Event system lets you subscribe to all manners of Terminal patches. See EventUsage for examples on how to subscribe to these events.
- Patches into the Awake method for the Teleporter class and provides referenceable Teleporter instances for both the normal teleporter and inverse teleporter
- Config to HTML file generation. Use this to create an html page that can be used to read/modify config files.
	- Will generate a code at the bottom of the page that can be used to update all config items at once via a decompress method.
	- For an example use-case, see ghostCodes v2.5.0, example [here](https://darmuh.github.io/OpenLib/OpenLib/Website/Examples/darmuh.ghostCodes.Setup.cfg_generator.htm)
- Faux Keywords are keywords that will only work in the terminal from within the TerminalNode they are assigned.
	- These will not overwrite existing keywords.
	- Feature is still WIP and is currently used in LethalConstellations
- Small hotfix for when terminalnodes have the acceptAnything property set to true without any valid compatible nouns.
	- will prevent soft locks from these particular nodes resulting in the terminal not accepting any further input.
- Automatic info compatible nouns created for every new keyword added through this library.
- New Player Update events for when a player is dead, in or out of the ship, etc. Must be enabled by the mod using
- Add compatible nouns manually.
- Support methods for LethalConfig and OpenBodyCams.

*The plan is to have this library support all of my mods (darmuhsTerminalStuff, ghostCodes, and suitsTerminal). So this library will likely have more features in the future*

Currently Supports:
 - darmuhsTerminalStuff 3.3.0+
 - suitsTerminal 1.4.0+
 - ShipColors 0.1.0+
 - LethalConstellations 0.1.0+
 - ghostCodes 2.5.0+

Icon Attribution:
[Online library icons created by Ajmal Naha - Flaticon](https://www.flaticon.com/free-icons/online-library)
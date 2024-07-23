# OpenLib

### Open-source development library for LethalCompany

- Initially ported from darmuhsTerminalStuff, this is a library of commonly used methods from darmuh's mods. Allowing for flexible additions to the terminal.
- This will remain open to the public for collaborative efforts. All I (darmuh) ask is that you maintain compatibility with any mod that uses this library.


### Current Features:
- ManagedConfig class will let you watch and assign values to certain config items.
	- Currently supporting ManagedBools and ManagedStrings
	- when a change is detected to the assigned value, your managedconfig item will also update.
	- use this to manage your keyword creation/removal between lobby loads.
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


*The plan is to have this library support all of my mods (darmuhsTerminalStuff, ghostCodes, and suitsTerminal). So this library will likely have more features in the future*
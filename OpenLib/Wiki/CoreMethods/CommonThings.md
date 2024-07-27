# OpenLib Documentation (WIP)
The below documentation has been created to assist other mod developers who may consider using OpenLib's features within their projects.
***
## CoreMethods.CommonThings

### CheckForAndDeleteKeyWord
 - `public static void CheckForAndDeleteKeyWord(string keyWord)`
	- used to delete a keyword matching the provided string keyWord
	- methods in CoreMethods.AddingThings use this
	- will only remove the keyword from the Terminal's keyword list. Will not delete the TerminalKeyword gameobject.
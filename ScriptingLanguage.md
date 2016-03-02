# Definition #
ScriptingLanguage is the constructor that contains, executes, and creates the scripting language, and defines certain customizable aspects.


# Details #
ScriptingLanguage contains:
  * The subclass ScriptParser, which translates the script into code
    * The script can be called line by line, and is self-debgging
  * A FunctionLibrary, which contains & references the callable methods of your script
  * A ParameterLibrary, which contains & references the supported variable types of your script
  * The ability to add to the FunctionLibrary & the ParameterLibrary in realtime
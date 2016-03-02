# Introduction #

This is the planned development of gCode SL. Development will proceed in linear order, straight down the list. After release, gCode will cease feature development and enter its tuning phase to make it faster.


# Details #

**_Alpha_** -> Developing basic functionality of scripting language _v.0.0.0_
  * Declare line preceder: **supported** _v.0.8.3_
> > - This means that variables can currently be declared
  * Execute line preceder: **supported** _v.1.6.7_
> > - This means that functions can be called
  * Real-time interactive code-to-script variables: **supported** _v.2.5.0_
> > - This means that variables from your code can be accessed and changed, in real time, via script.
  * User of DLL can create classes that inherit from ScriptingLanguage, Method, and IParameter: **supported** _v.3.3.3_
> > - This means that the programmer can create their own methods, variable types, and scripting languages that extend the basic functionality of gCode SL
  * Real-time error checking: **supported** _v.4.1.7_
> > - This means that the SL can tell you when an error occurs, what is the problem, and which line(s) it's on

**_Beta_** -> If-statements, loops, operators _v.5.0.0_:
  * Basic operators for strings, ints, and doubles: **supported** _v.5.8.3_
> > - This means that the declare line preceder allows varaibles to be operated upon (+-/x)
  * Parenthetical operators for arranging order of operations **supported**
> > - This means that you can force order of operations.
  * While loops: **supported** _v.6.6.7_
> > - Repeat the loop until the boolean condition evaluates to false
  * If-then statements: not supported _v.7.5.0_
> > - Boolean if-then statements
  * Elseif-else statements: not supported  _v.8.3.3_
> > - Extending if-then functionality
  * For-loops: not supported _v.9.1.7_
> > - A special kind of while loop that allows you to iterate a variable until it fails to satisfy some boolean condition
  * Nested loops & statements: not supported _v.1.0.0_
> > - Allowing for all loops and statements to be infinitely nested inside one another and themselves.
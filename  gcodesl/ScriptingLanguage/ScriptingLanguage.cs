using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using gInclude;

namespace ScriptingLanguage
{
    /// <summary>
    /// Creates a new scripting language using
    /// gCode parsing and methodology
    /// </summary>
    public class ScriptingLanguage
    {
        protected IParameterLibrary m_pLib;
        protected FunctionLibrary m_fLib;
        protected string m_name = "Scripting Language";

        /// <summary>
        /// Creates a new scripting language
        /// </summary>
        public ScriptingLanguage()
        {
            m_pLib = new IParameterLibrary(m_name + " Parameter Library");
            m_fLib = new FunctionLibrary(m_name + " Function Library");

            #region DefTypes
            m_pLib.AddSupportForIParameter(new pBool(false));
            m_pLib.AddSupportForIParameter(new pInt(0));
            m_pLib.AddSupportForIParameter(new pDouble(0.0));
            m_pLib.AddSupportForIParameter(new pString(""));
            #endregion

            #region DefFunc
            m_fLib.AddSupportForMethod(new mGeneric_RandomInt());
            m_fLib.AddSupportForMethod(new mGeneric_RandomDouble());

            m_fLib.AddSupportForMethod(new mGeneric_MessageBox());
            #endregion
        }

        /// <summary>
        /// Uses the scripting language to parse the lines
        /// of script code from a given file
        /// </summary>
        /// <param name="path">Path of the script file that contains script from this language</param>
        /// <returns>Returns a script parser that can be used to execute the lines</returns>
        public ScriptParser ParseCode(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException();
            else if (!File.Exists(path))
                throw new FileNotFoundException(path);
            return new ScriptParser(this, File.ReadAllLines(path));
        }

        /// <summary>
        /// Supported line preceders
        /// </summary>
        public enum ValidPreceder
        {
            Declare, Execute
        }

        /// <summary>
        /// Gets the ValidPreceder of a given line
        /// </summary>
        /// <param name="line">Line to parse</param>
        /// <returns></returns>
        public ValidPreceder GetPreceder(string line)
        {
            try
            {
                var tmp = Line_Preceders.GetPreceder(line).ToLower();
                tmp = tmp[0].ToString().ToUpper() + tmp.Substring(1);
                return (ValidPreceder)Enum.Parse(typeof(ValidPreceder), tmp);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// A parser for translating the script and
        /// executing all relevant statements, as well
        /// as manipulating program variables during
        /// run-time
        /// </summary>
        public class ScriptParser
        {
            protected string[] m_lines;
            protected int m_lineNo;
            protected Dictionary<string, IParameter> m_vars, m_interactVars = new Dictionary<string,IParameter>();
            protected ScriptingLanguage m_lang;
            protected char[] m_illegalChars = "~`!@#$%^&*()+-= {}|[]\\:;'<>?,./\"'".ToCharArray();
            private readonly char[] m_op = { '+', '-', '*', '/', '%' };//add, subtract, multiply, divide, mod

            /// <summary>
            /// Creates a new ScriptParser to parse the script
            /// </summary>
            /// <param name="relevantLines">Lines of the script</param>
            public ScriptParser(ScriptingLanguage lang, string[] lines)
            {
                if (lines == null || lang == null)
                    throw new ArgumentNullException();
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        throw new ArgumentNullException();
                }

                m_lines = lines;
                m_lang = lang;
                RestartExecution();
            }

            /// <summary>
            /// Adds an interactive variable to the script, so that the
            /// script can edit program variables during run-time
            /// </summary>
            /// <param name="name">Script name of the variable</param>
            /// <param name="value">Data of the variable</param>
            public void AddInteractiveVariable(string name, IParameter value)
            {
                if (string.IsNullOrWhiteSpace(name) || value == null)
                    throw new ArgumentNullException();
                m_interactVars.Add(name, value);
            }

            /// <summary>
            /// Gets all of the interactive variables that are usable by the script
            /// </summary>
            public Dictionary<string, IParameter> InteractiveVariables
            {
                get { return m_interactVars; }
            }

            /// <summary>
            /// Executes the next line of script,
            /// if possible
            /// </summary>
            public void ExecuteNextLine()
            {
                if (m_lineNo >= m_lines.Length)
                    return;

                m_lineNo++;
                string line = Line_Preceders.GetCleanLine(m_lines[m_lineNo]);
                while (string.IsNullOrWhiteSpace(line))
                {
                    m_lineNo++;
                    line = Line_Preceders.GetCleanLine(m_lines[m_lineNo]);
                }

                if (!line.EndsWith(ScriptUtility.LineEnd.ToString()))
                    Error("Doesn't end with '" + ScriptUtility.LineEnd.ToString() + "'");

                ValidPreceder preceder = m_lang.GetPreceder(line);
                string text = Line_Preceders.GetPostPreceder(line);

                if (preceder == ValidPreceder.Declare)
                {
                    #region VarDeclaration
                    string varName = ScriptUtility.GetVariableName(line).ToLower();
                    ConfirmVarName(varName);
                    if (m_interactVars.ContainsKey(varName))
                    {
                        string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                        if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                            Error("No declare operator found.");
                        toParse = toParse.Remove(0, 1).Trim();
                        toParse = toParse.Remove(toParse.Length - 1);
                        m_interactVars[varName] = OperateParam(m_interactVars[varName], toParse);
                    }
                    else if (m_vars.ContainsKey(varName))
                    {
                        string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                        if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                            Error("No declare operator found.");
                        toParse = toParse.Remove(0, 1).Trim();
                        toParse = toParse.Remove(toParse.Length - 1);
                        m_vars[varName] = OperateParam(m_vars[varName], toParse);
                    }
                    else
                    {
                        string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                        if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                            Error("No declare operator found.");
                        toParse = toParse.Remove(0, 1).Trim();
                        toParse = toParse.Remove(toParse.Length - 1);
                        m_vars.Add(varName, OperateParam(m_lang.m_pLib.GetIParameter(ScriptUtility.GetVariableType(line)), toParse));
                    }
                    #endregion
                }
                else if (preceder == ValidPreceder.Execute)
                {
                    #region Methods
                    string toExec = text;
                    if (line.Contains(ScriptUtility.DeclareOperator))
                        toExec = text.Substring(text.IndexOf(ScriptUtility.DeclareOperator)).Trim();

                    string methName = ScriptUtility.GetMethodName(line);
                    if (!m_lang.m_fLib.SupportsMethod(methName))
                        Error("Method not supported.");

                    var argsString = ScriptUtility.GetPassedIParameters(line);
                    IParameter[] args = new IParameter[argsString.Length];

                    Method method = m_lang.m_fLib.GetMethod(ScriptUtility.GetMethodName(line));
                    if (args.Length > method.RequiredParameters.Count + method.OptionalParameters.Count)
                        Error("Too many arguments passed.");
                    else if (args.Length < method.RequiredParameters.Count)
                        Error("Too few arguments passed.");

                    for (int i = 0; i < argsString.Length; i++)
                    {
                        IParameter param = null;
                        if (i < method.RequiredParameters.Count)
                            param = method.RequiredParameters[i];
                        else if (i >= method.RequiredParameters.Count && i <= method.RequiredParameters.Count + method.OptionalParameters.Count)
                            param = method.OptionalParameters[i - method.RequiredParameters.Count];
                        args[i] = OperateParam(param, argsString[i]);
                    }

                    IParameter ret = method.Call(args);

                    if (line.Contains(ScriptUtility.DeclareOperator))
                    {
                        string varName = ScriptUtility.GetVariableName(line).ToLower();
                        ConfirmVarName(varName);
                        if (m_interactVars.ContainsKey(varName))
                        {
                            string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                            if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                                Error("No declare operator found.");
                            m_interactVars[varName] = ret;
                        }
                        else if (m_vars.ContainsKey(varName))
                        {
                            string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                            if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                                Error("No declare operator found.");
                            m_vars[varName] = ret;
                        }
                        else
                        {
                            string toParse = text.Substring(text.IndexOf(varName) + varName.Length).Trim();
                            if (!toParse.StartsWith(ScriptUtility.DeclareOperator.ToString()))
                                Error("No declare operator found.");
                            if (ScriptUtility.GetVariableType(line).ToLower() != ret.Type.ToLower())
                                Error("Variable type and method return type do not match.");
                            m_vars.Add(varName, ret);
                        }
                    }
                    #endregion
                }
                else
                    Error("Line preceder not supported.");
            }

            /// <summary>
            /// Executes the next given amount of lines of script,
            /// if possible
            /// </summary>
            /// <param name="amount">Amount of lines to execute</param>
            public void ExecuteNextLine(int amount)
            {
                for (int i = 0; i < amount; i++)
                    ExecuteNextLine();
            }

            private void ConfirmVarName(string name)
            {
                foreach (char ch in m_illegalChars)
                    if (name.Contains(ch))
                        Error("Variable name cannot contain an illegal character - in this case, '" + ch + "'");
            }
            private IParameter OperateParam(IParameter type, string parse)
            {
                string toParse = parse.Trim();

                Dictionary<int, int> posOfString = new Dictionary<int, int>();
                for (int i = 0; i < toParse.Length; i++)
                {
                    if (toParse[i] == '"')
                    {
                        if (posOfString.Count > 0 && posOfString.ElementAt(posOfString.Count - 1).Value == -1)
                            posOfString[posOfString.ElementAt(posOfString.Count - 1).Key] = i;
                        else
                            posOfString.Add(i, -1);
                    }
                }
                if (posOfString.ElementAt(posOfString.Count - 1).Value == -1)
                    Error("String declaration not closed with another \"");

                string sType = new pString("").Type;
                foreach (var param in m_vars)
                {
                    if (toParse.Contains(param.Key))
                    {
                        int index = 0;
                        while ((index = toParse.IndexOf(param.Key, index)) != -1)
                        {
                            bool replace = true;
                            foreach (var pair in posOfString)
                            {
                                if (index > pair.Key && index < pair.Value)
                                {
                                    replace = false;
                                    break;
                                }
                            }
                            if (replace)
                            {
                                if (param.Value.Type == sType)
                                    toParse = toParse.Substring(0, index) + "\"" + param.Value.Data.ToString() + "\"" +
                                        parse.Substring(index + param.Key.Length);
                                else
                                    toParse = toParse.Substring(0, index) + param.Value.Data.ToString() + parse.Substring(index + param.Key.Length);
                            }
                        }
                    }
                }

                return GetParsing(type, toParse);
            }
            protected IParameter GetParsing(IParameter type, string toParse)
            {
                if (type.Type == new pInt(0).Type)
                    return OperateInt(toParse);
                else if (type.Type == new pDouble(0).Type)
                    return OperateDouble(toParse);
                else if (type.Type == new pString("").Type || toParse.Contains("\""))
                    return OperateString(toParse);
                if (type.Type == new pBool(false).Type)
                    return OperateBool(toParse);
                Error("Type '" + type.Type.ToLower() + "' not supported.");
                return null;
            }
            private pInt OperateInt(string toParse)
            {
                return new pInt(Parse.String2Int(toParse));
            }
            private pDouble OperateDouble(string toParse)
            {
                return new pDouble(Parse.String2Double(toParse));
            }
            private pBool OperateBool(string toParse)
            {
                return new pBool(Parse.String2Bool(toParse));
            }
            private pString OperateString(string toParse)
            {
                if (!toParse.Contains("\""))
                    Error("Strings must be declared with \"");

                Dictionary<int, int> posOfString = new Dictionary<int, int>();
                for (int i = 0; i < toParse.Length; i++)
                {
                    if (toParse[i] == '"')
                    {
                        if (posOfString.Count > 0 && posOfString.ElementAt(posOfString.Count - 1).Value == -1)
                            posOfString[posOfString.ElementAt(posOfString.Count - 1).Key] = i;
                        else
                            posOfString.Add(i, -1);
                    }
                }
                if (posOfString.ElementAt(posOfString.Count - 1).Value == -1)
                    Error("String declaration not closed with another \"");
                if (posOfString.Count == 0)
                    return null;

                string simple = toParse;
                foreach (var pair in posOfString)
                {
                    simple = simple.Substring(0, pair.Key);
                    for (int i = pair.Key; i < pair.Value; i++)
                        simple = simple + "\"";
                    simple = simple + toParse.Substring(pair.Value);
                }
                var element = posOfString.ElementAt(0);
                string ops = simple.Replace("\"", ""), val = toParse.Substring(element.Key, element.Value - element.Key);
                int relevantOps = 0;
                for (int i = 0; i < ops.Length; i++)
                {
                    if (ops[i] == ' ')
                        continue;
                    relevantOps++;
                    if (ops[i] == m_op[0])//addition
                    {
                        element = posOfString.ElementAt(relevantOps);
                        val += toParse.Substring(element.Key + 1, element.Value - element.Key - 1);
                    }
                    else
                    {
                        Error("All operators other than '+' are not supported by string.");
                    }
                }

                return new pString(val.Substring(1));
            }

            /// <summary>
            /// Gets the current line of script that is being executed
            /// </summary>
            public int CurrentLine
            {
                get { return m_lineNo; }
            }

            /// <summary>
            /// Gets the total number of lines in the script
            /// </summary>
            public int TotalLines
            {
                get { return m_lines.Length; }
            }

            /// <summary>
            /// Calculates if the parser is done executing
            /// all of the script
            /// </summary>
            /// <returns>Returns true if it is done, and false otherwise</returns>
            public bool IsDoneExecuting()
            {
                return CurrentLine == TotalLines - 1;
            }

            /// <summary>
            /// Restarts execution of the script
            /// </summary>
            public void RestartExecution()
            {
                m_lineNo = 0;
                m_vars = new Dictionary<string, IParameter>();
            }

            private void Error(string err)
            {
                throw new Exception("Line " + (CurrentLine + 1) + ": " + err);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gCodeSL
{

    /// <summary>
    /// Contains functions for parsing and seperating a line of script
    /// </summary>
    public static class ScriptUtility
    {
        public const char OpenCall = '(', CloseCall = ')', LineEnd = ';', DeclareOperator = '=', IParameterSeperator = ',', StringDeclare = '"';

        /// <summary>
        /// Gets the name of the called method
        /// </summary>
        /// <param name="line">Unedited line</param>
        /// <returns>Returns the name of the method if there is a method call and null otherwise</returns>
        public static string GetMethodName(string line)
        {
            string arg = Line_Preceders.GetPostPreceder(line);
            if (!arg.EndsWith(LineEnd.ToString()))
                throw new Exception("'" + line + "' must end with '" + LineEnd.ToString() + "'");
            while (arg.Contains(StringDeclare.ToString()))
            {
                int index = arg.IndexOf(StringDeclare.ToString());
                int indexEnd = arg.Remove(index, 2).IndexOf(StringDeclare.ToString());
                arg = arg.Remove(index, indexEnd - index + 3).Trim();
            }
            var temp = arg.Split(DeclareOperator);
            if (temp.Length > 2)
            {
                arg = temp[1];
                for (int i = 2; i < temp.Length; i++)
                    arg += "=" + temp[i];
            }
            else
                arg = temp[temp.Length - 1].Trim();
            if (!arg.Contains(OpenCall))
                throw new Exception("Method call doesn't follow the method name with '" + OpenCall + "'");
            if (!arg.Contains(CloseCall))
                throw new Exception("Method call doesn't close the the call with '" + CloseCall + "'");
            return arg.Remove(arg.IndexOf(OpenCall)).Trim();
        }

        /// <summary>
        /// Gets the variable type of the line
        /// </summary>
        /// <param name="line">Undedited line</param>
        /// <returns>Returns the variable type as a string if the line declares one and null otherwise</returns>
        public static string GetVariableType(string line)
        {
            string arg = Line_Preceders.GetPostPreceder(line);
            if (!arg.EndsWith(LineEnd.ToString()))
                throw new Exception("'" + line + "' must end with '" + LineEnd.ToString() + "'");
            if (!arg.Contains(DeclareOperator))
                return null;
            return arg.Split(DeclareOperator)[0].Trim().Split(' ')[0];
        }

        /// <summary>
        /// Gets the name of the variable of the line
        /// </summary>
        /// <param name="line">Unedited line</param>
        /// <returns>Returns the name of the variable if the line declares one and null otherwise</returns>
        public static string GetVariableName(string line)
        {
            string arg = Line_Preceders.GetPostPreceder(line);
            if (!arg.EndsWith(LineEnd.ToString()))
                throw new Exception("'" + line + "' must end with '" + LineEnd.ToString() + "'");
            if (!arg.Contains(DeclareOperator))
                return null;
            var temp = arg.Split(DeclareOperator)[0].Trim().Split(' ');
            return temp[temp.Length - 1];
        }

        /// <summary>
        /// Gets all of the passed parameters to the method call
        /// </summary>
        /// <param name="line">Undedited line</param>
        /// <returns>Returns all of the passed parameters of the method as an array of strings if there is a method call and null otherwise</returns>
        public static string[] GetPassedIParameters(string line)
        {
            string arg = Line_Preceders.GetPostPreceder(line), temp = arg;
            if (!arg.EndsWith(LineEnd.ToString()))
                throw new Exception("'" + line + "' must end with '" + LineEnd.ToString() + "'");
            while (temp.Contains(StringDeclare.ToString()))
            {
                int index = temp.IndexOf(StringDeclare.ToString());
                int indexEnd = temp.Remove(index, 2).IndexOf(StringDeclare.ToString());
                temp = temp.Remove(index, indexEnd - index + 3).Trim();
            }
            if (temp.Contains(DeclareOperator))
                arg = arg.Trim().Remove(0, temp.IndexOf(DeclareOperator)).Trim().Remove(0, 1).Trim();
            string methodName = GetMethodName(line);
            if (methodName == null)
                return null;
            arg = arg.Remove(0, methodName.Length).Replace(OpenCall, ' ').Replace(CloseCall, ' ');
            arg = arg.Remove(arg.Length - 1, 1).Trim();
            var tmp = arg.Split(IParameterSeperator);
            List<string> passed = new List<string>();
            foreach (string str in tmp)
            {
                if (!string.IsNullOrWhiteSpace(str.Trim()))
                    passed.Add(str.Trim());
            }
            return passed.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage
{
    /// <summary>
    /// Useful functions pertaining to the line preceder format
    /// </summary>
    public static class Line_Preceders
    {
        /// <summary>
        /// Gets the relevant text of the line, ie,
        /// the uncommented, detabbed, and despaced line
        /// </summary>
        /// <param name="line">Line to use</param>
        /// <returns>Returns the relevant text</returns>
        public static string GetCleanLine(string line)
        {
            try
            {
                line = line.Trim();
                while (line.StartsWith("\t"))
                    line = line.Remove(0, 1);
                line = line.Trim();
                if (line.StartsWith(@"//"))
                    return "";
                else if (line.Contains(@"//"))
                    line = line.Split(new string[] { @"//" }, StringSplitOptions.None)[0];
                line = line.Trim();
                return line;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Gets the preceder of the line, ie,
        /// the part that is immediately before the first ':'
        /// of the line
        /// </summary>
        /// <param name="line">Line to use</param>
        /// <returns>Returns the preceder</returns>
        public static string GetPreceder(string line)
        {
            try { return GetCleanLine(line).Split(':')[0]; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Gets the main text of the line, ie,
        /// the part that follows the line preceder
        /// </summary>
        /// <param name="line">Line to use</param>
        /// <returns>Returns the main text</returns>
        public static string GetPostPreceder(string line)
        {
            try { return GetCleanLine(GetCleanLine(line).Replace(GetPreceder(line) + ":", "")); }
            catch (Exception) { throw; }
            
        }
    }
}
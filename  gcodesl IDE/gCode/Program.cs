using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using gCodeSL;

namespace gCodeIDE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var lang = new ScriptingLanguage();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new IDEMain(lang));
        }
    }
}

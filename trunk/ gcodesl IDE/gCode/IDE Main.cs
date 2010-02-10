using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using gCodeSL;

namespace gCodeIDE
{
    public partial class IDEMain : Form
    {
        ScriptingLanguage m_lang = null;
        List<IDEParameter> m_params;
        List<IDEMethod> m_methods;
        List<Module> m_children = new List<Module>();

        public IDEMain(string xmlFilePath)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(xmlFilePath))
                throw new ArgumentNullException();
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException();
            Init(xmlFilePath);
        }
        public IDEMain(ScriptingLanguage lang)
        {
            InitializeComponent();
            if (lang == null)
                throw new ArgumentNullException();

            XMLInteract.GenerateIDE(lang, "ide.xml");
            Init("ide.xml");
            m_lang = lang;
        }
        private void Init(string xmlFilePath)
        {
            m_params = XMLInteract.SupportedParameters(xmlFilePath);
            m_methods = XMLInteract.SupportedMethods(xmlFilePath, m_params);
            Text = XMLInteract.NameOfScriptingLanguage(xmlFilePath) + " IDE";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_children.Add(new Module(this));
            m_children[m_children.Count - 1].Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Filter = "gCode SL Files (*.gsl)|*.gsl|All Files (*.*)|*.*";
            dialog.ShowDialog();
            if (File.Exists(dialog.FileName))
            {
                m_children.Add(new Module(this, dialog.FileName));
                m_children[m_children.Count - 1].Show();
                m_children[m_children.Count - 1].ShowIcon = false;
            }
        }

        public List<IDEParameter> Parameters
        {
            get { return m_params; }
        }

        public List<IDEMethod> Methods
        {
            get { return m_methods; }
        }

        public ScriptingLanguage Language
        {
            get { return m_lang; }
        }

        public bool CanDebug
        {
            get
            {
                if (m_lang == null)
                    return false;
                return true;
            }
        }
    }
}

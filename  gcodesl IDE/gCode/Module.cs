using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using gInclude;
using gCodeSL;
using System.Drawing.Printing;

namespace gCodeIDE
{
    public partial class Module : Form
    {
        bool m_saved = true, m_debugging = false;
        string m_path = "";
        IDEMain m_parent;
        int m_unchanged = 100, m_printLine = 0;
        ScriptingLanguage.ScriptParser m_script;

        public Module(IDEMain parent)
        {
            if (parent == null)
                throw new ArgumentNullException();
            InitializeComponent();
            m_parent = parent;

            MdiParent = m_parent;
            Text = "New Script";
            Init();
        }
        public Module(IDEMain parent, string filePath)
        {
            if (parent == null || string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException();
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            InitializeComponent();
            m_parent = parent;
            m_path = filePath;

            MdiParent = m_parent;
            richTextBoxScript.Lines = File.ReadAllLines(filePath);
            Text = new FilePath(filePath).FullName;

            Init();
        }
        private void Init()
        {
            if (!m_parent.CanDebug)
                debugToolStripMenuItem.Enabled = false;
            m_saved = true;
            FormatAllScript();
            toolStrip1.Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ChooseFilePath())
                Save();
        }
        private void Save()
        {
            if (m_path == "" && !ChooseFilePath())
                return;
            if (!m_saved)
                Text = Text.Substring(0, Text.Length - 1);
            m_saved = true;

            File.WriteAllLines(m_path, richTextBoxScript.Lines);
            MessageBox.Show("'" + Text + "' saved successfully!", "Save");
        }
        private bool ChooseFilePath()
        {
            var dialog = new SaveFileDialog();
            dialog.OverwritePrompt = true;
            dialog.Filter = "gCode SL Files (*.gsl)|*.gsl|All Files (*.*)|*.*";
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                m_path = dialog.FileName;
                Text = new FilePath(dialog.FileName).FullName;
                if (!m_saved)
                    Text += "*";
                return true;
            }
            return false;
        }

        private void Module_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!m_saved && MessageBox.Show("Do you want to save your work before closing?", "Save", MessageBoxButtons.YesNo)
                == System.Windows.Forms.DialogResult.Yes)
                Save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormatAllScript()
        {
            int pos = richTextBoxScript.SelectionStart;
            richTextBoxScript.SelectAll();
            richTextBoxScript.SelectionColor = richTextBoxScript.ForeColor;

            for (int i = 0; i < richTextBoxScript.Lines.Length; i++)
            {
                string eval = richTextBoxScript.Lines[i].Trim().ToLower();
                while (eval.StartsWith("\t"))
                    eval = eval.Remove(0, 2);

                var posOfString = GetPosOfString(eval);
                int min = i - 1;
                for (int j = 0; j < i; j++)
                    min += richTextBoxScript.Lines[j].Length;

                #region Comments
                int comment = eval.IndexOf("//");
                if (comment > -1)
                {
                    richTextBoxScript.Select(min + 1 + comment, richTextBoxScript.Lines[i].Length - comment);
                    richTextBoxScript.SelectionColor = Color.DarkGreen;
                }
                #endregion

                #region Preceders
                //make all the preceders red
                int preceder = Line_Preceders.GetPreceder(richTextBoxScript.Lines[i]).Length;

                if (min > 0)
                    richTextBoxScript.Select(min + 1 + richTextBoxScript.Lines[i].IndexOf(preceder.ToString()), preceder + 1);
                else
                    richTextBoxScript.DeselectAll();
                richTextBoxScript.SelectionColor = Color.Red;
                #endregion

                #region Strings
                foreach (var pair in posOfString)
                {
                    richTextBoxScript.Select(min + 1 + pair.Key, pair.Value - pair.Key + 1);
                    richTextBoxScript.SelectionColor = Color.Brown;
                }
                #endregion

                #region Parameters
                foreach (var param in m_parent.Parameters)
                {
                    if (!eval.Contains(param.Name.ToLower()))
                        continue;
                    int index = 0;
                    while ((index = eval.IndexOf(param.Name.ToLower(), index)) != -1)
                    {
                        richTextBoxScript.DeselectAll();
                        bool replace = true;
                        foreach (var pair in posOfString)
                        {
                            if (index > pair.Key && index < pair.Value)
                                replace = false;
                        }
                        if (replace)
                        {
                            richTextBoxScript.Select(min + 1 + index + richTextBoxScript.Lines[i].IndexOf(preceder.ToString()),
                                param.Name.Length + 1);
                            richTextBoxScript.SelectionColor = Color.Blue;
                        }
                        index += param.Name.Length;
                        if (index >= eval.Length)
                            break;
                    }
                }
                #endregion

                #region Methods
                foreach (var methods in m_parent.Methods)
                {
                    if (!eval.Contains(methods.Name.ToLower()))
                        continue;
                    int index = 0;
                    while ((index = eval.IndexOf(methods.Name.ToLower(), index)) != -1)
                    {
                        richTextBoxScript.DeselectAll();
                        bool replace = true;
                        foreach (var pair in posOfString)
                        {
                            if (index > pair.Key && index < pair.Value)
                                replace = false;
                        }
                        if (replace)
                        {
                            richTextBoxScript.Select(min + 1 + index + richTextBoxScript.Lines[i].IndexOf(preceder.ToString()),
                                methods.Name.Length + 1);
                            richTextBoxScript.SelectionColor = Color.DarkBlue;
                        }
                        index += methods.Name.Length;
                        if (index >= eval.Length)
                            break;
                    }
                }
                #endregion
            }
            richTextBoxScript.DeselectAll();
            richTextBoxScript.SelectionStart = pos;
        }
        private Dictionary<int, int> GetPosOfString(string toParse)
        {
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
            return posOfString;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_unchanged--;
            if (m_unchanged == 0)
            {
                timer1.Enabled = false;
                FormatAllScript();
            }
        }

        private void richTextBoxScript_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_saved)
                Text = Text + "*";
            m_saved = false;
            if (!timer1.Enabled)
            {
                timer1.Stop();
                timer1.Enabled = true;
                timer1.Start();
            }
            m_unchanged = 100;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_debugging)
            {
                timerDebug.Enabled = true;
                pauseDebuggingToolStripMenuItem.Enabled = true;
                runToolStripMenuItem.Enabled = false;
            }
            else
            {
                runToolStripMenuItem.Text = "Continue Debugging";
                runToolStripMenuItem.Enabled = false;
                pauseDebuggingToolStripMenuItem.Enabled = true;
                stopDebuggingToolStripMenuItem.Enabled = true;
                richTextBoxScript.Enabled = false;
                Text += " -> Debugging";
                m_script = new ScriptingLanguage.ScriptParser(m_parent.Language, richTextBoxScript.Lines);
                timerDebug.Stop();
                timerDebug.Enabled = true;
                timerDebug.Start();
            }
        }

        private void pauseDebuggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseDebuggingToolStripMenuItem.Enabled = false;
            runToolStripMenuItem.Enabled = true;
            timerDebug.Enabled = false;
        }

        private void stopDebuggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopDebugging();
        }
        private void StopDebugging()
        {
            m_debugging = false;
            timerDebug.Enabled = false;
            runToolStripMenuItem.Text = "Run";
            runToolStripMenuItem.Enabled = true;
            pauseDebuggingToolStripMenuItem.Enabled = false;
            stopDebuggingToolStripMenuItem.Enabled = false;
            richTextBoxScript.Enabled = true;
            m_script = null;
            try { Text = Text.Substring(0, Text.Length - " -> Debugging".Length); }
            catch { }
        }

        private void timerDebug_Tick(object sender, EventArgs e)
        {
            if (m_script.IsDoneExecuting())
            {
                StopDebugging();
                return;
            }
            try { m_script.ExecuteNextLine(); }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
                StopDebugging();
                return;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = new PrintDocument();
            doc.DocumentName = Text;

            PrintDialog dialog = new PrintDialog();
            dialog.UseEXDialog = true;
            dialog.Document = doc;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                doc.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);
                m_printLine = 0;
                doc.Print();
            }
        }
        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            ppeArgs.HasMorePages = false;
            Font verdana10Font = new Font("Verdana", 10);
            Graphics g = ppeArgs.Graphics;
            int count = 0;
            float leftMargin = ppeArgs.MarginBounds.Left, topMargin = ppeArgs.MarginBounds.Top,
                linesPerPage = ppeArgs.MarginBounds.Height /  verdana10Font.GetHeight(g), yPos = 0;
            for (int i = 0; i < richTextBoxScript.Lines.Length; i++)
            {
                if (i >= linesPerPage)
                {
                    ppeArgs.HasMorePages = true;
                    break;
                }
                else if (i + m_printLine >= richTextBoxScript.Lines.Length)
                    break;
                yPos = topMargin + (count * verdana10Font.GetHeight(g));
                g.DrawString(richTextBoxScript.Lines[i + m_printLine], verdana10Font, Brushes.Black,
                leftMargin, yPos, new StringFormat());
            }
            m_printLine += (int)linesPerPage;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBoxScript.CanUndo)
                richTextBoxScript.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBoxScript.CanRedo)
                richTextBoxScript.Redo();
        }

        private void toolStripButtonFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toolStripTextBoxFind.Text))
                return;
            richTextBoxScript.Find(toolStripTextBoxFind.Text, richTextBoxScript.SelectionStart, RichTextBoxFinds.None);
        }

        private void toolStripSplitButtonReplace_ButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toolStripTextBoxFind.Text) || string.IsNullOrWhiteSpace(toolStripTextBoxReplace.Text))
                return;
            int pos = richTextBoxScript.Find(toolStripTextBoxFind.Text, richTextBoxScript.SelectionStart, RichTextBoxFinds.None);
            richTextBoxScript.Text = richTextBoxScript.Text.Substring(0, pos) + toolStripTextBoxReplace.Text +
                richTextBoxScript.Text.Substring(pos + toolStripTextBoxFind.Text.Length);
        }

        private void replaceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toolStripTextBoxFind.Text) || string.IsNullOrWhiteSpace(toolStripTextBoxReplace.Text))
                return;
            richTextBoxScript.Text = richTextBoxScript.Text.Replace(toolStripTextBoxFind.Text, toolStripTextBoxReplace.Text);
        }
    }
}

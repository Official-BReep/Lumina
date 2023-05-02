using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IDE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Tabs.Selected += new TabControlEventHandler(Tabs_Selected);
            WebClient webclient = new WebClient();
            try
            {
                if (!webclient.DownloadString("https://raw.githubusercontent.com/Triskix/Lumina/main/version.txt").Contains("Version"))
                {
                    if (MessageBox.Show("An Update is available, do you want to download it?", "IDE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        using (var client = new WebClient())
                        {
                            Process.Start("Updater.exe");
                            this.Close();
                        }
                }
            }
            catch
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void rtxtB_TextChanged(object sender, EventArgs e)
        {

            Regex rx = new Regex("class");
            int index = rtxtB.SelectionStart;

            foreach (Match match in rx.Matches(rtxtB.Text))
            {
                rtxtB.Select(match.Index, match.Value.Length);
                rtxtB.SelectionColor = Color.Blue;
                rtxtB.SelectionStart = index;
                rtxtB.SelectionColor = Color.Black;
            }

            Regex rx2 = new Regex("out");
            foreach (Match match in rx2.Matches(rtxtB.Text))
            {
                rtxtB.Select(match.Index, match.Value.Length);
                rtxtB.SelectionColor = Color.Orange;
                rtxtB.SelectionStart = index;
                rtxtB.SelectionColor = Color.Black;
            }

            Regex rx3 = new Regex("define");
            foreach (Match match in rx3.Matches(rtxtB.Text))
            {
                rtxtB.Select(match.Index, match.Value.Length);
                rtxtB.SelectionColor = Color.BlueViolet;
                rtxtB.SelectionStart = index;
                rtxtB.SelectionColor = Color.Black;
            }

        }

        private void Lock_Click(object sender, EventArgs e)
        {
            if (rtxtB.ReadOnly)
            {
                rtxtB.ReadOnly = false;
                Lock.Text = "🔓 Read Only";
            }
            else
            {
                rtxtB.ReadOnly = true;
                Lock.Text = "🔒 Read Only";
            }
        }
        private void Tabs_Selected(object sender, EventArgs e)
        {
            if (Tabs.SelectedTab.Text == "+")
            {
                String title = "Tab" + Tabs.TabCount;
                TabPage myTabPage = new TabPage(title);
                var newTextField = new RichTextBox();
                newTextField.Dock = DockStyle.Fill;
                myTabPage.Controls.Add(newTextField);
                myTabPage.Name = title;

                myTabPage.Controls.Add(newTextField);
                Tabs.TabPages.Add(myTabPage);
                Tabs.SelectedTab = myTabPage;

                Tabs.TabPages.Remove(addnewtab);
                Tabs.TabPages.Add(addnewtab);

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox myTB = Tabs.SelectedTab.Controls[0] as RichTextBox;
            var Text = myTB.Text;
            byte[] bytes = Encoding.ASCII.GetBytes(Text);
            Console.WriteLine(Text);

            saveFileDialog1.Filter = "RichTextFormate|*.rtf|Text Files|*.txt|All Files|*.*";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < bytes.Length; i++)
                {
                    fs.WriteByte(bytes[i]);
                }

                fs.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog1.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog1.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                    rtxtB.Text = fileContent;
                }
            }
        }

        private void PopulateTreeView(string path, TreeNode parentNode)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            TreeNode node = new TreeNode(directory.Name);

            if (parentNode == null)
            {
                treeView1.Nodes.Add(node);
            }
            else
            {
                parentNode.Nodes.Add(node);
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                PopulateTreeView(subDirectory.FullName, node);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                node.Nodes.Add(file.Name);
            }
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog1.FileName;
                Console.WriteLine(filePath);
                byte[] bytesData;
                string stringData;

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    bytesData = new byte[fileStream.Length];
                    fileStream.Read(bytesData, 0, (int)fileStream.Length);
                }

                stringData = System.Text.Encoding.UTF8.GetString(bytesData);
                Console.WriteLine(stringData);

                TabPage myTabPage = new TabPage("Tab " + Tabs.TabCount);
                RichTextBox rtxtB2 = new RichTextBox();
                myTabPage.Controls.Add(rtxtB2);
                rtxtB2.Dock = DockStyle.Fill;
                Tabs.TabPages.Add(myTabPage);
                Tabs.SelectedTab = myTabPage;
                Tabs.TabPages.Remove(addnewtab);
                addnewtab = new TabPage("+");
                Tabs.TabPages.Add(addnewtab);

                RichTextBox myTB = Tabs.SelectedTab.Controls[0] as RichTextBox;
                //var Text = myTB.Text;
                myTB.Text = stringData;
            }
        }

        private void hexEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexEditorToolStripMenuItem.CheckOnClick = true;
            if (hexEditorToolStripMenuItem.Checked)
            {
                hexEditorToolStripMenuItem.Checked = true;
            }
            else
            {
                hexEditorToolStripMenuItem.Checked = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.FullPath;
            Console.WriteLine(path);
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string root = folderBrowserDialog1.SelectedPath;
                PopulateTreeView(root, null);
            }
        }
    }
}

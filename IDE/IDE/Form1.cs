using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
                TabPage myTabPage = new TabPage("Tab "+ Tabs.TabCount);
                Tabs.TabPages.Add(myTabPage);
                Tabs.SelectedTab = myTabPage;
                Tabs.TabPages.Remove(addnewtab);
                addnewtab = new TabPage("+");
                Tabs.TabPages.Add(addnewtab);

            }
        }
    }
}

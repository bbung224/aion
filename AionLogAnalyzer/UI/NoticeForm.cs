using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AionLogAnalyzer
{
    public partial class NoticeForm : Form
    {
        MainForm main;
        WebBrowser w;
        public NoticeForm(MainForm main)
        {
            this.main = main;
            InitializeComponent();
            w = new WebBrowser();
            w.Navigate("http://aion.plaync.co.kr");
        }

        public void SetNotice(String str)
        {
            str = str.Replace("\n", "\r\n");
            this.textBox1.AppendText(str);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            main.GotoHomepage(0);
        }

        private void buttonAion_Click(object sender, EventArgs e)
        {
            main.GotoHomepage(1);
        }

        private void buttonLuncher_Click(object sender, EventArgs e)
        {
            GoLauncher();
        }

        public void GoLauncher()
        {
            w.Document.InvokeScript("gameStart");
        }
    }
}

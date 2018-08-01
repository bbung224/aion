using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AES
{
    public partial class NoticeForm : Form
    {
        MainForm main;
        public NoticeForm(MainForm main)
        {
            this.main = main;
            InitializeComponent();
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
    }
}

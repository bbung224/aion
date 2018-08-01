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
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.okBtn.Enabled = false;
        }

        public void SetPos(string str, int pos)
        {
            this.label1.Text = str;
            if (this.progressBar1.Value + pos <= 100)
                this.progressBar1.Value += pos;
            else
                this.progressBar1.Value = 100;
        }

        public void SetExit(string str, int type)
        {
            this.label1.Text = str;
            if (type == 0)
            {
                this.okBtn.Enabled = true;
            }
            else if (type == 1)
            {
                this.okBtn.Enabled = true;
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


    }
}

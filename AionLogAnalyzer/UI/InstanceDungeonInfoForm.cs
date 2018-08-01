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
    public partial class InstanceDungeonInfoForm : Form
    {
        public InstanceDungeonInfoForm(MainForm mainForm)
        {
            InitializeComponent();
            this.TopMost = true;

            int x = mainForm.Location.X + mainForm.Width;
            if (x + this.Width > Screen.AllScreens[0].WorkingArea.Width)
            {
                x = mainForm.Location.X - this.Width;
            }
            this.Location = new Point(x, mainForm.Location.Y);
        }

        public void SetIndun(InstanceDungeon indun)
        {
            this.Text = indun.DungeonName;
            this.textBox1.AppendText(indun.GetInfo());
        }
    }
}

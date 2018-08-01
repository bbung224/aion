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
    public partial class TimerForm : Form
    {
        private Timer timer = null;
        private DateTime startTime;

        public TimerForm()
        {
            InitializeComponent();
        }

        private void TimerForm_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Start();
            startTime = DateTime.Now;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = DateTime.Now - startTime;
            int min = timeSpan.Minutes;
            int sec = timeSpan.Seconds;
            string str = (min > 9) ? (""+min) : ("0" + min);
            str = str + ":";
            str = str + ((sec > 9) ? ("" + sec) : ("0" + sec));
            this.timerLabel.Text = str;
        }


        private void TimerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}

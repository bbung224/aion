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
    public partial class TimerForm룬의보호탑 : Form
    {
        private Timer timer = null;
        private DateTime startTime;
        private MainForm main;
        private DateTime[] startTimes;
        private DateTime[] nextTimes;

        public TimerForm룬의보호탑(MainForm main)
        {
            this.main = main;
            InitializeComponent();
            this.listView1.Items.Clear();
            String[] ss = new string[] { "동부", "서부", "남부", "북부" };
            for (int i = 0; i < 4; i++)
            {
                ListViewItem item = new ListViewItem(new string[] { ss[i], "----", "----", "시작전" });
                this.listView1.Items.Add(item);
            }
            startTimes = new DateTime[4];
            nextTimes = new DateTime[4];
        }

        private void TimerForm_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 200;
            timer.Start();
            startTime = DateTime.Now;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            this.timerLabel.Text = GetStringTime(now);
            for (int i = 0; i < 4; i++)
            {
                if (nextTimes[i] != DateTime.MinValue)
                {
                    TimeSpan timeSpan = nextTimes[i] - DateTime.Now;
                    int min = timeSpan.Minutes;
                    int sec = timeSpan.Seconds;
                    this.listView1.Items[i].SubItems[1].Text = GetStringTime(min, sec);

                    if (min == 0 && sec == 0)
                    {
                        SetNextTime(i, nextTimes[i]);
                    }
                }
            }
            if ((now - startTime).Minutes >= 30) ProcessFail();
        }


        private void TimerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            this.main.timerFormDefence = null;
        }

        public void SetStart(string where)
        {
            int idx = GetIndex(where);
            startTimes[idx] = DateTime.Now;
            this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.listView1.Items[idx].SubItems[3].Text = GetStringTime(startTimes[idx]);
                SetNextTime(idx, startTimes[idx]);
            }));
        }

        public void SetStop(string where)
        {
            int idx = GetIndex(where);
            startTimes[idx] = DateTime.MinValue;
            nextTimes[idx] = DateTime.MinValue;
            this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.listView1.Items[idx].SubItems[1].Text = "----";
                this.listView1.Items[idx].SubItems[2].Text = "----";
                this.listView1.Items[idx].SubItems[3].Text = "파괴";
            }));
        }

        public void ProcessFail()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.listView1.Items[i].SubItems[1].Text = "----";
                    this.listView1.Items[i].SubItems[2].Text = "----";
                    this.listView1.Items[i].SubItems[3].Text = "실패";
                }
            }));
        }

        public void ProcessSuccess()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.listView1.Items[i].SubItems[1].Text = "----";
                    this.listView1.Items[i].SubItems[2].Text = "----";
                    this.listView1.Items[i].SubItems[3].Text = "성공";
                    this.startTimes[i] = DateTime.MinValue;
                    this.nextTimes[i] = DateTime.MinValue;
                }
            }));
        }

        // 2개는 begininvoke 
        public void Restart()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Start();
            startTime = DateTime.Now;
        }

        public void Finish()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }


        private void SetNextTime(int idx, DateTime d)
        {
            nextTimes[idx] = d.AddMinutes(2);
            this.listView1.Items[idx].SubItems[2].Text = GetStringTime(nextTimes[idx]);
        }

        private string GetStringTime(DateTime d)
        {
            TimeSpan timeSpan = d - startTime;
            int min = timeSpan.Minutes;
            int sec = timeSpan.Seconds;
            return GetStringTime(min, sec);
        }

        private string GetStringTime(int min, int sec)
        {
            if (min < 0) min *= -1;
            if (sec < 0) sec *= -1;

            string str = (min > 9) ? ("" + min) : ("0" + min);
            str = str + ":";
            str = str + ((sec > 9) ? ("" + sec) : ("0" + sec));
            return str;
        }

        private int GetIndex(string where)
        {
            if (where == "동부") return 0;
            else if (where == "서부") return 1;
            else if (where == "남부") return 2;
            else if (where == "북부") return 3;
            return 0;
        }
    }
}

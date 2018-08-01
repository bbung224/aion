using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace AionLauncher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        WebBrowser w;
        Timer t;
        bool bStart = true;
        public MainForm()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Width = 0;
            this.Height = 0;
            this.Location = new System.Drawing.Point(-1000, -1000);
            this.Load += new EventHandler(MainForm_Load);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Year > 2014)
            {
                MessageBox.Show("더이상 동작하지 않습니다", "기간만료");
                Application.Exit();
            }
            else
            {
                Process[] p = Process.GetProcessesByName("NCLauncher");
                t = new Timer();
                t.Tick += new EventHandler(t_Tick);
                t.Interval = 1000;
                t.Start();

                if (p != null && p.Length > 0) Application.Exit();
                else
                {
                    w = new WebBrowser();
                    w.Navigate("http://aion.plaync.co.kr");
                    w.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(w_DocumentCompleted);
                }
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("NCLauncher");
            if (p != null && p.Length > 0) Application.Exit();
        }

        void w_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (bStart)
            {
                GoLauncher();
                bStart = false;
                //Application.Exit();
            }
        }
        void GoLauncher()
        {
            w.Document.InvokeScript("gameStart");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  System.Windows.Forms;

namespace AionLogAnalyzer
{
   

    partial class MainForm
    {
        private HostedCheckbox topMostCheckbox, oneClassCheckbox, autoSortCheckbox;
        private System.Windows.Forms.ToolStripSeparator statusToolStripSeparator1, statusToolStripSeparator2, statusToolStripSeparator3;
        private AionLogAnalyzer.ToolStripTrackBar opacityTrackBar;
        private ToolStripMenuItem[] toolStripClassViewItems;

        private void InitializeComponentManually()
        {
            this.topMostCheckbox = new HostedCheckbox();
            this.statusToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.opacityTrackBar = new ToolStripTrackBar();
            this.statusToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.oneClassCheckbox = new HostedCheckbox();
            this.statusToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.autoSortCheckbox = new HostedCheckbox();

            // 
            // topMostCheckbox
            // 
            this.topMostCheckbox.Checked = true;
            this.topMostCheckbox.Name = "topMostCheckbox";
            this.topMostCheckbox.Size = new System.Drawing.Size(66, 22);
            this.topMostCheckbox.Text = "항상 위";
            this.topMostCheckbox.Checked = this.TopMost;
            this.topMostCheckbox.Click += new System.EventHandler(this.topMostCheckbox_Click);
            // 
            // statusToolStripSeparator1
            // 
            this.statusToolStripSeparator1.Name = "toolStripSeparator1";
            this.statusToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // opacityTrackBar
            // 
            this.opacityTrackBar.AutoSize = false;
            this.opacityTrackBar.Name = "opacityTrackBar";
            this.opacityTrackBar.Size = new System.Drawing.Size(100, 20);
            this.opacityTrackBar.Value = 0;
            this.opacityTrackBar.ValueChanged += new System.EventHandler(this.opacityTrackBar_ValueChanged);
            // 
            // statusToolStripSeparator2
            // 
            this.statusToolStripSeparator2.Name = "toolStripSeparator1";
            this.statusToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // oneClassCheckbox
            // 
            this.oneClassCheckbox.Checked = LogParser.OneClassFlag;
            this.oneClassCheckbox.Size = new System.Drawing.Size(66, 22);
            this.oneClassCheckbox.Text = "클래스별 1인";
            this.oneClassCheckbox.Click += new System.EventHandler(this.oneClassCheckbox_Click);
            // 
            // statusToolStripSeparator3
            // 
            this.statusToolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // autoSortCheckbox
            // 
            this.autoSortCheckbox.Checked = LogParser.AutoSortFlag;
            this.autoSortCheckbox.Size = new System.Drawing.Size(66, 22);
            this.autoSortCheckbox.Text = "자동정렬";
            this.autoSortCheckbox.Click += new System.EventHandler(this.autoSortCheckbox_Click);


            this.statusToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topMostCheckbox,
            this.statusToolStripSeparator1,
            this.opacityTrackBar,
            this.statusToolStripSeparator2,
            this.oneClassCheckbox,
            this.statusToolStripSeparator3,
            this.autoSortCheckbox});

            //2013-04-30
            this.labelVersion.Text += Version;
            //this.panelInformaion.Controls.Remove(this.panelUpdate);
            //this.panelUpdate.Controls.Clear();
            this.panelUpdate.Controls.Remove(this.progressBar1);


            toolStripClassViewItems = new ToolStripMenuItem[12];
            toolStripClassViewItems[0] = new ToolStripMenuItem("전체");
            toolStripClassViewItems[0].Checked = true;

            toolStripClassViewItems[1] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.검성));
            toolStripClassViewItems[2] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.수호성));
            toolStripClassViewItems[3] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.살성));
            toolStripClassViewItems[4] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.궁성));
            toolStripClassViewItems[5] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.마도성));
            toolStripClassViewItems[6] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.정령성));
            toolStripClassViewItems[7] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.치유성));
            toolStripClassViewItems[8] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.호법성));
            toolStripClassViewItems[9] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.사격성));
            toolStripClassViewItems[10] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.음유성));
            toolStripClassViewItems[11] = new ToolStripMenuItem(SkillDictionary.GetClassString(ClassType.기갑성));
            foreach (ToolStripMenuItem item in toolStripClassViewItems)
            {
                item.Click += new EventHandler(item_Click);
            }
            this.toolStripClassView.DropDownItems.AddRange(toolStripClassViewItems);

            comboBoxSelectServer.Items.Add("");
            comboBoxSelectServer.Items.AddRange(ServerList.ServerName);

            this.panelPopup.Controls.Remove(this.progressBarBoss);
            HidePopup();
            
            //2013-09-30 분간측정 메뉴로 이동
            for (int i = 0; i < 10; i++)
            {
                ToolStripMenuItem ii= new ToolStripMenuItem((i + 1) + "분");
                this.분간측정ToolStripMenuItem.DropDownItems.Add(ii);
                ii.Click += new EventHandler(ii_Click);
            }

            if (this.tabControl1.Controls.Contains(hiddenTabPage))
                this.tabControl1.Controls.Remove(this.hiddenTabPage);
        }

       

    }
}

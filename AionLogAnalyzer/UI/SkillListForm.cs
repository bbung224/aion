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
    public partial class SkillListForm : Form
    {
        private ListViewSorter _SkillSorter;
        public SkillListForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            _SkillSorter = new ListViewSorter(new int[] { 0 });
            _SkillSorter.SortColumn = 1;
            _SkillSorter.SortOrder = ListViewSortOrder.Descending;
            this.listView1.ListViewItemSorter = _SkillSorter;
        }

        public void Show(User player)
        {
            {
                ListViewItem item = new ListViewItem(new string[7]);
                item.SubItems[0].Text = "전체";
                item.SubItems[1].Text = player.TotalDamage + "";
                item.SubItems[2].Text = "100%";
                item.SubItems[3].Text = player.TotalDealCount + "회";
                if (player.TotalDealCount == 0)
                {
                    item.SubItems[4].Text = "0";
                    item.SubItems[6].Text = "0%";
                }
                else
                {
                    item.SubItems[4].Text = (player.TotalDamage / player.TotalDealCount) + "";
                    item.SubItems[6].Text = (player.TotalCriticalDealCount * 100 / player.TotalDealCount) + "%";
                }
                item.SubItems[5].Text = player.TotalCriticalDealCount + "회";

                this.listView1.Items.Add(item);
                item.BackColor = Color.Yellow;
            }

            foreach (SkillEntity entity in player.SkillList)
            {
                ListViewItem item = new ListViewItem(new string[7]);
                item.SubItems[0].Text = entity.SkillName;
                item.SubItems[1].Text = entity.TotalDamage + "";
                if (player.TotalDamage ==0) item.SubItems[2].Text = "0%";            
                else item.SubItems[2].Text = (entity.TotalDamage * 100 / player.TotalDamage) + "%";
                item.SubItems[3].Text = entity.Count + "회";
                if (entity.Count == 0) item.SubItems[4].Text = "";
                else item.SubItems[4].Text = (entity.TotalDamage / entity.Count) + "";
                item.SubItems[5].Text = entity.CriticalCount + "회";
                if (entity.Count == 0) item.SubItems[6].Text = "0%";
                else item.SubItems[6].Text = (entity.CriticalCount * 100 / entity.Count) + "%";
                this.listView1.Items.Add(item);
            }
            //this.listView1.Items.Add(user.ListItem);
            //ListItem = new ListViewItem(new string[11]);
            // 스킬 대미지 비율 사용횟수 평균대미지
            this.textBox1.Text = "";
            try
            {
                foreach (String log in player.LogList)
                {
                    this.textBox1.AppendText(log + "\r\n");
                }
            }
            catch { }
            this.Text = player.Name + " 사용 스킬 목록";
            this.Show();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _SkillSorter.SortColumn)
            {
                if (_SkillSorter.SortOrder == ListViewSortOrder.Ascending)
                {
                    _SkillSorter.SortOrder = ListViewSortOrder.Descending;
                }
                else
                {
                    _SkillSorter.SortOrder = ListViewSortOrder.Ascending;
                }
            }

            else
            {
                _SkillSorter.SortColumn = e.Column;
                _SkillSorter.SortOrder = ListViewSortOrder.Ascending;
            }
            listView1.Sort();
        }
    }
}

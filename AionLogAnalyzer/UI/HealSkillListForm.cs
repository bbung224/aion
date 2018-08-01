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
    public partial class HealSkillListForm : Form
    {
        private ListViewSorter _SkillSorter;
        public HealSkillListForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            _SkillSorter = new ListViewSorter(new int[] { 0 });
            _SkillSorter.SortColumn = 2;
            _SkillSorter.SortOrder = ListViewSortOrder.Descending;
            this.listView1.ListViewItemSorter = _SkillSorter;
        }

        public void Show(User player)
        {
            {
                ListViewItem item = new ListViewItem(new string[5]);
                item.SubItems[0].Text = "전체";
                item.SubItems[1].Text = player.HealAmount + "";
                item.SubItems[2].Text = "100%";
                item.SubItems[3].Text = player.HealLogList.Count + "회";
                this.listView1.Items.Add(item);
                item.BackColor = Color.Yellow;
            }

            foreach (HealSkillEntity hse in player.HealList)
            {
                ListViewItem item = new ListViewItem(new string[6]);
                item.SubItems[0].Text = hse.Target;
                item.SubItems[1].Text = hse.TotalRecover + "";
                if (hse.TotalRecover == 0) item.SubItems[2].Text = "0%";
                else item.SubItems[2].Text = (hse.TotalRecover * 100 / player.HealAmount) + "%";
                item.SubItems[3].Text = hse.Count + "회";
                if (hse.Count == 0) item.SubItems[4].Text = "";
                else item.SubItems[4].Text = (hse.TotalRecover / hse.Count) + "";
                this.listView1.Items.Add(item);
            }
            this.textBox1.Text = "";
            try
            {
                foreach (String log in player.HealLogList)
                {
                    this.textBox1.AppendText(log + "\r\n");
                }
            }
            catch { }
            this.Text = player.Name + " 치유 대상 목록";
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

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
    public partial class HPMPSkillListForm : Form
    {
        private ListViewSorter _SkillSorter;
        private bool isHp;
        public HPMPSkillListForm(bool isHp)
        {
            this.isHp = isHp;
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            _SkillSorter = new ListViewSorter(new int[] { 0 });
            /*
            _SkillSorter.SortColumn = 2;
            _SkillSorter.SortOrder = ListViewSortOrder.Descending;
             */
            this.listView1.ListViewItemSorter = _SkillSorter;
        }

        public void Show(User player)
        {
            {
                ListViewItem item = new ListViewItem(new string[6]);
                item.SubItems[0].Text = "전체";
                item.SubItems[2].Text = ((isHp) ? player.HPRecover : player.MPRecover) + "";
                item.SubItems[3].Text = "100%";
                item.SubItems[4].Text = ((isHp) ? player.HPRecoverLogList.Count : player.MPRecoverLogList.Count) + "회";
                this.listView1.Items.Add(item);
                item.BackColor = Color.Yellow;
            }
            List<HPMPWhoEntity> list = ((isHp) ? player.HPWhoList : player.MPWhoList);
            foreach (HPMPWhoEntity we in list)
            {
                foreach (HPMPSkillEntity se in we.SkillList)
                {
                    ListViewItem item = new ListViewItem(new string[6]);
                    if (String.IsNullOrEmpty(we.Who)) item.SubItems[0].Text = "---";
                    else item.SubItems[0].Text = we.Who;
                    if (String.IsNullOrEmpty(se.SkillName)) item.SubItems[1].Text = "---";
                    else item.SubItems[1].Text = se.SkillName;
                    item.SubItems[2].Text = se.TotalRecover + "";
                    if (player.HPRecover == 0) item.SubItems[3].Text = "0%";
                    else item.SubItems[3].Text = (se.TotalRecover * 100 / player.HPRecover) + "%";
                    item.SubItems[4].Text = se.Count + "회";
                    if (se.Count == 0) item.SubItems[5].Text = "";
                    else item.SubItems[5].Text = (se.TotalRecover / se.Count) + "";
                    this.listView1.Items.Add(item);
                    if (se == we.SkillList[0]) item.BackColor = Color.GreenYellow;
                }
            }
            //this.listView1.Items.Add(user.ListItem);
            //ListItem = new ListViewItem(new string[11]);
            // 스킬 대미지 비율 사용횟수 평균대미지
            this.textBox1.Text = "";
            try
            {
                List<string> logs = ((isHp) ? player.HPRecoverLogList : player.MPRecoverLogList);
                foreach (String log in logs)
                {
                    this.textBox1.AppendText(log + "\r\n");
                }
            }
            catch { }
            this.Text = player.Name + " " + ((isHp) ? "생명력" : "정신력") + " 회복";
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

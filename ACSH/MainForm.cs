using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;

namespace ACSH
{
    public partial class MainForm : Form
    {
        private HttpHandler httpHandler;
        public MainForm()
        {
            InitializeComponent();
            httpHandler = new HttpHandler(this);

            this.textBoxSearch.Text = "물리치명타 17";
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            //this.statusStrip1.Text = "검색중..";
            //ImageList imageList = new ImageList();
            this.listViewSearchResult.Items.Clear();
            bool ret = this.httpHandler.DownloadSearchHtml(this.textBoxSearch.Text);

            if (ret == false)
            {
                MessageBox.Show("검색결과가 없습니다.", "검색");
                return;
            }
            for (int i = 0; i < httpHandler.ResultList.Count; i++)
            {
               
                /*
                WebClient client = new WebClient();
                byte[] data = client.DownloadData(httpHandler.ResultList[i].ImageUrl);
                client.Dispose();
                Bitmap map = new Bitmap(new MemoryStream(data));
                imageList.Images.Add(map);
                */
                ListViewItem lvi = new ListViewItem((i+1)+"", i);
                lvi.SubItems.Add(httpHandler.ResultList[i].Name);
                lvi.SubItems.Add(httpHandler.ResultList[i].Count);
                lvi.SubItems.Add(httpHandler.ResultList[i].LowPrice);
                lvi.SubItems.Add(httpHandler.ResultList[i].HighPrice);
                lvi.SubItems.Add(httpHandler.ResultList[i].Level);
                lvi.SubItems.Add(httpHandler.ResultList[i].CommentCount);
                lvi.Tag = httpHandler.ResultList[i];
                this.listViewSearchResult.Items.Add(lvi);
            }
            //this.statusStrip1.Text = "검색완료";
            //this.listViewSearchResult.SmallImageList = imageList;
            //this.listViewSearchResult.LargeImageList = imageList;
        }

        public void AppendDebug(string str)
        {
            if (str == null) return;
            this.textBoxDebug.AppendText(str + "\r\n");
        }

        public void Debug(string str)
        {
            if (str == null) return;
            this.textBoxDebug.Text = "";
            AppendDebug(str);
        }

        private void AppendLineDetail(string str)
        {
            if (str == null) return;
            this.richTextBoxItemDetail.AppendText(str + "\r\n");
        }


        private void listViewSearchResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //this.statusStrip1.Text = "세부정보 다운로드중..";
                ListView.SelectedIndexCollection collection = this.listViewSearchResult.SelectedIndices;
                if (collection.Count == 0) return;
                EntitySearchResultItem item = (EntitySearchResultItem)this.listViewSearchResult.Items[collection[0]].Tag;
                this.httpHandler.DownloadItemDetail(item);
                this.richTextBoxItemDetail.Text = "";
                AppendLineDetail(this.httpHandler.ItemDetailInfo.Name + "\t\t" + "등록건수: " + this.httpHandler.ItemDetailInfo.CountStr);
                AppendLineDetail("등록 최저가: " + this.httpHandler.ItemDetailInfo.PriceStr[0] + "\t\t" + "등록 최고가: " + this.httpHandler.ItemDetailInfo.PriceStr[1]);
                AppendLineDetail("주간 평균가: " + this.httpHandler.ItemDetailInfo.PriceStr[4] + "\t\t" + "주간 최저가: " + this.httpHandler.ItemDetailInfo.PriceStr[2] + "\t\t" +
                                 "주간 최고가: " + this.httpHandler.ItemDetailInfo.PriceStr[3]);

                this.listView1.Items.Clear();

                List<EntityItemTimeInfo> newList = new List<EntityItemTimeInfo>();
                for (int i = 30; i > 23; i--) newList.Add(this.httpHandler.ItemDetailInfo.TimeList[i]);
                for (int i = 52; i > 30; i--) newList.Add(this.httpHandler.ItemDetailInfo.TimeList[i]);
                for (int i = 23; i > -1; i--) newList.Add(this.httpHandler.ItemDetailInfo.TimeList[i]);


                this.httpHandler.ItemDetailInfo.TimeList = newList;

                foreach (EntityItemTimeInfo info in this.httpHandler.ItemDetailInfo.TimeList)
                {
                    ListViewItem lvi = new ListViewItem(new string[5]);
                    lvi.SubItems[0].Text = info.Label;
                    lvi.SubItems[1].Text = info.Volume;
                    lvi.SubItems[2].Text = info.AvrPrice;
                    lvi.SubItems[3].Text = info.LowPrice;
                    lvi.SubItems[4].Text = info.HighPrice;

                    this.listView1.Items.Add(lvi);
                }
                //this.statusStrip1.Text = "세부정보 다운로드 완료..";
            }
            catch (Exception ee)
            {
                //this.statusStrip1.Text = "세부정보 다운로드 실패..";
                MessageBox.Show(ee.ToString());
            }



        }
    }
}

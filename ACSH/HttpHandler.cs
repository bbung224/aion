using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace ACSH
{
    /*
     * 384953e60f680dc83c29f924707284ac_88
http://static.plaync.co.kr/powerbook/aion/item/384953e60f680dc83c29f924707284ac_88.jpg
마석: 물리 치명타 +17 / 생명력 +47
60레벨
24
3,100,000
5,499,999
0
     * 
     * //http://aion.plaync.com/live/vendor/searchResult?serverID=2&race=1&name=%EB%AC%BC%EB%A6%AC%EC%B9%98%EB%AA%85%ED%83%80&page=4&category=&quality=63&from=1&to=200&exact=0&sort=rank%2Clevel-dec%2Cquality-dec
     */
    public class EntitySearchResultItem
    {
        public string ID;
        public string ImageUrl;
        public string Name;
        public string Level;
        public string Count;
        public string LowPrice;
        public string HighPrice;
        public string CommentCount;

        public string GetDebug()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name: " + Name);
            sb.AppendLine("ID: " + ID);
            sb.AppendLine("ImageUrl: " + ImageUrl);
            sb.AppendLine("Level: " + Level);
            sb.AppendLine("Count: " + Count);
            sb.AppendLine("LowPrice: " + LowPrice);
            sb.AppendLine("HighPrice: " + HighPrice);
            sb.AppendLine("CommentCount: " + CommentCount);
            return sb.ToString();
        }
    }

    public class EntityItemDetailInfo
    {
        public EntitySearchResultItem SearchResult;
        public string Name;
        public string CountStr;
        public string[] PriceStr;
        public List<EntityItemTimeInfo> TimeList;
        public EntityItemDetailInfo(EntitySearchResultItem sr)
        {
            this.SearchResult = sr;
            CountStr = "";
            PriceStr = new string[5];
            TimeList = new List<EntityItemTimeInfo>();
        }

        public void AddTimeListVolume(string s1, string s2)
        {
            foreach (EntityItemTimeInfo eiti in TimeList)
            {
                if (eiti.Label == s1)
                {
                    eiti.Volume = s2;
                    return;
                }
            }
            EntityItemTimeInfo newInfo = new EntityItemTimeInfo();
            newInfo.Label = s1;
            newInfo.Volume = s2;
            this.TimeList.Add(newInfo);
        }

        public void AddTimeListPrice(string s1, string low, string high, string avr) {
            foreach (EntityItemTimeInfo eiti in TimeList)
            {
                if (eiti.Label == s1)
                {
                    eiti.LowPrice = low;
                    eiti.HighPrice = high;
                    eiti.AvrPrice = avr;
                    return;
                }
            }
        }
    }

    public class EntityItemTimeInfo
    {
        public string Label;
        public string Volume;
        public string LowPrice;
        public string HighPrice;
        public string AvrPrice;
    }

    public class HttpHandler
    {
        private MainForm main;
        public List<EntitySearchResultItem> ResultList;
        public EntityItemDetailInfo ItemDetailInfo;
        private int ServerID = 2;
        private int Race = 1;

        public HttpHandler(MainForm main)
        {
            this.main = main;
            ResultList = new List<EntitySearchResultItem>();
        }


        private string SearchTextToURLEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        public bool DownloadSearchHtml(string str)
        {
            ResultList.Clear();
            string searchStr = SearchTextToURLEncode(str);

            int page = 1;
            int totalPage = DownloadSearchHtml(str, page);
            if (totalPage == -1) return false;
            for(int i = 2 ; i <= totalPage ; i++)
            {
                DownloadSearchHtml(str, i);
            }
            return true;
        }

        // 다음페이지가 있으면 true
        private int DownloadSearchHtml(string str, int page)
        {
            string searchStr = SearchTextToURLEncode(str);
            string url = "http://aion.plaync.com/live/vendor/searchResult?serverID="+ ServerID + "&race=" + Race + "&name=" + searchStr + "&page=" + page + "&category=&quality=63&from=1&to=200&exact=0&sort=rank%2Clevel-dec%2Cquality-dec";

            byte[] data = HttpDownload(url, null);
            if (data == null) return 0;
            String result = GetKorString(data);
            //main.AppendDebug(result);
            this.ParseSearchResult(result);

            if (page == 1)
            {
                int i1 = result.IndexOf("<div class=\"paging\">");
                if (i1 == -1) return -1;
                int i2 = result.IndexOf("</div>", i1);
                string pagingStr = Substring(result, i1, i2 + 6);
                string[] tmp = pagingStr.Split('\n');
                return (tmp.Length - 2);
            }
            else
            {
                return 0;
            }

        } 

        private void ParseSearchResult(string str)
        {
            int index = str.IndexOf("<div id=\"search_result_list\">");
            int startIndex = 0;
            int endIndex = 0;
            while(true)
            {
                startIndex = str.IndexOf("<div class=\"item\">", startIndex);
                if (startIndex == -1) break;
                endIndex = str.IndexOf("<div class=\"item\">", startIndex+1);
                if (endIndex == -1) endIndex = str.Length - 1;
                string itemStr = Substring(str, startIndex, endIndex - 1);
                ParseSearchResultItem(itemStr);
                startIndex = endIndex;
            }
        }

        private void ParseSearchResultItem(string str)
        {
            EntitySearchResultItem item = new EntitySearchResultItem();

            string s1 = "itemLayer[";
            int i1 = str.IndexOf(s1);
            i1 += s1.Length;
            int i2 = str.IndexOf("]", i1);
            item.ID = Substring(str, i1, i2 - 1);

            s1 = "img src=\"";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("\"", i1);
            item.ImageUrl = Substring(str, i1, i2 - 1);

            s1 = "vendorDetail["+ item.ID;
            i1 = str.IndexOf(s1, i2);
            s1 = "<strong>";
            i1 = str.IndexOf(s1, i1);
            i1 += s1.Length;
            i2 = str.IndexOf("</strong>", i1);
            item.Name = Substring(str, i1, i2 - 1);

            s1 = "<li>";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("<", i1);
            item.Level = Substring(str, i1, i2 - 1).Trim();

            s1 = item.ID + "]\">";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("<", i1);
            item.Count = Substring(str, i1, i2 - 1).Trim();

            s1 = "<strong>";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("<", i1);
            item.LowPrice = Substring(str, i1, i2 - 1).Trim();

            s1 = "<strong>";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("<", i1);
            item.HighPrice = Substring(str, i1, i2 - 1).Trim();

            s1 = item.ID + "]\">";
            i1 = str.IndexOf(s1, i2);
            i1 += s1.Length;
            i2 = str.IndexOf("<", i1);
            item.CommentCount = Substring(str, i1, i2 - 1).Trim();

            //this.main.AppendDebug(item.GetDebug());
            ResultList.Add(item);
        }

        public void DownloadItemDetail(EntitySearchResultItem item)
        {
            //http://aion.plaync.com/live/vendor/searchDetail?serverID=2&race=1&&itemNameID=384953e60f680dc83c29f924707284ac_88
            ItemDetailInfo = new EntityItemDetailInfo(item);
            string url = "http://aion.plaync.com/live/vendor/searchDetail?serverID=" + ServerID + "&race=" + Race + "&itemNameID=" + item.ID;
            byte[] data = HttpDownload(url, null);
            if (data == null) return;
            String result = GetKorString(data);
            ProcessDetailString(result);
        }

        
        private void ProcessDetailString(string str)
        {
            int i1 = str.IndexOf("strong class=\"item_");
            i1 = str.IndexOf(">", i1) + 1;
            int i2 = str.IndexOf("</strong", i1);
            ItemDetailInfo.Name = Substring(str, i1, i2 - 1);

            i1 = str.IndexOf("<span class=\"number\">", i2+1);
            i1 = str.IndexOf("<", i1 + 1);
            i2 = str.IndexOf("</span>", i1) - 1;
            string s1 = Substring(str, i1, i2 - 1);

            //건수
            int i3 = 0;
            while(true) {
                i3 = s1.IndexOf(".gif", i3+1);
                if (i3 == -1) break;
                ItemDetailInfo.CountStr += s1[i3 - 1];
            }

            for (int i = 0; i < 5; i++)
            {
                i1 = str.IndexOf("<span class=\"ttip\"", i2);
                i1 = str.IndexOf("title=\"", i1);
                i1 = str.IndexOf("\"", i1) + 1;
                i2 = str.IndexOf("\"", i1) - 1;
                s1 = Substring(str, i1, i2);
                ItemDetailInfo.PriceStr[i] = s1;
            }

            for (int i = 0; i < 6; i++)
            {
                i1 = str.IndexOf("setDataXML", i2);
                i1 = str.IndexOf("\"", i1) + 1;
                i2 = str.IndexOf("\"", i1) - 1;
                s1 = Substring(str, i1, i2);
                if (i < 3) ProcessDetailStringVolume(s1);
                else ProcessDetailStringPrice(s1);
            }
            i3 = 0; 

        }

        private void ProcessDetailStringVolume(string str)
        {
            int i1 = 0, i2 = 0;
            string s1 = null, s2 = null;
            while (true)
            {
                i1 = str.IndexOf("set label", i2);
                if (i1 == -1) break;
                i1 = str.IndexOf("'", i1) + 1;
                i2 = str.IndexOf("'", i1) - 1;
                s1 = Substring(str, i1, i2);

                i1 = str.IndexOf("value", i2);
                i1 = str.IndexOf("'", i1) + 1;
                i2 = str.IndexOf("'", i1) - 1;
                if (i2 < i1) s2 = "";
                else s2 = Substring(str, i1, i2);
                ItemDetailInfo.AddTimeListVolume(s1, s2);
            }
        }

        private void ProcessDetailStringPrice(string str)
        {
            int i1 = 0, i2 = 0;
            string s1 = null, s2 = null;
            List<string>[] data = new List<string>[4];
            for (int i = 0; i < 4; i++) data[i] = new List<string>();

            i1 = str.IndexOf("<categories>", i2);
            i2 = str.IndexOf("</categories>", i1) + 1;
            s1 = Substring(str, i1, i2);
            int i3 = 0, i4 = 0;
            while (true)
            {
                i3 = s1.IndexOf("label=", i4);
                if (i3 == -1) break;
                i3 = s1.IndexOf("'", i3) + 1;
                i4 = s1.IndexOf("'", i3) - 1;
                s2 = Substring(s1, i3, i4);
                data[0].Add(s2);
            }
            for (int i = 1; i < 4; i++)
            {
                i1 = str.IndexOf("seriesName", i2);
                i2 = str.IndexOf("</dataset>", i1) + 1;
                s1 = Substring(str, i1, i2);
                i3 = 0; i4 = 0;
                while (true)
                {
                    i3 = s1.IndexOf("value", i4);
                    if (i3 == -1) break;
                    i3 = s1.IndexOf("'", i3) + 1;
                    i4 = s1.IndexOf("'", i3) - 1;
                    if (i4 < i3) s2 = "";
                    else s2 = Substring(s1, i3, i4);
                    data[i].Add(s2);
                }
            }

            if (data[0].Count == data[1].Count && data[0].Count == data[2].Count && data[0].Count == data[3].Count && data[1].Count == data[2].Count && data[1].Count == data[3].Count && data[2].Count == data[3].Count)
            {
                for (int i = 0; i < data[0].Count; i++)
                    ItemDetailInfo.AddTimeListPrice(data[0][i], data[1][i], data[2][i], data[3][i]);
            }
            else
            {
                // 오면 안된다.
            }
        }



        private string Substring(string str, int i1, int i2)
        {
            return str.Substring(i1, i2 - i1 + 1);
        }

        public static byte[] HttpDownload(String url, String listener)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            Stream responseStream = null;
            MemoryStream memoryStream = null;
            byte[] data = new byte[4096];

            try
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";
                httpRequest.Accept = "*/*";
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727)";     WebHeaderCollection requestHeader = httpRequest.Headers;
                requestHeader.Add("Accept-Language: ko");
                //requestHeader.Add("UA-CPU: x86");
                //requestHeader.Add("Accept-Encoding: gzip, deflate");
                //requestHeader.Add("Cookie: " + "JSESSIONID=B256CE3DC62BFEE9D5E8C4B7B405803D; NB=GU2DSMRYG43TGOJQ; NNB=VMDIISXO7NQES; npic=uRs4s8Aso1Ff3X1UjhgzHrWowSyOS83WgCLiFq0JtCtLoYj3TB5j+VXFX5vE7noKCA==; keyenable=1; news_font_size=16; nsr_acl=1; nvad_lc=LO,2915560500; page_uid=sVQve3E94NGssuZSQMGiussssv4-419507; _naver_usersession_=-WNuJOUPLUoAAAYPXo8AAAAl");



                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                responseStream = httpResponse.GetResponseStream();
                //if (listener != null) listener.DownloadStatusChanged(HttpDownloadStatus.DOWNLOAD_READY, null, null);
                int len = 0, totalLen = 0;
                memoryStream = new MemoryStream();
                while ((len = responseStream.Read(data, 0, data.Length)) != 0)
                {
                    memoryStream.Write(data, 0, len);
                    totalLen += len;
                    //if (listener != null) listener.DownloadStatusChanged(HttpDownloadStatus.DOWNLOADING, totalLen, null);
                }
                //if (listener != null) listener.DownloadStatusChanged(HttpDownloadStatus.DOWNLOAD_END, totalLen, null);
                return memoryStream.ToArray();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (memoryStream != null) memoryStream.Close();
                if (responseStream != null) responseStream.Close();
                if (httpResponse != null) httpResponse.Close();
            }
        }

        public static String GetKorString(byte[] data)
        {
            if (data == null) return null;
            //return Encoding.GetEncoding("euc-kr").GetString(data, 0, data.Length);
            //return Encoding.Default.GetString(data, 0, data.Length);
            return Encoding.UTF8.GetString(data, 0, data.Length);

        }
    }
}

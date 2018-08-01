using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.Web;
using System.IO;
using System.Threading;


namespace AionLogAnalyzer
{
    public class AionWebHandler
    {
        private MainForm mainForm;
        public SearchResult user1, user2;

        public AionWebHandler(MainForm main)
        {
            this.mainForm = main;
        }

        /**
         * 같은 계정인지 비교
         */
        public int CompareSameAccount(string s1, string s2)
        {
            string[] ss1 = s1.Split('-');
            if (ss1.Length != 2) return -1; // s1에 - 표시가 없다
            SearchResult s1Ret = GetID1(ss1[0], ss1[1]);

            string[] ss2 = s2.Split('-');
            if (ss2.Length != 2) return -2; // s1에 - 표시가 없다
            SearchResult s2Ret = GetID1(ss2[0], ss2[1]);

            s1Ret.GUID = GetGUID(s1Ret);
            s2Ret.GUID = GetGUID(s2Ret);

            this.user1 = s1Ret;
            this.user2 = s2Ret;

            return String.Compare(s1Ret.GUID, s2Ret.GUID);
        }


        public struct SearchResult
        {
            public string ID1;
            public string ID2;
            public string NickName;
            public string Level;
            public string ImgUrl;
            public string Server;
            public string Race;
            public string Class;
            public string Legion;
            public string Rank;
            public string GP;
            public string AP;
            public string GUID;
        }

        private SearchResult GetID1(string name, string serverName)
        {
            SearchResult ret = new SearchResult();
            int serverIndex = GetServerIndex(serverName);
            if (serverIndex == -1) return ret;
            string url = "http://m.aion.plaync.com/searches/characters?q=" + HttpUtility.UrlEncode(name) + "&sort=&server=" + serverIndex + "&race=&clazz=";

            byte[] data = HttpDownload(url, null);
            if (data == null) return ret;
            String result = GetKorString(data);

            int nextStart = 0;
            nextStart = result.IndexOf("\"signatureInfo\"");
            if (nextStart == -1) return ret;
            ret.ID1 = SubString(result, "characters", nextStart, '/', '\"', out nextStart);
            ret.NickName = SubString(result, "strong", nextStart, '>', '<', out nextStart);
            ret.Level = SubString(result, "em", nextStart, '>', '<', out nextStart);
            ret.ImgUrl = SubString(result, "img src", nextStart, '\"', '\"', out nextStart);
            ret.Server = SubString(result, "\"server\"", nextStart, '>', '<', out nextStart);
            ret.Race = SubString(result, "\"race\"", nextStart, '>', '<', out nextStart);
            ret.Class = SubString(result, "\"class\"", nextStart, '>', '<', out nextStart);
            ret.Legion = SubString(result, "\"legion\"", nextStart, '>', '<', out nextStart);
            ret.Rank = SubString(result, "\"gameInfo\"", nextStart, '>', '<', out nextStart).Trim();
            ret.GP = SubString(result, "\"gp\"", nextStart, '>', '<', out nextStart).Trim();
            ret.AP = SubString(result, "\"abysspoint\"", nextStart, '>', '<', out nextStart).Trim();


            return ret;
        }

        private String GetGUID(SearchResult sr)
        {
            string url = "http://aion.plaync.com/characters/" + sr.ID1;
            byte[] data = HttpDownload(url, null);
            if (data == null) return null;
            String result = GetKorString(data);

            ///characters/sns/cancelProposeFriend?friendGuid=9EE8C0F4-D15F-E011-9A06-E61F135E992F"
            int nextStart = 0;
            return SubString(result, "friendGuid", nextStart, '=', '\"', out nextStart);
        }

        private string SubString(string src, string parseClass, int start, char startC, char endC, out int nextStart)
        {
            nextStart = start + 1;
            int t1 = src.IndexOf(parseClass, start);
            if (t1 == -1) return null;
            t1 = src.IndexOf(startC, t1 + 1) + 1;
            int t2 = src.IndexOf(endC, t1) - 1;
            nextStart = t2 + 1;
            return src.Substring(t1, t2 - t1 + 1);

        }


        public int GetServerIndex(string serverName)
        {
            if (serverName == "고르고스") return 35;
            else if (serverName == "네자칸") return 3;
            else if (serverName == "라비린토스") return 30;
            else if (serverName == "레파르") return 44;
            else if (serverName == "루그부그") return 24;
            else if (serverName == "루미엘") return 8;
            else if (serverName == "마르쿠탄") return 10;
            else if (serverName == "메스람타에다") return 14;
            else if (serverName == "메이화링") return 21;
            else if (serverName == "바이젤") return 5;
            else if (serverName == "발데르") return 39;
            else if (serverName == "보탄") return 38;
            else if (serverName == "브리트라") return 16;
            else if (serverName == "비다르") return 41;
            else if (serverName == "사크미스") return 29;
            else if (serverName == "수에론") return 42;
            else if (serverName == "스파탈로스") return 31;
            else if (serverName == "시엘") return 1;
            else if (serverName == "아리엘") return 11;
            else if (serverName == "아스칼론") return 28;
            else if (serverName == "아스펠") return 12;
            else if (serverName == "아에기르") return 43;
            else if (serverName == "에레슈키갈") return 15;
            else if (serverName == "유스티엘") return 9;
            else if (serverName == "유클레아스") return 26;
            else if (serverName == "이스라펠") return 2;
            else if (serverName == "젠카카") return 22;
            else if (serverName == "지켈") return 4;
            else if (serverName == "챈가룽") return 18;
            else if (serverName == "카사카") return 23;
            else if (serverName == "카스토르") return 33;
            else if (serverName == "카이시넬") return 7;
            else if (serverName == "콰이링") return 19;
            else if (serverName == "크로메데") return 36;
            else if (serverName == "키도룬") return 20;
            else if (serverName == "타라니스") return 27;
            else if (serverName == "텔레마커스") return 32;
            else if (serverName == "토르") return 37;
            else if (serverName == "트리니엘") return 6;
            else if (serverName == "티아마트") return 17;
            else if (serverName == "파시메데스") return 25;
            else if (serverName == "페렌토") return 34;
            else if (serverName == "프레기온") return 13;
            return -1;
        }


        public static String GetKorString(byte[] data)
        {
            if (data == null) return null;
            //return Encoding.GetEncoding("euc-kr").GetString(data, 0, data.Length);
            //return Encoding.Default.GetString(data, 0, data.Length);
            return Encoding.UTF8.GetString(data, 0, data.Length);

        }

        private string SearchTextToURLEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
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
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727)"; WebHeaderCollection requestHeader = httpRequest.Headers;
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
    }
}

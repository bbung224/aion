using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace AionLogAnalyzer
{
    static class Debug
    {
        public static void Write(String str)
        {

        }
    }
    class Logic
    {
        private MainForm form = null;
        public static String AionPath = null;
        public static String AionLogFileName = null;


        public Logic(MainForm form)
        {
            this.form = form;
            //Initialize();
            //aionPath = "C:\\Program Files (x86)\\PlayNC\\AION_KOR";
            //aionLogFileName = aionPath + "\\Chat.log";
        }

        // 아이온 설치경로를 가져온다.
        // 추후 환경설정
        public void Initialize()
        {
            // 이건 윈도7 64bit 아이온 설치 경로입니다.
            string logpath = "";
            String str = null;
            int testServer = 0;
            try
            {
                testServer = Properties.Settings.Default.TestServer;
            }
            catch { }
            if (testServer != 2)
            {
                try
                {// windows7 64bit
                    str = "SOFTWARE\\Wow6432Node\\plaync\\aion_kor";
                    if (testServer == 1) str += "_test";
                    RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                    logpath = reg.GetValue("basedir").ToString();
                    reg.Close();
                }
                catch (Exception e)
                {
                    Debug.Write("Win32 Reg 읽는중:오류아님 - " + e.Message);
                    try
                    {// windows7 32bit, winxp 32bit
                        str = "SOFTWARE\\plaync\\aion_kor";
                        if (testServer == 1) str += "_test";
                        RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                        logpath = reg.GetValue("basedir").ToString();
                        reg.Close();
                    }
                    catch (Exception e1)
                    {
                        logpath = "";
                        Debug.Write(e1.Message);
                    }
                }
            }
            else
            {
                logpath = Properties.Settings.Default.UserFolder;
            }
            if (logpath != "")
            {
                Logic.AionPath = logpath;
                Logic.AionLogFileName = logpath + "\\Chat.log";
            }
        }

        public bool IsExistSystemCfg()
        {
            string strsystemcfg = Logic.AionPath + "\\system.cfg";
            return File.Exists(strsystemcfg);
        }

        // system.cfg 파일 제어. read한후 ~연산을 하면 일반 txt가 된다
        // 일반 txt로 system.cfg로 저장할경우 ~연산을 한다.
        private bool ConfigFileEncoding(string filepath1, string filepath2)
        {
            char chr;
            int bytes = 0;

            try
            {
                FileStream fsin = new FileStream(filepath1, FileMode.Open, FileAccess.Read);
                FileStream fsout = new FileStream(filepath2, FileMode.Create, FileAccess.Write);
                BinaryReader br = new BinaryReader(fsin, Encoding.ASCII);
                BinaryWriter bw = new BinaryWriter(fsout, Encoding.ASCII);

                //~연산이 필요없는 부분?
                byte[] temp = br.ReadBytes(33 + 96);
                bw.Write(temp);

                while (true)
                {
                    try
                    {
                        chr = (char)br.ReadByte();
                        if (chr != '\r' && chr != '\n')
                        {
                            chr = (char)~chr;
                        }
                        bytes++;
                        bw.BaseStream.WriteByte((byte)chr);
                    }
                    catch 
                    {
                        break;
                    }
                }

                br.Close();
                bw.Close();
                fsin.Close();
                fsout.Close();
            }
            catch 
            {
                return false;
            }
            return true;
        }

        // 로그파일 생성하면 true 
        public bool isAvailableLogFile()
        {
            char chr;
            int bytes = 0;
            string strsystemcfg = Logic.AionPath + "\\system.cfg";
            bool ret = false;
            try
            {
                FileStream fsin = new FileStream(strsystemcfg, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fsin, Encoding.ASCII);
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                
                //~연산이 필요없는 부분?
                byte[] temp = br.ReadBytes(33 + 96);
                bw.Write(temp);

                while (true)
                {
                    try
                    {
                        chr = (char)br.ReadByte();
                        if (chr != '\r' && chr != '\n')
                        {
                            chr = (char)~chr;
                        }
                        bytes++;
                        bw.Write((byte)chr);
                    }
                    catch 
                    {
                        break;
                    }
                }
                byte[] b = ms.ToArray();

                String str = Encoding.ASCII.GetString(b);
                if (str.IndexOf("g_chatlog") == -1) ret = false;
                else if ( str.IndexOf("g_chatlog = \"1\"") != -1) ret = true;
                else if ( str.IndexOf("g_chatlog = \"0\"") != -1) ret = false;
                else ret = false;

                br.Close();
                bw.Close();
                fsin.Close();
            }
            catch
            {
            }
            return ret;
        }

        // cfg 파일을 수정한다. 시작일경우 log=1로
        public void ConfigFilePatch(bool isStart)  
        {
            try
            {
                string strsystemcfg = Logic.AionPath + "\\system.cfg";
                string strtemp = Logic.AionPath + "\\temp.txt";
                string strtemp2 = Logic.AionPath + "\\temp2.txt";

                //system.cfg -> temp.txt 생성
                ConfigFileEncoding(strsystemcfg, strtemp);

                if (File.Exists(strsystemcfg))
                {
                    File.Delete(strsystemcfg);  //  디코딩이 끝나면 system.cfg파일 삭제
                }

                StreamReader sr = new StreamReader(strtemp);
                FileStream fs = new FileStream(strtemp2, FileMode.Append);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII);
                string str = null;

                while ((str = sr.ReadLine()) != null)
                {
                    if (str.Contains("g_chatlog") | str.Contains("g_open_aion_web") |
                        str.Contains("log_Verbosity") | str.Contains("log_FileVerbosity"))
                    {
                    }
                    else
                    {
                        writer.Write(str);
                        writer.WriteLine();
                    }
                }
                if (isStart)
                {
                    writer.Write("g_chatlog = \"1\"");
                    writer.WriteLine();
                    writer.Write("g_open_aion_web = \"0\"");
                    writer.WriteLine();
                    writer.Write("log_Verbosity = \"1\"");
                    writer.WriteLine();
                    writer.Write("log_FileVerbosity = \"1\"");
                    writer.WriteLine();
                }
                else
                {
                    writer.Write("g_chatlog = \"0\"");
                    writer.WriteLine();
                    writer.Write("g_open_aion_web = \"1\"");
                    writer.WriteLine();
                    writer.Write("log_Verbosity = \"0\"");
                    writer.WriteLine();
                    writer.Write("log_FileVerbosity = \"0\"");
                    writer.WriteLine();
                }
                writer.Close();
                fs.Close();
                sr.Close();

                // temp2.txt -> system.cfg
                ConfigFileEncoding(strtemp2, strsystemcfg);
                if (File.Exists(strtemp))
                {
                    File.Delete(strtemp);
                    File.Delete(strtemp2);
                }
                // Chat.log도 삭제한다
                if (File.Exists(Logic.AionLogFileName))
                {
                    try
                    {
                        File.Delete(Logic.AionLogFileName);
                    }
                    catch { }
                }
                // 파일을 생성하여 쓰레드가 대기하도록 한다.
                
                if (isStart)
                {
                    FileStream ff = new FileStream(Logic.AionLogFileName, FileMode.Create);
                    StreamWriter ww = new StreamWriter(ff, System.Text.Encoding.ASCII);
                    ww.WriteLine();
                    ww.Close();
                    ff.Close();
                }
                // 2013-05-21
                // system.ovr
                string ovrFile = Logic.AionPath + "\\system.ovr";
                if (File.Exists(ovrFile))
                {
                    File.Delete(ovrFile);
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
    }


    public class Util
    {
        public static String GetSkillName(String str)
        {
            String[] tmp = str.Split(' ');
            String ret = "";
            foreach (string s in tmp)
            {
                if (s == "I" || s == "II" || s == "III" || s == "IV" || s == "V" || s == "VI" || s == "VII" || s == "VIII" || s == "IX" || s == "X")
                {
                    ret = ret + " " + s;
                    return ret.Trim(); ;
                }
                else
                {
                    ret = ret + " " + s;
                }
            }
            return ret.Trim(); ;
        }
    }
}

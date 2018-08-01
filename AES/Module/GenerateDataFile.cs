using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Threading;

namespace AES
{
    public enum GDFEvent {
        아이온폴더못찾음, 폴더삭제실패, 준비, ZIP생성, 압축해제, 변환, 종료
    }
    public class GDFEventArgs : EventArgs
    {
        public GDFEvent EventType;
        public string Args;
        public GDFEventArgs(GDFEvent EventType)
        {
            this.EventType = EventType;
        }
        public GDFEventArgs(GDFEvent EventType, string args)
        {
            this.EventType = EventType;
            this.Args = args;
        }
    }

    public class GenerateDataFile
    {
        private MainForm form;
        public event EventHandler GDFEventHandler;

        //2013-07-29 client_strings_item3 추가
        public static string[] fileListStrings = { "client_strings_item.xml", "client_strings_item2.xml", "client_strings_item3.xml", "client_strings_quest.xml", "client_strings_skill.xml" }; //strings
        public static string[] fileListSkills = { "client_skills.xml" };
        public static string[] fileListItems = { "client_items_etc.xml", "client_items_armor.xml", "client_items_misc.xml", "client_item_random_option.xml", "polish_bonus_setlist.xml", "client_setitem.xml" };
        public static string[] fileListPC = { "client_titles.xml" };

        public static string appPath = null;
        public static string dataPath = null;
        public static string tempPath = null;

        public GenerateDataFile(MainForm form)
        {
            this.form = form;
            appPath = AppDomain.CurrentDomain.BaseDirectory;
            dataPath = appPath + "items";
            tempPath = appPath + "temp";
        }

        // 데이터 파일이 이미 있는지 확인하자
        public bool ExistDataFile()
        {
            DirectoryInfo directory = new DirectoryInfo(dataPath);
            if (directory.Exists == false) return false;
            string[][] xmlFileList = { fileListStrings, fileListSkills, fileListItems, fileListPC };

            //bxml -> xml
            for (int i = 0; i < xmlFileList.Length; i++)
            {
                for (int j = 0; j < xmlFileList[i].Length; j++)
                {
                    if (File.Exists(dataPath + "\\" + xmlFileList[i][j]) == false) return false;
                }
            }
            return true;
        }


        private Thread generateThread;
        private bool isTestServer = false;
        public void GenerateData(bool isTestServer)
        {
            this.isTestServer = isTestServer; 
            generateThread = new Thread(new ThreadStart(ThreadMethod));
            generateThread.Start();
        }

        public void Stop()
        {
            if (generateThread != null) generateThread.Abort();
        }


        // 데이터 파일을 생성한다
        public void ThreadMethod()
        {
            string[] dirs = { dataPath, tempPath };
            for (int i = 0; i < dirs.Length; i++)
            {
                try
                {
                    if (Directory.Exists(dirs[i])) Directory.Delete(dirs[i], true);
                }
                catch// (Exception e)
                {
                    GDFEventHandler(this, new GDFEventArgs(GDFEvent.폴더삭제실패));
                    return;
                }
                finally
                {
                    Directory.CreateDirectory(dirs[i]);
                }
            }


            while (Directory.Exists(dirs[0]) == false)
            {
                Directory.CreateDirectory(dirs[0]);
                Thread.Sleep(100);
            }
            while (Directory.Exists(dirs[1]) == false)
            {
                Directory.CreateDirectory(dirs[1]);
                Thread.Sleep(100);
            }


            string aionPath = GetAionPath();
            if (appPath == "")
            {
                GDFEventHandler(this, new GDFEventArgs(GDFEvent.아이온폴더못찾음));
            }
            else
            {
                this.GDFEventHandler(this, new GDFEventArgs(GDFEvent.준비));
                aionPath += "\\data";
                string[] originalFileList = { aionPath + "\\Strings\\Strings.pak", aionPath + "\\skills\\skills.pak", aionPath + "\\Items\\items.pak", aionPath + "\\PC\\pc.pak" };
                foreach (string file in originalFileList)
                {
                    ConvertPakToZip(file);
                    this.GDFEventHandler(this, new GDFEventArgs(GDFEvent.ZIP생성, file));
                }
                
                //xml을 푼다
                string[] zipFileList = { tempPath + "\\Strings.zip", tempPath + "\\skills.zip", tempPath + "\\items.zip", tempPath + "\\pc.zip" };
                string[][] extractFileList = { fileListStrings, fileListSkills, fileListItems, fileListPC };

                for(int i = 0 ; i < zipFileList.Length ; i++)
                {
                    ExtractFile(zipFileList[i], extractFileList[i]);
                    this.GDFEventHandler(this, new GDFEventArgs(GDFEvent.압축해제, zipFileList[i]));
                }

                //bxml -> xml
                for (int i = 0; i < extractFileList.Length; i++)
                {
                    for (int j = 0; j < extractFileList[i].Length; j++)
                    {
                        ConvertBxml2xml(tempPath + "\\" + extractFileList[i][j] , dataPath + "\\" + extractFileList[i][j]);
                        this.GDFEventHandler(this, new GDFEventArgs(GDFEvent.변환, extractFileList[i][j]));
                    }
                }

                Directory.Delete(dirs[1], true);
                this.GDFEventHandler(this, new GDFEventArgs(GDFEvent.종료));
            }

        }

        private void ConvertBxml2xml(string src, string des)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = appPath+"file\\Gibbed.Aion.ConvertXml.exe";
                process.StartInfo.Arguments = "\"" + src + "\"" + " " + "\"" + des + "\"";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
            }
            catch { }

        }


        private void ExtractFile(String zip, String[] targetFiles)
        {
            if (!File.Exists(zip))
            {
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zip)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    bool bExist = false;
                    foreach (string target in targetFiles)
                    {
                        if (theEntry.Name == target)
                        {
                            bExist = true;
                            break;
                        }
                    }
                    if (bExist == false) continue;

                    string fileName = tempPath + "\\" + theEntry.Name;

                    if (fileName != String.Empty)
                    {
                        try
                        {
                            using (FileStream streamWriter = File.Create(fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    //SetProgress(s.Position);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        catch// (Exception e)
                        {

                        }
                    }
                }
            }
        }

        private void ConvertPakToZip(string src)
        {
            Process pak2zipProcess = new Process();
            pak2zipProcess.StartInfo.FileName = appPath+"file\\pak2zip.exe";
            int index = src.LastIndexOf("\\");
            string sub = src.Substring(index + 1);
            string[] subs = sub.Split('.');
            string target = tempPath + "\\" + subs[0] + ".zip";

            pak2zipProcess.StartInfo.Arguments = "\"" + src + "\"" + " " + "\"" + target + "\"";
            //pak2zipProcess.StartInfo.CreateNoWindow = false;
            pak2zipProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pak2zipProcess.StartInfo.UseShellExecute = true;
            pak2zipProcess.StartInfo.Verb = "runas";
            pak2zipProcess.Start();
            pak2zipProcess.WaitForExit();
        }

        public string GetAionPath()
        {
            string logpath = "";
            String str = null;

            try
            {// windows7 64bit
                str = "SOFTWARE\\Wow6432Node\\plaync\\aion_kor";
                if (isTestServer) str += "_test";
                RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                logpath = reg.GetValue("basedir").ToString();
                reg.Close();
            }
            catch// (Exception e)
            {
                try
                {// windows7 32bit, winxp 32bit
                    str = "SOFTWARE\\plaync\\aion_kor";
                    if (isTestServer) str += "_test";
                    RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                    logpath = reg.GetValue("basedir").ToString();
                    reg.Close();
                }
                catch// (Exception e1)
                {
                    logpath = "";
                }
            }

            //테섭
            if (logpath == "")
            {
                try
                {// windows7 64bit
                    str = "SOFTWARE\\Wow6432Node\\plaync\\aion_kor_test";
                    RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                    logpath = reg.GetValue("basedir").ToString();
                    reg.Close();
                }
                catch// (Exception e)
                {
                    try
                    {// windows7 32bit, winxp 32bit
                        str = "SOFTWARE\\plaync\\aion_kor_test";
                        RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                        logpath = reg.GetValue("basedir").ToString();
                        reg.Close();
                    }
                    catch// (Exception e1)
                    {
                        logpath = "";
                    }
                }
            }

            return logpath;
        }

    }
}

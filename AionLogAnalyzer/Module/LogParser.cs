//#define USING_FILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


namespace AionLogAnalyzer
{
    enum EnumStopBy
    {
        ByUser, ByTimerThread, ByInstanceDungeon, ByFormClosed
    }

    class LogParser
    {
        private MainForm mainForm;
        private bool bRunning = false;

        private FileStream fileStream = null;
        private StreamReader streamReader = null;
        private Thread worker = null;
        private object lockObject = new object();

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;
        public event EventHandler FileNotFound;
        public event EventHandler tempEvent;
      

        public delegate void DamageInflictedEventHandler(object sender, PlayerDamageEventArgs e);
        public delegate void ChatCommandEventHandler(object sender, ChatCommandEventArgs e);
        public delegate void MyStatusEventHandler(object sender, MyStatusEventArgs e);
        public delegate void InstanceDungeonEventHandler(object sender, InstanceDungeonEventArgs e); 
        public delegate void RecoverEventHander(object sender, RecoverEventArgs e); 

        public event DamageInflictedEventHandler DamageInflicted;
        public event ChatCommandEventHandler ChatCommandEvent;
        public event MyStatusEventHandler MyStatusEvent;
        public event InstanceDungeonEventHandler InstanceDungeonEvent;
        public event RecoverEventHander RecoverEvent;

        /** 정규식 **/
        private string timeStampRegex;
        private Regex 타인의채팅Regex;
        private Regex 파티지원포스지원Regex, 통합파티지원포스지원Regex;
        private Regex 자신의채팅Regex;
        private Regex 지역채널에입장했습니다Regex;


        private Regex 대미지를줬습니다Regex;
        private Regex 대미지를받았습니다Regex, 사용한영향으로대미지를받았습니다Regex, 공격이반사되어대미지를받았습니다Regex;

        private Regex 효과로회복했습니다Regex, 추가효과의효과로회복했습니다Regex, 사용해회복했습니다Regex, 사용한영향으로회복됐습니다Regex, 방법없이회복했습니다Regex, 속도를회복했습니다Regex;
        private Regex 만큼회복됐습니다Regex, 만큼회복했습니다Regex;

        private Regex 사용한영향으로상태가됐습니다Regex, 사용한영향으로기타등등Regex, 벗어났습니다Regex;
        private Regex 효과가발동했습니다Regex;


        private Regex 사용해상태가됐습니다Regex;
        private Regex 내스킬을써서효과가발생했습니다Regex;
        private Regex 사망했습니다Regex;
        private Regex 경험치를얻었습니다Regex; //2013-05-24

        private Regex 삼자가자신의무엇가를변경했습니다Regex;

        private Regex 무시Regex, 감정표현Regex, 명령표현Regex;


        public static String DefaultMyname = "<당신>";
        public static String Noname = "<미분류>";
        public static String Myname = DefaultMyname;
        public static bool OneClassFlag = Properties.Settings.Default.OneClassFlag;
        //public static String Version = "0.1.0.0";
        public static bool AutoSortFlag = Properties.Settings.Default.AutoSortFlag;
        public static bool NormalStartFlag = true;
        public static EnumStopBy StopByWhy = EnumStopBy.ByUser;

        public LogParser(MainForm mainForm)
        {
            this.mainForm = mainForm;
            Init();
        }

        private void Init()
        {
            timeStampRegex = "(?<time>[^a-zA-Z]+) : ";

            //2013.09.27 22:03:05 : [charname:소주빵;0.6275 1.0000 0.6275] 님의 귓속말: 65레벨 호법성 [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCMyFSf2E6oRjO/ARkAPeSAA==] [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCBFIJJAvhQDR0YvSr04J+Hw==]
            //2013.09.27 22:05:35 : [charname:소주빵;0.6275 1.0000 0.6275] 님의 귓속말: 65레벨 호법성 [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCU2f0SPAFix1Al20lxc5xrA==] [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCb1i817SotrLkxDzVpR19Tg==]
            파티지원포스지원Regex = new Regex(timeStampRegex + "\\[charname:(?<name>.+);(?<ignore>.+) 님의 귓속말: (?<level>.+) (?<class>.+) \\[cmd:(?<ignore2>.+) \\[cmd:(?<ignore3>.+)", RegexOptions.Compiled);

            ////2013.09.28 19:13:13 : 양맥-이스라펠 님의 귓속말: 65레벨 정령성 [cmd:QjDMbqN8fEG9MKRVzO8PGEVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tC5MwvBuvh5IrzUzKexwiXyw==] [cmd:QjDMbqN8fEG9MKRVzO8PGEVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCxTXqOAN7CGj3YfYHyAkDrQ==]
            통합파티지원포스지원Regex = new Regex(timeStampRegex + "(?<name>.+) 님의 귓속말: (?<level>.+) (?<class>.+) \\[cmd:(?<ignore2>.+) \\[cmd:(?<ignore3>.+)", RegexOptions.Compiled);
                
            타인의채팅Regex = new Regex("\\[charname:", RegexOptions.Compiled);
            //chatRegex = new Regex(timeStampRegex + "\\[charname:(?<name>.+);(?<ignore>.+): (?<chat>.+)", RegexOptions.Compiled);
            자신의채팅Regex = new Regex(timeStampRegex + "(?<name>.+): (?<chat>.+)", RegexOptions.Compiled);

            지역채널에입장했습니다Regex = new Regex(timeStampRegex + "(?<where>.+) 지역 채널에 입장했습니다", RegexOptions.Compiled);

            try
            {

                //대미지를줬습니다Regex = new Regex(timeStampRegex + "(치명타! )?((?<name>.+)[이가] )?(((?<skill>.+)[을를] 사용해 )|공격을 반사하여 )?(?<target>.+)(에게|[이가]) (?<damage>[^a-zA-Z]+)의 (|치명적인 )대미지를 (줬습니다|주고 문양 각인 효과가 발생됐습니다|(받고|입고) 일부 강화 마법이 제거됐습니다)", RegexOptions.Compiled);

                //2013-03-30 문약 각인 효가가 발생됐습니다 -> 발생했습니다.
                // 내 로그는 발생했습니다. 타인의 로그는 발생했습니다.
                //2013-03-30
                //2013.03.30 14:10:14 : 치명타! 맹수의 송곳니 II을 사용해 훈련용 허수아비에게 3,121의 대미지를 주고 문양 각인 효과가 발생했습니다. 
                //2013.03.30 14:23:53 : 치명타! 시야랑이 맹수의 송곳니 II을 사용해 훈련용 허수아비에게 3,153의 대미지를 주고 문양 각인 효과가 발생됐습니다.

                대미지를줬습니다Regex = new Regex(timeStampRegex + "(치명타! )?((?<name>.+)[이가] )?(((?<skill>.+)[을를] 사용해 )|공격을 반사하여 )?(?<target>.+)(에게|[이가]) (?<damage>[^a-zA-Z]+)의 (|치명적인 )대미지를 (줬습니다|주고 문양 각인 효과가 발생|(받고|입고) 일부 강화 마법이 제거됐습니다)", RegexOptions.Compiled);

                대미지를받았습니다Regex = new Regex(timeStampRegex + "(치명타! )?((?<name>.+)[이가] )?((?<from>.+)에게서 |(?<skill>.+)의 효과로 )(?<damage>[^a-zA-Z]+)의 (치명적인 )?((중독|출혈) )?대미지를 받았습니다", RegexOptions.Compiled);

                사용한영향으로대미지를받았습니다Regex = new Regex(timeStampRegex + "(치명타! )?(?<name>.+)[이가] 사용한 (?<skill>.+)의 영향으로 (?<damage>[^a-zA-Z]+)의 (치명적인 )?대미지를 (받았습니다|받고 일부 강화 마법이 제거됐습니다)", RegexOptions.Compiled);

                공격이반사되어대미지를받았습니다Regex = new Regex(timeStampRegex + "(?<name>.+)에 대한 공격이 반사되어 (?<damage>[^a-zA-Z]+)의 대미지를 받았습니다", RegexOptions.Compiled);

                효과로회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<skill>.+)의 효과로 (?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);

                추가효과의효과로회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<skill>.+)추가 효과의 효과로 (?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);

                사용해회복했습니다Regex = new Regex(timeStampRegex + "(?<name>.+)[이가] (?<skill>.+)[을를] 사용해 ((?<whom>.+)[이가] )?(?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);

                사용한영향으로회복됐습니다Regex = new Regex(timeStampRegex + "(?<name>.+)[이가] 사용한 (?<skill>.+)의 영향으로 (?<amount>[^a-zA-Z]+)의 (?<what>.+)[이가] 회복됐습니다", RegexOptions.Compiled);

                방법없이회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);

                속도를회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<what>.+)[을를이가] 회복[했|됐]습니다", RegexOptions.Compiled);

                만큼회복됐습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[을를] 사용해 (?<amount>[^a-zA-Z]+)만큼 (?<what>.+)[이가] 회복됐습니다", RegexOptions.Compiled);

                만큼회복했습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[을를] 사용해 (?<name>.+)[이가] (?<amount>[^a-zA-Z]+)만큼 (?<what>.+)[을를] 회복했습니다", RegexOptions.Compiled);


                사용한영향으로상태가됐습니다Regex = new Regex(timeStampRegex + "(치명타! )?(?<name>.+)[이가] 사용한 (?<skill>.+)의 영향으로 ((?<target>.+)[이가] )?(?<debuff>.+)상태가 됐습니다", RegexOptions.Compiled);

                사용한영향으로기타등등Regex = new Regex(timeStampRegex + "(치명타! )?(?<name>.+)[이가] 사용한 (?<skill>.+)의 영향으로 (?<debuff>.+)니다", RegexOptions.Compiled);


                벗어났습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?((?<debuff>.+) 상태에서 벗어났습니다|(?<debuff>.+)[을를] 회복했습니다)", RegexOptions.Compiled);

                사용해상태가됐습니다Regex = new Regex(timeStampRegex + "(치명타! )?((?<name>.+)[이가] )?(?<skill>.+)[을를] 사용해 ((?<target>.+)[이가] )?(?<debuff>.+) 상태가 됐습니다", RegexOptions.Compiled);

                내스킬을써서효과가발생했습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[을를] 사용해 ((?<target>.+)에게 )?(?<what>.+)[이가] 발생했습니다", RegexOptions.Compiled);

                효과가발동했습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[이가] 발동했습니다", RegexOptions.Compiled);

                사망했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<ignore>.+)?사망했습니다", RegexOptions.Compiled);

                경험치를얻었습니다Regex = new Regex(timeStampRegex + "(?<target>.+)에게 (?<amount>[^a-zA-Z]+)만큼의 경험치를 얻었습니다", RegexOptions.Compiled);


                삼자가자신의무엇가를변경했습니다Regex = new Regex(timeStampRegex + "(?<name>.+)[이가] (?<skill>.+)[을를] 사용해 (((?<target>.+)의 (?<what>.+)[을를] 변경했습니다)|(자신에게 영향을 줬습니다))", RegexOptions.Compiled);



                무시Regex = new Regex(timeStampRegex + "(?<ignore>.+) (막았습니다|소환했습니다|차단했습니다|저항했습니다|감소했습니다|지속적인 대미지 효과를 줬습니다|변동됐습니다|밀려났습니다|피했습니다|변신했습니다|발생됐습니다|넘어졌습니다|대신 받았습니다|감소됐습니다|감소하였습니다|사용합니다|쓰러트렸습니다|사용을 시작했습니다|사망했습니다|충돌했습니다|스킬을 발동합니다|스킬을 사용했습니다|발생합니다|로그오프했습니다|귀환을 사용했습니다|효과를 줬습니다|효과를 받았습니다|증가됐습니다)", RegexOptions.Compiled);

                명령표현Regex = new Regex(timeStampRegex + "(?<name>.+) 님이 ((?<target>.+) 님을 향해 )?고개를 끄덕입니다", RegexOptions.Compiled);

                감정표현Regex = new Regex(timeStampRegex + "(?<ignore>.+) (춥니다|웃습니다|끄덕입니다|고마워합니다|자랑합니다|웁니다|아니라고 합니다|깜짝 놀랍니다|입을 내밉니다)", RegexOptions.Compiled);
            }
            catch 
            {
            }
        }

        private void ParseLine(string line)
        {
            //bool matched = false;

            try
            {
                if (String.IsNullOrEmpty(line))
                {
                    return;
                }
                MatchCollection matches;//, matches2;

                //2013.09.27 22:03:05 : [charname:소주빵;0.6275 1.0000 0.6275] 님의 귓속말: 65레벨 호법성 [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCMyFSf2E6oRjO/ARkAPeSAA==] [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCBFIJJAvhQDR0YvSr04J+Hw==]
                //2013.09.27 22:05:35 : [charname:소주빵;0.6275 1.0000 0.6275] 님의 귓속말: 65레벨 호법성 [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCU2f0SPAFix1Al20lxc5xrA==] [cmd:NhkYrLsNIUzA4S2H8bKZ80VRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCb1i817SotrLkxDzVpR19Tg==]
                //2013.09.27 23:13:44 : [charname:양주오빠;0.6275 1.0000 0.6275] 님의 귓속말: 65레벨 마도성 [cmd:PRsw2mUTV6+xEJEbzsUmjUVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tC+rENLG7oLuN2jKnfHei2Cg==] [cmd:PRsw2mUTV6+xEJEbzsUmjUVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCkf5+HgnjLItxN1XZDgINPQ=
                //2013.09.28 19:13:13 : 양맥-이스라펠 님의 귓속말: 65레벨 정령성 [cmd:QjDMbqN8fEG9MKRVzO8PGEVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tC5MwvBuvh5IrzUzKexwiXyw==] [cmd:QjDMbqN8fEG9MKRVzO8PGEVRg5mfKVnkcIKURP7j+0JFUYOZnylZ5HCClET+4/tCxTXqOAN7CGj3YfYHyAkDrQ==]
                //파티지원포스지원Regex = new Regex(timeStampRegex + "\\[charname:(?<name>.+);(?<ignore>.+) 님의 귓속말: (?<level>.+) (?<class>.+) \\[cmd:(?<ignore2>.+) \\[cmd:(?<ignore3>.+)", RegexOptions.Compiled);
                matches = 파티지원포스지원Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 6, name));
                    return;
                }

                matches = 통합파티지원포스지원Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 6, name));
                    return;
                }


                matches = 타인의채팅Regex.Matches(line);
                if (matches.Count > 0)
                {
                    return;
                }

                //도움말, 공지사항도 ?
                matches = 자신의채팅Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String chat = matches[0].Groups["chat"].Value;
                    
                    if (name.IndexOf(' ') == -1) //채널창이 아니면..이름에 빈칸이 없다면
                    {
                        /*
                        if (name != Myname)
                        {// 내이름 세팅
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 0, name));
                        }
                         */
                        if (chat.StartsWith("//이름"))
                        {
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 0, name));
                        }
                        else if (chat.StartsWith("//템"))
                        {
                            String searchName = chat.Split(' ')[1];
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 1, searchName));
                        }
                        else if (chat.StartsWith("//모두지우기"))
                        {
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 2, null));
                        }
                        else if (chat.StartsWith("//도움말 바센1"))
                        {
                            String where = chat.Split(' ')[1];
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 3, where));
                        }
                        else if (chat.StartsWith("//타이머"))
                        {
                            ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 5, null));
                        }
                        else if (chat.StartsWith("//"))
                        {
                            String tmp = chat.Substring(2);
                            int step = 0;
                            try
                            {
                                step = Int32.Parse(tmp);
                                ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 4, step + ""));
                            }
                            catch
                            {

                            }
                        }
                            /*
                        else if (chat.Trim().Contains("사용 완료!"))
                        {
                            // 2013-07-15 
                            // 팻 18
                        }
                             */
                        else if (name == "티아마트" || name == "마르쿠탄")
                        {
                            InstanceDungeonEvent(this, new InstanceDungeonEventArgs(line, time, 1, name, chat));
                        }
                        else
                        {
                            //if (name == "도움말" || name == "공지사항") return;
                            //2013-07-15
                            if (name.Trim() == "도움말" || name.Trim() == "공지사항") { }
                            else if (chat.Trim().EndsWith(".") || chat.Trim().EndsWith("!") || chat.Trim().EndsWith("?")) { }
                            else if (chat.Contains("니다")) { }
                            //2013-07-16 명령: 
                            /*
                            else if (name.Trim() == "명령" && chat.Contains("사용합니다")) ;
                            else if (name.Trim() == "소환" && chat.Contains("소환했습니다")) ;
                            else if (name.Trim() == "변신" && (chat.Contains("발생했습니다") || chat.Contains("변신했습니다") || chat.Contains("회복했습니다"))) ;
                            else if (name.Trim() == "수면" && chat.EndsWith("니다.")) ;
                            */
                            else
                            {
                                // 2013-07-17 저주:
                                // 2013-07-15
                                // 여기까지 왔다면 이름을 세팅하자.
                                ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 0, name));
                            }
                        }
                    }
                    else if (name.EndsWith("소환") || name.EndsWith("변신") || name.EndsWith("강화") || name.EndsWith("보호") || name.EndsWith("저주"))
                    {
                        //스킬에 : 가 포함되어져 있는 것들이다.
                    }
                    else
                    {
                        InstanceDungeonEvent(this, new InstanceDungeonEventArgs(line, time, 1, name, chat));
                    }
                }

                //도움말, 공지사항도 ?
                matches = 지역채널에입장했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String where = matches[0].Groups["where"].Value;
                    InstanceDungeonEvent(this, new InstanceDungeonEventArgs(line, time, 0, where, null));
                    return;
                }
                
                
                

                matches = 대미지를줬습니다Regex.Matches(line);

                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String skill = matches[0].Groups["skill"].Value;
                    String target = matches[0].Groups["target"].Value;

                    int damage = matches[0].Groups["damage"].Value.GetDigits();
                    bool bCritical = false;

                    // 2013-05-07 평타는 계산하지 않는다.
                    // if (line.IndexOf("치명타") != -1) bCritical = true;
                    if (line.IndexOf("치명타") != -1 && skill != "") bCritical = true;


                    if (name == "")
                    {
                        name = Myname;
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Attack, ""));
                    }
                    else if (name.IndexOf("의 정령") != -1 || name.IndexOf("의 기운") != -1)
                    {
                        DamageInflicted(this, new PlayerDamageEventArgs(line, time, Noname, name + "-" + skill, target, damage, bCritical));
                        return;
                    }
                    else if (name.IndexOf(' ') != -1 && skill.IndexOf(name) != -1)
                    {
                        DamageInflicted(this, new PlayerDamageEventArgs(line, time, Noname, name + "-" + skill, target, damage, bCritical));
                        return;
                    }
                    else if (name == "빙판" && skill.StartsWith("빙판") && skill.EndsWith("효과"))
                    {
                        DamageInflicted(this, new PlayerDamageEventArgs(line, time, Noname, name + "-" + skill, target, damage, bCritical));
                        return;
                    }
                    //2013-09-27 이벤트때문에 이런 이름들이 존나 많아짐
                    else if (name == "티아마트" || name == "마르쿠탄" || name =="히페리온" ||name == "카이시넬" || name == "카룬" )
                    {// 이름에 빈칸이 없지만..
                        // 이미 유저목록에 이 이름이 있다면 패스하자
                        if (mainForm.data.GetPlayer(name) == null)
                        {
                            // 위의 이름이 들어오고 스킬목록에 없다면 추가하지 않는다.
                            if (SkillDictionary.GetClass(skill) == ClassType.NONE)
                            {
                                return;
                            }
                        }
                    }
                    else if ((name.IndexOf(" ")) != -1)
                    {
                        // 여기까지 왔다면 몹이다.
                        return;
                    }
                    
                    
                    DamageInflicted(this, new PlayerDamageEventArgs(line, time, name, skill, target, damage, bCritical));

                    //matched = true;
                    return;
                }

                matches = 대미지를받았습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String skill = matches[0].Groups["skill"].Value;
                    int damage = matches[0].Groups["damage"].Value.GetDigits();
                    String from = matches[0].Groups["from"].Value;

                    if (name == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.BeBeaten1, damage + ""));
                    }
                    else
                    {
                        // 다른사람 피해, 중독 출혈 신석 도트
                        // 지연폭발류
                        bool bCritical = false;
                        
                        // from은 없다.. 항상 효과
                        //2013.02.08 03:41:49 : 치명타! 땅의 정령이 시야랑에게서 564의 치명적인 대미지를 받았습니다. 
                        if (line.IndexOf("치명타") != -1) bCritical = true;

                        // 1818
                        if (line.IndexOf("어둠의 문양 파열") != -1) { }

                        if (name.EndsWith("어둠의 문양 파열 I 추"))
                        {
                            name = name.Substring(0, name.Length - 15);
                            skill = "어둠의 문양 파열 I 추가 효과";
                        }

                        DamageInflicted(this, new PlayerDamageEventArgs(line, time, Noname, skill, name, damage, bCritical));

                        //matched = true;
                    }
                    return;
                }

                matches = 사용한영향으로대미지를받았습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int damage = matches[0].Groups["damage"].Value.GetDigits();
                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.BeBeaten2, damage + ""));
                }

                matches = 공격이반사되어대미지를받았습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int damage = matches[0].Groups["damage"].Value.GetDigits();
                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.BeBeaten3, damage +""));
                    return;
                }

                //자힐.. 스킬..
                //만큼회복됐습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[을를] 사용해 (?<amount>[^a-zA-Z]+)만큼 (?<what>.+)[이가] 회복됐습니다", RegexOptions.Compiled);
                matches = 만큼회복됐습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String name = LogParser.Myname;
                    String skill = matches[0].Groups["skill"].Value;
                    
                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Heal1, ""));

                    RecoverEvent(this, new RecoverEventArgs(line, time, name, null, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }

                // 사용해만큼회복했습니다... 내가 치유다.. 힐을한다
                // 2013.09.27 00:39:19 : 치유의 빛 VI을 사용해 소주전용풍이 1,615만큼 생명력을 회복했습니다.
                //만큼회복했습니다Regex = new Regex(timeStampRegex + "(?<skill>.+)[을를] 사용해 (?<name>.+)[이가] (?<amount>[^a-zA-Z]+)만큼 (?<what>.+)[을를] 회복했습니다", RegexOptions.Compiled);
                matches = 만큼회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String who = LogParser.Myname;
                    String skill = matches[0].Groups["skill"].Value;
                    String name = matches[0].Groups["name"].Value;
                    RecoverEvent(this, new RecoverEventArgs(line, time, name, who, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }





                //도트류? WHO가 없다.. 미분류 힐
                //효과로회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<skill>.+)의 효과로 (?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);
                // 버그 : 2013.09.22 21:18:48 : 흑영귀-이스라펠이 고갈의 문양 폭발 III 추가 효과의 효과로 2,536의 생명력을 회복했습니다.
                matches = 추가효과의효과로회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String name = matches[0].Groups["name"].Value;
                    String skill = matches[0].Groups["skill"].Value;
                    if (name == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Heal1, ""));
                        name = LogParser.Myname;
                    }
                    else if (name.IndexOf(" ") != -1)
                    {
                        return;
                    }

                    RecoverEvent(this, new RecoverEventArgs(line, time, name, null, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }

                matches = 효과로회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String name = matches[0].Groups["name"].Value;
                    String skill = matches[0].Groups["skill"].Value;
                    if (name == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Heal1, ""));
                        name = LogParser.Myname;
                    }
                    else if (name.IndexOf(" ") != -1)
                    {
                        return;
                    } 
                    RecoverEvent(this, new RecoverEventArgs(line, time, name, null, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }

                //사용해회복했습니다Regex = new Regex(timeStampRegex + "(?<name>.+)[이가] (?<skill>.+)[을를] 사용해 ((?<whom>.+)[이가] )?(?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);
                //2013.09.22 21:11:40 : 나비가 온후한 울림 V을 사용해 에꾸가 1,778의 생명력을 회복했습니다.
                //2013.09.22 21:11:53 : 구라독종이 약초 치료 V을 사용해 818의 생명력을 회복했습니다.
                // 중요: 여기는 자기자신의 로그가 올수 없다. 자힐의 경우 만큼 회복이 된다
                // 치유의 기운이 소환: 치유의 기운 III을 사용해 깐따삐아가 394의 생명력을 회복했습니다.
                matches = 사용해회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String skill = matches[0].Groups["skill"].Value;
                    String name = matches[0].Groups["name"].Value;
                    String who = matches[0].Groups["whom"].Value;
                    // who가 있다면.. 실제회복한 사람은 who이다
                    if (String.IsNullOrEmpty(who) == false)
                    {
                        name = who;
                        who = matches[0].Groups["name"].Value;
                    }

                    if (name.IndexOf(" ") != -1)
                    {
                        return;
                    }
                    RecoverEvent(this, new RecoverEventArgs(line, time, name, who, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }


                // 이것도 무조건 나만.. 사용해회복했습니다와 쌍을 이룬다.
                // 사용해회복했습니다 : 무조건 다른사람
                // 사용한영향으로회복됐습니다 : 무조건 나만
                //사용한영향으로회복됐습니다Regex = new Regex(timeStampRegex + "(?<name>.+)[이가] 사용한 (?<skill>.+)의 영향으로 (?<amount>[^a-zA-Z]+)의 (?<what>.+)[이가] 회복됐습니다", RegexOptions.Compiled);
                matches = 사용한영향으로회복됐습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String name = LogParser.Myname;
                    String skill = matches[0].Groups["skill"].Value;
                    String who = matches[0].Groups["name"].Value;

                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Heal2, ""));
                    
                    RecoverEvent(this, new RecoverEventArgs(line, time, name, who, skill, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }


                // 비약류?
                //방법없이회복했습니다Regex = new Regex(timeStampRegex + "((?<name>.+)[이가] )?(?<amount>[^a-zA-Z]+)의 (?<what>.+)을 회복했습니다", RegexOptions.Compiled);
                matches = 방법없이회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    int amount = matches[0].Groups["amount"].Value.GetDigits();
                    String name = matches[0].Groups["name"].Value;

                    if (String.IsNullOrEmpty(name))
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Heal3, ""));
                        name = LogParser.Myname;
                    }
                    RecoverEvent(this, new RecoverEventArgs(line, time, name, null, null, amount, (matches[0].Groups["what"].Value == "생명력")));
                    return;
                }

               


                matches = 속도를회복했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String what = matches[0].Groups["what"].Value;
                    if (matches[0].Groups["name"].Value == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Repair1, what));
                    }
                    return;
                }


                matches = 사용한영향으로상태가됐습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String target = matches[0].Groups["target"].Value;
                    String debuff = matches[0].Groups["debuff"].Value.Trim();
                    if (target == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Debuff1, debuff));
                    }
                    return;
                }

                matches = 사용한영향으로기타등등Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String debuff = matches[0].Groups["debuff"].Value.Trim();

                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Status1, debuff));
                    return;
                }

                matches = 벗어났습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String debuff = matches[0].Groups["debuff"].Value;

                    if (name == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Repair2, debuff));
                    }
                    return;
                }

                matches = 사용해상태가됐습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    if (matches[0].Groups["name"].Value == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Status2, ""));
                    }
                    return;
                }

                matches = 내스킬을써서효과가발생했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Status3, ""));
                    return;
                }

                //효과가발동했습니다Regex
                matches = 효과가발동했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String skill = matches[0].Groups["skill"].Value.Trim();

                    MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Debuff2, skill));
                    return;
                }

                //2013-05-03
                matches = 사망했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value.Trim();
                    String ignore = matches[0].Groups["ignore"].Value.Trim();
                    if (name == "")
                    {
                        MyStatusEvent(this, new MyStatusEventArgs(line, time, MyStatus.Die, null));
                    }
                    return;
                }

                //2013-05-24
                matches = 경험치를얻었습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String target = matches[0].Groups["target"].Value.Trim();
                    String amount = matches[0].Groups["amount"].Value.Trim();
                    InstanceDungeonEvent(this, new InstanceDungeonEventArgs(line, time, 2, target, amount));
                    return;
                }


                matches = 삼자가자신의무엇가를변경했습니다Regex.Matches(line);
                if (matches.Count > 0)
                {
                    return;
                }

                matches = 무시Regex.Matches(line);
                if (matches.Count > 0)
                {
                    //tempEvent(this, new LogEventArgs(">>무시 " + line));
                    return;
                }

                matches = 명령표현Regex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups["time"].Value.GetTime(LogTimeFormat);
                    String name = matches[0].Groups["name"].Value;
                    String target = matches[0].Groups["target"].Value;

                    if (name == Myname && target != ""&& target.IndexOf(" ") == -1)
                    {
                        ChatCommandEvent(this, new ChatCommandEventArgs(line, time, 1, target));
                    }
                    return;
                }


                matches = 감정표현Regex.Matches(line);
                if (matches.Count > 0)
                {
                    return;
                }
                tempEvent(this, new LogEventArgs(line));
            }
            catch
            {

            }

        }

        private bool IsFile = false;
        public void Start(string file)
        {
            if (bRunning)
            {
                return;
            }

            if (Starting != null)
            {
                Starting(this, EventArgs.Empty);
            }

            if ((fileStream = OpenFileStream(file)) != null)
            {
                bRunning = true;
                // 파일의 마지막으로 점프
                
                //2013-05-04 개발용으로 파일에서 읽을 수 있다.
                //이 경우 마지막으러 점프 하면 안된다.
                // Chat.log인경우에만 점프
                if (file == Logic.AionLogFileName)
                {
                    IsFile = false;
                    fileStream.Position = fileStream.Length;
                }
                else
                {
                    IsFile = true;
                }

                streamReader = GetStreamReader(fileStream);
                StartWorker();
            }

            if (Started != null)
            {
                Started(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            if (!bRunning)
            {
                return;
            }
            else
            {
                bRunning = false;
            }

            if (Stopping != null)
            {
                Stopping(this, EventArgs.Empty);
            }

            // Working out how to avoid Abort()
            if (worker != null)
            {
                worker.Abort();
                worker = null;
            }

            if (streamReader != null)
            {
                streamReader.Close();
                streamReader = null;
            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }

            if (Stopped != null)
            {
                Stopped(this, EventArgs.Empty);
            }

            Debug.Write("Log parser stopped.");
        }


        private void StartWorker()
        {
            int SleepTime = (IsFile) ? 5 : 1;
            worker = new Thread
            (
                delegate()
                {
                    lock (lockObject)
                    {
                        while (bRunning)
                        {
                            string line = streamReader.ReadLine();

                            if (!String.IsNullOrEmpty(line))
                            {
                                ParseLine(line.Trim());
                            }

                            // Oops yah, it was eating up cpu without this.
                            Thread.Sleep(SleepTime);
                        }
                    }
                }
            );
            worker.IsBackground = true;
            worker.Start();
        }


        public FileStream OpenFileStream(string file)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    if (FileNotFound != null)
                    {
                        FileNotFound(this, EventArgs.Empty);
                    }
                }
            }
            return stream;
        }

        public  StreamReader GetStreamReader(FileStream stream)
        {
            if (stream != null)
            {
                return new StreamReader(stream, System.Text.Encoding.Default);
            }
            else
            {
                return null;
            }
        }

        private string LogTimeFormat = "yyyy.MM.dd hh:mm:ss";

    }


    
}

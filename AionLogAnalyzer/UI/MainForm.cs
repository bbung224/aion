//#define USING_FILE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace AionLogAnalyzer
{

    public partial class MainForm : Form
    {
        private Logic logic = null;
        private LogParser logParser = null;
        public Data data = null;
        private ListViewSorter _SkillSorter;
        private Update update;
        private bool bRunByUpdateModule = false;
        bool oneClassCheckBoxBeforeTimerStart = false;
        public String Version;
        private NoticeForm OnlyUseForAionLauncherNoticeForm = null;

        #region 생성자
        public MainForm(string[] args)
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            InitializeComponent();
            InitializeComponentManually();
            logic = new Logic(this);
            data = new Data();
            debuffList = new List<string>();
            _SkillSorter = new ListViewSorter(new int[] { 1, 2 });
            _SkillSorter.SortColumn = 4;
            _SkillSorter.SortOrder = ListViewSortOrder.Descending;
            this.listView1.ListViewItemSorter = _SkillSorter;
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.update = new Update(this);
            SetMyStatusLabel();
            OnlyUseForAionLauncherNoticeForm = new NoticeForm(this);
            if (args.Length > 0)
            {
                if (args[0].CompareTo("-update") == 0)
                {
                    bRunByUpdateModule = true;
                    update.RemoveBatchFile();
                    // 2013-05-08 업데이트로 왔다.
                    //MessageBox.Show(args[1]);
                    ProcessUpdate(args[1]);
                } 
            }
            
        }

        private void ProcessUpdate(String str)
        {
            try
            {
                String[] array = str.Split('&');
                foreach (string s in array)
                {
                    string[] t = s.Split('=');
                    if (t[0] == "WindowWidth")
                    {
                        Properties.Settings.Default.WindowWidth = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "WindowHeight")
                    {
                        Properties.Settings.Default.WindowHeight = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "WindowX")
                    {
                        Properties.Settings.Default.WindowX = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "WindowY")
                    {
                        Properties.Settings.Default.WindowY = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "TopMost")
                    {
                        Properties.Settings.Default.TopMost = bool.Parse(t[1]);
                    }
                    else if (t[0] == "OneClassFlag")
                    {
                        Properties.Settings.Default.OneClassFlag = bool.Parse(t[1]);
                    }
                    else if (t[0] == "AutoSortFlag")
                    {
                        Properties.Settings.Default.AutoSortFlag = bool.Parse(t[1]);
                    }
                    else if (t[0] == "ThisFileExist")
                    {
                        Properties.Settings.Default.ThisFileExist = bool.Parse(t[1]);
                    }
                    else if (t[0] == "OpacityValue")
                    {
                        Properties.Settings.Default.OpacityValue = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "UserName")
                    {
                        Properties.Settings.Default.UserName = t[1];
                    }
                    else if (t[0] == "UserServer")
                    {
                        Properties.Settings.Default.UserServer = t[1];
                    }
                    else if (t[0] == "UserGroup")
                    {
                        Properties.Settings.Default.UserGroup = t[1];
                    }
                    else if (t[0] == "MyInfoDamage")
                    {
                        Properties.Settings.Default.MyInfoDamage = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "TestServer")
                    {
                        Properties.Settings.Default.TestServer = Int32.Parse(t[1]);
                    }
                    else if (t[0] == "UserFolder")
                    {
                        Properties.Settings.Default.UserFolder = t[1];
                    }
                }
            }
            catch
            {
                
            }
            finally
            {
                try
                {
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }
        #endregion

        #region MainForm_Load
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " " + Version;
            // 설정파일이 없다면
            // 한번이상 저장되었다면 ThisFileExist 무조건 true이다
            bool bExistConfigFile = false;
            bExistConfigFile = Properties.Settings.Default.ThisFileExist;
            this.comboBoxMyInfoDamage.SelectedIndex = 1;
            if (bExistConfigFile)
            {
                // 2013-03-12 창최소화 버튼을 누르고 작업표시줄에서 창닫기를 누를경우 WIDTH, HEIGHT, X, Y 값이 이상하여 다음번 실행할때 제대로 표시되지 않는 문제 발생
                //  -32000으로 저장되어버리는 버그가 있다.

                int w = Properties.Settings.Default.WindowWidth;
                int h = Properties.Settings.Default.WindowHeight;
                this.Width = (w < this.Width) ? this.Width : w;
                this.Height = (h < this.Height) ? this.Height : h;

                
                int x = Properties.Settings.Default.WindowX;
                int y = Properties.Settings.Default.WindowY;
                x = (x < 0 || x >= Screen.AllScreens[0].WorkingArea.Width) ? 0 : x;
                y = (y < 0 || y >= Screen.AllScreens[0].WorkingArea.Height) ? 0 : y;
                this.Location = new Point(x,y);

                this.TopMost = this.topMostCheckbox.Checked = Properties.Settings.Default.TopMost;

                this.opacityTrackBar.Value = Properties.Settings.Default.OpacityValue;
                opacityTrackBar_ValueChanged(null, null);

                // 이름
                String tmp = Properties.Settings.Default.UserName;
                if (tmp == "") LogParser.Myname = tmp;
                logParser_chatCommandEvent(null, new ChatCommandEventArgs(null, DateTime.Now, 0, tmp));

                //서버
                tmp = Properties.Settings.Default.UserServer;
                if (tmp != "")
                {
                    this.comboBoxSelectServer.SelectedItem = tmp;
                }

                //종족
                tmp = Properties.Settings.Default.UserGroup;
                if (tmp != "")
                {
                    this.comboBox종족.SelectedItem = tmp;
                    if (tmp == "마족")
                    {
                        this.checkBox마족.Checked = true;
                        this.checkBox천족.Checked = false;
                    }
                    else
                    {
                        this.checkBox마족.Checked = false;
                        this.checkBox천족.Checked = true;
                    }
                }

                //얼마 이상 대미지
                int tmp2 = Properties.Settings.Default.MyInfoDamage;
                if (tmp2 >= 0 && tmp2 < this.comboBoxMyInfoDamage.Items.Count)
                {
                    this.comboBoxMyInfoDamage.SelectedIndex = tmp2;
                }
                //테섭
                int tmp3 = Properties.Settings.Default.TestServer;
                if (tmp3 >= 0 && tmp3 < this.comboBoxTestServer.Items.Count)
                {
                    this.comboBoxTestServer.SelectedIndex = tmp3;
                }
                else
                {
                    this.comboBoxTestServer.SelectedIndex = 0;
                }
                if (this.comboBoxTestServer.SelectedIndex == 1)
                {
                    this.Text += " (TestServer)";
                }
                this.textBoxWorkingServerSelect.Text = Properties.Settings.Default.UserFolder;
            }
            else
            {
                // 왜????????
                this.TopMost = true;
            }

            logParser_chatCommandEvent(null, new ChatCommandEventArgs(null, DateTime.Now, 0, LogParser.Myname));
            

            

            // 업데이트로 시작했으면 다시 업데이트 하지 않는다.
            // 0 : 업데이트후시작이 아니고 자동시작이다
            // 1 : 정보탭에서 눌렀다.
            // 2 : 업데이트후 시작했고. 버전정보만 보낸다.
            update.ServerName = (String)this.comboBoxSelectServer.SelectedItem;
            if (bRunByUpdateModule == false)
            {
                update.Start(0);
            }
            else
            {
                // 2013-05-13 업데이트로 시작했다. 버전 정보만 보내자
                update.Start(2);
            }

            // 2013-07-17 업데이트후 system.cfg 체크
            try
            {
                this.logic.Initialize();
                //2013-07-16 system.cfg 파일이 없을수도 있다.
                if (this.logic.IsExistSystemCfg() == false)
                {
                    MessageBox.Show("AION 환경설정 파일이 없습니다.\r\n일반 서버 유저분은 AION 실행 후 환경설정에서 아무 항목이나\n변경하시고 AION을 종료한 후 ALA를 재실행해주세요.\n테스트서버, 외국서버 유저분은 환경설정에서 동작서버를\n변경하신 후 재실행하세요.\n외국서버 유저분은 폴더까지 선택해주세요", "Error");
                    //Application.ExitThread();
                    //return;
                    OnlyDoChangeWorkingServer();
                }
                else
                {
                    this.logic.ConfigFilePatch(true);
                    InitializeLogParser();
                    data.Reset();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                //Application.ExitThread();
            }

            //2013-05-24
            try
            {
                PingForm pingForm = new PingForm(this);
                pingForm.Start();
            }
            catch { }


            //2014-02-16 깡통 버전
            // 0.2.9
            /*
            logParser.Stop();
            this.normalStartButton.Enabled = false;
            //this.stopButton.Enabled = false;
            //this.resetToolStripButton.Enabled = false;
            this.toolStripSplitButton1.Enabled = false;
            */









        }
        #endregion

        #region MainForm_FormClosed
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            update.Stop();
            LogParser.StopByWhy = EnumStopBy.ByFormClosed;
            if (logParser != null)
            {
                logParser.Stop();
                this.logic.ConfigFilePatch(false);
            }
            SettingsSave();
        }

        public void SettingsSave()
        {
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WindowX = this.Location.X;
            Properties.Settings.Default.WindowY = this.Location.Y;
            Properties.Settings.Default.TopMost = this.TopMost;
            Properties.Settings.Default.OneClassFlag = this.oneClassCheckbox.Checked;
            Properties.Settings.Default.AutoSortFlag = this.autoSortCheckbox.Checked;
            Properties.Settings.Default.ThisFileExist = true;
            //1818 Value 가져오는 것 자체가 UI접근으로 쓰레드 에라다
            //Properties.Settings.Default.OpacityValue = this.opacityTrackBar.Value;
            Properties.Settings.Default.OpacityValue = this.opacityTrackBar.Value18;
            Properties.Settings.Default.UserName = LogParser.Myname;
            Properties.Settings.Default.UserServer = (String)this.comboBoxSelectServer.SelectedItem;
            Properties.Settings.Default.UserGroup = (String)this.comboBox종족.SelectedItem;
            Properties.Settings.Default.MyInfoDamage = this.comboBoxMyInfoDamage.SelectedIndex;
            Properties.Settings.Default.TestServer = this.comboBoxTestServer.SelectedIndex;
            Properties.Settings.Default.UserFolder = this.textBoxWorkingServerSelect.Text;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region 동작서버를 변경하기 위해 환경설정만 변경하고 끝내도록 한다.
        private void OnlyDoChangeWorkingServer()
        {
            this.normalStartButton.Enabled = false;
            this.분간측정ToolStripMenuItem.Enabled = false;
            this.stopButton.Enabled = false;
            this.resetToolStripButton.Enabled = false;
            this.statusLabel.Text = "상태 : 환경파일 없음. 동작서버 변경후 재실행";
            this.statusLabel.ForeColor = Color.Red;

            ToolStripMenuItemLogFileOpen.Enabled = false;
            로그파일생성여부ToolStripMenuItem.Enabled = false;
        }
        #endregion

        #region LogParser
        private void InitializeLogParser()
        {
            logParser = new LogParser(this);
            logParser.Started += new EventHandler(logParser_Started);
            logParser.Stopped += new EventHandler(logParser_Stopped);
            logParser.tempEvent += new EventHandler(logParser_tempEvent);
            
            logParser.DamageInflicted += new LogParser.DamageInflictedEventHandler(logParser_DamageInflicted);
            logParser.ChatCommandEvent += new LogParser.ChatCommandEventHandler(logParser_chatCommandEvent);
            logParser.MyStatusEvent += new LogParser.MyStatusEventHandler(logParser_MyStatusEvent);
            logParser.InstanceDungeonEvent += new LogParser.InstanceDungeonEventHandler(logParser_InstanceDungeonEvent);
            logParser.RecoverEvent += new LogParser.RecoverEventHander(logParser_RecoverEvent);
            logParser.Start(Logic.AionLogFileName);
        }

        

        void logParser_Stopped(object sender, EventArgs e)
        {
            if (LogParser.StopByWhy == EnumStopBy.ByUser)
            {
                SetStatusLabel("상태: 중지");

            }
            else if (LogParser.StopByWhy == EnumStopBy.ByTimerThread)
            {
                SetStatusLabel("동작: 측정완료");
                SetAvailableRecord(1);
                //2013-07-15
                // 3분 기록 자동등록
                if (this.workingTime == 180)
                {
                    측정기록등록(LogParser.Myname);
                }
            }
            else if (LogParser.StopByWhy == EnumStopBy.ByInstanceDungeon)
            {
                if (this.indun != null && this.indun is InstanceDungeon용제의안식처)
                {
                    SetAvailableRecord(3);
                }
                else if ( this.indun != null && (this.indun is InstanceDungeon루나디움 ||this.indun is InstanceDungeon카탈라마이즈))
                {
                    SetAvailableRecord(2);
                }
                else if (this.indun != null && this.indun is InstanceDungeon룬의보호탑)
                {
                    SetAvailableRecord(4);
                }
                // 2013-07-15
                // 인던공략 정보를 자동등록한다.
                if (this.indun is InstanceDungeon용제의안식처)
                {
                    foreach (ListViewItem lvi in this.listView1.Items)
                    {
                        String playerName = lvi.SubItems[2].Text;
                        if ( playerName != LogParser.Noname) 
                            측정기록등록(playerName);
                    }
                }
                if (this.indun != null && this.indun is InstanceDungeon룬의보호탑) ;
                else 인던공략등록ToolStripMenuItem_Click(this, null);
            }
            else if (LogParser.StopByWhy == EnumStopBy.ByFormClosed)
            {
                //2013-07-31
                //인던이 끝나서 자동으로 종료된후, 프로그램을 끄게되면 여기가 다시 호출된다.
                // FormClosed 에서 결국 여기 호출
                //아무일도 하지 않으나 EnumStopBy.ByFormClosed를 넣어준다
            }
            this.normalStartButton.Enabled = true;
            //this.timeStartButton.Enabled = true;
            this.분간측정ToolStripMenuItem.Enabled = true;
            this.stopButton.Enabled = false;
            this.normalStartButton.BackColor = Color.Cyan;
            this.statusLabel.ForeColor = Color.Red;
        }

        void logParser_Started(object sender, EventArgs e)
        {//
            SetAvailableRecord(0);
            if (LogParser.NormalStartFlag)
            {
                SetStatusLabel("상태: 동작중");
            }
            this.normalStartButton.Enabled = false;
            //this.timeStartButton.Enabled = false;
            this.분간측정ToolStripMenuItem.Enabled = false;
            this.stopButton.Enabled = true;
            this.normalStartButton.BackColor = Color.Transparent;
            this.statusLabel.ForeColor = Color.Black;
        }
        #endregion

        #region 채팅명령어 처리
        private String HelpWhere;
        void logParser_chatCommandEvent(object sender, ChatCommandEventArgs e)
        {
            // 0 : 내이름설정
            // 1 : 캐릭검색
            // 2 : 모두지우기
            // 3 : 도움말
            // 4 : 도움말 항목 클립보드 복사
            // 5 : 타이머
            // 6 : 파티지원
            if (e.Command == 0)
            {
                // User에 있는 이름을 바꾼다.. 없을수 있음
                User my = data.GetPlayer(LogParser.Myname);
                if (my != null)
                {
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        my.SetName(e.Argument);
                    }));
                }
                LogParser.Myname = e.Argument;
                //2013-05-03
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                        this.labelUsername.Text = LogParser.Myname;
                }));
            }
            else if (e.Command == 1)
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    ShowPlayerInfo(e.Argument);
                }));
            }
            else if (e.Command == 2)
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    PlayerListClear();
                }));

            }
            else if (e.Command == 3)
            {
                HelpWhere = e.Argument;
            }
            else if (e.Command == 4)
            {
                int step = Int32.Parse(e.Argument);
                if (HelpWhere == "바센1")
                {
                    if (step - 1 < HelpInstanceDungeon.바센1.Length)
                    {
                        this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                        {
                            ClipboardSetText(HelpInstanceDungeon.바센1[step - 1]);
                        }));
                    }
                }
            }
            else if (e.Command == 5) // //타이머 채팅
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    // 2013-09-27
                    if (this.indun != null && this.indun is InstanceDungeon룬의보호탑)
                    {
                        ShowTimer룬의보호탑();
                    }
                    else
                    {
                        ShowTimer();
                    }
                }));
            }
            else if (e.Command == 6) //파티지원 
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    ShowPlayerInfo(e.Argument, true);
                }));
            }
        }
        #endregion


        #region 회복 추가
        void logParser_RecoverEvent(object sender, RecoverEventArgs e)
        {
            User user = null;
            user = data.GetPlayer(e.Name); //힐을 받은사람은 추가하지 말자..
            if (user == null)
            {
                /*
                user = new User(e.Name);
                data.UserList.Add(user);

                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    // 2013-05-03 클래스별 보기 상태에 따라 삽입여부를 결정한다.
                    // 현재 상태가 전체인 경우에만 삽입하도록 한다.
                    if (this.클래스별보기상태 == 클래스별보기기본)
                    {
                        user.ListItem.Text = (this.listView1.Items.Count + 1) + "";
                        this.listView1.Items.Add(user.ListItem);
                    }
                }));
                 */
            }
            else
            {
                user.AddRecover(e);
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    user.UpdataListView();
                }));
            }

        // 힐을 준사람
            if (String.IsNullOrEmpty(e.Who) == false && e.IsHP && e.Who.IndexOf(" ") == -1)
            {
                User healuser = data.GetPlayer(e.Who);
                if (healuser == null)
                {
                    healuser = new User(e.Who);
                    data.UserList.Add(healuser);

                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        // 2013-05-03 클래스별 보기 상태에 따라 삽입여부를 결정한다.
                        // 현재 상태가 전체인 경우에만 삽입하도록 한다.
                        if (this.클래스별보기상태 == 클래스별보기기본)
                        {
                            healuser.ListItem.Text = (this.listView1.Items.Count + 1) + "";
                            this.listView1.Items.Add(healuser.ListItem);
                        }
                    }));

                }
                healuser.AddHeal(e);
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    healuser.UpdataListView();
                }));
            }
        }

        
        #endregion


        #region 대미지 추가
        void logParser_DamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            // 미분류고 클래스별 1인이 체크되어 있다.
            bool bInsert = false;
            User user = null;

            if (e.Name == LogParser.Noname && LogParser.OneClassFlag)
            {
                // 미분류로 넘어왔다. 1클래스가 체크되어있다면
                string[] tmp = e.Skill.Split('-');
                string skill = Util.GetSkillName(tmp[tmp.Length - 1]);
                ClassType aionClass = SkillDictionary.GetClass(skill);

                if (aionClass == ClassType.NONE && (tmp[0].IndexOf("의 정령") != -1 || tmp[0].IndexOf("의 기운") != -1) )
                {
                    aionClass = ClassType.정령성;
                }

                // 스킬이 클래스의 스킬이다.
                if (aionClass != ClassType.NONE)
                {
                    user = data.GetFirsPlayerByClass(aionClass);
                    if (user != null)
                    {
                        user.AddDamage(e);
                        e.NameAfterAdd = user.Name;
                        this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                        {
                            user.UpdataListView();
                            if (LogParser.AutoSortFlag) Sort();
                        }));
                        bInsert = true;
                    }
                    else
                    {
                        // 아직 클래스가 없다.. 그럼 미분류로 갈수밖에 읍다.
                        // 클래스가 없다
                        //return;
                    }
                }
                else
                {
                    // 스킬목록에 없다
                    // 데미지 신석
                    if (!skill.StartsWith("마법("))
                    {
                        //몹이 사용한 도트류.. 스킬목록에 없는
                        return;
                    }
                }
            }
            //else
            if ( bInsert == false ) {
                user = data.GetPlayer(e.Name);
                if (user == null)
                {
                    user = new User(e.Name);
                    data.UserList.Add(user);
                    if (LogParser.AutoSortFlag == false)
                    {
                        _SkillSorter.SortOrder = ListViewSortOrder.None;
                    }
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        // 2013-05-03 클래스별 보기 상태에 따라 삽입여부를 결정한다.
                        // 현재 상태가 전체인 경우에만 삽입하도록 한다.
                        if (this.클래스별보기상태 == 클래스별보기기본)
                        {
                            user.ListItem.Text = (this.listView1.Items.Count + 1) + "";
                            this.listView1.Items.Add(user.ListItem);
                        }
                    }));
                }
                user.AddDamage(e);
                e.NameAfterAdd = user.Name;
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    user.UpdataListView();
                    if (LogParser.AutoSortFlag) Sort();
                }));
            }
            // 2013-05-05
            if (indun != null)
            {
                indun.AddDamage(e);

                // 2013-05-07 용제는 티아마트의 데미지를 계산해야한다.
                // 티아마트와의 첫 교전때 addEvent
                if (indun is InstanceDungeon용제의안식처)
                {
                    InstanceDungeon용제의안식처 id = (InstanceDungeon용제의안식처)indun;
                    if (id.BossName == e.Target)
                    {
                        TargetTotalDamage bossTarget = id.GetBossTarget();
                        if (indun.EventList.Count == 1)
                        {
                            // 최초 보스몹 타격 티아마트 피통 팝업
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                data.ResetExceptName();
                                user.AddDamage(e);
                                user.UpdataListView();
                                SetBossPercent(0, id.BossTotalDamage);
                            }));
                            indun.AddEvent(new InstanceDungeonEventArgs(e.log, e.Time, -1, null, null));
                        }
                        else
                        {
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                SetBossPercent(bossTarget.TotalDamage, id.BossTotalDamage);
                            }));
                            // 49퍼 이상 때리면 addEvent
                        }
                        if (id.bCheckPercent == false && bossTarget.TotalDamage >= id.BossTotalDamage * 0.5)
                        {
                            id.bCheckPercent = true;
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                //PlayerListClear();
                                // 2013-05-08 유저는 그대로 두고 대미지만 초기화 시킨다.
                                // 이름과 클래스만 있어야 한다.
                                data.ResetExceptName();
                                user.AddDamage(e);
                                user.UpdataListView();
                            }));
                            indun.AddEvent(new InstanceDungeonEventArgs(e.log, e.Time, -1, null, null));
                        }
                    }
                }
                else if (indun is InstanceDungeon카탈라마이즈)
                {
                    InstanceDungeon카탈라마이즈 id = (InstanceDungeon카탈라마이즈)indun;
                    if (id.BossName == e.Target)
                    {
                        TargetTotalDamage bossTarget = id.GetBossTarget();
                        if (indun.EventList.Count == 1)
                        {
                            // 히페리온 첫 타격시 피통팝업, 데이터 리셋, 타이머
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                data.ResetExceptName();
                                user.AddDamage(e);
                                user.UpdataListView();
                                SetBossPercent(0, id.BossTotalDamage);
                                ShowTimer();
                            }));
                            indun.AddEvent(new InstanceDungeonEventArgs(e.log, e.Time, -1, null, null));
                        }
                        else
                        {
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                SetBossPercent(bossTarget.TotalDamage, id.BossTotalDamage);
                            }));
                        }
                    }
                }
                else if (indun is InstanceDungeon룬의보호탑)
                {
                    InstanceDungeon룬의보호탑 id = (InstanceDungeon룬의보호탑)indun;
                    if (id.BossName == e.Target)
                    {
                        TargetTotalDamage bossTarget = id.GetBossTarget();
                        if (indun.EventList.Count == 1)
                        {
                            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                            {
                                if (this.timerFormDefence != null) timerFormDefence.Restart();
                            }));
                            indun.AddEvent(new InstanceDungeonEventArgs(e.log, e.Time, -1, null, null));
                        }
                    }
                }
            }
        }



        #endregion

        #region 개발용 & 룬탑처리
        void logParser_tempEvent(object sender, EventArgs e)
        {
            String str = e.ToString();
            // 2013-09-05
            // 룬의 보호탑 처리
            // 30분 후 약화된 보호막이 완전히 사라집니다.
            if (indun != null && indun is InstanceDungeon룬의보호탑)
            {
                int idx1 = str.IndexOf("30분 후 약화된 보호막이");
                int idx2 = str.IndexOf("동력장치 방향에 차원의 문이 열렸습니다.");
                int idx3 = str.IndexOf("동력장치가 파괴되었습니다.");
                int idx4 = str.IndexOf("제43 파괴부대 병사들이 퇴각합니다");

                if (idx1 != -1)
                {
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        ShowTimer룬의보호탑();
                    }));
                }
                else if (idx2 != -1)
                {
                    // 시작
                    string where = str.Substring(idx2 - 3, 2);
                    this.timerFormDefence.SetStart(where);
                }
                else if (idx3 != -1)
                {
                    // 파괴
                    string where = str.Substring(idx3 - 3, 2);
                    this.timerFormDefence.SetStop(where);
                }
                else if (idx4 != -1)
                {
                    //2013.09.04 18:34:19 : 보호막이 정상적으로 가동되어 제43 파괴부대 병사들이 퇴각합니다. 보호막 생성실 이동 장치가 나타났습니다.
                    //종료
                    this.timerFormDefence.ProcessSuccess();
                }
            }
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.textBoxEtc.AppendText(str + "\r\n");
            }));
        }
        #endregion

        #region 정렬
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
            Sort();
            
        }

        private void Sort()
        {
            this.listView1.Sort();
            int index = 0;
            foreach (ListViewItem item in this.listView1.Items)
            {
                item.Text = (++index) + "";
            }
        }
        #endregion

        #region 캐릭터 검색
        private void 아이템정보보기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if ( collection.Count == 0) return;
            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            playerName = data.GetPlayerByDisplayName(playerName).Name;
            if (playerName != LogParser.Noname)
            {
                ShowPlayerInfo(playerName);
            }
        }

        private void ShowPlayerInfo(String playerName)
        {
            ShowPlayerInfo(playerName, true);
        }
        private void ShowPlayerInfo(String playerName, bool isAutoTabChange)
        {
            //서비스 점검중일수 있다.
            try
            {
                String[] temp = playerName.Split('-');
                String userName = "";
                String userServer = "";

                if (temp.Length == 1)
                {
                    userName = playerName;
                    userServer = (String)this.comboBoxSelectServer.SelectedItem;
                }
                else
                {
                    userName = temp[0];
                    userServer = temp[1];
                }
                //if (String.IsNullOrEmpty(userServer)) userServer = "이스라펠";

                HtmlDocument doc = this.webBrowser1.Document;
                HtmlElement ele = null;

                if (userServer != "")
                {
                    foreach (HtmlElement tt in webBrowser1.Document.All)
                    {
                        if (tt.TagName == "A")
                        {
                            if (tt.InnerText == userServer)
                            {
                                tt.InvokeMember("click");
                                //ele.InvokeMember("submit");
                                break;
                            }
                        }
                    }
                }
                ele = doc.GetElementById("schar");
                ele.InnerText = userName;
                //ele.InnerText = "이스라펠";
                ele = doc.GetElementById("schar_srch");
                ele = doc.GetElementById("schar_submit");
                ele.InvokeMember("click");
                if ( isAutoTabChange ) this.tabControl1.SelectedIndex = 2;
            }
            catch
            {
                MessageBox.Show("캐릭 검색 오류");
                return;
            }
        }
        #endregion

        #region Status 툴팁
        private void topMostCheckbox_Click(object sender, EventArgs e)
        {
            this.TopMost = this.topMostCheckbox.Checked;
        }

        private void oneClassCheckbox_Click(object sender, EventArgs e)
        {
            LogParser.OneClassFlag = this.oneClassCheckbox.Checked;
        }

        private void autoSortCheckbox_Click(object sender, EventArgs e)
        {
            LogParser.AutoSortFlag = this.autoSortCheckbox.Checked;
        }


        void opacityTrackBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.Opacity = 1.0 - (this.opacityTrackBar.Value / 100.0); 
        }
        #endregion

        #region 사용 스킬 목록
        private SkillListForm skillListForm = null;
        private void 사용스킬목록ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skillListForm == null)
            {
                skillListForm = new SkillListForm();
            }
            else
            {
                skillListForm.Dispose();
                skillListForm = new SkillListForm();
            }

            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0) return;
            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;

            User player = data.GetPlayerByDisplayName(playerName);
            skillListForm.Size = new System.Drawing.Size(this.Width, this.Height);
            int x = this.Location.X + this.Width;
            if (x + this.skillListForm.Width > Screen.AllScreens[0].WorkingArea.Width)
            {
                x = this.Location.X - this.skillListForm.Width;
            }
            skillListForm.Location = new Point(x, this.Location.Y);
            skillListForm.TopMost = this.TopMost;
            skillListForm.Opacity = this.Opacity;

            if (player != null) skillListForm.Show(player);
        }
        #endregion

        #region 지우기 
        private void 모두지우기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerListClear();
        }

        private void 지우기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skillListForm != null)
            {
                skillListForm.Dispose();
                skillListForm = null;
            }

            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0) return;

            // 2013-05-03 선택한거 모두 지우기
            String[] removePlayerNameList = new String[collection.Count];
            for (int i = 0; i < removePlayerNameList.Length; i++)
            {
                removePlayerNameList[i] = this.listView1.Items[collection[i]].SubItems[2].Text;
            }

            for (int i = 0; i < removePlayerNameList.Length; i++)
            {
                User user = data.GetPlayerByDisplayName(removePlayerNameList[i]);
                data.RemovePlayer(user.Name);
                this.listView1.Items.Remove(user.ListItem);
            }

            //String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            //playerName = data.GetPlayerByDisplayName(playerName).Name;
            //data.RemovePlayer(playerName);

            //this.listView1.Items.RemoveAt(collection[0]);
            // 번호를 다시?
            Sort();
        }

        // 2013-05-04 그냥 내정보 탭도 같이 리셋
        // 2013-07-16 메모리 문제로 기타탭도 같이 리셋
        private void PlayerListClear()
        {
            if (skillListForm != null)
            {
                skillListForm.Dispose();
                skillListForm = null;
            }
            data.Reset();
            this.listView1.Items.Clear();

            this.textBoxMyInfo.Text = "";
            this.debuffList.Clear();
            SetMyStatusLabel();

            this.textBoxEtc.Text = "";
        }
        #endregion

        #region 클립보드
        private void 클립보드MenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0) return;

            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            User user = data.GetPlayerByDisplayName(playerName);
            //[color:1등; 0 1 0]  [color:<당신>;1 1 0]  총딜-[color:4139;0 1 1]  DPS-[color:2069;1 0 0]  치명-[color:0%;1 0 1] 평타-[color:0%;1 0 1]
            //색깔표시 글자는 최대 5개다
            //이름은 5자까지만.. 딜량은 2개로 나눠서
            string s1 = "[color:"+this.listView1.Items[collection[0]].SubItems[0].Text+"등;0 1 0]"; //등수
            string s2 = "";
            if (user.DisplayName.Length > 5)
            {
                s2 = "[color:" + user.DisplayName.Substring(0, 5) + ";1 1 0][color:" + user.DisplayName.Substring(5) + ";1 1 0]";
            }
            else
            {
                s2 = "[color:" + user.DisplayName + ";1 1 0]";
            }

            string s3 = "";
            string tmp = user.TotalDamage.ToString();
            if (tmp.Length > 5)
            {
                s3 = "총딜-[color:" + tmp.Substring(0, 5) + ";0 1 1][color:" + tmp.Substring(5) + ";0 1 1]";
            }
            else
            {
                s3 = "총딜-[color:" + tmp + ";0 1 1]";
            }


            ClipboardSetText(String.Format("{0}  {1}  {2}  DPS-[color:{3};1 0 0]  스킬치명-[color:{4}%;1 0 1] 평타-[color:{5}%;1 0 1] 총평캔-{6}회 {8}% 최대평캔-{7}회", s1, s2, s3, user.DPS, user.percentCritical, user.percentNormal, user.TotalNormalCancelCount, user.MaxNormalCancelCount, user.PercentNormalCancel));
        }

        private void 클립보드순위MenuItem_Click(object sender, EventArgs e)
        {
            // 2013-03-12 클립보드를 통해 전달할수 있는 최대 문자 길이
            int maxLength = 255;
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (ListViewItem item in this.listView1.Items)
            {
                String playerName = item.SubItems[2].Text;
                User user = data.GetPlayerByDisplayName(playerName);
                sb.Append((index + 1) + "등:" + user.DisplayName + "(" + user.TotalDamage);
                if (user.DisplayName != LogParser.Noname) sb.Append("-" + SkillDictionary.GetClassStringShort(user.Class));
                sb.Append(")  ");
                index++;
                if (sb.Length > maxLength) break;
            }
            string str = (sb.Length > maxLength) ? sb.ToString(0, maxLength) : sb.ToString();
            if ( str.Length > 0 ) ClipboardSetText(str);
        }
        #endregion

        #region 시작 중지 버튼
        //private DateTime startDateTime;
        //private Timer timer;
        private Thread timerThread;
        private int workingTime; //측정시간

        //일반 시작
        private void normalStartButton_Click(object sender, EventArgs e)
        {
            indun = null;
            LogParser.NormalStartFlag = true;
            logParser.Start(Logic.AionLogFileName);
        }


        //중지 버튼 클릭
        private void stopButton_Click(object sender, EventArgs e)
        {
            LogParser.StopByWhy = EnumStopBy.ByUser;
            if (timerThread != null)
            {
                timerThread.Abort();
                timerThread = null;
                LogParser.OneClassFlag = this.oneClassCheckbox.Checked = oneClassCheckBoxBeforeTimerStart;
            }
            logParser.Stop();
        }

        //분간 측정 시작
#if false
        private void timeStartButton_Click(object sender, EventArgs e)
        {
            indun = null;
            int timeText = 0;
            try
            {
                timeText = Int32.Parse(this.workingTimeTextBox1.Text);
                if (timeText < 1 || timeText > 10) throw new Exception();
            }
            catch
            {
                MessageBox.Show(this, "1부터 10까지 설정해주세요", "시간 설정 오류");
                return;
            }
            this.workingTime = timeText * 60;
            //this.workingTime = 5;
            if (timerThread != null)
            {
                timerThread.Abort();
                timerThread = null;
            }
            timerThread = new Thread(new ThreadStart(ThreadMethod));
            timerThread.Start();

            this.normalStartButton.Enabled = false;
            //this.timeStartButton.Enabled = false;
            this.분간측정ToolStripMenuItem.Enabled = false;
            this.stopButton.Enabled = false;
            
            // 모두 지우고, 클래별1인 체크
            PlayerListClear();
            oneClassCheckBoxBeforeTimerStart = LogParser.OneClassFlag;
            this.oneClassCheckbox.Checked = true;
            LogParser.OneClassFlag = true;
        }
#endif 

        private void ThreadMethod()
        {
            try
            {
                int tmp = 5;
                do
                {
                    SetStatusLabelByOtherThread("동작: " + (tmp) + "초 후 시작");
                    tmp--;
                    Thread.Sleep(1000);
                } while ( tmp > 0);
                DateTime startDateTime = DateTime.Now;
                LogParser.NormalStartFlag = false;
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    logParser.Start(Logic.AionLogFileName);
                }));
                SetStatusLabelByOtherThread("동작: 시작중");

                do
                {
                    TimeSpan timeSpan = DateTime.Now - startDateTime;
                    int seconds = timeSpan.Minutes * 60 + timeSpan.Seconds;
                    //SetStatusLabelByOtherThread("측정중: " + timeSpan.Minutes + "분 " + timeSpan.Seconds + "초");
                    SetStatusLabelByOtherThread("측정중: " + seconds + "초/" + this.workingTime + "초");
                    if (seconds >= workingTime) break;
                    Thread.Sleep(1000);
                } while (true);

                // 측정완료
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    LogParser.StopByWhy = EnumStopBy.ByTimerThread;
                    logParser.Stop();
                    LogParser.OneClassFlag = this.oneClassCheckbox.Checked = oneClassCheckBoxBeforeTimerStart;
                    // 딜 시간을 통일
                    data.SetDealTime(this.workingTime);
                }));
            }
            catch (Exception ee)
            {
                ee.ToString();
            }
        }

        #endregion 

        #region 내정보 탭 관련
        private System.Windows.Forms.Timer tt;
        private List<string> debuffList;
        
        private void logParser_MyStatusEvent(object sender, MyStatusEventArgs e)
        {
            if (e.Status == MyStatus.Debuff1)
            {//상태가 도ㅒ씁니다
                AddMyStatusLabel(e.Argument);
            }
            else if (e.Status == MyStatus.Status1)
            { //속도 감소
                if (e.Argument.IndexOf("속도가 감소") != -1)
                {
                    AddMyStatusLabel(e.Argument.Substring(0, 4) + "감소");
                }
            }
            else if (e.Status == MyStatus.Repair1)
            {
                // 이동속도감소, 공격속도감소 회복됐습니다
                RemoveMyStatusLabel(e.Argument + "감소");

            }
            else if (e.Status == MyStatus.Repair2)
            {
                //벗어났씁니다.
                RemoveMyStatusLabel(e.Argument);
            }
            else if (e.Status == MyStatus.Debuff2)
            {
                if (e.Argument.StartsWith("빙설의 갑주"))
                {
                    AddMyStatusLabel("빙갑");

                    //마지막 빙갑으로부터 5초가가 되어야함
                    if (tt != null)
                    {
                        tt.Dispose();
                        tt = null;
                    }
                    tt = new System.Windows.Forms.Timer();
                    tt.Interval = 5 * 1000;
                    tt.Tick += delegate
                    {
                        logParser_MyStatusEvent(this, new MyStatusEventArgs(null, DateTime.Now, MyStatus.Debuff2Release, "빙갑"));
                        tt.Dispose();
                        tt = null;
                    };
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        tt.Start();
                    }));
                }
            }
            else if (e.Status == MyStatus.BeBeaten1 || e.Status == MyStatus.BeBeaten2 || e.Status == MyStatus.BeBeaten3)
            {
                int damage = Int32.Parse(e.Argument);
                int idx = this.comboBoxMyInfoDamage.SelectedIndex;
                if (idx != 0)
                {
                    if (damage > (idx+1) * 1000)
                    {
                        String str = "피해" + damage;
                        AddMyStatusLabel(str);

                        System.Timers.Timer damageTimer = new System.Timers.Timer(2000);
                        damageTimer.AutoReset = false;
                        damageTimer.Elapsed += delegate
                        {
                            logParser_MyStatusEvent(this, new MyStatusEventArgs(null, DateTime.Now, MyStatus.Debuff2Release, str));
                        };
                        this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                        {
                            damageTimer.Start();
                        }));
                    }
                }
            }
            else if (e.Status == MyStatus.Debuff2Release)
            {
                RemoveMyStatusLabel(e.Argument);
                //return;
            }
            else if (e.Status == MyStatus.Die)
            {
                this.debuffList.Clear();
            }
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                SetMyStatusLabel();
                if ( e.Status != MyStatus.Debuff2Release) this.textBoxMyInfo.AppendText(e.ToString() + "\r\n");
            }));
        }

        private bool AddMyStatusLabel(String str)
        {
            bool bExist = false;
            try
            {
                foreach (String s in debuffList)
                {
                    if (s == str)
                    {
                        bExist = true;
                        break;
                    }
                }
                if (bExist == false)
                {
                    debuffList.Add(str);
                }
            }
            catch { }
            return !bExist;
        }

        private void RemoveMyStatusLabel(String str)
        {
            try
            {
                if (debuffList.Count == 0)
                {

                }
                else
                {
                    bool bExist = false;
                    foreach (String s in debuffList)
                    {
                        if (s == str)
                        {
                            bExist = true;
                            break;
                        }
                    }
                    if (bExist == true)
                    {
                        debuffList.Remove(str);
                    }
                }
            }
            catch { }
        }

        private void SetMyStatusLabel()
        {
            String str = "";
            if (debuffList.Count > 0)
            {
                foreach (string s in debuffList)
                {
                    str += (s + " ");
                }
                str = str.Trim();
            }
            if (str == "")
            {
                this.myStatusLabel.Text = str;
                this.myStatusLabel.BackColor = Color.White;
            }
            else
            {
                this.myStatusLabel.Text = str;
                this.myStatusLabel.BackColor = Color.Yellow;
            }
        }
        #endregion

        #region 타이머 아이온홈페이지
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowTimer();
        }

        private void ShowTimer()
        {
            TimerForm timerForm = new TimerForm();
            ShowTimer(timerForm);
        }

        public TimerForm룬의보호탑 timerFormDefence = null;
        private void ShowTimer룬의보호탑()
        {
            timerFormDefence = new TimerForm룬의보호탑(this);
            ShowTimer(timerFormDefence);
        }

        private void ShowTimer(Form timerForm)
        {
            timerForm.TopMost = true;
            int x = this.Location.X + this.Width;
            if (x + timerForm.Width > Screen.AllScreens[0].WorkingArea.Width)
            {
                x = this.Location.X - timerForm.Width;
            }
            //2013-09-04
            // this.Location.Y 가 음수이면..창을 내려놓은걸로 간주하여 0,0에 뿌리자
            if (this.Location.Y < 0)
            {
                timerForm.Location = new Point(0, 0);
            }
            else
            {
                timerForm.Location = new Point(x, this.Location.Y);
            }
            timerForm.Show();
        }


        private void aION홈페이지ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GotoHomepage(1);
        }
        #endregion

        #region 정보탭 홈페이지
        private void linkLabelGotoHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GotoHomepage(0);
        }
        public void GotoHomepage(int where)
        {
            String url = null;
            if (where == 0)
            {
                url = "";
            }
            else if (where == 1)
            {
                url = "http://aion.plaync.co.kr";
            }
            GotoHomepage(url);
        }
        public void GotoHomepage(string url)
        {
            System.Diagnostics.Process IEProcess = new System.Diagnostics.Process();
            IEProcess.StartInfo.FileName = "iexplore.exe";
            IEProcess.StartInfo.Arguments = url;
            IEProcess.Start();
        }
        #endregion

        #region update에서 호출하는 메소드
        private void linkLabelUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            update.ServerName = (String)this.comboBoxSelectServer.SelectedItem;
            update.Start(1);
        }

        public void InsertUpdateProgress()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.labelUpdateStatus.Text = "";
                this.progressBar1.Value = 0;
                this.panelUpdate.Controls.Add(this.progressBar1);
            }));
        }

        public void DeleteUpdateProgress()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.panelUpdate.Controls.Remove(this.progressBar1);
            }));
        }

        public void SetUpdateLabel(String str)
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.labelUpdateStatus.Text = str;
            }));
        }

        public void SetUpdateProgress(int i)
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.progressBar1.Value = i;
            }));
        }

        public void ShowInformationTab()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.tabControl1.SelectedIndex = 5;
            }));
        }

        public void Exit()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                Application.Exit();
            }));
        }

        #endregion

        #region 클래스별보기
        private const String 클래스별보기기본 = "전체";
        private String 클래스별보기상태 = 클래스별보기기본;

        void item_Click(object sender, EventArgs e)
        {
            String className = ((ToolStripMenuItem)sender).Text;
            if (클래스별보기상태 == className) return;
            클래스별보기상태 = className;

            foreach (ToolStripMenuItem item in this.toolStripClassViewItems)
            {
                if (item == sender) item.Checked = true;
                else item.Checked = false;
            }
            this.listView1.Items.Clear();
            foreach (User user in data.UserList)
            {
                if (sender == this.toolStripClassViewItems[0] || SkillDictionary.GetClassString(user.Class) == className)
                {
                    this.listView1.Items.Add(user.ListItem);
                    user.ListItem.Selected = false;
                }
            }
            this.Sort();
        }
        #endregion```


        #region 기록 업로드
        private void ShowPopup(String str)
        {
            this.labelPopup.Text = str;
            if (!this.tabPage1.Controls.Contains(this.panelPopup))
            {
                this.tabPage1.Controls.Add(this.panelPopup);
            }
        }   

        private void HidePopup()
        {
            this.tabPage1.Controls.Remove(this.panelPopup);
        }

        // 0: 모두 false, 1:시간등록가능, 2:인던등록가능 3: 둘다 가능
        private int RecordType = 0;
        private void SetAvailableRecord(int args)
        {
            this.RecordType = args;
            this.panelPopup.Controls.Remove(this.progressBarBoss);
            if (RecordType == 0)
            {
                HidePopup();
                this.측정기록등록ToolStripMenuItem.Enabled = false;
                this.인던공략확인ToolStripMenuItem.Enabled = false;
                this.인던공략등록ToolStripMenuItem.Enabled = false;
            }
            else if (RecordType == 1)
            {
                ShowPopup(this.workingTime + "초 측정 기록을 등록할 수 있습니다");
                this.측정기록등록ToolStripMenuItem.Enabled = true;
            }
            else if (RecordType == 2)
            {
                ShowPopup("인던공략 정보를 보기/등록할 수 있습니다");
                this.인던공략확인ToolStripMenuItem.Enabled = true;
                this.인던공략등록ToolStripMenuItem.Enabled = true;
            }
            else if (RecordType == 3)
            {
                ShowPopup("개인기록 및 인던공략 정보를 보기/등록할 수 있습니다");
                this.측정기록등록ToolStripMenuItem.Enabled = true;
                this.인던공략확인ToolStripMenuItem.Enabled = true;
                this.인던공략등록ToolStripMenuItem.Enabled = true;
            }
            else if (RecordType == 4)
            {
                // 기록등록은 안하지만, 정보만 본다 // 룬의보호탑
                ShowPopup("인던공략 정보를 확인할 수 있습니다");
                this.측정기록등록ToolStripMenuItem.Enabled = false;
                this.인던공략확인ToolStripMenuItem.Enabled = true;
                this.인던공략등록ToolStripMenuItem.Enabled = false;
            }
        }

        private void 측정기록등록ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // 메뉴 클릭
            // 이름이 없을경우, 서버가 없을경우
            // 클래스가 없을경우
            // 본인이 아닐경우
            // 선택하지 않을경우
            // 몇등입니다 표시
            // 서버 이름 클래스 측정시간 DPS 총데미지 공격횟수 스킬비율 평타비율 치명비율
            // 날짜 User여부 주요타겟
            // 통합서버는?

            // 1단계 아무도 선택하지 않을 경우
            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0)
            {
                ShowPopup("등록하실 기록을 선택하세요");
                return;
            }
            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            측정기록등록(playerName);
        }

        private void 측정기록등록(string playerName) 
        {
            //String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            User user = data.GetPlayerByDisplayName(playerName);
            playerName = user.Name;

            //2-0단계 이미 올린기록
            if (user.SendRecord)
            {
                ShowPopup("등록한 기록입니다");
                return;
            }

            // 2단계 미분류를 선택할 경우
            if (playerName == LogParser.Noname)
            {
                ShowPopup(LogParser.Noname + " 기록은 등록할 수 없습니다");
                return;
            }

            // 2-1단계 평타만 쳐서 올릴경우
            if (user.Class == ClassType.NONE)
            {
                ShowPopup("평타 기록은 등록할 수 없습니다");
                return;
            }

            //용제는 같은 클래스라도 올린다.
            if (RecordType == 1)
            {
                // 3단계 같은 클래스가 2명이상이라면 올릴수 없다
                int sameClassCount = 0;
                foreach (User u in data.UserList)
                {
                    if (u.Class == user.Class)
                    {
                        sameClassCount++;
                    }
                    if (sameClassCount > 1)
                    {
                        ShowPopup("같은 클래스가 2명 이상인 경우 등록할 수 없습니다");
                        return;
                    }
                }
            }

            // 통합서버 고려
            String[] temp = playerName.Split('-');
            String userName = "";
            String userServer = "";

            if (temp.Length == 1)
            {
                userName = playerName;
                userServer = (String)this.comboBoxSelectServer.SelectedItem;
            }
            else
            {
                userName = temp[0];
                userServer = temp[1];
            }

            // 4단계 서버가 없는 경우
            if (userServer == "")
            {
                ShowPopup("환경설정 탭에서 서버정보를 입력하세요");
                return;
            }

            // 5단계 이름이 <당신>인경우
            if (userName == LogParser.DefaultMyname)
            {
                ShowPopup("시작버튼을 누르시고 아이온 채팅창에 //이름 입력하여 이름 설정 후 재측정하세요");
                return;
            }

            ShowPopup("등록중");

            // 본인, 서버, 이름, user, 
            //int ret = update.SendDealRecord((userName == LogParser.Myname), userServer, userName, user, workingTime);
            int ret = update.SendDealRecord((userName == LogParser.Myname), userServer, userName, user, user.TotalDealTime, RecordType);
            if (ret <= 0)
            {
                ShowPopup("등록에 실패하였습니다");
                return;
            }
            else if (ret == 1)
            {
                if ( RecordType == 1) ShowPopup("등록하였습니다");
                else if (RecordType == 3) ShowPopup("개인기록을 등록하였습니다");
                user.SendRecord = true;
                return;
            }
            else
            {
                return;
            }
        }
        private void 인던공략등록ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (indun.SendRecord)
            {
                ShowPopup("등록한 기록입니다");
                return;
            }

            // 내이름이 없을경우
            // 5단계 이름이 <당신>인경우
            if (LogParser.Myname == LogParser.DefaultMyname)
            {
                ShowPopup("시작 - //이름 입력하여 이름을 설정하세요");
                //return;
            }

            // 4단계 서버가 없는 경우
            if ((String)this.comboBoxSelectServer.SelectedItem == "")
            {
                ShowPopup("환경설정 탭에서 서버정보를 입력하세요");
                return;
            }

            User user = indun.UserList[0]; //딜 1등
            String playerName = user.Name;
            String[] temp = playerName.Split('-');

            if (temp.Length == 1)
            {
                // 통합서버가 아니다.

            }
            else
            {
                // 통합서버이다.
            }
            ShowPopup("등록중");

            // 본인, 서버, 이름, user,
            int ret = update.SendRecordInDun(indun);
            if (ret <= 0)
            {
                ShowPopup("등록에 실패하였습니다");
                return;
            }
            else if (ret == 1)
            {
                ShowPopup("인던기록을 등록하였습니다");
                indun.SendRecord = true;
                return;
            }
            else
            {
                return;
            }
        }

        #endregion

        #region 로그파일 열기
        private void ToolStripMenuItemLogFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.InitialDirectory = Logic.AionPath;
            openPanel.Filter = "log(*.log)|*.log|txt(*.txt)|*.txt|All(*.*)|*.*";
            
            if(openPanel.ShowDialog() == DialogResult.OK)
            {
                String file = openPanel.FileName;
                try
                {
                    FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);
                    
                    Regex defaultRegex = new Regex("(?<time>[^a-zA-Z]+) : (?<ignore>.+)", RegexOptions.Compiled);
                    String str = null;
                    MatchCollection matches = null;
                    bool okFile = true;
                    for (int i = 0; i < 10; i++)
                    {
                        str = streamReader.ReadLine();
                        if (String.IsNullOrEmpty(str)) continue;
                        matches = defaultRegex.Matches(str);
                        if (matches.Count == 0)
                        {
                            okFile = false;
                            break;
                        }
                    }
                    if (okFile)
                    {
                        streamReader.Close();
                        fileStream.Close();
                        StartByLogFile(file);
                    }
                    else
                    {
                        MessageBox.Show("정상적인 로그 파일이 아님", "경고", MessageBoxButtons.OK,
               MessageBoxIcon.Warning);
                    }
                }
                catch 
                {
                    //throw ee;
                }
            }
        }

        private void StartByLogFile(String file)
        {
            //정지 버튼
            stopButton_Click(null, null);
            Thread.Sleep(500);

            PlayerListClear();
            indun = null; //2013-05-07
            LogParser.NormalStartFlag = true;
            logParser.Start(file);
        }
        #endregion


        #region 루나디움
        private InstanceDungeon indun;

        // 인던 처리
        void logParser_InstanceDungeonEvent(object sender, InstanceDungeonEventArgs e)
        {
            if (LogParser.NormalStartFlag == false) return;
            if (e.Command == 0)
            { // 지역 입장
                if (indun == null && InstanceDungeon.IsProcessDungeon(e.Argument1))
                {
                    indun = InstanceDungeon.MakeInstance(e.Argument1, this);
                    
                    // 루나디움 시작
                    // 모두지우기. 클래스별1인 체크, 상태:루나디움 측정중
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        SetStatusLabel("상태: " + e.Argument1 + " 측정중");
                        PlayerListClear();
                        LogParser.OneClassFlag = this.oneClassCheckbox.Checked = true;
                    }));
                    indun.AddEvent(e);
                }
                else if (indun!= null && indun.DungeonName != e.Argument1)
                {
                    // 지금 루나인데 루나가 아닌곳에 입장했다. 측정중지
                    // 재접할수도 있다.
                    
                    this.SetStatusLabelByOtherThread("상태: 동작중");

                    //룬의보호탑 실패
                    if (indun != null && indun is InstanceDungeon룬의보호탑 && timerFormDefence != null)
                    {
                        timerFormDefence.ProcessFail();
                    }
                    indun = null;
                }
            }
            #region 채팅으로 인던처리
            else if ( e.Command == 1) // 채팅
            {
                if (indun != null && indun is InstanceDungeon루나디움)
                {
                    // 2013-05-07 0.2.1.2
                    if (e.Argument1 == "저주받은 마녀 그렌달" && e.Argument2.Contains("어서 물러가라!"))
                    {
                        //타이머시작
                        indun.AddEvent(e);
                        this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                        {
                            ShowTimer();
                        }));
                    }
                    else if (e.Argument1 == indun.BossName && e.Argument2.Contains("하지만 여기까지다!"))
                    {
                        indun.AddEvent(e);
                    }
                    else if (e.Argument1 == indun.BossName && e.Argument2.Contains("끝나다니…"))
                    {
                        ProcessIndunEnd(e);
                    }
                }
                else if (indun != null && indun is InstanceDungeon용제의안식처)
                {
                    if (e.Argument1 == indun.BossName && e.Argument2.Contains("죽지… 않는다…"))
                    {
                        ProcessIndunEnd(e);
                    }
                }
            }
            #endregion
            else if (e.Command == 2)
            {
                // 모든 경험치를 얻었습니다 가 올꺼다. // 채팅으로 안끝날수도 있다. 버그
                if (indun != null && e.Argument1 == indun.BossName)
                {
                    ProcessIndunEnd(e);
                }
            }
        }

        private void ProcessIndunEnd(InstanceDungeonEventArgs e)
        {
            try
            {
                if (indun.bProcessEnd == false)
                {
                    indun.bProcessEnd = true;
                    indun.AddEvent(e);
                    this.SetStatusLabelByOtherThread("상태: " + indun.DungeonName + " 측정완료");
                    // 측정완료
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        _SkillSorter.SortColumn = 4;
                        _SkillSorter.SortOrder = ListViewSortOrder.Descending;
                        Sort();
                        indun.GetInfo();
                        LogParser.StopByWhy = EnumStopBy.ByInstanceDungeon;
                        logParser.Stop();
                        //보호탑
                        if (this.timerFormDefence != null) this.timerFormDefence.Finish();
                    }));
                }
            }
            catch (Exception ee)
            {
                ee.ToString();
            }
        }

        
        private void SetBossPercent(int damage, int totalDamage)
        {
            double d = damage * 100.0 / totalDamage;
            if (d == 0)
            {
                this.progressBarBoss.Value = 100;
                this.panelPopup.Controls.Add(this.progressBarBoss);
            }
            else if (d < 100)
            {
                this.progressBarBoss.Value = 100-(int)d;
                ShowPopup( String.Format("{0:0.00}", (100.0-d)) + "% 남음");
            }
            else
            {
                this.panelPopup.Controls.Remove(this.progressBarBoss);
                HidePopup();
            }
        }
         

        private InstanceDungeonInfoForm instanceDungeonInfoForm;
        private void 인던공략확인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (indun != null)
            {
                if (instanceDungeonInfoForm != null)
                {
                    instanceDungeonInfoForm.Dispose();
                }
                instanceDungeonInfoForm = new InstanceDungeonInfoForm(this);
                instanceDungeonInfoForm.SetIndun(this.indun);
                instanceDungeonInfoForm.Show();
            }
        }
        #endregion



        #region 번역
        private Trans trans;
        private void checkBox마족_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox천족.Checked = !this.checkBox마족.Checked;
        }

        private void checkBox천족_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox마족.Checked = !this.checkBox천족.Checked;
        }

        private void buttonTrans_Click(object sender, EventArgs e)
        {
            if (trans == null)
            {
                trans = new Trans();
            }
            String str = trans.Translate(this.textBoxSource.Text, this.checkBox마족.Checked);
            if (str == null)
            {
                this.labelTransWarning.Text = "영어만 입력해주세요";
            }
            else if (str != "")
            {
                this.labelTransWarning.Text = "";
                this.textBoxResult.Text = str;
                ClipboardSetText(str);
            }
        }

        private void comboBoxSentence_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxSentence.SelectedIndex != 0)
            {
                this.textBoxSource.Text = (String)this.comboBoxSentence.SelectedItem;
            }
        }
        #endregion


        public void ShowNotice(String str)
        {

            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                NoticeForm f = new NoticeForm(this);
                f.SetNotice(str);
                ShowForm(f, 2);
                f.Show();
            }));
            
        }


        #region 기타
        public String GetServer()
        {
            return (String)this.comboBoxSelectServer.SelectedItem;
        }
        private void SetStatusLabel(String str)
        {
            this.statusLabel.Text = str;
        }

        private void SetStatusLabelByOtherThread(String str)
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                SetStatusLabel(str);
            }));
        }

        public void ClipboardSetText(String str)
        {
            try
            {
                Clipboard.SetText(str);
            }
            catch 
            {
            }
        }

        public void ShowForm(Form form, int where)
        {
            form.TopMost = true;
            int x = form.Location.X;
            int y = form.Location.Y;

            if (where == 1)
            {
                x = this.Location.X + this.Width;
                if (x + form.Width > Screen.AllScreens[0].WorkingArea.Width)
                {
                    x = this.Location.X - form.Width;
                }
                y = this.Location.Y;
            }
            else if (where == 2) //센타
            {
                x = this.Location.X;
                x += ((this.Width - form.Width) / 2);
                y = this.Location.Y;
                y += ((this.Height - form.Height) / 2);

            }
            form.Location = new Point(x, y);
        }
        #endregion

        private void 로그파일생성여부ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ret = logic.isAvailableLogFile();
            String str = (ret) ? "로그 파일을 생성합니다." : "로그 파일을 생성하지 않습니다";
            MessageBox.Show(this, str, "로그파일생성여부");
        }

        private void 네트워크패핑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PingForm pingForm = new PingForm(this);
            pingForm.View();
        }

        private void aION런처ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.OnlyUseForAionLauncherNoticeForm.GoLauncher();
            }
            catch { }
        }

        private void 클립보드에AION모바일웹링크복사ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String str = "[ncmsg:http://m.aion.plaync.com;모바일 웹]";
            this.ClipboardSetText(str);
        }

        private HPMPSkillListForm hpSkillListForm = null;

        private void 생명력ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRecoverForm(true);
        }

        private void 정신력ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRecoverForm(false);
        }

        private void ShowRecoverForm(bool isHp)
        {
            if (hpSkillListForm == null)
            {
                hpSkillListForm = new HPMPSkillListForm(isHp);
            }
            else
            {
                hpSkillListForm.Dispose();
                hpSkillListForm = new HPMPSkillListForm(isHp);
            }
            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0) return;
            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            User player = data.GetPlayerByDisplayName(playerName);
            SetSkillListForm(hpSkillListForm);
            if (player != null) hpSkillListForm.Show(player);
        }

        private HealSkillListForm healSkillListForm = null;
        private void 치유스킬목록ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (healSkillListForm == null)
            {
                healSkillListForm = new HealSkillListForm();
            }
            else
            {
                healSkillListForm.Dispose();
                healSkillListForm = new HealSkillListForm();
            }
            ListView.SelectedIndexCollection collection = this.listView1.SelectedIndices;
            if (collection.Count == 0) return;
            String playerName = this.listView1.Items[collection[0]].SubItems[2].Text;
            User player = data.GetPlayerByDisplayName(playerName);
            SetSkillListForm(healSkillListForm);
            if (player != null) healSkillListForm.Show(player);
        }

        private void SetSkillListForm(Form form)
        {
            form.Size = new System.Drawing.Size(this.Width, this.Height);
            int x = this.Location.X + this.Width;
            if (x + form.Width > Screen.AllScreens[0].WorkingArea.Width)
            {
                x = this.Location.X - form.Width;
            }
            form.Location = new Point(x, this.Location.Y);
            form.TopMost = this.TopMost;
            form.Opacity = this.Opacity;
        }

        //분간 측정
        void ii_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ii = (ToolStripMenuItem)sender;
            int i = 0;
            for ( ; i < 10; i++)
            {
                if (ii.Text == (i + 1) + "분")
                {
                    break;
                }
            }

            indun = null;

            int timeText = i + 1;
            /*
            try
            {
                timeText = Int32.Parse(this.workingTimeTextBox1.Text);
                if (timeText < 1 || timeText > 10) throw new Exception();
            }
            catch
            {
                MessageBox.Show(this, "1부터 10까지 설정해주세요", "시간 설정 오류");
                return;
            }
            */

            this.workingTime = timeText * 60;
            //this.workingTime = 5;
            if (timerThread != null)
            {
                timerThread.Abort();
                timerThread = null;
            }
            timerThread = new Thread(new ThreadStart(ThreadMethod));
            timerThread.Start();

            this.normalStartButton.Enabled = false;
            //this.timeStartButton.Enabled = false;
            this.분간측정ToolStripMenuItem.Enabled = false;
            this.stopButton.Enabled = false;

            // 모두 지우고, 클래별1인 체크
            PlayerListClear();
            oneClassCheckBoxBeforeTimerStart = LogParser.OneClassFlag;
            this.oneClassCheckbox.Checked = true;
            LogParser.OneClassFlag = true;
        }

        private void resetToolStripButton_Click(object sender, EventArgs e)
        {
            PlayerListClear();
        }



        private void hiddenStartTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.hiddenStartTextBox.Text == "뜨락")
            {
//                this.normalStartButton.Enabled = true;
//                this.toolStripSplitButton1.Enabled = true;

                if ( this.tabControl1.Controls.Contains(hiddenTabPage) == false)
                    this.tabControl1.Controls.Add(this.hiddenTabPage);
            }
            else if (this.hiddenStartTextBox.Text == "")
            {
                if (this.tabControl1.Controls.Contains(hiddenTabPage))
                    this.tabControl1.Controls.Remove(this.hiddenTabPage);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GotoHomepage(this.accountUser1linkLabel.Text);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GotoHomepage(this.accountUser2linkLabel.Text);
        }

        public AionWebHandler webHandler;
        private void accountButton_Click(object sender, EventArgs e)
        {
            if ( webHandler == null) webHandler = new AionWebHandler(this);

            string[] tmp = this.accountTextBox.Text.Split(' ');
            if (tmp.Length == 3 && tmp[0].StartsWith("//계정"))
            {
                this.accountResultTextBox.Text = "Wait";

                Thread t = new Thread(new ThreadStart(delegate
                {
                    int ret = webHandler.CompareSameAccount(tmp[1], tmp[2]);
                    this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                    {
                        this.accountResultTextBox.Text = ((ret==0)?"같은 계정" : "다른 계정");
                        this.accountUser1linkLabel.Text = "http://sandbox.plaync.com/" + webHandler.user1.GUID;
                        this.accountUser2linkLabel.Text = "http://sandbox.plaync.com/" + webHandler.user2.GUID;  
                    }));
                }));
                t.Start();
            }
            else
            {
                this.accountResultTextBox.Text = "?";
            }
        }

        private void buttonWorkingServerSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                this.textBoxWorkingServerSelect.Text = folderName;
            }

        }

    }
}

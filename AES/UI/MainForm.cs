using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace AES
{
    public partial class MainForm : Form
    {
        public XMLHandler xmlHandler = null;
        private GenerateDataFile generageDataFile = null;
        public Stat CurrentStat;
        private Update update = null;
        public DataFileHandle FileHandle = null;
        public String Version;
        private bool bRunByUpdateModule = false;
        private ProgressForm progressForm = null;

        #region 생성자
        public MainForm(string[] args)
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Information.Initialize();
            InitializeComponent();
            InitializeComponentManually();
            TreeInitialize();
            this.xmlHandler = new XMLHandler(this);
            this.xmlHandler.XMLEventHandler += new EventHandler(xmlHandler_XMLEventHandler);
            this.generageDataFile = new GenerateDataFile(this);
            this.generageDataFile.GDFEventHandler += new EventHandler(generageDataFile_GDFEventHandler);
            update = new Update(this);
            FileHandle = new DataFileHandle(this);
            CurrentStat = new Stat();

            //MessageBox.Show(args.Length + "");
            if (args.Length > 0)
            {
                if (args[0].CompareTo("-update") == 0)
                {
                    //MessageBox.Show(args[1]);
                    bRunByUpdateModule = true;
                    update.RemoveBatchFile();
                    // 2013-05-08 업데이트로 왔다.
                    //MessageBox.Show(args[1]);
                    ProcessUpdate(args[1]);
                }
            }
        }

        private void TreeInitialize()
        {
            foreach (string s in Information.FirstType)
            {
                this.treeView1.Nodes.Add(s);
            }

            for (int i = 0; i < Information.SecondType.Count; i++)
            {
                for (int j = 0; j < Information.SecondType[i].Length; j++)
                {
                    this.treeView1.Nodes[i].Nodes.Add(Information.SecondType[i][j]);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Information.ArmorSecondType.Length; j++)
                {
                    this.treeView1.Nodes[1].Nodes[i].Nodes.Add(Information.ArmorSecondType[j]);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Information.SkillSecondType.Length; j++)
                {
                    this.treeView1.Nodes[9].Nodes[i].Nodes.Add(Information.SkillSecondType[j]);
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
                    else if (t[0] == "FlagCommon")
                    {
                        Properties.Settings.Default.FlagCommon = bool.Parse(t[1]);
                    }
                    else if (t[0] == "FlagRare")
                    {
                        Properties.Settings.Default.FlagRare = bool.Parse(t[1]);
                    }
                    else if (t[0] == "FlagLegend")
                    {
                        Properties.Settings.Default.FlagLegend = bool.Parse(t[1]);
                    }
                    else if (t[0] == "FlagUnique")
                    {
                        Properties.Settings.Default.FlagUnique = bool.Parse(t[1]);
                    }
                    else if (t[0] == "FlagEpic")
                    {
                        Properties.Settings.Default.FlagEpic = bool.Parse(t[1]);
                    }
                    else if (t[0] == "FlagMythic")
                    {
                        Properties.Settings.Default.FlagMythic = bool.Parse(t[1]);
                    }
                    else if (t[0] == "ItemLevel")
                    {
                        Properties.Settings.Default.ItemLevel = Int32.Parse(t[1]);
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
            this.comboBoxLevel.SelectedIndex = 3;
            bool bExistConfigFile = false;
            bExistConfigFile = Properties.Settings.Default.ThisFileExist;
            if (bExistConfigFile)
            {
                int w = Properties.Settings.Default.WindowWidth;
                int h = Properties.Settings.Default.WindowHeight;
                this.Width = (w < this.Width) ? this.Width : w;
                this.Height = (h < this.Height) ? this.Height : h;

                int x = Properties.Settings.Default.WindowX;
                int y = Properties.Settings.Default.WindowY;
                x = (x < 0 || x >= Screen.AllScreens[0].WorkingArea.Width) ? 0 : x;
                y = (y < 0 || y >= Screen.AllScreens[0].WorkingArea.Height) ? 0 : y;
                this.Location = new Point(x, y);

                this.checkBox일반.Checked = Properties.Settings.Default.FlagCommon;
                this.checkBox희귀.Checked = Properties.Settings.Default.FlagRare;
                this.checkBox전승.Checked = Properties.Settings.Default.FlagLegend;
                this.checkBox유일.Checked = Properties.Settings.Default.FlagUnique;
                this.checkBox영웅.Checked = Properties.Settings.Default.FlagEpic;
                this.checkBox신화.Checked = Properties.Settings.Default.FlagMythic;

                this.comboBoxLevel.SelectedIndex = Properties.Settings.Default.ItemLevel;
            }

            if (bRunByUpdateModule == false)
            {
                update.Start(0);
            }
            else
            {
                update.Start(2);
            }
        }
        #endregion

        #region MainForm_FormClosed
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (xmlHandler != null) xmlHandler.Stop();
            if (update != null) update.Stop();
            if (generageDataFile != null) generageDataFile.Stop();
            SettingsSave();
        }

        public void SettingsSave()
        {
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WindowX = this.Location.X;
            Properties.Settings.Default.WindowY = this.Location.Y;
            Properties.Settings.Default.FlagCommon = this.checkBox일반.Checked;
            Properties.Settings.Default.FlagRare = this.checkBox희귀.Checked;
            Properties.Settings.Default.FlagLegend = this.checkBox전승.Checked;
            Properties.Settings.Default.FlagUnique = this.checkBox유일.Checked;
            Properties.Settings.Default.FlagEpic = this.checkBox영웅.Checked;
            Properties.Settings.Default.FlagMythic = this.checkBox신화.Checked;
            Properties.Settings.Default.ItemLevel = this.comboBoxLevel.SelectedIndex;
            Properties.Settings.Default.ThisFileExist = true;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region 아이템 트리 관련
        private void 로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bool ret = generageDataFile.ExistDataFile();
                DialogResult dr = DialogResult.None;
                if (ret == false)
                {
                    dr = MessageBox.Show("데이터 파일이 없습니다.\r데이터 파일을 먼저 생성하세요", "아이템 파일 로딩", MessageBoxButtons.OK);
                    return;
                }
                if (ret == false || dr == DialogResult.Yes)
                {
                    progressForm = new ProgressForm();
                    progressForm.Text = "아이템 정보파일 생성";
                    ShowForm(progressForm, 2);
                    progressForm.Show();

                    generageDataFile.GenerateData(false);
                }

                dr = DialogResult.None;
                if (xmlHandler.ItemList != null && xmlHandler.ItemList.Count != 0)
                {
                    dr = MessageBox.Show("이미 아이템 정보를 로딩하였습니다.\r다시 로딩하시겠습니까?", "아이템 파일 로딩", MessageBoxButtons.YesNo);
                }
                if (xmlHandler.ItemList == null || xmlHandler.ItemList.Count == 0 || dr == DialogResult.Yes)
                {
                    progressForm = new ProgressForm();
                    progressForm.Text = "아이템 파일 로딩";
                    ShowForm(progressForm, 2);
                    progressForm.Show();
                    this.xmlHandler.Load();
                }
            }
            catch
            {

            }
        }

        private void 아이템트리새로고침ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateTree();
        }

        private void button7_Click(object sender, EventArgs e) // 환경설정
        {
            this.CreateTree();
        }

        public void CreateTree()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.treeView1.Nodes.Clear();
                this.TreeInitialize();

                if (xmlHandler.ItemList != null)
                {
                    IDictionaryEnumerator items = xmlHandler.ItemList.GetEnumerator();
                    while (items.MoveNext())
                    {
                        EntityItem i = (EntityItem)items.Value;
                        TreeNode node = new TreeNode();
                        node.Tag = i;
                        node.Text = i.GetTreeName();
                        TreeNode parent = null;
                        int[] ret = i.TreeIndex;
                        string quality = i.ValueStringInDictionary("quality");
                        if (ret == null || String.IsNullOrEmpty(quality))
                        {
                            //여기가 테스트용
                            //ret = new int[] { 10, -1, -1 };
                            continue;
                        }
                        else if (i.IsWeapon() || i.IsArmor() || i.IsAccessory() || i.Is방패())
                        {
                            // 등급
                            switch (quality)
                            {
                                case "common": if (this.checkBox일반.Checked == false) continue; break;
                                case "rare": if (this.checkBox희귀.Checked == false) continue; break;
                                case "legend": if (this.checkBox전승.Checked == false) continue; break;
                                case "unique": if (this.checkBox유일.Checked == false) continue; break;
                                case "epic": if (this.checkBox영웅.Checked == false) continue; break;
                                case "mythic": if (this.checkBox신화.Checked == false) continue; break;
                            }
                            int level = Int32.Parse(i.ValueStringInDictionary("level"));
                            int comboLevel = Int32.Parse(this.comboBoxLevel.SelectedItem.ToString());

                            if (level < comboLevel) continue;
                        }
                        else if (i.Is이디안())
                        {
                            if (i.RandomOption == null) continue;
                        }

                        switch (quality)
                        {
                            case "common": break;
                            case "rare": node.BackColor = Color.GreenYellow; break;
                            case "legend": node.BackColor = Color.DeepSkyBlue; break;
                            case "unique": node.BackColor = Color.Yellow; break;
                            case "epic": node.BackColor = Color.Gold; break;
                            case "mythic": node.BackColor = Color.Violet; break;
                        }

                        parent = this.treeView1.Nodes[ret[0]];
                        if (ret[1] != -1)
                        {
                            if (ret[2] != -1)
                            {
                                parent = parent.Nodes[ret[1]].Nodes[ret[2]];
                            }
                            else
                            {
                                parent = parent.Nodes[ret[1]];
                            }
                        }
                        parent.Nodes.Add(node);

                        // 랜덤 옵션
                        if (i.RandomOption != null)
                        {
                            foreach (EntityOption iro in i.RandomOption.OptionList)
                            {
                                TreeNode tn = new TreeNode();
                                tn.Tag = iro;
                                tn.Text = "옵션 " + iro.GroupId;
                                node.Nodes.Add(tn);
                            }
                        }
                    }
                }

                if (xmlHandler.TitleList != null)
                {
                    IDictionaryEnumerator titles = xmlHandler.TitleList.GetEnumerator();
                    while (titles.MoveNext())
                    {
                        EntityTitle title = (EntityTitle)titles.Value;
                        TreeNode node = new TreeNode();
                        node.Tag = title;
                        node.Text = title.GetTreeName();
                        int[] ret = new int[] { 6, -1, -1 };
                        TreeNode parent = this.treeView1.Nodes[ret[0]];
                        parent.Nodes.Add(node);
                    }
                }

                if (xmlHandler.SetItemList != null)
                {
                    foreach (EntitySetItem title in xmlHandler.SetItemList)
                    {
                        TreeNode node = new TreeNode();
                        node.Tag = title;
                        node.Text = title.GetTreeName();
                        int[] ret = new int[] { 7, -1, -1 };
                        TreeNode parent = this.treeView1.Nodes[ret[0]];
                        parent.Nodes.Add(node);
                    }
                }
                if (xmlHandler.ItemRandomOptionList != null)
                {
                    foreach (EntityItemRandomOption rp in xmlHandler.ItemRandomOptionList)
                    {
                        if (rp.Item == null) continue;
                        TreeNode node = new TreeNode();
                        node.Tag = rp;
                        node.Text = rp.GetTreeName();
                        int[] ret = new int[] { 8, -1, -1 };
                        TreeNode parent = this.treeView1.Nodes[ret[0]];
                        parent.Nodes.Add(node);
                        if (rp.Item.NodeData.ContainsKey("quality"))
                        {
                            string quality = rp.Item.NodeData["quality"];
                            switch (quality)
                            {
                                case "common": break;
                                case "rare": node.BackColor = Color.GreenYellow; break;
                                case "legend": node.BackColor = Color.DeepSkyBlue; break;
                                case "unique": node.BackColor = Color.Yellow; break;
                                case "epic": node.BackColor = Color.Gold; break;
                                case "mythic": node.BackColor = Color.Violet; break;
                            }
                        }
                    }
                }

                if (xmlHandler.SkillList != null)
                {
                    IDictionaryEnumerator skill = xmlHandler.SkillList.GetEnumerator();
                    while (skill.MoveNext())
                    {
                        EntitySkill es = ((EntitySkill)(skill.Value));
                        TreeNode node = new TreeNode();
                        node.Tag = skill.Value;
                        node.Text = es.GetTreeName();
                        int[] ret = new int[] { 9, -1, -1 };
                        TreeNode parent = this.treeView1.Nodes[ret[0]];
                        switch (es.SType)
                        {
                            /*
                    case SkillType.None: ret[1] = 0; break;
                    case SkillType.NoneActive: ret[1] = 1; break;
                    case SkillType.Passive: ret[1] = 2; break;
                    case SkillType.Toggle: ret[1] = 3; break;
                    case SkillType.Active: ret[1] = 4; break;
                    case SkillType.Charge: ret[1] = 5; break;
                    case SkillType.Provoked: ret[1] = 6; break;
                    case SkillType.Maintain: ret[1] = 7; break;
                         */
                            case SkillType.Toggle:
                                ret[1] = 0;
                                // 토글이면서 메인옵션이.레벨별오션이 있다면 빼버리자
                                if (es.MainOption.Count > 0) continue;
                                break;
                            case SkillType.Active:
                                ret[1] = 1;
                                if (es.ViewName == null || es.ViewName.Contains("효과")) continue;
                                break;
                            case SkillType.Provoked:
                                ret[1] = 2;
                                if (es.ViewName != null && es.ViewName.Contains("진언") && es.ViewName.Contains("효과"))
                                {
                                    parent = parent.Nodes[ret[1]];
                                    parent.Nodes.Add(node);
                                    continue;
                                }
                                else
                                {
                                    continue;
                                }
                            default: continue;
                        }
                        parent = parent.Nodes[ret[1]];

                        if (es.NodeData.ContainsKey("first_target"))
                        {
                            if (es.NodeData["first_target"].ToLower() == "mypet")
                            {
                                continue;
                            }
                        }

                        if (es.NodeData.ContainsKey("target_relation_restriction"))
                        {
                            if (es.NodeData["target_relation_restriction"].ToLower() == "enemy")
                            {
                                continue;
                            }
                        }

                        //if (es.NodeData.ContainsKey("effect1_type") && es.NodeData["effect1_type"].ToLower().Contains("stat"))
                        if (es.MainOption.Count > 0 || es.BonusOption.Count > 0 || es.PercentOption.Count > 0)
                        {
                            // 스킬
                            if (es.Name.StartsWith("fi")) parent = parent.Nodes[0];
                            else if (es.Name.StartsWith("kn_")) parent = parent.Nodes[1];
                            else if (es.Name.StartsWith("as_")) parent = parent.Nodes[2];
                            else if (es.Name.StartsWith("ra_")) parent = parent.Nodes[3];
                            else if (es.Name.StartsWith("wi_")) parent = parent.Nodes[4];
                            else if (es.Name.StartsWith("el_") || es.Name.StartsWith("order_")) parent = parent.Nodes[5];
                            else if (es.Name.StartsWith("pr_")) parent = parent.Nodes[6];
                            else if (es.Name.StartsWith("ch_")) parent = parent.Nodes[7];
                            else if (es.Name.StartsWith("gu_")) parent = parent.Nodes[8];
                            else if (es.Name.StartsWith("ba_") || es.Name.StartsWith("ar_")) parent = parent.Nodes[9];
                            else if (es.Name.StartsWith("wa_")) parent = parent.Nodes[10];
                            else if (es.Name.StartsWith("sc_")) parent = parent.Nodes[11];
                            else if (es.Name.StartsWith("ma_")) parent = parent.Nodes[12];
                            else if (es.Name.StartsWith("cl_")) parent = parent.Nodes[13];
                            else if (es.Name.Contains("food") || es.Name.Contains("test") || es.Name.StartsWith("q_") || es.Name.StartsWith("gm_") || es.Name.Contains("item_") || es.Name.Contains("event_") || es.Name.Contains("stigma_") || es.Name.Contains("p_")) continue;//parent = this.treeView1.Nodes[10];
                            else
                            {
                                continue;
                                //parent = this.treeView1.Nodes[10];
                            }
                        }
                        else
                        {
                            continue;
                            //parent = this.treeView1.Nodes[10];
                        }
                        parent.Nodes.Add(node);
                    }
                }
            }));
        }
        #endregion



        public void AppentText(String str)
        {
            this.textBox1.AppendText(str + "\r\n");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Tag is Entity)
            {
                Entity entity = (Entity)e.Node.Tag;
                this.textBox1.Text = entity.GetInfo();
                OnOffContextMenu();
            }
            else
            {
                OnOffContextMenu(-1);
            }
            if (e.Node.Tag != null && (e.Node.Tag is EntityOption || e.Node.Tag is EntityItem))
            {
                this.toolStripMenuItemClipboard.Enabled = true;
            }
            else
            {
                this.toolStripMenuItemClipboard.Enabled = false;
            }
        }

        private void OnOffContextMenu()
        {
            Entity entity = (Entity)this.treeView1.SelectedNode.Tag;
            int i = -1;

            if (entity is EntityItem && ((EntityItem)entity).RandomOption != null) i = -1;
            else
            {
                if (entity is EntityOption)
                {
                    entity = ((EntityOption)entity).parent.Item;
                }

                if (entity is EntityItem)
                {
                    EntityItem item = (EntityItem)entity;

                    if (item.Is2Hand())
                    {
                        i = 0;
                    }
                    else if (item.Is1Hand())
                    {
                        i = 1;
                    }
                    else if (item.Is방패())
                    {
                        i = 2;
                    }
                    else if (item.Is귀고리() || item.Is반지())
                    {
                        i = 4;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                else if (entity is EntityTitle)
                {
                    i = 3;
                }
                else if (entity is EntitySkill)
                {
                    i = 3;
                }
            }
            OnOffContextMenu(i);
        }

        private void OnOffContextMenu(int i)
        {
            if (i == 0) // 오른손만
            {
                this.ToolStripMenuItem오른손.Enabled = true;
                this.ToolStripMenuItem왼손.Enabled = true;
                this.ToolStripMenuItem적용추가.Enabled = false;
                this.ToolStripMenuItem귀고리2.Enabled = false;
            }
            else if (i == 1) // 양손다
            {
                this.ToolStripMenuItem오른손.Enabled = true;
                this.ToolStripMenuItem왼손.Enabled = true;
                this.ToolStripMenuItem적용추가.Enabled = false;
                this.ToolStripMenuItem귀고리2.Enabled = false;
            }
            else if (i == 2) // 방패 왼손만
            {
                this.ToolStripMenuItem오른손.Enabled = false;
                this.ToolStripMenuItem왼손.Enabled = true;
                this.ToolStripMenuItem적용추가.Enabled = false;
                this.ToolStripMenuItem귀고리2.Enabled = false;
            }
            else if (i == 3) // 적용 추가, 귀고리 반지 제외
            {
                this.ToolStripMenuItem오른손.Enabled = false;
                this.ToolStripMenuItem왼손.Enabled = false;
                this.ToolStripMenuItem적용추가.Enabled = true;
                this.ToolStripMenuItem귀고리2.Enabled = false;
            }
            else if (i == 4) // 귀고리 반지
            {
                this.ToolStripMenuItem오른손.Enabled = false;
                this.ToolStripMenuItem왼손.Enabled = false;
                this.ToolStripMenuItem적용추가.Enabled = true;
                this.ToolStripMenuItem귀고리2.Enabled = true;
            }
            else
            {
                this.ToolStripMenuItem오른손.Enabled = false;
                this.ToolStripMenuItem왼손.Enabled = false;
                this.ToolStripMenuItem적용추가.Enabled = false;
                this.ToolStripMenuItem귀고리2.Enabled = false;
            }
          
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.treeView1.SelectedNode = e.Node;

            }
        }

        private void ToolStripMenuItem오른손_Click(object sender, EventArgs e)
        {
            ToolStripClick(0);
        }
        private void ToolStripMenuItem왼손_Click(object sender, EventArgs e)
        {
            ToolStripClick(1);
        }

        private void ToolStripMenuItem적용추가_Click(object sender, EventArgs e)
        {
            ToolStripClick(2);
        }

        private void ToolStripMenuItem귀고리2_Click(object sender, EventArgs e)
        {
            ToolStripClick(3);
        }

        private void ToolStripClick(int where)
        {
            try
            {
                this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
                MenuItemClick(where);
                CheckSetItem();
                Calc();
            }
            finally
            {
                this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
        }

        private void MenuItemClick(int where)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node.Tag != null && node.Tag is Entity)
            {
                if (node.Tag is EntityOption)
                {
                    EntityItem cloneItem = ((EntityOption)node.Tag).parent.Item.Clone();
                    cloneItem.RandomOptionGroupID = ((EntityOption)node.Tag).GroupId;
                    Apply아이템(cloneItem, where);
                }
                else if (node.Tag is EntityItem)
                {
                    EntityItem cloneItem = ((EntityItem)node.Tag).Clone();
                    Apply아이템(cloneItem, where);
                }
                else
                {
                    Apply아이템((Entity)node.Tag, where);
                }
            }
        }


        private void ApplyClass(InfoClassDefault classDefault)
        {
            if (classDefault == null) return;
            int rowIndex = 1;
            ClearRow(rowIndex);
            CurrentStat.클래스 = classDefault;
            this.dataGridView[Information.ColumnIndex이름, rowIndex].Value = CurrentStat.클래스.ClassName;

            IDictionaryEnumerator options = CurrentStat.클래스.AttrList.GetEnumerator();
            while (options.MoveNext())
            {
                SetAttr((string)options.Key, (string)options.Value, rowIndex);
            }
        }


        private void Apply아이템(Entity entity, int where)
        {
            int rowIndex = -1;
            if (entity is EntityItem)
            {
                EntityItem item = ((EntityItem)entity);
                if (where == 0)
                {
                    this.CurrentStat.오른손 = item;
                    rowIndex = 2;
                }
                else if (where == 1)
                {
                    this.CurrentStat.왼손 = item;
                    rowIndex = 3;
                }
                else if (where == 2 || where == 3)
                {
                    if (item.Is상의()) { this.CurrentStat.상의 = item; rowIndex = 4; }
                    else if (item.Is어깨()) { this.CurrentStat.어깨 = item; rowIndex = 5; }
                    else if (item.Is장갑()) { this.CurrentStat.장갑 = item; rowIndex = 6; }
                    else if (item.Is하의()) { this.CurrentStat.하의 = item; rowIndex = 7; }
                    else if (item.Is신발()) { this.CurrentStat.신발 = item; rowIndex = 8; }
                    else if (item.Is머리()) { this.CurrentStat.머리 = item; rowIndex = 9; }
                    else if (item.Is목걸이()) { this.CurrentStat.목걸이 = item; rowIndex = 10; }
                    else if (item.Is귀고리() && where == 2) { this.CurrentStat.귀고리1 = item; rowIndex = 11; }
                    else if (item.Is귀고리() && where == 3) { this.CurrentStat.귀고리2 = item; rowIndex = 12; }
                    else if (item.Is반지() && where == 2) { this.CurrentStat.반지1 = item; rowIndex = 13; }
                    else if (item.Is반지() && where == 3) { this.CurrentStat.반지2 = item; rowIndex = 14; }
                    else if (item.Is허리띠()) { this.CurrentStat.허리띠 = item; rowIndex = 15; }
                    else if (item.Is날개()) { this.CurrentStat.날개 = item; rowIndex = 16; }
                    else if (item.Is이디안()) { this.CurrentStat.이디안 = item; rowIndex = 18; }
                    else if (item.Is음식()) { this.CurrentStat.음식 = item; rowIndex = 19; }
                    else if (item.Is캔디()) { this.CurrentStat.캔디 = item; rowIndex = 20; }
                    // 21 세트 22 마석 23 주문서
                    else if (item.Is마석() || item.Is고대마석())
                    {
                        int count세트효과 = CurrentStat.세트효과목록.Count;
                        if (count세트효과 == 0 || count세트효과 == 1) rowIndex = 21;
                        else
                        {
                            rowIndex = 21 + (CurrentStat.세트효과목록.Count - 1);
                        }
                        rowIndex++;
                        if (CurrentStat.마석목록.Count == 0)
                        {
                            //Apply아이템ByRowIndex(entity, rowIndex);
                        }
                        else
                        {
                            rowIndex += this.CurrentStat.마석목록.Count;
                            AddItemAtRowIndex("마석", entity, rowIndex);
                        }
                        //((EntityItem)entity).SetLevelOrCount(1);
                        this.CurrentStat.마석목록.Add((EntityItem)entity);
                        //((EntityItem)entity).SetLevelOrCount(1);
                    }
                    else if (item.Is주문서())
                    {
                        int count세트효과 = CurrentStat.세트효과목록.Count;
                        int count마석 = CurrentStat.마석목록.Count;
                        if (count세트효과 == 0 || count세트효과 == 1) rowIndex = 21;
                        else
                        {
                            rowIndex = 21 + (CurrentStat.세트효과목록.Count - 1);
                        }
                        rowIndex++; // 마석시작

                        if (count마석 > 1)
                        {
                            rowIndex += (CurrentStat.마석목록.Count - 1);
                        }
                        rowIndex++; // 주문서시작

                        if (CurrentStat.주문서목록.Count == 0)
                        {
                            //Apply아이템ByRowIndex(entity, rowIndex);
                        }
                        else
                        {
                            rowIndex += this.CurrentStat.주문서목록.Count;
                            AddItemAtRowIndex("주문서", entity, rowIndex);
                        }
                        this.CurrentStat.주문서목록.Add((EntityItem)entity);
                    }
                    if (where == 3 && (rowIndex == 11 || rowIndex == 13))
                    {
                        rowIndex++;
                    }
                }
            }
            else if (entity is EntityTitle)
            {
                this.CurrentStat.타이틀 = (EntityTitle)entity;
                rowIndex = 17;
            }
            else if (entity is EntitySkill)
            {
                // 스킬이라고 써져있는게 row 24부터 시작
                rowIndex = 24;
                for (int i = 24; i < this.dataGridView.Rows.Count; i++)
                {
                    if ((string)this.dataGridView[0, i].Value == "스킬")
                    {
                        rowIndex = i;
                        break;
                    }
                }
                if (CurrentStat.스킬목록.Count == 0)
                {
                }
                else
                {
                    rowIndex += this.CurrentStat.스킬목록.Count;
                    AddItemAtRowIndex("스킬", entity, rowIndex);
                }
                CurrentStat.스킬목록.Add((EntitySkill)entity);
            }
            /*
        else if (entity is EntitySetItem)
        {
            rowIndex = 21;
            if (CurrentStat.세트효과목록.Count == 0)
            {
                //Apply아이템ByRowIndex(entity, rowIndex);
            }
            else
            {
                // 이미 있다면 추가해야한다...
                AddItemAtRowIndex("세트효과", entity, 21 + this.CurrentStat.세트효과목록.Count);
            }
            this.CurrentStat.세트효과목록.Add((EntitySetItem)entity);

        }
             */
            Apply아이템ByRowIndex(entity, rowIndex);
        }

        public void AddItemAtRowIndex(string type, Entity entity, int rowIndex)
        {
            DataGridViewCellStyle s = new DataGridViewCellStyle();
            s.Alignment = DataGridViewContentAlignment.MiddleRight;
            s.BackColor = Color.White;

            string secondButtonName = (type == "세트효과") ? "" : "삭제";
            this.dataGridView.Rows.Insert(rowIndex, new string[] { type, secondButtonName });
            if (type == "마석")
            {
                this.dataGridView[3, rowIndex].Style = s;
                this.dataGridView[3, rowIndex].ReadOnly = false;
            }
            else
            {
                s.BackColor = Color.AliceBlue;
                this.dataGridView[3, rowIndex].Style = s;
                this.dataGridView[3, rowIndex].ReadOnly = true;
            }
        }

        private void Apply아이템ByRowIndex(Entity entity, int rowIndex)
        {
            if (entity is EntityItem)
            {
                EntityItem item = (EntityItem)entity;
                ClearRow(rowIndex);

                this.dataGridView[Information.ColumnIndex이름, rowIndex].Value = item.ViewName + ((item.RandomOptionGroupID != 0) ? (" 옵션 " + item.RandomOptionGroupID) : "");

                if (item.IsWeapon() || item.IsArmor() || item.Is방패())
                {
                    this.dataGridView[Information.ColumnIndex강화레벨, rowIndex].Value = item.GetLevelOrCount();
                    this.dataGridView[Information.ColumnIndex최대강화, rowIndex].Value = item.ValueMaxEnchantString;
                    this.dataGridView[Information.ColumnIndex마석슬롯, rowIndex].Value = item.ValueOptionSlotValue;
                    this.dataGridView[Information.ColumnIndex고대마석슬롯, rowIndex].Value = item.ValueSpecialSlotValue;
                }
                else if (item.Is마석() || item.Is고대마석())
                {
                    this.dataGridView[Information.ColumnIndex강화레벨, rowIndex].Value = item.GetLevelOrCount();
                }

                List<Dictionary<string, string>> options = new List<Dictionary<string, string>>();
                options.Add(item.MainOption);
                options.Add(item.BonusOption);
                options.Add(item.BonusAOption);
                options.Add(item.BonusBOption);
                options.Add(item.EnchantOption);

                if (item.RandomOptionGroupID != 0)
                {
                    foreach (EntityOption op in item.RandomOption.OptionList)
                    {
                        if (op.GroupId == item.RandomOptionGroupID)
                        {
                            options.Add(op.AttrList);
                            break;
                        }
                    }
                }
                foreach (Dictionary<string, string> op in options)
                {
                    IDictionaryEnumerator o = op.GetEnumerator();
                    while (o.MoveNext())
                    {
                        SetAttr((string)o.Key, (string)o.Value, rowIndex, (rowIndex == Information.GetRowIndex("왼손") && (op == item.MainOption || op == item.EnchantOption)));
                    }
                }
            }
            else if (entity is EntityTitle)
            {
                EntityTitle title = (EntityTitle)entity;
                ClearRow(rowIndex);
                this.dataGridView[Information.ColumnIndex이름, rowIndex].Value = title.ViewName;
                IDictionaryEnumerator options = title.MainOption.GetEnumerator();
                while (options.MoveNext())
                {
                    SetAttr((string)options.Key, (string)options.Value, rowIndex);
                }
            }
            else if (entity is EntitySetItemOption)
            {
                EntitySetItemOption sio = (EntitySetItemOption)entity;
                ClearRow(rowIndex);
                this.dataGridView[Information.ColumnIndex이름, rowIndex].Value = sio.ViewName;
                IDictionaryEnumerator options = sio.Option.GetEnumerator();
                while (options.MoveNext())
                {
                    SetAttr((string)options.Key, (string)options.Value, rowIndex);
                }
            }
            else if (entity is EntitySkill)
            {
                EntitySkill s = (EntitySkill)entity;
                ClearRow(rowIndex);
                this.dataGridView[Information.ColumnIndex이름, rowIndex].Value = s.ViewName;

                List<Dictionary<string, string>> options = new List<Dictionary<string, string>>();
                options.Add(s.MainOption);
                options.Add(s.BonusOption);
                options.Add(s.PercentOption);

                foreach (Dictionary<string, string> op in options)
                {
                    IDictionaryEnumerator o = op.GetEnumerator();
                    while (o.MoveNext())
                    {
                        SetAttr((string)o.Key, (string)o.Value, rowIndex);
                    }
                }
            }
        }

        private void ApplySetItemEffect()
        {
            RemoveSetItemRow();
            if (CurrentStat.세트효과목록.Count != 0)
            {
                Apply아이템ByRowIndex(CurrentStat.세트효과목록[0], Information.RowIndex세트효과);
                for (int i = 1; i < CurrentStat.세트효과목록.Count; i++)
                {
                    // 추가
                    AddItemAtRowIndex("세트효과", CurrentStat.세트효과목록[i], Information.RowIndex세트효과 + i);
                    Apply아이템ByRowIndex(CurrentStat.세트효과목록[i], Information.RowIndex세트효과 + i);
                }
            }
        }

        private void SetAttr(string key, string value, int rowIndex)
        {
            SetAttr(key, value, rowIndex, false);
        }
        private void SetAttr(string key, string value, int rowIndex, bool 왼손메인옵션이나강화옵션)
        {
            int columnIndex = Information.GetColumnIndex(key);
            if (왼손메인옵션이나강화옵션 && (columnIndex - Information.ColumnHeaders.Length >= 2 && columnIndex - Information.ColumnHeaders.Length <= 6)) columnIndex += 5;

            if (columnIndex != -1)
            {
                double d1 = GetDataGridViewValue(columnIndex, rowIndex);
                double d2 = StringToDouble(value);
                this.dataGridView[columnIndex, rowIndex].Value = d1 + d2;
            }
            else
            {

            }
        }






        /*
         * 삭제 관련
         */
        #region 삭제
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1 && e.ColumnIndex != 2) return;
            if (e.ColumnIndex == 1)
            {
                this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
                if (e.RowIndex == 0) // 모두지우기
                {
                    RemoveRow();
                    ClearRow();
                    CurrentStat.Clear();
                }
                else if (e.RowIndex == 1) // 클래스 선택
                {
                    InfoClassDefault id = CurrentStat.클래스;
                    if (id == null)
                    {
                        id = Information.InfoClassDefaultList["검성"];
                    }
                    else
                    {
                        id = Information.NextClassDefultList(id.ClassName);
                    }
                    ApplyClass(id);
                    Calc();
                }
                else if (this.dataGridView[0, e.RowIndex].Value != null && (string)this.dataGridView[0, e.RowIndex].Value != "세트효과") // 지우기
                {
                    object obj = this.dataGridView[1, e.RowIndex].Value;
                    CurrentStat.SetNull(e.RowIndex);
                    CheckSetItem();
                    if (obj != null && obj is string && (string)obj == "삭제") RemoveRow(e.RowIndex);
                    else ClearRow(e.RowIndex);
                    Calc();
                }
                this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
            else if (e.ColumnIndex == 2)
            {
                //템정보 보기
                /*
                EntityItem entity = CurrentStat.GetEntityItemByRowIndex(e.RowIndex);
                if (entity != null)
                {
                    textBox1.Text = entity.GetInfo();
                }
                 */
            }
        }


        private void ClearRow()
        {
            for (int i = 0; i < this.dataGridView.Rows.Count; i++)
            {
                ClearRow(i);
            }
        }

        private void ClearRow(int rowIndex)
        {
            for (int i = 2; i < this.dataGridView.Columns.Count; i++)
            {
                this.dataGridView[i, rowIndex].Value = "";
                //this.dataGridView[i, rowIndex].Value = 0.0;
            }
        }

        private void RemoveRow()
        {
            RemoveSetItemRow();
            //먼저 삭제표시가 있는 것들은 줄을 삭제하자
            int rowCount = this.dataGridView.Rows.Count;
            int currentRowIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                object obj = this.dataGridView[1, currentRowIndex].Value;
                if (obj != null && obj is string && (string)obj == "삭제")
                {
                    this.dataGridView.Rows.RemoveAt(currentRowIndex);
                }
                else
                {
                    currentRowIndex++;
                }
            }
        }

        private void RemoveSetItemRow()
        {
            // 무조건 한줄만?
            ClearRow(Information.RowIndex세트효과);
            int currentRowIndex = Information.RowIndex세트효과 + 1;
            int count = this.dataGridView.Rows.Count; // 줄수가 변하기 때문에
            for (int i = currentRowIndex; i < count; i++)
            {
                // 22 번이 세트효과인가?
                if ((string)this.dataGridView[0, currentRowIndex].Value == "세트효과")
                {
                    RemoveRow(currentRowIndex);
                }
                else
                {
                    break;
                }
            }
        }


        private void RemoveRow(int rowIndex)
        {
            this.dataGridView.Rows.RemoveAt(rowIndex);
        }

        #endregion


        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            try
            {
                if (dataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == false)
                {
                    object obj = dataGridView[e.ColumnIndex, e.RowIndex].Value;
                    int level = -1;
                    if (obj is int)
                    {
                        level = (int)obj;
                    }
                    else if (obj is string)
                    {
                        try
                        {
                            level = Int32.Parse((string)obj);
                        }
                        catch { }
                    }

                    EntityItem item = CurrentStat.GetEntityItemByRowIndex(e.RowIndex);

                    if (item == null) return;
                    if (item.Is고대마석() || item.Is마석())
                    {
                        if (level < 1 || level > 42) return;
                    }
                    else
                    {
                        if (level < 0 || level > 15) return;
                    }
                    if (item != null)
                    {
                        item.SetLevelOrCount(level);
                        Apply아이템ByRowIndex(item, e.RowIndex);
                        Calc();
                    }
                }
            }
            finally
            {
                this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
        }






































        #region 유틸리티
        public double GetDataGridViewValue(int col, int row)
        {
            double ret = 0;
            try
            {
                if (this.dataGridView[col, row].Value == null) return ret;
                object o = this.dataGridView[col, row].Value;
                if (o is string)
                    ret = StringToDouble((string)this.dataGridView[col, row].Value);
                else if (o is double)
                    ret = (double)this.dataGridView[col, row].Value;
            }
            catch
            {
            }
            return ret;
        }

        public double StringToDouble(string str)
        {
            if (str.Contains('%')) str = str.Substring(0, str.Length - 1);
            if (str == "") return 0;
            try
            {
                return double.Parse(str);
            }
            catch
            {
            }
            return 0;
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



        private void LoadStat(Stat il)
        {
            CurrentStat.Clear();
            CurrentStat.FullFileName = il.FullFileName;
            CurrentStat.OnlyFileName = il.OnlyFileName;
            CurrentStat.Desc = il.Desc;
            ApplyClass(il.클래스);
            Apply아이템(il.오른손, 0);
            Apply아이템(il.왼손, 1);
            Apply아이템(il.상의, 2);
            Apply아이템(il.어깨, 2);
            Apply아이템(il.장갑, 2);
            Apply아이템(il.하의, 2);
            Apply아이템(il.신발, 2);
            Apply아이템(il.머리, 2);
            Apply아이템(il.목걸이, 2);
            Apply아이템(il.귀고리1, 2);
            Apply아이템(il.귀고리2, 3);
            Apply아이템(il.반지1, 2);
            Apply아이템(il.반지2, 3);
            Apply아이템(il.허리띠, 2);
            Apply아이템(il.날개, 2);
            Apply아이템(il.이디안, 2);
            Apply아이템(il.음식, 2);
            Apply아이템(il.캔디, 2);
            Apply아이템(il.타이틀, 2);

            foreach (EntityItem item in il.마석목록)
            {
                Apply아이템(item, 2);
            }
            foreach (EntityItem item in il.주문서목록)
            {
                Apply아이템(item, 2);
            }
            foreach (EntitySkill item in il.스킬목록)
            {
                Apply아이템(item, 2);
            }
            CheckSetItem();
            Calc();
        }



        // 세트아이템 목록을 갱신한다.. 일단 추가
        private void CheckSetItem()
        {
            // 오른손, 왼손, 방어구, 악세
            // SetItem  목록을 리스트에 집어넣는다
            // 세트아이템 ID 가 유니크
            // EntitysSetItem : Count 구조체로 파악을 다한다..
            EntityItem[] checkItemList = { 
                                          CurrentStat.오른손, 
                                          CurrentStat.왼손, 
                                          CurrentStat.상의, 
                                          CurrentStat.어깨, 
                                          CurrentStat.장갑, 
                                          CurrentStat.하의, 
                                          CurrentStat.신발, 
                                          CurrentStat.머리, 
                                          CurrentStat.목걸이, 
                                          CurrentStat.귀고리1, 
                                          CurrentStat.귀고리2, 
                                          CurrentStat.반지1, 
                                          CurrentStat.반지2, 
                                          CurrentStat.허리띠, 
                                         };

            List<EntitySetItem> setItemList = new List<EntitySetItem>();
            foreach (EntityItem item in checkItemList)
            {
                if (item != null && item.SetItem != null)
                {
                    bool bExist = false;
                    foreach (EntitySetItem set in setItemList)
                    {
                        if (set.ID == item.SetItem.ID)
                        {
                            bExist = true;
                            break;
                        }
                    }
                    if (bExist == false) setItemList.Add(item.SetItem);
                }

            }

            // 같은 세트효과는 제외하고 리스트에 집어넣었다.
            // 이제 세트효과별로 몇개 혹은 몇번째 효과를 넣어야 하는지 파악
            CurrentStat.세트효과목록.Clear();
            foreach (EntitySetItem set in setItemList)
            {
                int count = 0;
                foreach (EntityItem item in set.ItemList)
                {
                    bool bSelect = false;
                    foreach (EntityItem sItem in checkItemList)
                    {
                        if (item != null && sItem != null && item.Name == sItem.Name)
                        {
                            bSelect = true;
                            break;
                        }
                    }
                    if (bSelect)
                    {
                        count++;
                    }
                }
                // set count
                foreach (EntitySetItemOption sio in set.BonusList)
                {
                    if (sio.Count != 0 && sio.Count <= count)
                    {
                        CurrentStat.세트효과목록.Add(sio);
                    }
                    else if (sio.Count == 0 && count == sio.Parent.ItemList.Count)
                    {
                        CurrentStat.세트효과목록.Add(sio);
                    }
                }
            }
            //if ( CurrentStat.세트효과목록.Count > 0 ) ApplySetItemEffect();
            ApplySetItemEffect();
        }




        public void xmlHandler_XMLEventHandler(object sender, EventArgs a)
        {
            try
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {

                    XMLEventArgs e = (XMLEventArgs)a;
                    if (e.EventType == XMLEvent.준비)
                    {
                        this.progressForm.SetPos("준비", 3);
                    }
                    else if (e.EventType == XMLEvent.스트링)
                    {
                        this.progressForm.SetPos("String : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.스킬)
                    {
                        this.progressForm.SetPos("Skill : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.아이템)
                    {
                        this.progressForm.SetPos("Item : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.랜덤옵션)
                    {
                        this.progressForm.SetPos("Random : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.세트아이템)
                    {
                        this.progressForm.SetPos("SetItem : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.타이틀)
                    {
                        this.progressForm.SetPos("Title : " + e.Args, 7);
                    }
                    else if (e.EventType == XMLEvent.트리)
                    {
                        this.progressForm.SetPos("Tree 생성", 10);
                    }
                    else if (e.EventType == XMLEvent.완료)
                    {
                        this.progressForm.SetPos("아이템 파일 로딩 완료", 3);
                        this.progressForm.SetExit("아이템 파일 로딩 완료", 1);
                    }
                    else if (e.EventType == XMLEvent.에러)
                    {
                        this.progressForm.SetExit("로딩 에러\n" + e.Args, 0);
                    }

                    // 12개 파일 : 7 = 84 트리:10, 준비:3, 완료:3
                }));
            }
            catch { }
        }




        private void 아이온에서아이템정보파일생성하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenetateData(false);
        }

        private void 테스트서버에서아이템정보파일생성ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenetateData(true);
        }

        private void GenetateData(bool isTestServer)
        {
            bool ret = generageDataFile.ExistDataFile();
            DialogResult dr = DialogResult.None;
            if (ret)
            {
                dr = MessageBox.Show("이미 데이터 파일이 존재합니다. 다시 생성하시겠습니까?", "아이템 파일 생성", MessageBoxButtons.YesNo);
            }
            if (ret == false || dr == DialogResult.Yes)
            {
                progressForm = new ProgressForm();
                progressForm.Text = "아이템 정보파일 생성";
                ShowForm(progressForm, 2);
                progressForm.Show();

                generageDataFile.GenerateData(isTestServer);
            }
        }

        private void 저장ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveForm saveForm = new SaveForm(this);
            ShowForm(saveForm, 2);
            saveForm.Show();
        }

        private void 읽기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.xmlHandler.ItemList == null || this.xmlHandler.ItemList.Count == 0)
                {
                    MessageBox.Show("아이템 로딩을 먼저하세요", "아이템 로딩", MessageBoxButtons.OK);
                    return;
                }
                OpenFileDialog openPanel = new OpenFileDialog();
                openPanel.InitialDirectory = FileHandle.DataPath;
                openPanel.Filter = "aes(*.aes)|*.aes|All(*.*)|*.*";

                if (openPanel.ShowDialog() == DialogResult.OK)
                {
                    String file = openPanel.FileName;
                    Stat ret = FileHandle.Load(file);
                    if (ret == null)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
                        RemoveRow();
                        ClearRow();
                        LoadStat(ret);
                        this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
                    }
                }
            }
            catch //(Exception ee)
            {
                MessageBox.Show("스탯 파일 로드에 실패하였습니다", "스탯 로드 실패", MessageBoxButtons.OK);
                return;
            }
        }

        private void 폴더읽기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.xmlHandler.ItemList == null || this.xmlHandler.ItemList.Count == 0)
                {
                    MessageBox.Show("아이템 로딩을 먼저하세요", "아이템 로딩", MessageBoxButtons.OK);
                    return;
                }
                List<Stat> ret = FileHandle.LoadFolder();
                this.listView1.Items.Clear();
                foreach (Stat s in ret)
                {
                    ListViewItem lvi = new ListViewItem(new string[2]);
                    lvi.SubItems[0].Text = s.OnlyFileName.Split('.')[0];
                    lvi.SubItems[1].Text = s.Desc;
                    lvi.Tag = s;
                    this.listView1.Items.Add(lvi);
                }
            }
            catch //(Exception ee)
            {
                MessageBox.Show("스탯 파일 로드에 실패하였습니다", "스탯 로드 실패", MessageBoxButtons.OK);
                return;
            }
        }

        public void generageDataFile_GDFEventHandler(object sender, EventArgs a)
        {
            try
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    GDFEventArgs e = (GDFEventArgs)a;
                    if (e.EventType == GDFEvent.준비)
                    {
                        this.progressForm.SetPos("준비", 4);
                    }
                    else if (e.EventType == GDFEvent.ZIP생성)
                    {
                        this.progressForm.SetPos("Zip 파일 생성 : " + e.Args, 4);
                    }
                    else if (e.EventType == GDFEvent.압축해제)
                    {
                        this.progressForm.SetPos("Zip 해제 : " + e.Args, 4);
                    }
                    else if (e.EventType == GDFEvent.변환)
                    {
                        this.progressForm.SetPos("BXML -> XML : " + e.Args, 5);
                    }
                    else if (e.EventType == GDFEvent.종료)
                    {
                        this.progressForm.SetPos("아이템 정보파일 생성 완료", 4);
                        this.progressForm.SetExit("아이템 정보파일 생성 완료", 1);
                    }
                    else if (e.EventType == GDFEvent.폴더삭제실패)
                    {
                        this.progressForm.SetExit("데이터 폴더 삭제에 실패하였습니다.", 0);
                    }
                    else if (e.EventType == GDFEvent.아이온폴더못찾음)
                    {
                        this.progressForm.SetExit("아이온 미설치", 0);
                    }
                }));
            }
            catch { }
            // ZIP생성 4회 
            // 압축해제 4회
            // 변환 12회 
            // 2 + 32 + 60 + 48 

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            try
            {
                if (this.listView1.SelectedItems.Count == 0) return;
                Stat s = (Stat)this.listView1.SelectedItems[0].Tag;
                RemoveRow();
                ClearRow();
                LoadStat(s);
            }
            catch //(Exception e1)
            {

            }
            finally
            {
                this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
        }

        // UI 이벤트 : 파일삭제
        private void toolStripMenuItemFileDelete_Click(object sender, EventArgs e)
        {
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            try
            {
                if (this.listView1.SelectedItems.Count == 0) return;
                ListViewItem lv = this.listView1.SelectedItems[0];
                Stat s = (Stat)lv.Tag;
                File.Delete(s.FullFileName);
                this.listView1.SelectedIndexChanged -= new EventHandler(this.listView1_SelectedIndexChanged);
                this.listView1.Items.Remove(lv);
                this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
            }
            finally
            {
                this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
        }


        //2013-08-29
        private void toolStripMenuItemClipboard_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node.Tag != null && node.Tag is Entity)
            {
                if (node.Tag is EntityOption)
                {
                    ClipboardSetText("[item:" + ((EntityOption)node.Tag).parent.Item.ID + ";ver5;;;;;;]");
                }
                else if (node.Tag is EntityItem)
                {
                    ClipboardSetText("[item:" + ((Entity)node.Tag).ID +";ver5;;;;;;]");
                }
                else
                {
                }
            }
        }

        private void ClipboardSetText(String str)
        {
            try
            {
                Clipboard.SetText(str);
            }
            catch
            {
            }
        }
    }
}

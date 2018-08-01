using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AES
{
    partial class MainForm
    {
        private void InitializeComponentManually()
        {
            InitDataGridView();
            this.treeView1.ContextMenuStrip = this.contextMenuStripTreeView;
            this.labelVersion.Text += Version;
            this.panelUpdate.Controls.Remove(this.progressBar1);
        }

        private void InitDataGridView()
        {
            //해더 정렬
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.AllowUserToAddRows = false;
           
            DataGridViewColumn col = null;

            for (int i = 0; i < Information.ColumnHeaders.Length + Information.InfoAttrList.Count; i++)
            {
                if (i > Information.ColumnHeaders.Length && Information.InfoAttrList[i - Information.ColumnHeaders.Length].ColumnIndex == -1) continue;
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Alignment = DataGridViewContentAlignment.MiddleRight;
                style.BackColor = Color.AliceBlue;

                string name = (i < Information.ColumnHeaders.Length) ? Information.ColumnHeaders[i] : Information.InfoAttrList[i - Information.ColumnHeaders.Length].HanAttrName;
                if (i == 1) // 전체
                {
                    col = new DataGridViewButtonColumn();
                }
                else
                {
                    col = new DataGridViewTextBoxColumn();
                }

                col.DefaultCellStyle = style;
                col.Name = name;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
                this.dataGridView.Columns.Add(col);

                //width 처리
                if (i == 0) //전체
                {
                    col.Width = 60;
                    style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (i == 1) //지우기
                {
                    col.Width = 50;
                    style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (i == 2) //이름
                {
                    col.Width = 160;
                    style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (i == 3) //강화
                {
                    col.Width = 40;
                }
                else
                {
                    col.Width = 40;
                }
            }

            for (int i = 0; i < Information.RowHeaders.Length/2; i++)
            {
                this.dataGridView.Rows.Add(new string[] {Information.RowHeaders[i,0], Information.RowHeaders[i,1]});
            }

            // 강화만 바꾼다
            DataGridViewCellStyle s = new DataGridViewCellStyle();
            s.Alignment = DataGridViewContentAlignment.MiddleRight;
            s.BackColor = Color.White;

            int[] writeRow = { 2, 3, 4, 5, 6, 7, 8, 22 };

            for (int i = 0; i < writeRow.Length; i++)
            {
                this.dataGridView[3, writeRow[i]].Style = s;
                this.dataGridView[3, writeRow[i]].ReadOnly = false;
            }
        }

        private void Append(StringBuilder sb, string s1, object s2)
        {
            sb.AppendLine(String.Format("{0} : {1}", s1, s2.ToString()));
        }

        private void Calc()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 7; i < this.dataGridView.ColumnCount; i++)
            {
                //if (i == colIndex) continue;
                double total = 0;
                for (int j = 1; j < this.dataGridView.Rows.Count; j++)
                {
                    try
                    {
                        //if ((string)this.dataGridView[0, j].Value == "스킬") break;
                        double tmp = 0;
                        object o = this.dataGridView[i, j].Value;
                        if (o is double) tmp = (double)o;
                        else if (o is string)
                        {
                            string t1 = (string)o;
                            if (t1 == null || t1 == "") continue;
                            tmp = StringToDouble(t1);
                        }
                        //if (t1.Contains('%')) t1 = t1.Substring(0, t1.Length - 1);
                        //double tmp = Double.Parse(t1);
                        total += tmp;
                    }
                    catch
                    {
                    }
                }
                if (total == 0)
                {
                    this.dataGridView[i, 0].Value = "";
                }
                else
                {
                    this.dataGridView[i, 0].Value = total;
                }
            }
            // 스킬!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            int 스킬공격력퍼센트 = 0;
            int 스킬물리방어퍼센트 = 0;
            int 스킬생명력퍼센트 = 0;
            double 스킬이속절대값 = 0;
            int 공격력컬럼인덱스 = Information.GetColumnIndex("공격력");
            int 물리방어컬럼인덱스 = Information.GetColumnIndex("물리방어");
            int 생명력컬럼인덱스 = Information.GetColumnIndex("생명력");
            for (int i = 0; i < CurrentStat.스킬목록.Count; i++)
            {
                //if (CurrentStat.스킬목록[i].SType == SkillType.Toggle || CurrentStat.스킬목록[i].SType == SkillType.Provoked)
                {
                    if (CurrentStat.스킬목록[i].PercentOption.ContainsKey("phyattack"))
                    {
                        int temp = Int32.Parse(CurrentStat.스킬목록[i].PercentOption["phyattack"]);
                        스킬공격력퍼센트 += temp;
                        double 현재공격력수치 = (GetDataGridViewValue(공격력컬럼인덱스, 0));
                        this.dataGridView[공격력컬럼인덱스, 0].Value = 현재공격력수치 - (double)temp;
                    }
                    if (CurrentStat.스킬목록[i].PercentOption.ContainsKey("physicaldefend"))
                    {
                        int temp = Int32.Parse(CurrentStat.스킬목록[i].PercentOption["physicaldefend"]);
                        스킬물리방어퍼센트 += temp;
                        double 현재물리방어수치 = (GetDataGridViewValue(물리방어컬럼인덱스, 0));
                        this.dataGridView[물리방어컬럼인덱스, 0].Value = 현재물리방어수치 - (double)temp;
                    }
                    if (CurrentStat.스킬목록[i].PercentOption.ContainsKey("maxhp"))
                    {
                        int temp = Int32.Parse(CurrentStat.스킬목록[i].PercentOption["maxhp"]);
                        스킬생명력퍼센트 += temp;
                        double 현재생명력수치 = (GetDataGridViewValue(생명력컬럼인덱스, 0));
                        this.dataGridView[생명력컬럼인덱스, 0].Value = 현재생명력수치 - (double)temp;
                    }
                }

                // 질주의진언, 진격의선율
                if (CurrentStat.스킬목록[i].ViewName.Contains("질주의 진언") || CurrentStat.스킬목록[i].ViewName.Contains("진격의 선율"))
                {
                    int 이동속도컬럼인덱스 = Information.GetColumnIndex("이동속도");
                    int temp = Int32.Parse(CurrentStat.스킬목록[i].BonusOption["speed"]);
                    스킬이속절대값 += (temp / 10.0);
                    double 현재이동속도 = (GetDataGridViewValue(이동속도컬럼인덱스, 0));
                    this.dataGridView[이동속도컬럼인덱스, 0].Value = 현재이동속도 - (double)temp;
                }
            }
            Append(sb, "스킬공격력퍼센트", 스킬공격력퍼센트);
            Append(sb, "스킬물리방어퍼센트", 스킬물리방어퍼센트);
            Append(sb, "스킬생명력퍼센트", 스킬생명력퍼센트);
            Append(sb, "스킬이속절대값", 스킬이속절대값);


            // 스킬!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // 마석개수 / 
            int matterCount = 0;
            foreach (EntityItem item in CurrentStat.마석목록)
            {
                matterCount += item.GetLevelOrCount();
            }
            this.dataGridView[3, 0].Value = matterCount + "";

            EntityItem[] slotItems = { CurrentStat.오른손, CurrentStat.왼손, CurrentStat.상의, CurrentStat.어깨, CurrentStat.장갑, CurrentStat.하의, CurrentStat.신발 };
            int maxSum = 0, bonusSum = 0, specialMatterCount = 0;
            foreach (EntityItem i in slotItems)
            {
                if (i == null) continue;
                maxSum += (int)i.ValueDoubleInDictionary("option_slot_value");
                bonusSum += (int)i.ValueDoubleInDictionary("option_slot_bonus");
                specialMatterCount += (int)i.ValueDoubleInDictionary("special_slot_value");
            }
            this.dataGridView[5, 0].Value = maxSum + "+"+ bonusSum;
            this.dataGridView[6, 0].Value = specialMatterCount;



            // 무기가 있든말든 처리할 정보 : 이동속도
            // 이동속도
            int 이동속도columnIndex = Information.GetColumnIndex("이동속도");
            double 현재전체이속 = GetDataGridViewValue(이동속도columnIndex, 0);
            double 이동속도 = 6.0 + (6.0 * 현재전체이속 / 100.0);
            이동속도 += 스킬이속절대값;
            if (이동속도 > 12.0) 이동속도 = 12.0;
            this.dataGridView[이동속도columnIndex, 0].Value = Math.Round(이동속도, 1);
            Append(sb, "이동속도", 이동속도);
            

            // 물리방어
            // 옷 악세 메인옵션
            {
                int 물리방어columnIndex = Information.GetColumnIndex("물리방어");
                double 현재물리방어 = GetDataGridViewValue(물리방어columnIndex, 0);
                EntityItem[] items = { CurrentStat.상의, CurrentStat.어깨, CurrentStat.장갑, CurrentStat.하의, CurrentStat.신발, CurrentStat.머리, CurrentStat.목걸이, CurrentStat.귀고리1, CurrentStat.귀고리2, CurrentStat.반지1, CurrentStat.반지2, CurrentStat.허리띠 };
                double 메인옵션물리방어합 = 0;
                foreach (EntityItem item in items)
                {
                    if (item == null) continue;
                    메인옵션물리방어합 += item.ValueDoubleInDictionary("physicaldefend", item.MainOption);
                    메인옵션물리방어합 += item.ValueDoubleInDictionary("physicaldefend", item.EnchantOption);
                }
                double 그외옵션물리방어합 = 현재물리방어 - 메인옵션물리방어합;
                //double 물리방어 = 메인옵션물리방어합 + 그외옵션물리방어합 * CurrentStat.맨몸.물리방어상수;
                double 물리방어 = 메인옵션물리방어합 + 그외옵션물리방어합;
                double 스킬에의한물리방어 = 메인옵션물리방어합 * (스킬물리방어퍼센트 / 100.0);
                물리방어 += 스킬에의한물리방어;
                this.dataGridView[물리방어columnIndex, 0].Value = Math.Round(물리방어);
                Append(sb, "흰색물리방어", 메인옵션물리방어합);
                Append(sb, "녹색물리방어", 그외옵션물리방어합);
                Append(sb, "스킬에의한물리방어", 스킬에의한물리방어);
                Append(sb, "물리방어", 물리방어);
            }
           

            // 생명력 : 클래스별 흰색 생명 + 방어구 강화 옵션 = 흰색
            // 녹색 = 흰색 * 클래스별 생명력강화퍼센트 + 추가옵션
            // 옷 악세 메인옵션
            if (CurrentStat.클래스 != null)
            {
                int 생명력columnIndex = Information.GetColumnIndex("생명력");
                double 현재생명력 = GetDataGridViewValue(생명력columnIndex, 0);
                EntityItem[] items = { CurrentStat.상의, CurrentStat.어깨, CurrentStat.장갑, CurrentStat.하의, CurrentStat.신발 };
                double 기본및강화로인한생명력합 = 0;
                foreach (EntityItem item in items)
                {
                    if (item == null) continue;
                    기본및강화로인한생명력합 += item.ValueDoubleInDictionary("maxhp", item.MainOption);
                    기본및강화로인한생명력합 += item.ValueDoubleInDictionary("maxhp", item.EnchantOption);
                }
                double 흰색생명력 = CurrentStat.클래스.ValueDouble("maxhp");
                흰색생명력 += 기본및강화로인한생명력합;
                double 옵션에의한생명력 = 현재생명력 - (흰색생명력);
                double 녹색생명력 = 흰색생명력 * CurrentStat.클래스.생명력강화퍼센트 / 100.0 + 옵션에의한생명력;
                double 생명력 = 흰색생명력 + 녹색생명력;
                double 스킬에의한생명력 = 흰색생명력 * (스킬생명력퍼센트 / 100.0);
                생명력 += 스킬에의한생명력;
                this.dataGridView[생명력columnIndex, 0].Value = Math.Round(생명력);
                Append(sb, "기본및강화로인한생명력합", 기본및강화로인한생명력합);
                Append(sb, "흰색생명력", 흰색생명력);
                Append(sb, "녹색생명력", 녹색생명력);
                Append(sb, "스킬에의한생명력", 스킬에의한생명력);
                Append(sb, "생명력", 생명력);
            }

            // 쌍수든 합성인든 상관없는 정보 : 마적 무방은 오른손만 처리
            if (CurrentStat.클래스 != null && CurrentStat.오른손 != null && CurrentStat.왼손 != null)
            {
                int 마법적중columnIndex = Information.GetColumnIndex("마법적중");
                double 두손합마법적중 = GetDataGridViewValue(마법적중columnIndex, 0);
                double 왼손마법적중 = CurrentStat.왼손.ValueDoubleInDictionary("magicalhitaccuracy", CurrentStat.왼손.MainOption);
                double 마법적중 = 두손합마법적중 - 왼손마법적중;
                this.dataGridView[마법적중columnIndex, 0].Value = Math.Round(마법적중, 2);
                Append(sb, "마법적중", 마법적중);

                int 무기방어columnIndex = Information.GetColumnIndex("무기방어");
                double 두손합무기방어 = GetDataGridViewValue(무기방어columnIndex, 0);
                double 왼손무기방어 = CurrentStat.왼손.ValueDoubleInDictionary("parry", CurrentStat.왼손.MainOption);
                double 무기방어 = 두손합무기방어 - 왼손무기방어;
                this.dataGridView[무기방어columnIndex, 0].Value = Math.Round(무기방어, 2);
                Append(sb, "무기방어", 무기방어);

                // PVP공격력 높은것만 적용
                int PVP공격력columnIndex = Information.GetColumnIndex("PVP공격력");
                double PVP공격력전체합계 = GetDataGridViewValue(PVP공격력columnIndex, 0);
                double 오른손PVP공격력 = CurrentStat.오른손.ValueDoubleInDictionary("pvpattackratio", CurrentStat.오른손.BonusOption);
                double 왼손PVP공격력 = CurrentStat.왼손.ValueDoubleInDictionary("pvpattackratio", CurrentStat.왼손.BonusOption);
                double 낮은쪽 = (오른손PVP공격력 > 왼손PVP공격력) ? 왼손PVP공격력 : 오른손PVP공격력;
                double PVP공격력 = (PVP공격력전체합계 - 낮은쪽)/10.0;
                this.dataGridView[PVP공격력columnIndex, 0].Value = Math.Round(PVP공격력, 1) + "%";
                Append(sb, "PVP공격력", PVP공격력);
            }

            // PVP
            int PVP방어력columnIndex = Information.GetColumnIndex("PVP방어력");
            double PVP방어력 = GetDataGridViewValue(PVP방어력columnIndex, 0) / 10.0;
            Append(sb, "PVP방어력", PVP방어력);
            this.dataGridView[PVP방어력columnIndex, 0].Value = Math.Round(PVP방어력, 1) + "%";

            //시속..
            {
                int 시전속도columnIndex = Information.GetColumnIndex("시전속도");
                double 현재전체시속 = GetDataGridViewValue(시전속도columnIndex, 0);
                double 오른손시속 = (CurrentStat.오른손 == null) ? 0 : CurrentStat.오른손.ValueDoubleInDictionary("boostcastingtime", CurrentStat.오른손.BonusOption);
                double 왼손시속 = (CurrentStat.왼손 == null) ? 0 : CurrentStat.왼손.ValueDoubleInDictionary("boostcastingtime", CurrentStat.왼손.BonusOption);
                double 시속이낮은쪽 = (오른손시속 > 왼손시속) ? 왼손시속 : 오른손시속;
                double 낮은쪽제외시속 = 현재전체시속 - 시속이낮은쪽;
                double 시전속도 = 1 - (낮은쪽제외시속 / 100.0);
                this.dataGridView[시전속도columnIndex, 0].Value = Math.Round(시전속도, 1);
                Append(sb, "시전속도", 시전속도);
            }

            if (CurrentStat.클래스 != null && CurrentStat.오른손 != null && CurrentStat.왼손 != null && CurrentStat.오른손.Is1Hand() && CurrentStat.왼손.Is1Hand())
            {
                #region 쌍수
                this.dataGridView[Information.GetColumnIndex("min_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("min_damage") + 5, 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage") + 5, 0].Value = "";


                // 공격력
                int colIndex = Information.GetColumnIndex("공격력");
                double 마석옵션공격력수치 = (GetDataGridViewValue(colIndex, 0)); // 다른 모든 + 공격력

                double 무기평균대미지 = (CurrentStat.오른손.ValueDoubleInDictionary("min_damage") + CurrentStat.오른손.ValueDoubleInDictionary("max_damage")) / 2.0;
                double 무기강화대미지 = CurrentStat.오른손.ValueDoubleInDictionary("min_damage", CurrentStat.오른손.EnchantOption);
                double 무기대미지 = 무기평균대미지 + 무기강화대미지;
                double 흰색공격력 = 무기대미지 * (CurrentStat.클래스.Power / 100.0);
                double 녹색공격력 = 무기대미지 * Information.GetPassiveValue(CurrentStat.오른손) / 100;

                //녹색공격력 += 마석옵션공격력수치;
                녹색공격력 += (CurrentStat.클래스.IsMagical == false) ? 마석옵션공격력수치 : GetDataGridViewValue(Information.GetColumnIndex("마법공격력"), 0);
                double 오른손공격력 = 흰색공격력 + 녹색공격력;
                double 오른손스킬에의한공격력 = 무기대미지 * (스킬공격력퍼센트 / 100.0);
                오른손공격력 += 오른손스킬에의한공격력;
                this.dataGridView[colIndex, 0].Value = Math.Round(오른손공격력, 2);
                Append(sb, "오른손 무기대미지", 무기대미지);
                Append(sb, "오른손 흰색공격력", 흰색공격력);
                Append(sb, "오른손 녹색공격력", 녹색공격력);
                Append(sb, "오른손 스킬에의한공격력", 오른손스킬에의한공격력);
                Append(sb, "오른손 공격력", 오른손공격력);

                // 왼손
                if (CurrentStat.왼손 != null)
                {
                    double 왼손무기평균대미지 = (CurrentStat.왼손.ValueDoubleInDictionary("min_damage") + CurrentStat.왼손.ValueDoubleInDictionary("max_damage")) / 2.0;
                    double 왼손무기강화대미지 = CurrentStat.왼손.ValueDoubleInDictionary("min_damage", CurrentStat.왼손.EnchantOption);
                    double 왼손무기대미지 = 왼손무기평균대미지 + 왼손무기강화대미지;

                    double 왼손흰색공격력 = 왼손무기대미지 * (CurrentStat.클래스.Power / 100.0);
                    double 왼손녹색공격력 = 왼손무기대미지 * Information.GetPassiveValue(CurrentStat.왼손) / 100;
                    //왼손녹색공격력 += 마석옵션공격력수치;
                    왼손녹색공격력 += (CurrentStat.클래스.IsMagical == false) ? 마석옵션공격력수치 : GetDataGridViewValue(Information.GetColumnIndex("마법공격력"), 0);
                    // 왼손.. 상수
                    왼손흰색공격력 *= CurrentStat.클래스.DualWieldConstValue;
                    왼손녹색공격력 *= CurrentStat.클래스.DualWieldConstValue;
                    double 왼손공격력 = 왼손흰색공격력 + 왼손녹색공격력;
                    double 왼손스킬에의한공격력 = 왼손무기대미지 * (스킬공격력퍼센트 / 100.0) * CurrentStat.클래스.DualWieldConstValue;
                    왼손공격력 += 왼손스킬에의한공격력;
                    this.dataGridView[colIndex + 5, 0].Value = Math.Round(왼손공격력, 2);
                    Append(sb, "왼손무기대미지", 왼손무기대미지);
                    Append(sb, "왼손흰색공격력", 왼손흰색공격력);
                    Append(sb, "왼손녹색공격력", 왼손녹색공격력);
                    Append(sb, "왼손스킬에의한공격력", 왼손스킬에의한공격력);
                    Append(sb, "왼손공격력", 왼손공격력);
                }

                int 명중columnIndex = Information.GetColumnIndex("명중");
                double 오른손명중 = GetDataGridViewValue(명중columnIndex, 0);
                double 오른손무기명중 = CurrentStat.오른손.ValueDoubleInDictionary("hitaccuracy", CurrentStat.오른손.MainOption);
                double 무기제외명중 = 오른손명중 - 오른손무기명중;
                double 왼손무기명중 = GetDataGridViewValue(명중columnIndex + 5, 0);
                double 왼손명중 = 왼손무기명중 + 무기제외명중;
                this.dataGridView[명중columnIndex + 5, 0].Value = Math.Round(왼손명중, 2);
                Append(sb, "오른손명중", 오른손명중);
                Append(sb, "왼손명중", 왼손명중);

                int 치명타columnIndex = Information.GetColumnIndex("치명타");
                double 오른손치명타 = GetDataGridViewValue(치명타columnIndex, 0);
                double 오른손무기치명타 = CurrentStat.오른손.ValueDoubleInDictionary("critical", CurrentStat.오른손.MainOption);
                double 무기제외치명타 = 오른손치명타 - 오른손무기치명타;
                double 왼손무기치명타 = GetDataGridViewValue(치명타columnIndex + 5, 0);
                double 왼손치명타 = 왼손무기치명타 + 무기제외치명타;
                this.dataGridView[치명타columnIndex + 5, 0].Value = Math.Round(왼손치명타, 2);
                Append(sb, "오른손치명타", 오른손치명타);
                Append(sb, "왼손치명타", 왼손치명타);

                //마증 처리 : 오른손값만
                int 마법증폭력columnIndex = Information.GetColumnIndex("마법증폭력");
                double 두손합마법증폭력 = GetDataGridViewValue(마법증폭력columnIndex, 0);
                double 왼손마법증폭력 = CurrentStat.왼손.ValueDoubleInDictionary("magicalskillboost", CurrentStat.왼손.MainOption);
                double 마법증폭력 = 두손합마법증폭력 - 왼손마법증폭력;
                this.dataGridView[마법증폭력columnIndex, 0].Value = Math.Round(마법증폭력, 2);
                Append(sb, "마법증폭력", 마법증폭력);


                //공속
                // Overall Speed = (MainHandWeaponBase + OffhandWeaponBase/4) * (100% – WeaponBonus – WeaponBonus/4 – OtherEquipmentBonus – TitleBonus – TitleBonus/4)
                int 공격속도columnIndex = Information.GetColumnIndex("공격속도");
                double 오른손공속 = CurrentStat.오른손.ValueDoubleInDictionary("attack_delay") / 1000;
                double 왼손공속 = CurrentStat.왼손.ValueDoubleInDictionary("attack_delay") / 1000;
                double 오른손추가공속 = GetDataGridViewValue(공격속도columnIndex, 2);
                double 왼손손추가공속 = GetDataGridViewValue(공격속도columnIndex, 3);
                double 공속이낮은쪽 = (오른손추가공속 > 왼손손추가공속) ? 왼손손추가공속 : 오른손추가공속;
                double 현재전체공속 = GetDataGridViewValue(공격속도columnIndex, 0);
                double 낮은쪽제외공속 = 현재전체공속 - 공속이낮은쪽;
                //double 타이트공속 = (
                double 흰색공격속도 = (오른손공속 + 왼손공속 * 0.25);
                //double 녹색공격속도 = -(흰색공격속도 * 낮은쪽제외공속 / 100.0);
                double 공격속도 = 흰색공격속도 * (100 - 낮은쪽제외공속) / 100;
                this.dataGridView[공격속도columnIndex, 0].Value = Math.Round(공격속도, 1);
                Append(sb, "공격속도", 공격속도);
                #endregion
            }
            else if (CurrentStat.클래스 != null && CurrentStat.오른손 != null && CurrentStat.왼손 != null && CurrentStat.오른손.Is2Hand() && CurrentStat.왼손.Is2Hand() && CurrentStat.오른손.Type == CurrentStat.왼손.Type)
            {
                #region 합성
                this.dataGridView[Information.GetColumnIndex("min_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("min_damage") + 5, 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage") + 5, 0].Value = "";


                // 공격력
                int colIndex = Information.GetColumnIndex("공격력");
                double 마석옵션공격력수치 = (GetDataGridViewValue(colIndex, 0)); // 다른 모든 + 공격력

                double 무기평균대미지 = (CurrentStat.오른손.ValueDoubleInDictionary("min_damage") + CurrentStat.오른손.ValueDoubleInDictionary("max_damage")) / 2.0;
                double 무기강화대미지 = CurrentStat.오른손.ValueDoubleInDictionary("min_damage", CurrentStat.오른손.EnchantOption);
                double 메인무기대미지 = 무기평균대미지 + 무기강화대미지;
                double 보조무기대미지 = (CurrentStat.왼손.ValueDoubleInDictionary("min_damage") + CurrentStat.왼손.ValueDoubleInDictionary("max_damage")) / 2.0;
                double 보조무기10퍼센트 = Math.Truncate(보조무기대미지 / 10); // 정수? 
                double 무기대미지 = 메인무기대미지 + 보조무기10퍼센트;

                double 흰색공격력 = 무기대미지 * (CurrentStat.클래스.Power / 100.0);
                double 녹색공격력 = 무기대미지 * Information.GetPassiveValue(CurrentStat.오른손) / 100;

                //녹색공격력 += 마석옵션공격력수치;
                녹색공격력 += (CurrentStat.클래스.IsMagical == false) ? 마석옵션공격력수치 : GetDataGridViewValue(Information.GetColumnIndex("마법공격력"), 0);
                double 합성공격력 = 흰색공격력 + 녹색공격력;
                double 스킬에의한공격력 = 무기대미지 * (스킬공격력퍼센트 / 100.0);
                합성공격력 += 스킬에의한공격력;
                this.dataGridView[colIndex, 0].Value = Math.Round(합성공격력, 2);
                Append(sb, "메인 무기대미지", 메인무기대미지);
                Append(sb, "보조무기대미지", 보조무기대미지);
                Append(sb, "보조무기10퍼센트", 보조무기10퍼센트);
                Append(sb, "무기대미지", 무기대미지);
                Append(sb, "흰색공격력", 흰색공격력);
                Append(sb, "녹색공격력", 녹색공격력);
                Append(sb, "스킬에의한공격력", 스킬에의한공격력);
                Append(sb, "합성공격력", 합성공격력);

                int 명중columnIndex = Information.GetColumnIndex("명중");
                this.dataGridView[명중columnIndex + 5, 0].Value = "";

                int 치명타columnIndex = Information.GetColumnIndex("치명타");
                this.dataGridView[치명타columnIndex + 5, 0].Value = "";

                //마증 +10
                int 마법증폭력columnIndex = Information.GetColumnIndex("마법증폭력");
                double 현재마증합 = GetDataGridViewValue(마법증폭력columnIndex, 0);
                double 오른손무기마법증폭력 = CurrentStat.오른손.ValueDoubleInDictionary("magicalskillboost", CurrentStat.오른손.MainOption);
                double 왼손무기마법증폭력 = CurrentStat.왼손.ValueDoubleInDictionary("magicalskillboost", CurrentStat.왼손.MainOption);
                double 마법증폭력 = 현재마증합 - 왼손무기마법증폭력 + (Math.Truncate(왼손무기마법증폭력 / 10));
                this.dataGridView[마법증폭력columnIndex, 0].Value = Math.Round(마법증폭력, 2);
                Append(sb, "마법증폭력", 마법증폭력);

                //공속
                // Overall Speed = (MainHandWeaponBase + OffhandWeaponBase/4) * (100% – WeaponBonus – WeaponBonus/4 – OtherEquipmentBonus – TitleBonus – TitleBonus/4)
                int 공격속도columnIndex = Information.GetColumnIndex("공격속도");
                double 오른손공속 = CurrentStat.오른손.ValueDoubleInDictionary("attack_delay") / 1000;
                double 왼손공속 = CurrentStat.왼손.ValueDoubleInDictionary("attack_delay") / 1000;
                double 오른손추가공속 = GetDataGridViewValue(공격속도columnIndex, 2);
                double 왼손손추가공속 = GetDataGridViewValue(공격속도columnIndex, 3);
                double 공속이낮은쪽 = (오른손추가공속 > 왼손손추가공속) ? 왼손손추가공속 : 오른손추가공속;
                double 현재전체공속 = GetDataGridViewValue(공격속도columnIndex, 0);
                double 낮은쪽제외공속 = 현재전체공속 - 공속이낮은쪽;
                //double 타이트공속 = (
                double 흰색공격속도 = (오른손공속);
                //double 녹색공격속도 = -(흰색공격속도 * 낮은쪽제외공속 / 100.0);
                double 공격속도 = 흰색공격속도 * (100 - 낮은쪽제외공속) / 100;
                this.dataGridView[공격속도columnIndex, 0].Value = Math.Round(공격속도, 1);
                Append(sb, "공격속도", 공격속도);
                #endregion
            }
            else if (CurrentStat.클래스 != null && CurrentStat.오른손 != null)
            {
                #region 오른손만 있을경우.. 방방일때도
                this.dataGridView[Information.GetColumnIndex("min_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage"), 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("min_damage") + 5, 0].Value = "";
                this.dataGridView[Information.GetColumnIndex("max_damage") + 5, 0].Value = "";

                // 공격력
                int colIndex = Information.GetColumnIndex("공격력");
                double 마석옵션공격력수치 = (GetDataGridViewValue(colIndex, 0)); // 다른 모든 + 공격력

                double 무기평균대미지 = (CurrentStat.오른손.ValueDoubleInDictionary("min_damage") + CurrentStat.오른손.ValueDoubleInDictionary("max_damage")) / 2.0;
                double 무기강화대미지 = CurrentStat.오른손.ValueDoubleInDictionary("min_damage", CurrentStat.오른손.EnchantOption);
                double 무기대미지 = 무기평균대미지 + 무기강화대미지;
                double 흰색공격력 = 무기대미지 * (CurrentStat.클래스.Power / 100.0);
                double 녹색공격력 = 무기대미지 * Information.GetPassiveValue(CurrentStat.오른손) / 100;

                //녹색공격력 += 마석옵션공격력수치;
                녹색공격력 += (CurrentStat.클래스.IsMagical == false) ? 마석옵션공격력수치 : GetDataGridViewValue(Information.GetColumnIndex("마법공격력"), 0);
                double 공격력 = 흰색공격력 + 녹색공격력;
                double 스킬에의한공격력 = 무기대미지 * (스킬공격력퍼센트 / 100.0);
                공격력 += 스킬에의한공격력;
                this.dataGridView[colIndex, 0].Value = Math.Round(공격력, 2);
                Append(sb, "무기대미지", 무기대미지);
                Append(sb, "흰색공격력", 흰색공격력);
                Append(sb, "녹색공격력", 녹색공격력);
                Append(sb, "스킬에의한공격력", 스킬에의한공격력);
                Append(sb, "공격력", 공격력);

                //마증
                // 따로 계산할 필요가 없다?

                //공속
                int 공격속도columnIndex = Information.GetColumnIndex("공격속도");
                double 오른손공속 = CurrentStat.오른손.ValueDoubleInDictionary("attack_delay") / 1000;
                double 현재전체공속 = GetDataGridViewValue(공격속도columnIndex, 0);
                double 흰색공격속도 = (오른손공속);
                double 공격속도 = 흰색공격속도 * (100 - 현재전체공속) / 100;
                this.dataGridView[공격속도columnIndex, 0].Value = Math.Round(공격속도, 1);
                Append(sb, "공격속도", 공격속도);
                #endregion

            }
            this.textBox2.Text = "";
            this.textBox2.AppendText(sb.ToString());
        }


        #region update에서 호출하는 메소드
        private void linkLabelUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            update.Start(1);
        }

        private void linkLabelGotoHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GotoHomepage(0);
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
            try
            {
                this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                this.panelUpdate.Controls.Remove(this.progressBar1);
            }));
            }
            catch { }
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
                this.tabControl1.SelectedIndex = 2;
            }));
        }

        public void Exit()
        {
            this.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
            {
                Application.Exit();
            }));
        }

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

        #endregion


        public void GotoHomepage(int where)
        {
            String url = null;
            if (where == 0)
            {
                url = "http://soju6jan.cafe24.com/xe/page_aion_main";
            }
            else if (where == 1)
            {
                url = "http://aion.plaync.co.kr";
            }
            System.Diagnostics.Process IEProcess = new System.Diagnostics.Process();
            IEProcess.StartInfo.FileName = "iexplore.exe";
            IEProcess.StartInfo.Arguments = url;
            IEProcess.Start();
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AionLogAnalyzer
{
    
    public class SkillEntity
    {
        public String SkillName;
        public int TotalDamage = 0;
        public int Percent = 0;
        public int Count = 0;
        public int AverageDamage = 0;
        public int CriticalCount = 0;
    }

    public class HPMPWhoEntity
    {
        public string Who;
        public List<HPMPSkillEntity> SkillList;
        public HPMPWhoEntity(string who)
        {
            this.Who = who;
            SkillList = new List<HPMPSkillEntity>();
            HPMPSkillEntity total = new HPMPSkillEntity();
            total.SkillName = "전체";
            SkillList.Add(total);
        }
    }

    public class HPMPSkillEntity
    {
        public String SkillName;
        public int TotalRecover = 0;
        public int Count = 0;
    }

    public class HealSkillEntity
    {
        public string Target;
        public int TotalRecover = 0;
        public int Count = 0;
    }


    public class User
    {
        public ClassType Class;
        public String Name;
        public String DisplayName;
        public int TotalDamage;
        public int MaxDamage;
        public int TotalDealTime = 1;
        public int DPS;
        public ListViewItem ListItem;

        public int TotalDealCount = 0;
        private DateTime lastDataTime = DateTime.Now;
        private int totalSkillDealCount = 0;
        private int totalNormalDealCount = 0;
        public int TotalCriticalDealCount = 0;

        public int percentSkill = 0;
        public int percentNormal = 0;
        public int percentCritical = 0;

        public List<string> LogList;
        public List<string> HPRecoverLogList;
        public List<string> MPRecoverLogList;
        public List<string> HealLogList;

        public List<SkillEntity> SkillList;
        public List<HPMPWhoEntity> HPWhoList;
        public List<HPMPWhoEntity> MPWhoList;
        public List<HealSkillEntity> HealList;

        public bool SendRecord = false;
        public String LastTarget;

        // 2013--05-21
        public int NewTotalDealCount = 0;
        public int TotalNormalCancelCount = 0;
        public int MaxNormalCancelCount = 0;
        public int CurrentNormalCancelCount = 0;
        public int PercentNormalCancel = 0;
        public int NewTotalNormalDealCount = 0;
        public int NewTotalSkillDealCount = 0;

        //2013-05-30
        public int NewTotalNormalDamage = 0;
        public int NewTotalSkillDamage = 0;

        private bool isLastSkill = true;

        //2013-09-26
        public int HPRecover = 0;
        public int MPRecover = 0;
        public int HealAmount = 0;

        public User(String name)
        {
            Class = ClassType.NONE;
            this.Name = this.DisplayName = name;
            if (this.Name == "다솔이")
            {
                this.DisplayName = "다솔이병신새끼";
            }
            // 2013-09-26 생명력 / 정신력
            ListItem = new ListViewItem(new string[21 + 3]);
            SkillList = new List<SkillEntity>();
            HPWhoList = new List<HPMPWhoEntity>();
            MPWhoList = new List<HPMPWhoEntity>();
            HealList = new List<HealSkillEntity>();

            ListItem.SubItems[2].Text = DisplayName;
            LogList = new List<string>();
            HPRecoverLogList = new List<string>();
            MPRecoverLogList = new List<string>();
            HealLogList = new List<string>();

            if (name == LogParser.Myname)
            {
                ListItem.BackColor = System.Drawing.Color.Yellow;
            }
            else if (name == LogParser.Noname)
            {
                Class = ClassType.EMPTY;
                ListItem.BackColor = System.Drawing.Color.GreenYellow;
            }
        }
        // 순위, 클래스, 이름, dps, 총데, 딜시간, 최대데미지, 
        // 공격횟수, 스킬비율, 평타비율, 치명타비율
        public void UpdataListView()
        {
            if (ListItem.SubItems[1].Text == "")
            {
                ListItem.SubItems[1].Text = SkillDictionary.GetClassString(Class);
            }

            ListItem.SubItems[3].Text = DPS + "";
            ListItem.SubItems[4].Text = TotalDamage + "";
            ListItem.SubItems[5].Text = TotalDealTime + "초";
            ListItem.SubItems[6].Text = HPRecover + "";
            ListItem.SubItems[7].Text = HealAmount + "";

            ListItem.SubItems[8].Text = MaxDamage + "";
            ListItem.SubItems[9].Text = TotalDealCount + "회";
            ListItem.SubItems[10].Text = percentSkill + "%";
            ListItem.SubItems[11].Text = percentNormal + "%";
            ListItem.SubItems[12].Text = percentCritical + "%";

            ListItem.SubItems[13].Text = NewTotalNormalDealCount + "회";
            ListItem.SubItems[14].Text = NewTotalSkillDealCount + "회";
            ListItem.SubItems[15].Text = NewTotalDealCount + "회";
            ListItem.SubItems[16].Text = TotalNormalCancelCount + "회";
            ListItem.SubItems[17].Text = PercentNormalCancel + "%";
            ListItem.SubItems[18].Text = MaxNormalCancelCount + "회";
            ListItem.SubItems[19].Text = CurrentNormalCancelCount + "회";
            ListItem.SubItems[20].Text = (NewTotalNormalDealCount == 0) ? "0" : (NewTotalNormalDamage / NewTotalNormalDealCount) + "";
            ListItem.SubItems[21].Text = (NewTotalSkillDealCount == 0) ? "0" : (NewTotalSkillDamage / NewTotalSkillDealCount) + "";
            ListItem.SubItems[22].Text = (NewTotalDealCount == 0) ? "0" : (TotalDamage / NewTotalDealCount) + "";
            
            ListItem.SubItems[23].Text = MPRecover +"";
            


        }

        public void AddDamage(PlayerDamageEventArgs e)
        {
            //5초간 딜이 없다면 마지막시간으로
            //int temp = DateTime.Compare(e.Time, lastDataTime);
            int temp = e.Time.Subtract(lastDataTime).Seconds;

#if TICK
                        if (temp > 0) totalDealTime++;
#else
            if (temp < 5 && temp >= 0)
            {
                TotalDealTime += temp;
            }
            else
            {
                TotalDealTime++;
            }
#endif
            TimeSpan timeSpan = e.Time - lastDataTime;
            lastDataTime = e.Time;

            TotalDamage += e.Damage;
            if (e.Damage > MaxDamage) MaxDamage = e.Damage;

            this.TotalDealCount++;
            if (e.Skill != "")
            {
                this.totalSkillDealCount++;
                if (Class == ClassType.NONE)
                {
                    Class = SkillDictionary.GetClass(e.Skill);
                    if (Class == ClassType.NONE)
                    {

                    }
                }
            }
            else
            {
                e.Skill = "평타";
                this.totalNormalDealCount++;
            }


            // NewTotalDealCount TotalNormalCancelCount MaxNormalCancelCount CurrentNormalCancelCount
            // 2013-05-30
            if (e.Skill == "평타") NewTotalNormalDamage += e.Damage;
            else NewTotalSkillDamage += e.Damage;

            // 2013-05-21
            if (NewTotalDealCount == 0) // 첫타는..
            {
                NewTotalDealCount++;
                if (e.Skill == "평타") NewTotalNormalDealCount++;
                else NewTotalSkillDealCount++;
            }
            else
            {
                if (isLastSkill == true && e.Skill != "평타") //스킬 & 스킬
                {
                    NewTotalDealCount++;
                    CurrentNormalCancelCount = 0;
                    NewTotalSkillDealCount++;
                }
                else if ((isLastSkill == true && e.Skill == "평타") || (isLastSkill == false && e.Skill != "평타")) // 스킬 & 평타
                {
                    NewTotalDealCount++;
                    if (timeSpan.TotalSeconds <= 3)
                    {
                        TotalNormalCancelCount++;
                        CurrentNormalCancelCount++;
                    }
                    else
                    {
                        CurrentNormalCancelCount = 0;
                    }
                    if (e.Skill == "평타") NewTotalNormalDealCount++;
                    else NewTotalSkillDealCount++;
                }
                else if (isLastSkill == false && e.Skill == "평타") //평타 & 평타 
                {
                    if (timeSpan.TotalSeconds > 0)
                    {
                        NewTotalDealCount++;
                        NewTotalNormalDealCount++;
                        CurrentNormalCancelCount = 0;
                    }
                }
            }
            isLastSkill = (e.Skill != "평타");

            MaxNormalCancelCount = (CurrentNormalCancelCount > MaxNormalCancelCount) ? CurrentNormalCancelCount : MaxNormalCancelCount;
            PercentNormalCancel = (NewTotalDealCount > 1) ? (TotalNormalCancelCount * 100 / (NewTotalDealCount - 1)) : 0;

            SkillEntity skillEntity = null;
            foreach (SkillEntity skill in SkillList)
            {
                if (skill.SkillName == e.Skill)
                {
                    skillEntity = skill;
                    break;
                }
            }
            if (skillEntity == null)
            {
                SkillEntity se = new SkillEntity();
                se.SkillName = e.Skill;
                SkillList.Add(se);
                skillEntity = se;
            }
            skillEntity.TotalDamage += e.Damage;
            skillEntity.Count++;
            if (e.bCritical) skillEntity.CriticalCount++;


            this.TotalCriticalDealCount = this.TotalCriticalDealCount + ((e.bCritical) ? 1 : 0);
            DPS = (TotalDealTime > 0) ? (TotalDamage / this.TotalDealTime) : 0;
            percentSkill = (TotalDealCount > 0) ? (totalSkillDealCount * 100 / TotalDealCount) : 0;
            percentNormal = (TotalDealCount > 0) ? (totalNormalDealCount * 100 / TotalDealCount) : 0;
            // 2013-05-10 평타는 아예 빼기 때문에 스킬만 계산
            percentCritical = (totalSkillDealCount > 0) ? (TotalCriticalDealCount * 100 / totalSkillDealCount) : 0;

            LogList.Add(e.log);
            LastTarget = e.Target;
        }

        public void AddRecover(RecoverEventArgs e)
        {
            List<HPMPWhoEntity> whoList = null;

            if (e.IsHP)
            {
                this.HPRecover += e.Amount;
                HPRecoverLogList.Add(e.log);
                whoList = HPWhoList;
            }
            else
            {
                this.MPRecover += e.Amount;
                MPRecoverLogList.Add(e.log);
                whoList = MPWhoList;
            }


            HPMPWhoEntity whoEntity = null;
            HPMPSkillEntity skillEntity = null;
            e.Who = (String.IsNullOrEmpty(e.Who)) ? null : e.Who;
            e.Skill = (String.IsNullOrEmpty(e.Skill)) ? null : e.Skill;
            foreach (HPMPWhoEntity entity in whoList)
            {
                if (entity.Who == e.Who)
                {
                    whoEntity = entity;
                    break;
                }
            }

            if (whoEntity == null)
            {
                whoEntity = new HPMPWhoEntity((String.IsNullOrEmpty(e.Who)) ? null : e.Who);
                whoList.Add(whoEntity);
            }

            foreach (HPMPSkillEntity se in whoEntity.SkillList)
            {
                if (se.SkillName == e.Skill)
                {
                    skillEntity = se;
                    break;
                }
            }

            if (skillEntity == null)
            {
                skillEntity = new HPMPSkillEntity();
                skillEntity.SkillName = e.Skill;
                whoEntity.SkillList.Add(skillEntity);
            }
            skillEntity.TotalRecover += e.Amount;
            skillEntity.Count++;
            whoEntity.SkillList[0].TotalRecover += e.Amount;
            whoEntity.SkillList[0].Count++;
        }

        public void AddHeal(RecoverEventArgs e)
        {
            this.HealAmount += e.Amount;
            if (Class == ClassType.NONE)
            {
                Class = SkillDictionary.GetClass(e.Skill);
            }
            this.HealLogList.Add(e.log);
            HealSkillEntity entity = null;
            foreach (HealSkillEntity hse in HealList)
            {
                if (hse.Target == e.Name)
                {
                    entity = hse;
                    break;
                }
            }
            if (entity == null)
            {
                entity = new HealSkillEntity();
                entity.Target = e.Name;
                this.HealList.Add(entity);
            }
            entity.TotalRecover += e.Amount;
            entity.Count++;
        }



        public void Dispose()
        {
            LogList.Clear();
            HPRecoverLogList.Clear();
            MPRecoverLogList.Clear();
            HealLogList.Clear();
            SkillList.Clear();
            HPWhoList.Clear();
            MPWhoList.Clear();
            HealList.Clear();
        }

        public void SetName(String name)
        {
            this.Name = name;
            this.DisplayName = name;
            ListItem.SubItems[2].Text = Name;
        }

        public void ResetData()
        {
            TotalDamage = 0;
            MaxDamage = 0;
            TotalDealTime = 1;
            DPS = 0;
            TotalDealCount = 0;
            lastDataTime = DateTime.Now;
            totalSkillDealCount = 0;
            totalNormalDealCount = 0;
            TotalCriticalDealCount = 0;
            percentSkill = 0;
            percentNormal = 0;
            percentCritical = 0;
            LogList = new List<string>();
            HPRecoverLogList = new List<string>();
            MPRecoverLogList = new List<string>();
            HealLogList = new List<string>();
            SkillList = new List<SkillEntity>();
            HPWhoList = new List<HPMPWhoEntity>();
            MPWhoList = new List<HPMPWhoEntity>();
            HealList = new List<HealSkillEntity>();
            NewTotalDealCount = 0;
            TotalNormalCancelCount = 0;
            MaxNormalCancelCount = 0;
            CurrentNormalCancelCount = 0;
            PercentNormalCancel = 0;
            NewTotalNormalDealCount = 0;
            NewTotalSkillDealCount = 0;
            NewTotalNormalDamage = 0;
            NewTotalSkillDamage = 0;
            SendRecord = false;
            HPRecover = 0;
            MPRecover = 0;
            HealAmount = 0;
            UpdataListView();
        }
    }

    public class Data
    {
        // User
        public List<User> UserList;

        public Data()
        {
            UserList = new List<User>();
        }

        public void Reset()
        {
            if (UserList != null)
            {
                UserList.Clear();
            }
        }

        // 이름 클래스 정보를 제외하고 딜 관련된 사항은 모두 지운다
        public void ResetExceptName()
        {
            if (UserList != null)
            {
                foreach (User u in UserList)
                {
                    u.ResetData();
                }
            }
        }



        public User GetPlayer(String name)
        {
            foreach (User u in UserList)
            {
                if (u.Name == name) return u;
            }
            return null;
        }
        public User GetPlayerByDisplayName(String name)
        {
            foreach (User u in UserList)
            {
                if (u.DisplayName == name) return u;
            }
            return null;
        }

        public User GetFirsPlayerByClass(ClassType aionClass)
        {
            foreach (User u in UserList)
            {
                if (u.Class == aionClass) return u;
            }
            return null;
        }

        public void RemovePlayer(String name)
        {
            User remove = null;
            foreach (User u in UserList)
            {
                if (u.Name == name)
                {
                    remove = u;
                    break;
                }
            }
            if (remove != null)
            {
                remove.Dispose();
                UserList.Remove(remove);
            }
        }

        public void SetDealTime(int time)
        {
            foreach (User u in UserList)
            {
                u.TotalDealTime = time;
                u.DPS = (u.TotalDealTime > 0) ? (u.TotalDamage / u.TotalDealTime) : 0;
                u.UpdataListView();
            }
        }
    }
}

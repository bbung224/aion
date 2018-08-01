using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AionLogAnalyzer
{
    public class TargetUser
    {
        public String Name;
        public int Damage = 0;
        public TargetUser(String s)
        {
            this.Name = s;
        }
    }

    public class TargetTotalDamage
    {
        public String Target;
        public int TotalDamage = 0;
        public List<TargetUser> UserList;


        public TargetTotalDamage(String t)
        {
            this.Target = t;
            UserList = new List<TargetUser>();
        }
        public void AddDamage(PlayerDamageEventArgs e)
        {
            this.TotalDamage += e.Damage;
            TargetUser user = GetTargetUser(e.NameAfterAdd);
            if (user == null)
            {
                user = new TargetUser(e.NameAfterAdd);
                UserList.Add(user);
            }
            user.Damage += e.Damage;
        }

        public TargetUser GetTargetUser(String name)
        {
            foreach (TargetUser t in UserList)
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            return null;
        }

        public void SortByDamage()
        {
            this.UserList.Sort(SortList);
        }

        int SortList(TargetUser u1, TargetUser u2)
        {
            if (u1.Damage < u2.Damage) return 1;
            if (u1.Damage > u2.Damage) return -1;
            return 0;
        }
    }
    

    public abstract class InstanceDungeon
    {
        public enum DungeonList
        {
            루나디움, 용제의안식처, 카탈라마이즈, 룬의보호탑
        }
        public static String[] InDunNameList = new String[] 
        { 
          "루나디움", "용제의 안식처", "카탈라마이즈", "룬의 보호탑"                                       
        };

        public static bool IsProcessDungeon(String str)
        {
            foreach (String tmp in InDunNameList)
            {
                if (tmp == str) return true;
            }
            return false;
        }

        public static InstanceDungeon MakeInstance(String str, MainForm main)
        {
            InstanceDungeon indun = null;
            if (InstanceDungeon.InDunNameList[(int)InstanceDungeon.DungeonList.루나디움] == str)
            {
                indun = new InstanceDungeon루나디움(str, main);
            }
            else if (InstanceDungeon.InDunNameList[(int)InstanceDungeon.DungeonList.용제의안식처] == str)
            {
                indun = new InstanceDungeon용제의안식처(str, main);
            }
            else if (InstanceDungeon.InDunNameList[(int)InstanceDungeon.DungeonList.카탈라마이즈] == str)
            {
                indun = new InstanceDungeon카탈라마이즈(str, main);
            }
            else if (InstanceDungeon.InDunNameList[(int)InstanceDungeon.DungeonList.룬의보호탑] == str)
            {
                indun = new InstanceDungeon룬의보호탑(str, main);
            }
            return indun;
        }

        
        public List<InstanceDungeonEventArgs> EventList;
        public String DungeonName;
        public List<TargetTotalDamage> TargetList;
        protected MainForm main;
        public List<User> UserList;
        protected String info;
        protected String ClearTime;
        protected int TotalDamage = 0;
        public bool SendRecord = false;
        protected StringBuilder sb;

        // 2013-05-24
        public bool bProcessEnd = false;

        public String BossName = null;
        public int BossTotalDamage = 0;
        public TargetTotalDamage BossTarget = null;

        public InstanceDungeon(String dungeonName, MainForm main)
        {
            this.DungeonName = dungeonName;
            EventList = new List<InstanceDungeonEventArgs>();
            TargetList = new List<TargetTotalDamage>();
            this.main = main;
        }

        public virtual String GetInfo()
        {
            User[] userList = new User[main.data.UserList.Count];
            main.data.UserList.CopyTo(userList);
            UserList = new List<User>();
            UserList.AddRange(userList);
            SortUserList();
            SortTargetByDamage();

            TotalDamage = 0;
            foreach (TargetTotalDamage t in TargetList)
            {
                TotalDamage += t.TotalDamage;
            }
            return null;
        }

        public abstract String GetQuery();

        public void AddEvent(InstanceDungeonEventArgs e)
        {
            EventList.Add(e);
        }

        public void AddDamage(PlayerDamageEventArgs e)
        {
            TargetTotalDamage target = GetTarget(e.Target);
            if (target == null)
            {
                target = new TargetTotalDamage(e.Target);
                TargetList.Add(target);
            }
            target.AddDamage(e);

        }

        public TargetTotalDamage GetTarget(String target)
        {
            foreach (TargetTotalDamage t in TargetList)
            {
                if (t.Target == target)
                {
                    return t;
                }
            }
            return null;
        }

        public void SortTargetByDamage()
        {
            this.TargetList.Sort(SortList);
        }

        public TargetTotalDamage GetBossTarget()
        {
            if (BossTarget == null)
            {
                BossTarget = GetTarget(BossName);
            }
            return BossTarget;
        }

        int SortList(TargetTotalDamage u1, TargetTotalDamage u2)
        {
            if (u1.TotalDamage < u2.TotalDamage) return 1;
            if (u1.TotalDamage > u2.TotalDamage) return -1;
            return 0;
        }

        protected void SortUserList()
        {
            this.UserList.Sort(SortUser);
        }

        int SortUser(User u1, User u2)
        {
            if (u1.TotalDamage < u2.TotalDamage) return 1;
            if (u1.TotalDamage > u2.TotalDamage) return -1;
            return 0;
        }

        protected void AppendTitle()
        {
            sb.AppendLine("< " + DungeonName + " 공략 >");
            sb.AppendLine();

            
        }

        protected void AppendUser()
        {
            int i = 0;
            foreach (User u in UserList)
            {
                sb.AppendLine("  " + (++i) + ". " + SkillDictionary.GetClassString(u.Class) + " " + u.Name + "(" + u.TotalDamage + " " + (100 * u.TotalDamage / TotalDamage) + "%)");
            }
        }

        protected void AppendTarget()
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("-- 타겟별 대미지");
            sb.AppendLine();
            
            foreach (TargetTotalDamage t in TargetList)
            {
                sb.AppendLine(t.Target + " : " + t.TotalDamage);
                t.SortByDamage();
                int idx = 0;
                foreach (TargetUser u in t.UserList)
                {
                    sb.AppendLine("  " + (++idx) + ". " + u.Name + "(" + u.Damage + " " + (100 * u.Damage / t.TotalDamage) + "%)");
                }
                sb.AppendLine();
            }
        }
        protected String TimeText(TimeSpan time)
        {
            return ((time.Minutes < 10) ? "0" : "") + time.Minutes + "분 " + ((time.Seconds < 10) ? "0" : "") + time.Seconds + "초";

            /*int m = (int)time.Minutes;
            int s = (int)time.Seconds;

            //return String.Format("{0,00}", m) + "분 " + String.Format("{0,00}", s) + "초";
*/
        }
    }

    public class InstanceDungeon루나디움 : InstanceDungeon
    {
        public InstanceDungeon루나디움(String dungeonName, MainForm main) : base(dungeonName, main)
        {
            BossName = "분노한 마녀 그렌달";
            BossTotalDamage = 1800000;
        }

        public override String GetInfo()
        {
            if (info != null) return info;
            base.GetInfo();
            sb = new StringBuilder();

            AppendTitle();

            if (EventList.Count == 4)
            {
                sb.AppendLine("-- 시간");
                sb.AppendLine();

                sb.AppendLine("  입장시간 : " + EventList[0].Time);
                sb.AppendLine("  종료시간 : " + EventList[3].Time);
                sb.AppendLine();

                TimeSpan time;
                time = (EventList[3].Time - EventList[1].Time);

                // 2013-05-07 웹에서 정렬할때 10초 미만에 버그
                // 2013-05-12 분도 마찬가지
                ClearTime = TimeText(time);
                //ClearTime = time.Minutes + "분 " + ((time.Seconds < 10) ? "0" : "") + time.Seconds + "초";
                sb.AppendLine("  공략시간 (저주받은 그렌달 소환부터) : " + ClearTime);


                time = (EventList[3].Time - EventList[0].Time);
                sb.AppendLine("  입장시부터 공략시간 : " + TimeText(time));
                time = (EventList[1].Time - EventList[0].Time);
                sb.AppendLine("  입장부터 저주받은 그렌달 소환까지 : " + TimeText(time));
                time = (EventList[2].Time - EventList[1].Time);
                sb.AppendLine("  저주받은 그렌달 처리시간 : " + TimeText(time));
                time = (EventList[3].Time - EventList[2].Time);
                sb.AppendLine("  분노한 그렌달 처리시간 : " + TimeText(time));

                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine("-- 전체 대미지 : " + TotalDamage);
            sb.AppendLine();
            AppendUser();
            AppendTarget();

            info = sb.ToString();
            return info;
        }

        public override String GetQuery()
        {
            String url = "";
            StringBuilder sb = new StringBuilder(url);
            sb.Append("?" + "starttime" + "=" + EventList[0].Time);
            sb.Append("&" + "endtime" + "=" + EventList[3].Time);
            sb.Append("&" + "cleartime" + "=" + ClearTime);
            sb.Append("&" + "totaldamage" + "=" + TotalDamage);

            // 2013-05-08 최대 유저수는 7을 안넘는다
            int tmp = UserList.Count;
            if (tmp > 7) tmp = 7;
            sb.Append("&" + "usercount" + "=" + tmp);
            sb.Append("&" + "userinfo" + "=");
            int count = 0;
            String userName = "";
            String userServer = "";
            foreach (User u in UserList)
            {
                sb.Append(SkillDictionary.GetClassString(u.Class));
                String playerName = u.Name;
                String[] temp = playerName.Split('-');
                if (temp.Length == 1)
                {
                    // 통합서버가 아니다.
                    userName = playerName;
                    userServer = (String)this.main.GetServer();
                }
                else
                {
                    // 통합서버다.
                    userName = temp[0];
                    userServer = temp[1];
                }
                sb.Append(":" + userServer);
                sb.Append(":" + userName);
                sb.Append(":" + u.TotalDamage);
                sb.Append(":" + ((u.Name == LogParser.Myname) ? 1 : 0));
                sb.Append(":" + u.DPS);
                sb.Append(":" + u.PercentNormalCancel);
                sb.Append(":" + u.HPRecover);
                sb.Append(":" + u.HealAmount);
                sb.Append("|");
                count++;
                if (count == 7) break;
            }
            sb.Append("&" + "version" + "=" + this.main.Version);
            return sb.ToString();
        }
    }


    public class InstanceDungeon용제의안식처 : InstanceDungeon
    {
        public bool bCheckPercent = false;

        public InstanceDungeon용제의안식처(String dungeonName, MainForm main)
            : base(dungeonName, main)
        {
            BossName = "티아마트";
            BossTotalDamage = 9000000;
        }

        public override String GetInfo()
        {
            if (info != null) return info;
            base.GetInfo();
            
            sb = new StringBuilder();

            AppendTitle();

            // 1.지역 채널 입장
            // 2.티아마트 첫 교전
            // 3.티아마트 50퍼
            // 4.종료
            if (EventList.Count == 4)
            {
                sb.AppendLine("-- 시간");
                sb.AppendLine();

                sb.AppendLine("  입장시간 : " + EventList[0].Time);
                sb.AppendLine("  종료시간 : " + EventList[3].Time);
                sb.AppendLine();

                TimeSpan time;
                time = (EventList[3].Time - EventList[1].Time);

                // 2013-05-07 웹에서 정렬할때 10초 미만에 버그
                ClearTime = TimeText(time);
                sb.AppendLine("  공략시간 (티아마트) : " + ClearTime);

                time = (EventList[2].Time - EventList[1].Time);
                sb.AppendLine("  공략시간 (티아마트 50% 까지) : " + TimeText(time));

                time = (EventList[3].Time - EventList[2].Time);
                sb.AppendLine("  공략시간 (티아마트 50% 이후) : " + TimeText(time));

                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine("-- 티아마트 50% 대미지 : ");
            sb.AppendLine();
            AppendUser();
            AppendTarget();

            info = sb.ToString();
            return info;
        }

        public override String GetQuery()
        {
            String url = "";
            StringBuilder sb = new StringBuilder(url);
            sb.Append("?" + "starttime" + "=" + EventList[0].Time);
            sb.Append("&" + "endtime" + "=" + EventList[3].Time);
            sb.Append("&" + "cleartime" + "=" + ClearTime);

            TimeSpan time = (EventList[2].Time - EventList[1].Time);
            String time1 = TimeText(time);

            time = (EventList[3].Time - EventList[2].Time);
            String time2 = TimeText(time);

            sb.Append("&" + "time1" + "=" + time1);
            sb.Append("&" + "time2" + "=" + time2);

            
            int tmp = UserList.Count;
            if (tmp > 13) tmp = 13;
            sb.Append("&" + "usercount" + "=" + tmp);
            sb.Append("&" + "userinfo" + "=");
            int count = 0;
            String userName = "";
            String userServer = "";
            foreach (User u in UserList)
            {
                sb.Append(SkillDictionary.GetClassString(u.Class));
                String playerName = u.Name;
                String[] temp = playerName.Split('-');
                if (temp.Length == 1)
                {
                    // 통합서버가 아니다.
                    userName = playerName;
                    userServer = (String)this.main.GetServer();
                }
                else
                {
                    // 통합서버다.
                    userName = temp[0];
                    userServer = temp[1];
                }
                sb.Append(":" + userServer);
                sb.Append(":" + userName);
                sb.Append(":" + u.TotalDamage);
                sb.Append(":" + ((u.Name == LogParser.Myname) ? 1 : 0));
                // 2013-05-21추가
                sb.Append(":" + u.DPS);
                sb.Append(":" + u.PercentNormalCancel);
                sb.Append(":" + u.HPRecover);
                sb.Append(":" + u.HealAmount);
                sb.Append("|");
                count++;
                if (count == 13) break;
            }
            sb.Append("&" + "version" + "=" + this.main.Version);
            return sb.ToString();
        }
    }

    public class InstanceDungeon카탈라마이즈 : InstanceDungeon
    {
        

        public InstanceDungeon카탈라마이즈(String dungeonName, MainForm main)
            : base(dungeonName, main)
        {
            BossName = "히페리온";
            BossTotalDamage = 9220000;
        }

        public override String GetInfo()
        {
            if (info != null) return info;
            base.GetInfo();

            sb = new StringBuilder();

            AppendTitle();

            // 1.지역 채널 입장
            // 2.히페리온 첫 교전
            // 3.히페리온 경험치
            if (EventList.Count == 3)
            {
                sb.AppendLine("-- 시간");
                sb.AppendLine();

                sb.AppendLine("  입장시간 : " + EventList[0].Time);
                sb.AppendLine("  종료시간 : " + EventList[2].Time);
                sb.AppendLine();

                TimeSpan time;
                time = (EventList[2].Time - EventList[1].Time);

                // 2013-05-07 웹에서 정렬할때 10초 미만에 버그
                ClearTime = TimeText(time);
                sb.AppendLine("  공략시간 (히페리온) : " + ClearTime);

                sb.AppendLine();
                sb.AppendLine();
            }
            sb.AppendLine("-- 전체 대미지 : " + TotalDamage);
            sb.AppendLine();
            AppendUser();
            AppendTarget();

            info = sb.ToString();
            return info;
        }

        public override String GetQuery()
        {
            String url = "";
            StringBuilder sb = new StringBuilder(url);
            sb.Append("?" + "cleartime" + "=" + ClearTime);

            int tmp = UserList.Count;
            if (tmp > 13) tmp = 13;
            sb.Append("&" + "usercount" + "=" + tmp);
            sb.Append("&" + "userinfo" + "=");
            int count = 0;
            String userName = "";
            String userServer = "";
            foreach (User u in UserList)
            {
                sb.Append(SkillDictionary.GetClassString(u.Class));
                String playerName = u.Name;
                String[] temp = playerName.Split('-');
                if (temp.Length == 1)
                {
                    // 통합서버가 아니다.
                    userName = playerName;
                    userServer = (String)this.main.GetServer();
                }
                else
                {
                    // 통합서버다.
                    userName = temp[0];
                    userServer = temp[1];
                }
                sb.Append(":" + userServer);
                sb.Append(":" + userName);
                sb.Append(":" + u.TotalDamage);
                sb.Append(":" + ((u.Name == LogParser.Myname) ? 1 : 0));
                sb.Append(":" + u.DPS);
                sb.Append(":" + u.PercentNormalCancel);
                sb.Append(":" + u.HPRecover);
                sb.Append(":" + u.HealAmount);
                sb.Append("|");
                count++;
                if (count == 13) break;
            }
            sb.Append("&" + "version" + "=" + this.main.Version);
            return sb.ToString();
        }



    }


    public class InstanceDungeon룬의보호탑 : InstanceDungeon
    {


        public InstanceDungeon룬의보호탑(String dungeonName, MainForm main)
            : base(dungeonName, main)
        {
            BossName = "시험병기 다이나툼";
            BossTotalDamage = 0;
        }

        public override String GetInfo()
        {
            if (info != null) return info;
            base.GetInfo();

            sb = new StringBuilder();

            AppendTitle();

            // 1.지역 채널 입장
            // 2.히페리온 첫 교전
            // 3.히페리온 경험치
            if (EventList.Count == 3)
            {
                sb.AppendLine("-- 시간");
                sb.AppendLine();

                sb.AppendLine("  입장시간 : " + EventList[0].Time);
                sb.AppendLine("  종료시간 : " + EventList[2].Time);
                sb.AppendLine();

                TimeSpan time;
                time = (EventList[2].Time - EventList[1].Time);

                // 2013-05-07 웹에서 정렬할때 10초 미만에 버그
                ClearTime = TimeText(time);
                //sb.AppendLine("  공략시간 (히페리온) : " + ClearTime);

                sb.AppendLine();
                sb.AppendLine();
            }
            sb.AppendLine("-- 전체 대미지 : " + TotalDamage);
            sb.AppendLine();
            AppendUser();
            AppendTarget();

            info = sb.ToString();
            return info;
        }

        public override String GetQuery()
        {
            return "";
        }



    }
}

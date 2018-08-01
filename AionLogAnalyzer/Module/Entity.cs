using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AionLogAnalyzer
{
    public static class Extensions
    {
        /// <summary>
        /// Get all digits from a specified string.
        /// </summary>
        public static int GetDigits(this string expression)
        {
            if (String.IsNullOrEmpty(expression))
            {
                return 0;
            }

            char c;
            string result = String.Empty;

            for (int i = 0; i < expression.Length; i++)
            {
                c = Convert.ToChar(expression.Substring(i, 1));

                if (Char.IsNumber(c))
                {
                    result += c;
                }
            }

            if (result.Length > 0)
            {
                return Convert.ToInt32(result);
            }

            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DateTime GetTime(this string expression, string format)
        {
            DateTime dateTime;
            if (DateTime.TryParse(expression, out dateTime))
            {
                return dateTime;
            }
            return DateTime.Now;
        }

        public static void ShowMessage(this string message)
        {
            //MessageBox.Show(message);
        }
    }


    public class LogEventArgs : EventArgs
    {
        private DateTime time;
        public String log;

        public DateTime Time
        {
            get
            {
                return time;
            }
        }
        public LogEventArgs(String log)
        {
            this.log = log;
        }
        public LogEventArgs(String log, DateTime time)
        {
            this.log = log;
            if (time == null)
            {
                time = DateTime.Now;
            }
            this.time = time;
        }

        public override string ToString()
        {
            return log;
        }
    }

    public class ChatCommandEventArgs : LogEventArgs
    {
        public int Command;
        public String Argument;
        public ChatCommandEventArgs(String log, DateTime time, int c, String arg)
            : base(log, time)
        {
            this.Command = c;
            this.Argument = arg;
        }
    }

    public enum MyStatus
    {
        Attack, // 대미지를 줬습니다
        BeBeaten1, //대미지를 받았습니다.
        BeBeaten2, //사용한영향으로대미지를받았습니다Regex
        BeBeaten3, //공격이반사되어대미지를받았습니다Regex
        Heal1, //효과로회복했습니다Regex
        Heal2, //사용한영향으로회복됐습니다Regex
        Heal3, //방법없이회복했습니다Regex 물약
        Repair1, //속도를회복했습니다Regex
        Debuff1, //사용한영향으로상태가됐습니다Regex
        Status1, //사용한영향으로기타등등Regex
        Repair2, //벗어났습니다Regex
        Status2, //사용해상태가됐습니다Regex
        Status3, //내스킬을써서효과가발생했습니다Regex
        Debuff2, //효과가발동했습니다.
        Debuff2Release,
        Die, //사망했습니다.


    }
    public class MyStatusEventArgs : LogEventArgs
    {
        public MyStatus Status;
        public String Argument;
        public MyStatusEventArgs(String log, DateTime time, MyStatus c, String arg)
            : base(log, time)
        {
            this.Status = c;
            this.Argument = arg;
        }

        public override string ToString()
        {
            String ret = "";
            ret = log.Substring(22);
            return ret;
        }
    }

    public class PlayerEventArgs : LogEventArgs
    {
        public string Name;

        public PlayerEventArgs(String log, DateTime time, string name)
            : base(log, time)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "Unknown Player";
            }
            this.Name = name;
        }
    }

    public class PlayerDamageEventArgs : PlayerEventArgs
    {
        public String Skill = null;
        public String Target = null;
        public int Damage;
        public bool bCritical;
        public String NameAfterAdd;


        public PlayerDamageEventArgs(String log, DateTime time, string name, string skill, string target, int damage, bool bCritical)
            : base(log, time, name)
        {
            if (damage < 0)
            {
                damage = 0;
            }
            this.Damage = damage;
            this.Skill = skill;
            this.Target = target;
            this.bCritical = bCritical;
        }
    }

    public class InstanceDungeonEventArgs : LogEventArgs
    {
        public int Command;
        public String Argument1 = null;
        public String Argument2 = null;
        public InstanceDungeonEventArgs(String log, DateTime time, int cmd, String agr1, String arg2) : base(log, time)
        {
            this.Command = cmd;
            this.Argument1 = agr1;
            this.Argument2 = arg2;
        }
    }


    public class RecoverEventArgs : LogEventArgs
    {
        public String Name = null;  //실제 회복하는 사람
        public String Who = null;   // 스킬을 써주는 사람
        public String Skill = null; // 스킬
        public int Amount;          // 량
        public bool IsHP;           // 생명력 or 정신력

        public RecoverEventArgs(String log, DateTime time, string Name, string Who, string Skill, int Amount, bool IsHP)
            : base(log, time)
        {
            this.Name = Name;
            this.Who = Who;
            this.Skill = Skill;
            this.Amount = Amount;
            this.IsHP = IsHP;
        }
    }
}

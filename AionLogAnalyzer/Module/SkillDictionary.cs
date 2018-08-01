using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AionLogAnalyzer
{
    public enum ClassType
    {
        NONE, 검성, 수호성, 살성, 궁성, 마도성, 정령성, 치유성, 호법성, 사격성, 음유성,기갑성,EMPTY
    }

    public struct SameSkill
    {
        public String SkillName;
        public ClassType LowLevelClass;
        public ClassType HighLevelClass;
        public SameSkill(String SkillName, ClassType LowLevelClass, ClassType HighLevelClass)
        {
            this.SkillName = SkillName;
            this.LowLevelClass = LowLevelClass;
            this.HighLevelClass = HighLevelClass;
        }
    }

    public static class SkillDictionary
    {
        private static Dictionary<string, ClassType> skillDic = new Dictionary<string, ClassType>();
        private static char[] _Numerals = { ' ', 'I', 'V', 'X' };

        public static String GetClassString(ClassType c)
        {
            if (c == ClassType.검성) return "검성";
            else if (c == ClassType.수호성) return "수호성";
            else if (c == ClassType.살성) return "살성";
            else if (c == ClassType.궁성) return "궁성";
            else if (c == ClassType.마도성) return "마도성";
            else if (c == ClassType.정령성) return "정령성";
            else if (c == ClassType.치유성) return "치유성";
            else if (c == ClassType.호법성) return "호법성";
            else if (c == ClassType.사격성) return "사격성";
            else if (c == ClassType.음유성) return "음유성";
            else if (c == ClassType.기갑성) return "기갑성";
            else if (c == ClassType.EMPTY) return "----";
            else return "";
        }

        public static String GetClassStringShort(ClassType c)
        {
            if (c == ClassType.검성) return "검";
            else if (c == ClassType.수호성) return "수";
            else if (c == ClassType.살성) return "살";
            else if (c == ClassType.궁성) return "궁";
            else if (c == ClassType.마도성) return "마";
            else if (c == ClassType.정령성) return "정";
            else if (c == ClassType.치유성) return "치";
            else if (c == ClassType.호법성) return "호";
            else if (c == ClassType.사격성) return "사";
            else if (c == ClassType.음유성) return "음";
            else if (c == ClassType.기갑성) return "기";
            else if (c == ClassType.EMPTY) return "";
            else return "";
        }

        public static SameSkill[] SameSkillList = new SameSkill[] {
            new SameSkill("침식", ClassType.마도성, ClassType.정령성),
            new SameSkill("바람의 약속", ClassType.치유성, ClassType.호법성)
        };

        public static ClassType GetClass(string skill)
        {
            if (skillDic.Count < 1)
            {
                PopulateDictionary();
            }

            String trimSkill = skill.TrimEnd(_Numerals);

            if (skillDic.ContainsKey(trimSkill))
            {
                return skillDic[trimSkill];
            }
            else
            {
                foreach (SameSkill sameSkill in SameSkillList)
                {
                    if (skill.StartsWith(sameSkill.SkillName))
                    {
                        if (skill == sameSkill.SkillName +" I") return sameSkill.LowLevelClass;
                        else return sameSkill.HighLevelClass;
                    }
                }
                return ClassType.NONE;
            }
        }

        private static void PopulateFromArray(string[] skills, ClassType classType)
        {
            foreach (string skill in skills)
            {
                if (!skillDic.ContainsKey(skill))
                {
                    string s = skill.Trim();
                    skillDic.Add(s, classType);
                }
            }
        }

        private static void PopulateDictionary()
        {
            char[] sep = new char[] { ',' };
            PopulateFromArray(SkillNameList.검성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.검성);
            PopulateFromArray(SkillNameList.수호성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.수호성);
            PopulateFromArray(SkillNameList.살성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.살성);
            PopulateFromArray(SkillNameList.궁성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.궁성);
            PopulateFromArray(SkillNameList.마도성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.마도성);
            PopulateFromArray(SkillNameList.정령성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.정령성);
            PopulateFromArray(SkillNameList.치유성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.치유성);
            PopulateFromArray(SkillNameList.호법성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.호법성);
            PopulateFromArray(SkillNameList.사격성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.사격성);
            PopulateFromArray(SkillNameList.음유성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.음유성);
            PopulateFromArray(SkillNameList.기갑성.Split(sep, StringSplitOptions.RemoveEmptyEntries), ClassType.기갑성);



        }
    }

    


    
}
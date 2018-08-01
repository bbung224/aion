using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AES
{
    public class Stat
    {
        public InfoClassDefault 클래스;
        public EntityItem 오른손;
        public EntityItem 왼손;
        public EntityItem 상의;
        public EntityItem 어깨;
        public EntityItem 장갑;
        public EntityItem 하의;
        public EntityItem 신발;
        public EntityItem 머리;
        public EntityItem 목걸이;
        public EntityItem 귀고리1;
        public EntityItem 귀고리2;
        public EntityItem 반지1;
        public EntityItem 반지2;
        public EntityItem 허리띠;
        public EntityItem 날개;

        public EntityTitle 타이틀;
        public EntityItem 이디안;
        public EntityItem 음식;
        public EntityItem 캔디;
        //public List<EntitySetItem> 세트효과목록 = new List<EntitySetItem>();
        public List<EntitySetItemOption> 세트효과목록 = new List<EntitySetItemOption>();
        public List<EntityItem> 마석목록 = new List<EntityItem>();
        public List<EntityItem> 주문서목록 = new List<EntityItem>();
        public List<EntitySkill> 스킬목록 = new List<EntitySkill>();

        // 파일 핸들링
        public string FullFileName = null;
        public string OnlyFileName = null;
        public string Desc = null;
        public string Version = null;


        public EntityItem GetEntityItemByRowIndex(int rowIndex)
        {
            if (rowIndex == 2) return 오른손;
            else if (rowIndex == 3) return 왼손;
            else if (rowIndex == 4) return 상의;
            else if (rowIndex == 5) return 어깨;
            else if (rowIndex == 6) return 장갑;
            else if (rowIndex == 7) return 하의;
            else if (rowIndex == 8) return 신발;
            else
            {
                //세트효과 21
                int countSetEffect = 세트효과목록.Count;
                int 마석시작 = (countSetEffect > 1) ? 22 + (countSetEffect - 1) : 22;
                int index = rowIndex - 마석시작;
                if (index < 마석목록.Count)
                    return 마석목록[index];
                else return null;
            }
        }

        public void SetNull(int rowIndex)
        {
            switch (rowIndex)
            {
                case 2: 오른손 = null; return;
                case 3: 왼손 = null; return;
                case 4: 상의 = null; return;
                case 5: 어깨 = null; return;
                case 6: 장갑 = null; return;
                case 7: 하의 = null; return;
                case 8: 신발 = null; return;
                case 9: 머리 = null; return;
                case 10: 목걸이 = null; return;
                case 11: 귀고리1 = null; return;
                case 12: 귀고리2 = null; return;
                case 13: 반지1 = null; return;
                case 14: 반지2 = null; return;
                case 15: 허리띠 = null; return;
                case 16: 날개 = null; return;
                case 17: 타이틀 = null; return;
                case 18: 이디안 = null; return;
                case 19: 음식 = null; return;
                case 20: 캔디 = null; return;
            }

            int count세트효과목록 = 세트효과목록.Count;
            int count마석목록 = 마석목록.Count;
            int count주문서목록 = 주문서목록.Count;
            int count스킬목록 = 스킬목록.Count;
            int start세트효과목록 = Information.RowIndex세트효과;
            int startIndex마석목록 = 0;
            int startIndex주문서목록 = 0;
            int startIndex스킬목록 = 0;

            if (count세트효과목록 == 0 || count세트효과목록 == 1)
            {
                startIndex마석목록 = start세트효과목록 + 1; // 22
            }
            else
            {
                startIndex마석목록 = start세트효과목록 + 1 + (count세트효과목록 - 1);
            }

            if (count마석목록 == 0 || count마석목록 == 1)
            {
                startIndex주문서목록 = startIndex마석목록 + 1;
            }
            else
            {
                startIndex주문서목록 = startIndex마석목록 + 1 + (count마석목록 - 1);
            }


            if (count주문서목록 == 0 || count주문서목록 == 1)
            {
                startIndex스킬목록 = startIndex주문서목록 + 1;
            }
            else
            {
                startIndex스킬목록 = startIndex주문서목록 + 1 + (count주문서목록 - 1);
            }

            if (rowIndex >= start세트효과목록 && rowIndex < startIndex마석목록)
            {

            }
            else if (rowIndex >= startIndex마석목록 && rowIndex < startIndex주문서목록)
            {
                //마석
                int index = rowIndex - startIndex마석목록;
                if (마석목록.Count > 0)
                {
                    마석목록.RemoveAt(index);
                }
            }
            else if (rowIndex >= startIndex주문서목록 && rowIndex < startIndex스킬목록)
            {
                //주문서
                int index = rowIndex - startIndex주문서목록;
                if (주문서목록.Count > 0)
                {
                    주문서목록.RemoveAt(index);
                }
            }
            else if (rowIndex >= startIndex스킬목록)
            {
                //주문서
                int index = rowIndex - startIndex스킬목록;
                if (스킬목록.Count > 0)
                {
                    스킬목록.RemoveAt(index);
                }
            }
        }

        public void Clear()
        {
            클래스 = null;
            오른손 = 왼손 = null;
            상의 = 어깨 = 장갑 = 하의 = 신발 = null;
            머리 = 목걸이 = 귀고리1 = 귀고리2 = 반지1 = 반지2 = 허리띠 = 날개 = null;
            타이틀 = null;
            이디안 = 음식 = 캔디 = null;
            세트효과목록.Clear();
            마석목록.Clear();
            주문서목록.Clear();
            스킬목록.Clear();
            FullFileName = null;
            OnlyFileName = null;
            Desc = null;
        }

        public void SetClass(string className)
        {
            if ( Information.InfoClassDefaultList.ContainsKey(className))
                this.클래스 = Information.InfoClassDefaultList[className];
        }
    }


    public class InfoAttr
    {
        public String HanAttrName;
        public String EngAttrName;
        public int ColumnIndex;
        public String TagName;
        public InfoAttr(string hanAttrName, string engAttrName, int columnIndex, string TagName)
        {
            this.HanAttrName = hanAttrName;
            this.EngAttrName = engAttrName;
            this.ColumnIndex = columnIndex;
            this.TagName = TagName;
        }
    }

    public class InfoClassDefault
    {
        public String ClassName;
        public int Power;
        public double DualWieldConstValue = 0.0;
        public double 생명력강화퍼센트 = 0.0;
        public Dictionary<string, string> AttrList;
        public bool IsMagical;

        public InfoClassDefault(string name, int power, bool isMagical)
        {
            this.ClassName = name;
            this.Power = power;
            this.IsMagical = isMagical;
            AttrList = new Dictionary<string, string>();
        }

        public double ValueDouble(string key)
        {
            string ret = "";
            if (AttrList.ContainsKey(key))
            {
                ret = (string)AttrList[key];
            }
            if (ret == null || ret == "") return 0;
            else
            {
                try
                {
                    return Double.Parse(ret);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }

    
    public class Information
    {
        public static String[] FirstType = { "무기", "방어구", "장신구", "마석", "이디안", "도핑", "타이틀", "세트효과", "랜덤옵션", "스킬", "기타"};

        public static List<string[]> SecondType = new List<string[]>(){
new string[]{ "장검", "단검", "전곤", "보주", "법서", "대검", "미늘창", "법봉", "활", "마력총", "마력포", "현악기", "기동쇠" }, 
new string[]{ "로브", "가죽", "사슬", "판금", "방패"}, 
new string[] {"귀고리", "목걸이", "반지", "허리띠", "머리장식", "날개"},
new string[] { "마석", "고대마석"},
new string[] {},
new string[] {"음식", "주문서", "캔디"},
new string[] {},
new string[] {},
new string[] {},
//new string[] {"None", "NoneActivation" , "Passive", "Toggle", "Active", "Charge", "Provoked", "Maintain" },
new string[] {"Toggle", "Active","진언"},
                                             };

        public static String[] ArmorSecondType = { "상의", "어깨", "장갑", "하의", "신발" };

        //// 검 수 살 궁 마 정 치 호 사 음
        public static String[] SkillSecondType = { "검성", "수호성", "살성", "궁성", "마도성", "정령성", "치유성", "호법성", "사격성", "음유성", "전사", "정찰자", "마법사", "사제" };

        

        public const int ColumnIndex이름 = 2;
        public const int ColumnIndex강화레벨 = 3;
        public const int ColumnIndex최대강화 = 4;
        public const int ColumnIndex마석슬롯 = 5;
        public const int ColumnIndex고대마석슬롯 = 6;

        public const int RowIndex세트효과 = 21;

        public static string[] ColumnHeaders = {
        "구분", "선택", "이름", "강화/마석", "최대강화", "마석슬롯", "고대마석슬롯",
                                         };

        public static string[,] RowHeaders = 
        {
        {"전체", "지우기"},
        {"클래스", "선택"},
        {"오른손", "지우기"}, 
        {"왼손", "지우기"},  

        {"상의", "지우기"}, 
        {"어깨", "지우기"}, 
        {"장갑", "지우기"}, 
        {"하의", "지우기"}, 
        {"신발", "지우기"}, 
        {"머리", "지우기"}, 
        {"목걸이", "지우기"}, 
        {"귀고리1", "지우기"},
        {"귀고리2", "지우기"}, 
        {"반지1", "지우기"}, 
        {"반지2", "지우기"}, 
        {"허리띠", "지우기"}, 
        {"날개", "지우기"}, 
        {"타이틀", "지우기"}, 
        {"이디안", "지우기"}, 
        {"음식", "지우기"}, 
        {"캔디", "지우기"}, 
        {"세트효과", ""}, 
        {"마석", "지우기"},
        {"주문서", "지우기"},
        {"스킬", "지우기"},
        };

        // 
       

        /*
            string[] 살성 = {"7068", "4671", "", "790", "128", "", "", "", "1.5", "6.0",
                              "", "899", "899", "999", "90", "",
                          "", "926", "50", "", "1.0",
                          "", "", "30", "", "", "", "", "", "", "", "","60"};
             
            //for (int i = 0; i < 살성.Length; i++)
            {
               // this.dataGridView[6 + i, 1].Value = 살성[i];
            }
         * */

        public static List<InfoAttr> InfoAttrList;
        public static Dictionary<string, InfoClassDefault> InfoClassDefaultList;
        public static Dictionary<string, int> InfoWeaponPassiveValueList;
        public static void Initialize()
        {
            InitInfoAttrList();
            InitDefaulList();
            InitInfoWeaponPassiveValueList();
        }


        private static void InitInfoAttrList()
        {
            InfoAttrList = new List<InfoAttr>();
            InfoAttrList.Add(new InfoAttr("생명력", "maxhp", 0, "max_hp"));
            InfoAttrList.Add(new InfoAttr("정신력", "maxmp", 1, null));

            InfoAttrList.Add(new InfoAttr("최소대미지", "min_damage", 2, "min_damage"));
            InfoAttrList.Add(new InfoAttr("최대대미지", "max_damage", 3, "max_damage"));
            InfoAttrList.Add(new InfoAttr("공격력", "phyattack", 4, null));
            InfoAttrList.Add(new InfoAttr("명중", "hitaccuracy", 5, "hit_accuracy"));
            InfoAttrList.Add(new InfoAttr("치명타", "critical", 6, "critical"));

            InfoAttrList.Add(new InfoAttr("최소대미지", "Lmin_damage", 7, null));
            InfoAttrList.Add(new InfoAttr("최대대미지", "Lmax_damage", 8, null));
            InfoAttrList.Add(new InfoAttr("공격력", "Lphyattack", 9, null));
            InfoAttrList.Add(new InfoAttr("명중", "Lhitaccuracy", 10, null));
            InfoAttrList.Add(new InfoAttr("치명타", "Lcritical", 11, null));

            InfoAttrList.Add(new InfoAttr("마법증폭력", "magicalskillboost", 12, "magical_skill_boost"));
            InfoAttrList.Add(new InfoAttr("마법적중", "magicalhitaccuracy", 13, "magical_hit_accuracy"));
            InfoAttrList.Add(new InfoAttr("마법치명타", "magicalcritical", 14, null));
            InfoAttrList.Add(new InfoAttr("마법저항", "magicalresist", 15, "magical_resist"));
            InfoAttrList.Add(new InfoAttr("마법상쇄", "magicalskillboostresist", 16, "magical_skill_boost_resist"));

            InfoAttrList.Add(new InfoAttr("PVP공격력", "pvpattackratio", 17, null));
            InfoAttrList.Add(new InfoAttr("PVP방어력", "pvpdefendratio", 18, null));

            InfoAttrList.Add(new InfoAttr("공격속도", "attackdelay", 19, "attackdelay"));
            InfoAttrList.Add(new InfoAttr("공격속도", "attack_delay", -1, "attack_delay"));
            InfoAttrList.Add(new InfoAttr("이동속도", "speed", 20, null));
            InfoAttrList.Add(new InfoAttr("시전속도", "boostcastingtime", 21, null));
            InfoAttrList.Add(new InfoAttr("비행속도", "flyspeed", -1, null));

            InfoAttrList.Add(new InfoAttr("물리방어", "physicaldefend", 22, "physical_defend"));
            InfoAttrList.Add(new InfoAttr("방패방어", "block", 23, "block"));
            InfoAttrList.Add(new InfoAttr("무기방어", "parry", 24, "parry"));
            InfoAttrList.Add(new InfoAttr("회피", "dodge", 25, "dodge"));
            InfoAttrList.Add(new InfoAttr("물리치명타저항", "physicalcriticalreducerate", 26, "physical_critical_reduce_rate"));
            InfoAttrList.Add(new InfoAttr("물리치명타방어", "physicalcriticaldamagereduce", 27, null));

            InfoAttrList.Add(new InfoAttr("마법방어", "magicaldefend", 28, "magical_defend"));
            InfoAttrList.Add(new InfoAttr("마법치명타저항", "magicalcriticalreducerate", 29, null));
            InfoAttrList.Add(new InfoAttr("마법치명타방어", "magicalcriticaldamagereduce", 30, null));

            InfoAttrList.Add(new InfoAttr("치유량증가", "healskillboost", 31, null));
            InfoAttrList.Add(new InfoAttr("대미지감소", "damagereduce", 32, "damage_reduce"));
            InfoAttrList.Add(new InfoAttr("집중", "concentration", 33, null));
            InfoAttrList.Add(new InfoAttr("최대비행시간", "maxfp", 34, null));

            InfoAttrList.Add(new InfoAttr("불속성방어", "ed_fire", -1, null));
            InfoAttrList.Add(new InfoAttr("바람속성방어", "ed_air", -1, null));
            InfoAttrList.Add(new InfoAttr("물속성방어", "ed_water", -1, null));
            InfoAttrList.Add(new InfoAttr("땅속성방어", "ed_earth", -1, null));

            InfoAttrList.Add(new InfoAttr("불속성방어", "edfire", -1, null));
            InfoAttrList.Add(new InfoAttr("불속성방어", "elementaldefendfire", 35, null));
            InfoAttrList.Add(new InfoAttr("바람속성방어", "elementaldefendair", 36, null));
            InfoAttrList.Add(new InfoAttr("물속성방어", "elementaldefendwater", 37, null));
            InfoAttrList.Add(new InfoAttr("땅속성방어", "elementaldefendearth", 38, null));




            InfoAttrList.Add(new InfoAttr("마비저항", "arparalyze", 39, null));
            InfoAttrList.Add(new InfoAttr("마비저항돌파", "paralyze_arp", 40, null));
            InfoAttrList.Add(new InfoAttr("침묵저항", "arsilence", 41, null));
            InfoAttrList.Add(new InfoAttr("침묵저항돌파", "silence_arp", 42, null));

            InfoAttrList.Add(new InfoAttr("생명력자연회복량", "hpregen", -1, null));
            InfoAttrList.Add(new InfoAttr("정신력자연회복량", "mpregen", -1, null));
            InfoAttrList.Add(new InfoAttr("적대치증폭력", "boosthate", 43, null));
            InfoAttrList.Add(new InfoAttr("넘어짐저항값", "arstumble", -1, null));
            InfoAttrList.Add(new InfoAttr("기절저항값", "arstun", -1, null));
            InfoAttrList.Add(new InfoAttr("밀려남저항값", "arstagger", -1, null));
            InfoAttrList.Add(new InfoAttr("마법공격력", "magicalattack", 44, null));
            InfoAttrList.Add(new InfoAttr("대미지감소최대값", "reducemax", -1, "reduce_max"));
            InfoAttrList.Add(new InfoAttr("PVP물리방어력", "pvpdefendratio_physical", -1, "pvpdefendratio_physical"));
            InfoAttrList.Add(new InfoAttr("PVP마법방어력", "pvpdefendratio_magical", -1, "pvpdefendratio_magical"));
            InfoAttrList.Add(new InfoAttr("모든속성방어", "elementaldefendall", -1, "elementaldefendall"));
            InfoAttrList.Add(new InfoAttr("모든속성방어력", "allresist", -1, "allresist"));

        }

        /* 2차 직업 별 스테이터스
         * 직업    힘  체력 정확 민첩 지식  의지
         * 검성    115 115 100  100   90   90      
         * 수호성  115 100 100  100   90   105
         * 궁성    100 100 115  115   90   90 
         * 살성    110 100 110  110   90   90 
         * 마도성   90  90 100  100  120   110
         * 정령성   90  90 100  100  115   115
         * 치유성  105 110  90   90  105   110
         * 호법성  110 105  90   90  105   110
         */
        private static void InitDefaulList()
        {
            InfoClassDefaultList = new Dictionary<string, InfoClassDefault>();
            InfoClassDefault id = null;

            // 검 수 살 궁 마 정 치 호 사 음
            id = new InfoClassDefault("검성", 115, false);
            id.AttrList.Add("maxhp", "8705");  // 8705 * 9퍼 = 9488  
            id.AttrList.Add("maxmp", "4671");
            id.AttrList.Add("phyattack", "35");
            id.AttrList.Add("hitaccuracy", "770");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "968");
            id.AttrList.Add("parry", "948");
            id.AttrList.Add("dodge", "868");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalresist", "0");
            id.AttrList.Add("maxfp", "60");
            id.DualWieldConstValue = 0.8747;
            id.생명력강화퍼센트 = 9; // 생명력강화 3단계 9퍼
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("수호성", 115, false);
            id.AttrList.Add("maxhp", "9031");  // 8705 * 9퍼 = 9488  
            id.AttrList.Add("maxmp", "4731");
            id.AttrList.Add("phyattack", "35");
            id.AttrList.Add("hitaccuracy", "770");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "1018");
            id.AttrList.Add("parry", "948");
            id.AttrList.Add("dodge", "868");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalresist", "0");
            id.AttrList.Add("elementaldefendfire", "30");
            id.AttrList.Add("elementaldefendair", "30");
            id.AttrList.Add("elementaldefendwater", "30");
            id.AttrList.Add("elementaldefendearth", "30");
            id.AttrList.Add("maxfp", "60");
            id.생명력강화퍼센트 = 15; // 생명력강화 5단계 15퍼
            InfoClassDefaultList.Add(id.ClassName, id);


            id = new InfoClassDefault("살성", 110, false);
            id.AttrList.Add("maxhp", "7068"); //살성은 녹색 생명력이 없다.
            id.AttrList.Add("maxmp", "4671");
            id.AttrList.Add("hitaccuracy", "790");
            id.AttrList.Add("critical", "128");
            id.AttrList.Add("block", "899");
            id.AttrList.Add("parry", "899");
            id.AttrList.Add("dodge", "999");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalresist", "30");
            id.AttrList.Add("maxfp", "60");
            id.DualWieldConstValue = 0.9791;
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("궁성", 100, false);
            id.AttrList.Add("maxhp", "5497"); 
            id.AttrList.Add("maxmp", "4671");
            id.AttrList.Add("hitaccuracy", "840");
            id.AttrList.Add("critical", "3");
            id.AttrList.Add("block", "914");
            id.AttrList.Add("parry", "994");
            id.AttrList.Add("dodge", "974");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("maxfp", "60");
            id.DualWieldConstValue = 0.8747;
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("마도성", 120, true); // 정령은 지식
            id.AttrList.Add("maxhp", "5078");
            id.AttrList.Add("maxmp", "7624");
            id.AttrList.Add("hitaccuracy", "710");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "868");
            id.AttrList.Add("parry", "868");
            id.AttrList.Add("dodge", "868");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalskillboost", "180"); //마증
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalskillboostresist", "180");
            id.AttrList.Add("magicalcriticalreducerate", "90");
            id.AttrList.Add("elementaldefendfire", "100");
            id.AttrList.Add("elementaldefendair", "100");
            id.AttrList.Add("elementaldefendwater", "100");
            id.AttrList.Add("elementaldefendearth", "100");
            id.AttrList.Add("maxfp", "60");
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("정령성", 115, true); // 정령은 지식
            id.AttrList.Add("maxhp", "5469");
            id.AttrList.Add("maxmp", "7156");
            id.AttrList.Add("hitaccuracy", "710");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "868");
            id.AttrList.Add("parry", "868");
            id.AttrList.Add("dodge", "868");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalskillboostresist", "180");
            id.AttrList.Add("magicalcriticalreducerate", "90");
            id.AttrList.Add("elementaldefendfire", "100");
            id.AttrList.Add("elementaldefendair", "100");
            id.AttrList.Add("elementaldefendwater", "100");
            id.AttrList.Add("elementaldefendearth", "100");
            id.AttrList.Add("maxfp", "60");
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("호법성", 110, false); // 호법 힘
            id.AttrList.Add("maxhp", "7086");
            id.AttrList.Add("maxmp", "7126");
            id.AttrList.Add("phyattack", "14");
            id.AttrList.Add("hitaccuracy", "690");
            id.AttrList.Add("critical", "1");
            id.AttrList.Add("block", "837");
            id.AttrList.Add("parry", "837");
            id.AttrList.Add("dodge", "837");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalskillboostresist", "100"); //마상
            id.AttrList.Add("elementaldefendfire", "30");
            id.AttrList.Add("elementaldefendair", "30");
            id.AttrList.Add("elementaldefendwater", "30");
            id.AttrList.Add("elementaldefendearth", "30");
            id.AttrList.Add("maxfp", "60");
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("치유성", 105, false); // 치유 힘
            id.AttrList.Add("maxhp", "6315");
            id.AttrList.Add("maxmp", "7126");
            id.AttrList.Add("hitaccuracy", "690");
            id.AttrList.Add("critical", "1");
            id.AttrList.Add("block", "837");
            id.AttrList.Add("parry", "837");
            id.AttrList.Add("dodge", "837");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalskillboostresist", "140"); //마상
            id.AttrList.Add("magicalcriticalreducerate", "90"); //마치저항
            id.AttrList.Add("maxfp", "60");
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("사격성", 100, true); // 사격성 힘, 지식 둘다 100
            id.AttrList.Add("maxhp", "7086");
            id.AttrList.Add("maxmp", "4711");
            id.AttrList.Add("hitaccuracy", "710");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "883");
            id.AttrList.Add("parry", "883");
            id.AttrList.Add("dodge", "943");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("maxfp", "60");
            id.DualWieldConstValue = 0.82; //0.8178
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("음유성", 110, true); // 음유 지식.. 
            id.AttrList.Add("maxhp", "6283");
            id.AttrList.Add("maxmp", "6608"); //최대정신력증가로 흰색*7 이나 생명과는 다르게 흰색정신력옵션이 맨몸 이외에는 없다.. 따라서 그냥 전체 정신력을 넣자
            id.AttrList.Add("hitaccuracy", "710");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "868");
            id.AttrList.Add("parry", "868");
            id.AttrList.Add("dodge", "868");
            id.AttrList.Add("physicalcriticalreducerate", "90");
            id.AttrList.Add("magicalskillboost", "180"); //마증
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("magicalcriticalreducerate", "90"); //마치저항
            id.AttrList.Add("elementaldefendfire", "100");
            id.AttrList.Add("elementaldefendair", "100");
            id.AttrList.Add("elementaldefendwater", "100");
            id.AttrList.Add("elementaldefendearth", "100");
            id.AttrList.Add("maxfp", "60");
            InfoClassDefaultList.Add(id.ClassName, id);

            id = new InfoClassDefault("기갑성", 100, true); // 사격성 힘, 지식 둘다 100
            id.AttrList.Add("maxhp", "8246");
            id.AttrList.Add("maxmp", "5677");
            id.AttrList.Add("phyattack", "18");
            id.AttrList.Add("hitaccuracy", "710");
            id.AttrList.Add("critical", "2");
            id.AttrList.Add("block", "868");
            id.AttrList.Add("parry", "868");
            id.AttrList.Add("dodge", "888");
            id.AttrList.Add("magicalhitaccuracy", "926");
            id.AttrList.Add("magicalcritical", "50");
            id.AttrList.Add("maxfp", "60");
            id.DualWieldConstValue = 0.82; //0.8178
            InfoClassDefaultList.Add(id.ClassName, id);
        }

        public static InfoClassDefault NextClassDefultList(string name)
        {
            IDictionaryEnumerator e = InfoClassDefaultList.GetEnumerator();
            while (e.MoveNext())
            {
                if ((string)e.Key == name)
                {
                    if (e.MoveNext()) return (InfoClassDefault)e.Value;
                    else return InfoClassDefaultList["검성"];
                }
            }
            return InfoClassDefaultList["검성"];
        }

        private static void InitInfoWeaponPassiveValueList()
        {
            InfoWeaponPassiveValueList = new Dictionary<string, int>();
            InfoWeaponPassiveValueList.Add("장검", 38);
            InfoWeaponPassiveValueList.Add("단검", 45);
            InfoWeaponPassiveValueList.Add("전곤", 42);
            InfoWeaponPassiveValueList.Add("보주", 44);
            InfoWeaponPassiveValueList.Add("법서", 8);
            InfoWeaponPassiveValueList.Add("대검", 15);
            InfoWeaponPassiveValueList.Add("미늘창", 15);
            InfoWeaponPassiveValueList.Add("법봉", 25);
            InfoWeaponPassiveValueList.Add("활", 8);
            InfoWeaponPassiveValueList.Add("마력총", 38);
            InfoWeaponPassiveValueList.Add("마력포", 12);
            InfoWeaponPassiveValueList.Add("현악기", 32);
        }
       

        public static string GetHangulAttrName(string attrName)
        {
            foreach (InfoAttr i in InfoAttrList)
            {
                if (i.EngAttrName == attrName)
                {
                    return i.HanAttrName;
                }
            }
            return attrName;
        }

       

        /*
        public static String AttrInfo(String str)
        {
            string ret = null;
            switch (str)
            {
                case "maxhp": ret = "생명력"; break;
                case "physicaldefend": ret = "물리방어"; break;
                case "magicalskillboost": ret = "마법증폭력"; break;
                case "speed": ret = "이동속도"; break;
                case "hitaccuracy": ret = "명중"; break;
                case "critical": ret = "물리치명타"; break;
                case "maxfp": ret = "최대비행시간"; break;
                case "parry": ret = "무기방어"; break;
                case "maxmp": ret = "정신력"; break;
                case "dodge": ret = "회피"; break;
                case "block": ret = "방패방어"; break;
                case "phyattack": ret = "공격력"; break;
                case "magicalresist": ret = "마법저항"; break;
                case "magicalhitaccuracy": ret = "마법적중"; break;
                case "flyspeed": ret = "비행속도"; break;
                case "attackdelay": ret = "공격속도"; break;
                case "boostcastingtime": ret = "시전속도"; break;
                case "hpregen": ret = "생명력자연회복량"; break;
                case "mpregen": ret = "정신력자연회복량"; break;
                case "healskillboost": ret = "치유량증가"; break;
                case "boosthate": ret = "적대치증폭력"; break;
                case "pvpattackratio": ret = "PVP공격력"; break;
                case "pvpdefendratio": ret = "PVP방어력"; break;
                case "magicalcritical": ret = "마법치명타"; break;
                case "magicalskillboostresist": ret = "마법상쇄"; break;
                case "elementaldefendfire":
                case "edfire":
                case "ed_fire": ret = "불속성방어"; break;
                case "ed_water": ret = "물속성방어"; break;
                case "ed_air": ret = "바람속성방어"; break;
                case "ed_earth": ret = "땅속성방어"; break;
                case "arstumble": ret = "넘어짐저항값"; break;
                case "arstun": ret = "기절저항값"; break;
                case "arstagger": ret = "밀려남저항값"; break;


                case "magicalattack": ret = "마법공격력"; break;
                case "arroot": ret = "?"; break;
                case "arsleep": ret = "?"; break;
                case "aropenareial": ret = "?"; break;
                case "arpoison": ret = "?"; break;

                //아이템 랜덤옵션에서 추가

                case "concentration": ret = "집중"; break;
                case "arparalyze": ret = "마비저항"; break;
                case "arsilence": ret = "침묵저항"; break;
                case "paralyze_arp": ret = "마비저항돌파"; break;
                case "silence_arp": ret = "침묵저항돌파"; break;

                //이디안 랜덤옵션에서 추가
                case "physicalcriticalreducerate": ret = "물리치명타저항"; break;
                case "magicalcriticalreducerate": ret = "마법치명타저항"; break;

                // 아이템에서 추가
                case "min_damage": ret = "최소공격력"; break;
                case "max_damage": ret = "최대공격력"; break;

            }

            if (ret == null)
            {
                ret = str;
            }
            return ret;
        }
        */

        public static int GetColumnIndex(string attrName)
        {
            InfoAttr attr = null;
            foreach (InfoAttr a in InfoAttrList)
            {
                if (attrName == a.EngAttrName || attrName == a.HanAttrName)
                {
                    attr = a;
                    break;
                }
            }

            if (attr == null || attr.ColumnIndex == -1)
            {
                return -1;
            }
            else
            {
                return attr.ColumnIndex + ColumnHeaders.Length;
            }
        }

        public static int GetRowIndex(string name)
        {
            for (int i = 0; i < RowHeaders.Length / 2; i++)
            {
                if (RowHeaders[i, 0] == name) return i;
            }
            return -1;
        }

        // 아이템 무기에 따라 패시브 수치를 전달한다.
        public static int GetPassiveValue(EntityItem item)
        {
            if (item.IsWeapon())
            {
                if (item.Is장검()) return InfoWeaponPassiveValueList["장검"];
                else if (item.Is단검()) return InfoWeaponPassiveValueList["단검"];
                else if (item.Is전곤()) return InfoWeaponPassiveValueList["전곤"];
                else if (item.Is보주()) return InfoWeaponPassiveValueList["보주"];
                else if (item.Is법서()) return InfoWeaponPassiveValueList["법서"];
                else if (item.Is대검()) return InfoWeaponPassiveValueList["대검"];
                else if (item.Is미늘창()) return InfoWeaponPassiveValueList["미늘창"];
                else if (item.Is법봉()) return InfoWeaponPassiveValueList["법봉"];
                else if (item.Is활()) return InfoWeaponPassiveValueList["활"];
                else if (item.Is마력총()) return InfoWeaponPassiveValueList["마력총"];
                else if (item.Is마력포()) return InfoWeaponPassiveValueList["마력포"];
                else if (item.Is현악기()) return InfoWeaponPassiveValueList["현악기"];
            }
            return 0;
        }

        public static int[] GetItemType(EntityItem item)
        {
            int[] ret = null;
            String id = item.NodeData["id"];
            string quality = null;
            if (item.NodeData.ContainsKey("quality"))
            {
                quality = (string)item.NodeData["quality"];
            }
            string name = (string)item.NodeData["name"];
            int level = -1;
            if (item.NodeData.ContainsKey("level"))
            {
                string tmp = (string)item.NodeData["level"];
                try
                {
                    level = Int32.Parse(tmp);
                }
                catch { }
            }

            if (id.StartsWith("10"))
            {
                if (id.StartsWith("1000")) // sword 장검
                {
                    item.Type = 1000; ret = new int[] { 0, 0, -1 };
                }
                else if (id.StartsWith("1002")) // dagger 단검
                {
                    item.Type = 1002; ret = new int[] { 0, 1, -1 };
                }
                else if (id.StartsWith("1001")) // mace 전곤
                {
                    item.Type = 1001; ret = new int[] { 0, 2, -1 };
                }
                else if (id.StartsWith("1005")) // orb 보주
                {
                    item.Type = 1005; ret = new int[] { 0, 3, -1 };
                }
                else if (id.StartsWith("1006")) // book 법서
                {
                    item.Type = 1006; ret = new int[] { 0, 4, -1 };
                }
                else if (id.StartsWith("1009")) // 2hsword 대검
                {
                    item.Type = 1009; ret = new int[] { 0, 5, -1 };
                }
                else if (id.StartsWith("1013")) // polearm 미늘창
                {
                    item.Type = 1013; ret = new int[] { 0, 6, -1 };
                }
                else if (id.StartsWith("1015")) // staff 지팡이
                {
                    item.Type = 1015; ret = new int[] { 0, 7, -1 };
                }
                else if (id.StartsWith("1017")) // bow 활
                {
                    item.Type = 1017; ret = new int[] { 0, 8, -1 };
                }
                else if (id.StartsWith("1018")) // gun 총
                {
                    item.Type = 1018; ret = new int[] { 0, 9, -1 };
                }
                else if (id.StartsWith("1019")) // cannon 마력포
                {
                    item.Type = 1019; ret = new int[] { 0, 10, -1 };
                }
                else if (id.StartsWith("1020")) // harp 현악기
                {
                    item.Type = 1020; ret = new int[] { 0, 11, -1 };
                }
                else if (id.StartsWith("1021"))
                {
                    item.Type = 1020; ret = new int[] { 0, 12, -1 };
                }
                else
                {

                }
            }
            else if (id.StartsWith("11"))// 방어구
            {
                if (id.StartsWith("1101")) // 로브 상의
                {
                    item.Type = 1101; ret = new int[] { 1, 0, 0 };
                }
                else if (id.StartsWith("1121")) // 로브 어깨
                {
                    item.Type = 1121; ret = new int[] { 1, 0, 1 };
                }

                else if (id.StartsWith("1111")) // 로브 장갑
                {
                    item.Type = 1111; ret = new int[] { 1, 0, 2 };
                }
                else if (id.StartsWith("1131")) // 로브 하의
                {
                    item.Type = 1131; ret = new int[] { 1, 0, 3 };
                }
                else if (id.StartsWith("1141")) // 로브 신발
                {
                    item.Type = 1141; ret = new int[] { 1, 0, 4 };
                }
                else if (id.StartsWith("1103")) // 가죽 상의
                {
                    item.Type = 1103; ret = new int[] { 1, 1, 0 };
                }
                else if (id.StartsWith("1123")) // 가죽 어깨
                {
                    item.Type = 1123; ret = new int[] { 1, 1, 1 };
                }
                else if (id.StartsWith("1113")) // 가죽 장갑
                {
                    item.Type = 1113; ret = new int[] { 1, 1, 2 };
                }
                else if (id.StartsWith("1133")) // 가죽 하의
                {
                    item.Type = 1133; ret = new int[] { 1, 1, 3 };
                }
                else if (id.StartsWith("1143")) // 가죽 신발
                {
                    item.Type = 1143; ret = new int[] { 1, 1, 4 };
                }
                else if (id.StartsWith("1105")) // 사슬 상의
                {
                    item.Type = 1105; ret = new int[] { 1, 2, 0 };
                }
                else if (id.StartsWith("1125")) // 사슬 어깨
                {
                    item.Type = 1125; ret = new int[] { 1, 2, 1 };
                }
                else if (id.StartsWith("1115")) // 사슬 장갑
                {
                    item.Type = 1115; ret = new int[] { 1, 2, 2 };
                }
                else if (id.StartsWith("1135")) // 사슬 하의
                {
                    item.Type = 1135; ret = new int[] { 1, 2, 3 };
                }
                else if (id.StartsWith("1145")) // 사슬 신발
                {
                    item.Type = 1145; ret = new int[] { 1, 2, 4 };
                }
                else if (id.StartsWith("1106")) // 판금 상의
                {
                    item.Type = 1106; ret = new int[] { 1, 3, 0 };
                }
                else if (id.StartsWith("1126")) // 판금 어깨
                {
                    item.Type = 1126; ret = new int[] { 1, 3, 1 };
                }
                else if (id.StartsWith("1116")) // 판금 장갑
                {
                    item.Type = 1116; ret = new int[] { 1, 3, 2 };
                }
                else if (id.StartsWith("1136")) // 판금 하의
                {
                    item.Type = 1136; ret = new int[] { 1, 3, 3 };
                }
                else if (id.StartsWith("1146")) // 판금 신발
                {
                    item.Type = 1146; ret = new int[] { 1, 3, 4 };
                }
                else if (id.StartsWith("1150")) // 방패
                {
                    item.Type = 1150; ret = new int[] { 1, 4, -1 };
                }
            }
            else if (id.StartsWith("12"))
            {
                if (id.StartsWith("120")) // 귀고리
                {
                    item.Type = 1200; ret = new int[] { 2, 0, -1 };
                }
                else if (id.StartsWith("121")) // 목걸이
                {
                    item.Type = 1210; ret = new int[] { 2, 1, -1 };
                }
                else if (id.StartsWith("122")) // 반지
                {
                    item.Type = 1220; ret = new int[] { 2, 2, -1 };
                }
                else if (id.StartsWith("123")) // 허리띠
                {
                    item.Type = 1230; ret = new int[] { 2, 3, -1 };
                }
                else if (id.StartsWith("125")) // 머리
                {
                    item.Type = 1250; ret = new int[] { 2, 4, -1 };
                }
            }
            else if (id.StartsWith("1870")) // 날개
            {
                item.Type = 1260; ret = new int[] { 2, 5, -1 };
            }
            else if (id.StartsWith("167000")) // 마석 16701:이벤트 마석
            {
                item.Type = 1670; ret = new int[] { 3, 0, -1 };
            }
            else if (id.StartsWith("16702")) // 고대마석
            {
                item.Type = 1672; ret = new int[] { 3, 1, -1 };
            }
            else if (id.StartsWith("16605")) //이디안
            {
                item.Type = 1665; ret = new int[] { 4, -1, -1 };
            }
            else if (id.StartsWith("16000")) // 음식
            {
                item.Type = 1600; ret = new int[] { 5, 0, -1 };
            }
            else if (id.StartsWith("16001")) // 캔디
            {
                item.Type = 1601; ret = new int[] { 5, 2, -1 };
            }
            else if (id.StartsWith("164") && name.StartsWith("scroll_")) // 주문서
            {
                item.Type = 1640; ret = new int[] { 5, 1, -1 };
            }
            else if (id.StartsWith("16961"))
            {
            }
            else if (id.StartsWith("102") || id.StartsWith("103") || id.StartsWith("1400") || id.StartsWith("15") || id.StartsWith("165") || id.StartsWith("1660") || id.StartsWith("1661") || id.StartsWith("1662") || id.StartsWith("16701") || id.StartsWith("16704") || id.StartsWith("168") || id.StartsWith("169") || id.StartsWith("17") || id.StartsWith("18") || id.StartsWith("19"))
            {
                // 102 : 키블레이드
                // 103 : 호기/곡괭이
                // 1400 : 스티그마
                // 15 : 제작
                // 16500 : 어비스템 제련
                // 16501 : 합성도구 한개
                // 1660 : 강화석
                // 1661 : 강화보조제
                // 1662 : 장비용 재감별 테스트 아이템. 조율주문서
                // 16701 : 이벤트 복합 마석
                // 16704 : 멀티마석?
                // 168 : 신석 , 신성부여
                // 1690 : 봉혼석
                // 1691 : 도색제
                // 1692 : 염색제
                // 1695 : 스킬북
                // 17 : 가구
                // 182 : 퀘스트
                // 19 : 펫
            }
            else if (id.StartsWith("11"))
            {
                // 1109 : 외변 상의
                // 1100 1110 1120 1130 1140
            }
            else if (id.StartsWith("141") || id.StartsWith("16"))
            {
                // 14100001 샤드
                // 162 물약
            }
            else
            {
            }
            if (name.Contains("test")) return null;
            if (name.Contains("npc")) return null;
            if (item.ViewName != null && item.ViewName.Contains("NPC")) return null;

            int m = item.Type / 100;
            if (m == 10 || m == 11 || m == 12)
            {
                //if (quality == null || quality.CompareTo("common") == 0 || quality.CompareTo("rare") == 0 || quality.CompareTo("legend") == 0) return null;
                //if (level < LEVEL) return null;
                //if (quality == null || quality.CompareTo("common") == 0 || quality.CompareTo("rare") == 0 || quality.CompareTo("legend") == 0) return null;
                //|| quality.CompareTo("unique") == 0

                //영웅(epic/주황), 신화(mythic/보라)
                //if (level < 55) return null;
            }
            if (item.Is음식())
            {
                if (item.Skill == null) return null;
                if (item.Skill.NodeData.ContainsKey("conflict_id") == false) return null;
                if (item.Skill.NodeData["conflict_id"] != "22") return null;
            }
            if (item.Is캔디())
            {
                // effect1_type : ShapeChange
                if (item.Skill == null) return null;
                if (item.Skill.NodeData.ContainsKey("effect1_type") == false) return null;
                if (item.Skill.NodeData["effect1_type"].ToLower() != "shapechange") return null;
            }
            if (item.Is음식() || item.Is주문서() || item.Is캔디())
             {
                if (item.Skill == null) return null;
                // dp 스킬같이 stat 영향이 없다면 걍 널
                if (item.Skill.MainOption.Count == 0 && item.Skill.BonusOption.Count == 0) return null;
            }
            //if (item.Is이디안())
            {
//                if (item.RandomOption == null) return null;
            }

            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;


namespace AES
{
    public class EntityItem : Entity
    {
        private int LevelOrCount = 0;
        public int RandomOptionGroupID = 0;
        public EntitySkill Skill = null;
        public EntitySetItem SetItem = null;

        public Dictionary<string, string> NodeData = null;
        public EntityItemRandomOption RandomOption = null;
        public int[] TreeIndex = null;
        public int Type = -1;
        public Dictionary<string, string> MainOption = null;
        public Dictionary<string, string> BonusOption = null;
        public Dictionary<string, string> BonusAOption = null;
        public Dictionary<string, string> BonusBOption = null;
        public Dictionary<string, string> EnchantOption = null;

        public int MaxEnchantValue = 0;
        public int MaxEnchantBonus = 0;

        public EntityItem(XMLHandler handler, XmlNode node)
        {
            this.xml = node.InnerXml;
            NodeData = new Dictionary<string, string>();
            MainOption = new Dictionary<string, string>();
            BonusOption = new Dictionary<string, string>();
            BonusAOption = new Dictionary<string, string>();
            BonusBOption = new Dictionary<string, string>();
            EnchantOption = new Dictionary<string, string>();

            foreach (XmlNode child in node.ChildNodes)
            {
                this.NodeData.Add(child.Name, child.InnerText);
                if (child.Name.Contains("bonus_attr"))
                {
                    Dictionary<string, string> opt = BonusOption;
                    if (child.Name.Contains("bonus_attr_a"))
                    {
                        opt = BonusAOption;
                    }
                    else if (child.Name.Contains("bonus_attr_b"))
                    {
                        opt = BonusBOption;
                    }
                    string str = child.InnerText;
                    str = str.Replace("  ", " ");
                    string[] tmp = str.Trim().Split(' ');

                    if (opt.ContainsKey(tmp[0].Trim().ToLower()) == false && tmp.Length == 2 && tmp[0].Trim().Length > 4)
                    {
                        opt.Add(tmp[0].Trim().ToLower(), tmp[1].Trim());
                    }
                    else if (tmp.Length == 1)
                    {

                    }
                    else
                    {
                        //handler.main.AppentText(NodeData["id"] + " ");
                        // 오면 안된다
                        // 101301026 123001344 123001363 123001371
                    }
                }
            }
            if (this.NodeData.ContainsKey("id"))
            {
                this.ID = (int)handler.main.StringToDouble(NodeData["id"]);
            }
            if (this.NodeData.ContainsKey("desc"))
            {
                this.ViewName = (string)handler.StrHash[((string)this.NodeData["desc"]).ToUpper()];
            }
            if (this.NodeData.ContainsKey("name"))
            {
                this.Name = (string)NodeData["name"].ToLower();
            }

            // 주문서 음식 캔디 등등
            if (NodeData.ContainsKey("activation_skill"))
            {
                string skillName = NodeData["activation_skill"].ToLower();
                if (handler.SkillList.ContainsKey(skillName))
                {
                    Skill = handler.SkillList[skillName];
                    // 스킬레벨
                    if (NodeData.ContainsKey("activation_level"))
                    {
                        string tmp = NodeData["activation_level"];
                        int value = 0;
                        try
                        {
                            value = Int32.Parse(tmp);
                        }
                        catch { }

                        if (value >= 1)
                        {
                            IDictionaryEnumerator e = Skill.MainOption.GetEnumerator();
                            while (e.MoveNext())
                            {
                                string attrName = (string)e.Key;
                                string attrValue = (string)e.Value;
                                int multipleValue = 0;
                                try
                                {
                                    multipleValue = Int32.Parse(attrValue) * value;
                                    MainOption.Add(attrName, multipleValue + "");
                                }
                                catch { }
                            }
                        }
                        IDictionaryEnumerator e2 = Skill.BonusOption.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            string attrName = (string)e2.Key;
                            string attrValue = (string)e2.Value;
                            MainOption.Add(attrName, attrValue);
                        }

                        e2 = Skill.PercentOption.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            string attrName = (string)e2.Key;
                            string attrValue = (string)e2.Value;
                            this.BonusOption.Add(attrName, attrValue);
                        }
                    }
                }
            }
            TreeIndex = Information.GetItemType(this);

            // AttrList에 있는 것들.. 즉 태그 이름으로 있는것들은 그냥 다 처리?
            foreach (InfoAttr attr in Information.InfoAttrList)
            {
                if (attr.TagName == null) continue;
                if (NodeData.ContainsKey(attr.TagName))
                {
                    string tmp = NodeData[attr.TagName];
                    if (tmp == "0" || tmp == "") continue;
                    MainOption.Add(attr.EngAttrName, tmp);
                }
            }

            if (Is마석() || Is고대마석())
            {
                string key1 = null, value1 = null;
                string key2 = null, value2 = null;
                if ( NodeData.ContainsKey("stat_enchant_type1") ) key1 = NodeData["stat_enchant_type1"];
                if ( NodeData.ContainsKey("stat_enchant_value1") ) value1 = NodeData["stat_enchant_value1"];
                if ( NodeData.ContainsKey("stat_enchant_type2") ) key2 = NodeData["stat_enchant_type2"];
                if ( NodeData.ContainsKey("stat_enchant_value2") ) value2 = NodeData["stat_enchant_value2"];
                if ( key1 != null && value1 != null) MainOption.Add(key1.ToLower(), value1.ToLower());
                if ( key2 != null && value2 != null) MainOption.Add(key2.ToLower(), value2.ToLower());
            }

            if (Is마석() || Is고대마석()) SetLevelOrCount(1);
            else SetLevelOrCount(0);
        }

        private EntityItem() {}
        //강화효과
        public void SetLevelOrCount(int level)
        {
            this.LevelOrCount = level;
            this.EnchantOption = new Dictionary<string, string>();
            if (this.IsWeapon())
            {
                int damage = 0;
                int magicalskillboost = 0;
                if (Is단검() || Is장검() )
                {
                    damage = 2;
                }
                else if (Is마력총())
                {
                    damage = 2;
                    magicalskillboost = 20;
                }
                else if (Is전곤() || Is법봉() || Is법서())
                {
                    damage = 3;
                    magicalskillboost = 20;
                }
                else if (Is대검() || Is미늘창() || Is활())
                {
                    damage = 4;
                }
                else if (Is보주() || Is마력포() || Is현악기())
                {
                    damage = 4;
                    magicalskillboost = 20;
                }
                EnchantOption.Add("min_damage", (damage * LevelOrCount) + "");
                EnchantOption.Add("max_damage", (damage * LevelOrCount) + "");
                EnchantOption.Add("magicalskillboost", (magicalskillboost * LevelOrCount) + "");
            }
            else if (this.IsArmor())
            {
                int physicaldefend = 0, magicaldefend = 0, maxhp = 0, physicalcriticalreducerate = 0;
                if (Is판금() && Is상의())
                {
                    physicaldefend = 6;
                    magicaldefend = 3;
                    maxhp = 8;
                    physicalcriticalreducerate = 4;
                }
                else if (Is판금() && Is하의())
                {
                    physicaldefend = 5;
                    magicaldefend = 2;
                    maxhp = 6;
                    physicalcriticalreducerate = 3;
                }
                else if (Is판금())
                {
                    physicaldefend = 4;
                    magicaldefend = 2;
                    maxhp = 4;
                    physicalcriticalreducerate = 2;
                }
                else if (Is사슬() && Is상의())
                {
                    physicaldefend = 5;
                    magicaldefend = 3;
                    maxhp = 10;
                    physicalcriticalreducerate = 4;
                }
                else if (Is사슬() && Is하의())
                {
                    physicaldefend = 4;
                    magicaldefend = 2;
                    maxhp = 8;
                    physicalcriticalreducerate = 3;
                }
                else if (Is사슬())
                {
                    physicaldefend = 3;
                    magicaldefend = 2;
                    maxhp = 6;
                    physicalcriticalreducerate = 2;
                }
                else if (Is가죽() && Is상의())
                {
                    physicaldefend = 4;
                    magicaldefend = 3;
                    maxhp = 12;
                    physicalcriticalreducerate = 4;
                }
                else if (Is가죽() && Is하의())
                {
                    physicaldefend = 3;
                    magicaldefend = 2;
                    maxhp = 10;
                    physicalcriticalreducerate = 3;
                }
                else if (Is가죽())
                {
                    physicaldefend = 2;
                    magicaldefend = 2;
                    maxhp = 8;
                    physicalcriticalreducerate = 2;
                }
                else if (Is로브() && Is상의())
                {
                    physicaldefend = 3;
                    magicaldefend = 3;
                    maxhp = 14;
                    physicalcriticalreducerate = 4;
                }
                else if (Is로브() && Is하의())
                {
                    physicaldefend = 2;
                    magicaldefend = 2;
                    maxhp = 12;
                    physicalcriticalreducerate = 3;
                }
                else if (Is로브())
                {
                    physicaldefend = 1;
                    magicaldefend = 2;
                    maxhp = 10;
                    physicalcriticalreducerate = 2;
                }
                else
                {
                }
                EnchantOption.Add("physicaldefend", (physicaldefend * LevelOrCount) + "");
                EnchantOption.Add("magicaldefend", (magicaldefend * LevelOrCount) + "");
                EnchantOption.Add("maxhp", (maxhp * LevelOrCount) + "");
                EnchantOption.Add("physicalcriticalreducerate", (physicalcriticalreducerate * LevelOrCount) + "");
            }
            else if (Is방패())
            {
                if (LevelOrCount < 10)
                {
                    EnchantOption.Add("damagereduce", (LevelOrCount * 2) + "");
                    EnchantOption.Add("block", (0) + "");
                }
                else
                {
                    EnchantOption.Add("damagereduce", (10 * 2) + "");
                    EnchantOption.Add("block", ((LevelOrCount-10)*30) + "");
                }
            }
            else if (Is마석() || Is고대마석())
            {
                if (LevelOrCount == 1)
                {
                }
                else if (LevelOrCount > 1)
                {
                    IDictionaryEnumerator ie = MainOption.GetEnumerator();
                    while (ie.MoveNext())
                    {
                        string key = (string)ie.Key;
                        string value = (string)ie.Value;
                        int val = 0;
                        try
                        {
                            val = Int32.Parse(value);
                        }
                        catch { }
                        EnchantOption.Add(key, ((val * (LevelOrCount - 1)) + ""));
                    }
                }
            }
        }

        public int GetLevelOrCount()
        {
            return this.LevelOrCount;
        }

        public override String GetTreeName()
        {
            //return NodeData["id"] + " " + ViewName + " " + NodeData["name"];
            return ViewName;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ViewName);
            sb.AppendLine();

            if (Skill != null) sb.AppendLine(Skill.GetInfo());
            if (SetItem != null) sb.AppendLine(SetItem.GetInfo());

            if (MainOption.Count > 0)
            {
                sb.AppendLine("메인옵션");
                IDictionaryEnumerator ee = MainOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }
            if (BonusOption.Count > 0)
            {
                sb.AppendLine("추가옵션");
                IDictionaryEnumerator ee = BonusOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }
            if (BonusAOption.Count > 0)
            {
                sb.AppendLine("1단계옵션");
                IDictionaryEnumerator ee = BonusAOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }
            if (BonusBOption.Count > 0)
            {
                sb.AppendLine("2단계옵션");
                IDictionaryEnumerator ee = BonusBOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }


            //if (IsWeapon())
            {
                if (NodeData.ContainsKey("hit_count")) sb.AppendLine("1회 타수 : " + NodeData["hit_count"]);
                if (NodeData.ContainsKey("weapon_type")) sb.AppendLine("무기타입 : " + NodeData["weapon_type"]);
                if (NodeData.ContainsKey("item_type")) sb.AppendLine("아이템타입 : " + NodeData["item_type"]);
                if (NodeData.ContainsKey("equipment_slots")) sb.AppendLine("장비슬롯 : " + NodeData["equipment_slots"]);
                if (NodeData.ContainsKey("attack_type")) sb.AppendLine("공격타입 : " + NodeData["attack_type"]);
                if (NodeData.ContainsKey("attack_range")) sb.AppendLine("공격거리 : " + NodeData["attack_range"]);
                if (NodeData.ContainsKey("quality")) sb.AppendLine("등급 : " + NodeData["quality"]);
                if (NodeData.ContainsKey("level")) sb.AppendLine("레벨 : " + NodeData["level"]);
                if (NodeData.ContainsKey("can_exchange")) sb.AppendLine("거래가능 : " + NodeData["can_exchange"]);
                if (NodeData.ContainsKey("can_sell_to_npc")) sb.AppendLine("판매 : " + NodeData["can_sell_to_npc"]);
                if (NodeData.ContainsKey("can_deposit_to_character_warehouse")) sb.AppendLine("창고보관 : " + NodeData["can_deposit_to_character_warehouse"]);
                if (NodeData.ContainsKey("can_deposit_to_account_warehouse")) sb.AppendLine("계정창고보관 : " + NodeData["can_deposit_to_account_warehouse"]);
                if (NodeData.ContainsKey("can_deposit_to_guild_warehouse")) sb.AppendLine("레기온창고보관 : " + NodeData["can_deposit_to_guild_warehouse"]);
                if (NodeData.ContainsKey("remove_when_logout")) sb.AppendLine("로그아웃시 사라짐 : " + NodeData["remove_when_logout"]);
                if (NodeData.ContainsKey("option_slot_value")) sb.AppendLine("마석슬롯 : " + NodeData["option_slot_value"]);
                if (NodeData.ContainsKey("special_slot_value")) sb.AppendLine("고대마석슬롯 : " + NodeData["special_slot_value"]);
                if (NodeData.ContainsKey("option_slot_bonus")) sb.AppendLine("보너스마석슬롯 : " + NodeData["option_slot_bonus"]);
                if (NodeData.ContainsKey("max_enchant_value")) sb.AppendLine("최대강화레벨 : " + NodeData["max_enchant_value"]);
                if (NodeData.ContainsKey("max_enchant_bonus")) sb.AppendLine("강화보너스 : " + NodeData["max_enchant_bonus"]);
                //sb.AppendLine("합성가능 : " + NodeData["can_composite_weapon"]);
                if (NodeData.ContainsKey("can_polish")) sb.AppendLine("이디어부여가능 : " + NodeData["can_polish"]);
                if (NodeData.ContainsKey("race_permitted")) sb.AppendLine("종족 : " + NodeData["race_permitted"]);
                if (NodeData.ContainsKey("breakable")) sb.AppendLine("추출 : " + NodeData["breakable"]);
                if (NodeData.ContainsKey("soul_bind")) sb.AppendLine("영혼각인 : " + NodeData["soul_bind"]);

                if (NodeData.ContainsKey("reidentify_count")) sb.AppendLine("재조율 : " + NodeData["reidentify_count"]);
                if (NodeData.ContainsKey("random_option_set")) sb.AppendLine("랜덤옵션 : " + NodeData["random_option_set"]);

                if (NodeData.ContainsKey("abyss_point")) sb.AppendLine("어비스포인트 : " + NodeData["abyss_point"]);
                if (NodeData.ContainsKey("abyss_item")) sb.AppendLine("공훈훈장 : " + NodeData["abyss_item"]);
                if (NodeData.ContainsKey("abyss_item_count")) sb.AppendLine("공훈훈장개수: " + NodeData["abyss_item_count"]);
                if (NodeData.ContainsKey("extra_currency_item")) sb.AppendLine("주화: " + NodeData["extra_currency_item"]);
                if (NodeData.ContainsKey("extra_currency_item_count")) sb.AppendLine("주화개수: " + NodeData["extra_currency_item_count"]);

                if (NodeData.ContainsKey("disposable_trade_item")) sb.AppendLine("우편첨부허가: " + NodeData["disposable_trade_item"]);
                if (NodeData.ContainsKey("disposable_trade_item_count")) sb.AppendLine("우편첨부허가개수: " + NodeData["disposable_trade_item_count"]);

                if (NodeData.ContainsKey("cash_item")) sb.AppendLine("캐쉬아이템 : " + NodeData["cash_item"]);
                if (NodeData.ContainsKey("expire_time")) sb.AppendLine("기간제아이템시간: " + NodeData["expire_time"]);
                if (NodeData.ContainsKey("can_ap_extraction")) sb.AppendLine("어비스템추출: " + NodeData["can_ap_extraction"]);

                if (NodeData.ContainsKey("charge_way")) sb.AppendLine("충전: " + NodeData["charge_way"]);
                if (NodeData.ContainsKey("charge_level")) sb.AppendLine("충전단계: " + NodeData["charge_level"]);
                if (NodeData.ContainsKey("charge_price1")) sb.AppendLine("충전1단계: " + NodeData["charge_price1"]);
                if (NodeData.ContainsKey("charge_price2")) sb.AppendLine("충전2단계: " + NodeData["charge_price2"]);
                if (NodeData.ContainsKey("armor_type")) sb.AppendLine("방어구타입: " + NodeData["armor_type"]);
                sb.AppendLine();
            }
            sb.AppendLine();


#if FALSE
            IDictionaryEnumerator e = NodeData.GetEnumerator();
            while (e.MoveNext())
            {
                string k = (string)e.Key;
                if (k.StartsWith("bonus_attr")) continue;
                
                /*
                switch (k)
                {
                    //case "name":
                    case "desc":
                    case "min_damage":
                    case "max_damage":
                    case "magical_skill_boost":
                    case "attack_delay":
                    case "hit_accuracy":
                    case "critical":
                    case "parry":
                    case "magical_hit_accuracy":
                    case "magical_skill_boost_resist":

                    //방어구
                    case "physical_defend":
                        case "magical_defend":
                        case "magical_resist":
                        case "dodge":
                    case "max_hp":
                    case "physical_critical_reduce_rate":
                  //case "magical_skill_boost_resist":

                    //방패
                    case "block":
                    case "damage_reduce":
                    case "reduce_max":


                    // 의미를 파악했고 내가 보여준다
                    case "hit_count":
                    case "weapon_type":
                    case "item_type":
                    case "equipment_slots":
                    case "attack_type":
                    case "attack_range":
                    case "quality":
                    case "level":
                    case "can_exchange":
                    case "can_sell_to_npc":
                    case "can_deposit_to_character_warehouse":
                    case "can_deposit_to_account_warehouse":
                    case "can_deposit_to_guild_warehouse":
                    case "remove_when_logout":
                    case "option_slot_value":
                    case "special_slot_value":
                    case "option_slot_bonus":
                    case "max_enchant_value":
                    case "max_enchant_bonus":
                    //case "can_composite_weapon":
                    case "can_polish":
                    case "race_permitted":
                    case "breakable":
                    case "soul_bind":
                    case "reidentify_count":
                    case "random_option_set":
                    case "abyss_point":
                    case "abyss_item":
                    case "abyss_item_count":
                    case "extra_currency_item":
                    case "extra_currency_item_count":
                    case "disposable_trade_item":
                    case "disposable_trade_item_count":
                    case "cash_item":
                    case "expire_time":
                    case "can_ap_extraction":
                    case "charge_way":
                    case "charge_level":
                    case "charge_price1":
                    case "charge_price2":
                    case "armor_type":


                    // 의미는 알지만 보여줄필요는 없다
                    case "warrior":
                    case "warrior_max":
                    case "scout":
                    case "scout_max":
                    case "mage":
                    case "mage_max":
                    case "cleric":
                    case "cleric_max":
                    case "engineer":
                    case "engineer_max":
                    case "artist":
                    case "artist_max":
                    case "fighter":
                    case "fighter_max":
                    case "knight":
                    case "knight_max":
                    case "assassin":
                    case "assassin_max":
                    case "ranger":
                    case "ranger_max":
                    case "wizard":
                    case "wizard_max":
                    case "elementalist":
                    case "elementalist_max":
                    case "chanter":
                    case "chanter_max":
                    case "priest":
                    case "priest_max":
                    case "gunner":
                    case "gunner_max":
                    case "bard":
                    case "bard_max":
                    case "rider":
                    case "rider_max":


                    // 의미는 정확하지 않으나 중요하지 않는 항목
                    case "desc_long":
                    case "mesh":
                    case "mesh_change":
                    case "material":
                    case "dmg_decal":
                    case "combat_item_fx":
                    case "icon_name":
                    case "blade_fx":
                    case "trail_tex":
                    case "max_stack_count":
                    case "str":
                    case "agi":
                    case "kno":
                    case "attack_gap":
                    case "equip_bone":
                    case "combat_equip_bone":
                    case "item_fx":
                    case "ui_sound_type":
                    case "lore": // 항상 false
                    case "gender_permitted": // 항상 all
                    case "bonus_apply": // 항상 equip
                    case "can_split": // 항상 false
                    case "item_drop_permitted": // 항상 false


                        continue;

                }
                */
                // 중요하지는 않아 보이나 그냥 처리하지 못한 항목이다
                sb.AppendLine(e.Key + " : " + e.Value);

            }

            // 디버깅용
            /*
            if (this.NodeData.ContainsKey("lore"))
            {
                if (((string)this.NodeData["lore"]).ToLower() != "false")
                {
                }
            }
            if (this.NodeData.ContainsKey("gender_permitted"))
            {
                if (((string)this.NodeData["gender_permitted"]).ToLower() != "all")
                {
                }
            }
            if (this.NodeData.ContainsKey("bonus_apply"))
            {
                if (((string)this.NodeData["bonus_apply"]).ToLower() != "equip")
                {
                }
            }
            if (this.NodeData.ContainsKey("can_split"))
            {
                if (((string)this.NodeData["can_split"]).ToLower() != "false")
                {
                }
            }
            if (this.NodeData.ContainsKey("item_drop_permitted"))
            {
                if (((string)this.NodeData["item_drop_permitted"]).ToLower() != "false")
                {
                }
            }
             */
            /*
            if (this.NodeData.ContainsKey("no_enchant"))
            {
                if (((string)this.NodeData["no_enchant"]).ToLower() != "false")
                {
                }
            }
            if (this.NodeData.ContainsKey("can_proc_enchant"))
            {
                if (((string)this.NodeData["can_proc_enchant"]).ToLower() != "true")
                {
                }
            }
            if (this.NodeData.ContainsKey("can_composite_weapon"))
            {
                if (((string)this.NodeData["can_composite_weapon"]).ToLower() != "true")
                {
                }
            }
            */
            sb.AppendLine();
            //sb.AppendLine(xml);
#endif            
            return sb.ToString();
        }

        #region 아이템 타입 등등
        public bool IsWeapon()
        {
            return ((Type / 100) == 10);
        }

        public bool IsArmor()
        {
            return ((Type / 100) == 11 && Type != 1150);
        }

        public bool IsAccessory()
        {
            return ((Type / 100) == 12);
        }

        public bool Is2Hand()
        {
            return (Is보주() || Is법서() || Is대검() || Is미늘창() || Is법봉() || Is활() || Is마력포() || Is현악기());
        }
        public bool Is1Hand()
        {
            return (Is장검() || Is단검() || Is전곤() || Is마력총());
        }


        public bool Is장검()
        {
            return (Type == 1000);
        }
        public bool Is단검()
        {
            return (Type == 1002);
        }
        public bool Is전곤()
        {
            return (Type == 1001);
        }
        public bool Is보주()
        {
            return (Type == 1005);
        }
        public bool Is법서()
        {
            return (Type == 1006);
        }
        public bool Is대검()
        {
            return (Type == 1009);
        }
        public bool Is미늘창()
        {
            return (Type == 1013);
        }
        public bool Is법봉()
        {
            return (Type == 1015);
        }
        public bool Is활()
        {
            return (Type == 1017);
        }
        public bool Is마력총()
        {
            return (Type == 1018);
        }
        public bool Is마력포()
        {
            return (Type == 1019);
        }
        public bool Is현악기()
        {
            return (Type == 1020);
        }
        public bool Is방패()
        {
            return (Type == 1150);
        }

        public bool Is상의()
        {
            return ((Type == 1101) || (Type == 1103) || (Type == 1105) || (Type == 1106));
        }
        public bool Is어깨()
        {
            return ((Type == 1121) || (Type == 1123) || (Type == 1125) || (Type == 1126));
        }
        public bool Is장갑()
        {
            return ((Type == 1111) || (Type == 1113) || (Type == 1115) || (Type == 1116));
        }
        public bool Is하의()
        {
            return ((Type == 1131) || (Type == 1133) || (Type == 1135) || (Type == 1136));
        }
        public bool Is신발()
        {
            return ((Type == 1141) || (Type == 1143) || (Type == 1145) || (Type == 1146));
        }
        public bool Is머리()
        {
            return (Type == 1250);
        }
        public bool Is목걸이()
        {
            return (Type == 1210);
        }
        public bool Is귀고리()
        {
            return (Type == 1200);
        }
        public bool Is반지()
        {
            return (Type == 1220);
        }
        public bool Is허리띠()
        {
            return (Type == 1230);
        }
        public bool Is날개()
        {
            return (Type == 1260);
        }
        public bool Is이디안()
        {
            return (Type == 1665);
        }
        public bool Is마석()
        {
            return (Type == 1670);
        }
        public bool Is고대마석()
        {
            return (Type == 1672);
        }
        public bool Is음식()
        {
            return (Type == 1600);
        }
        public bool Is캔디()
        {
            return (Type == 1601);
        }
        public bool Is주문서()
        {
            return (Type == 1640);
        }
        public bool Is로브()
        {
            return (IsArmor() && (Type % 10) == 1);
        }
        public bool Is가죽()
        {
            return (IsArmor() && (Type % 10) == 3);
        }
        public bool Is사슬()
        {
            return (IsArmor() && (Type % 10) == 5);
        }
        public bool Is판금()
        {
            return (IsArmor() && (Type % 10) == 6);
        }
        #endregion




        public double ValueDoubleInDictionary(string key)
        {
            return ValueDoubleInDictionary(key, NodeData);
        }

        public double ValueDoubleInDictionary(string key, Dictionary<string, string> option)
        {
            string ret = ValueStringInDictionary(key, option);
            if (ret == null || ret == "") return 0;
            else
            {
                try
                {
                    return Double.Parse(ret);
                }
                catch
                {
                    if (ret.EndsWith("%")) ret = ret.Substring(0, ret.Length - 1);
                    try { return Double.Parse(ret); }
                    catch { return 0; }
                }
            }
        }


        public string ValueStringInDictionary(string key)
        {
            return ValueStringInDictionary(key, NodeData);
        }

        private string ValueStringInDictionary(string key, Dictionary<string, string> option)
        {
            string ret = "";
            if (option.ContainsKey(key))
            {
                ret = (string)option[key];
            }
            return ret;
        }

        public double ValueMaxEnchantValue
        {
            get
            {
                double max = ValueDoubleInDictionary("max_enchant_value");
                double bonus = ValueDoubleInDictionary("max_enchant_bonus");
                return max + bonus;
            }
        }

        public string ValueMaxEnchantString
        {
            get
            {
                double max = ValueDoubleInDictionary("max_enchant_value");
                double bonus = ValueDoubleInDictionary("max_enchant_bonus");
                return max + "+" + bonus;
            }
        }

        public string ValueOptionSlotValue
        {
            get { return ValueStringInDictionary("option_slot_value") + "+" + ValueStringInDictionary("option_slot_bonus"); }
        }

        public string ValueSpecialSlotValue
        {
            get { return ValueStringInDictionary("special_slot_value"); }
        }

        public EntityItem Clone()
        {
            EntityItem copy = new EntityItem();
            copy.xml = this.xml;
            copy.ID = this.ID;
            copy.Name = this.Name;
            copy.Desc = this.Desc;
            copy.ViewName = this.ViewName;

            copy.LevelOrCount = this.LevelOrCount;
            copy.RandomOptionGroupID = this.RandomOptionGroupID;
            copy.Skill = this.Skill;
            copy.SetItem = this.SetItem;
            copy.NodeData = this.NodeData;
            copy.RandomOption = this.RandomOption;
            copy.TreeIndex = this.TreeIndex;
            copy.Type = this.Type;
            copy.MainOption = this.MainOption;
            copy.BonusOption = this.BonusOption;
            copy.BonusAOption = this.BonusAOption;
            copy.BonusBOption = this.BonusBOption;
            copy.EnchantOption = this.EnchantOption;
            copy.MaxEnchantValue = this.MaxEnchantValue;
            copy.MaxEnchantBonus = this.MaxEnchantBonus;
            return copy;
        }
    }
}

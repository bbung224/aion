using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace AES
{

    



    public abstract class Entity
    {
        public string xml;
        public int ID;
        public string Name;
        public string Desc;
        public string ViewName;
        public abstract String GetTreeName(); // 트리뷰에서 보일 이름
        public abstract String GetInfo(); // 아이템 정보창에서 보일 내용

        
    }


    public class EntityTitle : Entity
    {
        public string ViewTitleDesc;
        public Dictionary<string, string> MainOption;

        public EntityTitle(XMLHandler handler, XmlNode node)
        {
            MainOption = new Dictionary<string, string>();
            this.xml = node.InnerXml;
            this.ID = Int32.Parse(node["id"].InnerText);
            this.Name = (string)node["name"].InnerText;
            this.Desc = (string)node["desc"].InnerText.ToUpper();
            this.ViewName = (string)handler.StrHash[this.Desc];
            this.ViewTitleDesc = (string)handler.StrHash[node["title_desc"].InnerText];
            XmlNode bonusArrtNode = node.SelectSingleNode("bonus_attrs");
            if (bonusArrtNode != null)
            {
                XmlNodeList attrList = bonusArrtNode.SelectNodes("data");
                foreach (XmlNode attrNode in attrList)
                {
                    string[] tmp = attrNode.InnerText.Split(' ');
                    if (this.MainOption.ContainsKey(tmp[0].ToLower()) == false)
                        this.MainOption.Add(tmp[0].ToLower(), tmp[1].ToLower());
                }
            }
        }

        public override String GetTreeName()
        {
            return ViewName;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            IDictionaryEnumerator e = MainOption.GetEnumerator();
            //sb.AppendLine("ID : " + ID);
            sb.AppendLine("타이틀 - " + ViewName);
            sb.AppendLine(ViewTitleDesc);
            sb.AppendLine();
            while (e.MoveNext())
            {
                sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
            }
            //sb.AppendLine();
            //sb.AppendLine(this.xml);
            return sb.ToString();
        }

       
    }


    public class EntitySetItemOption : Entity
    {
        public EntitySetItem Parent;
        public int Count = 0;
        public Dictionary<string, string> Option;
        public EntitySetItemOption(EntitySetItem parent, int count)
        {
            this.Parent = parent;
            this.Count = count;
            Option = new Dictionary<string, string>();
            if (count == 0)
            {
                this.ViewName = Parent.ViewName + " 풀세트";
            }
            else
            {
                this.ViewName = Parent.ViewName + " " + count + "세트효과";
            }
        }
        public override string GetInfo() { return null; }
        public override string GetTreeName() { return Parent.ViewName; }
    }

    public class EntitySetItem : Entity
    {
        public Dictionary<string, string> NodeData;
        public List<EntityItem> ItemList;
        //public Dictionary<int, Dictionary<string, string>> BonusList;
        public List<EntitySetItemOption> BonusList;

        public EntitySetItem(XMLHandler handler, XmlNode node)
        {
            NodeData = new Dictionary<string, string>();
            ItemList = new List<EntityItem>();
            //BonusList = new Dictionary<int, Dictionary<string, string>>();
            BonusList = new List<EntitySetItemOption>();

            this.xml = node.InnerXml;
            this.ID = Int32.Parse(node["id"].InnerText);
            this.Name = (string)node["name"].InnerText.ToLower();
            this.Desc = (string)node["desc"].InnerText.ToUpper();
            this.ViewName = (string)handler.StrHash[this.Desc];
            foreach (XmlNode child in node.ChildNodes)
            {
                string tagName = child.Name;
                string value = child.InnerText.ToLower();
                this.NodeData.Add(tagName, value);
                if (tagName.Contains("item"))
                {
                    EntityItem i = null;
                    try
                    {
                        i = handler.ItemList[value];
                        ItemList.Add(i);
                        i.SetItem = this;
                    }
                    catch
                    { }
                }
                else if (tagName.Contains("bonus"))
                {
                    int key = -1;
                    Dictionary<string, string> attrList = new Dictionary<string, string>();
                    if (tagName.Contains("piece_bonus"))
                    {
                        try
                        {
                            key = Int32.Parse(tagName.Substring(11));
                        }
                        catch { }
                    }
                    else if (tagName == "fullset_bonus")
                    {
                        //key = ItemList.Count;
                        key = 0;
                    }
                    string[] attrs = value.Split(';');
                    foreach (string s in attrs)
                    {
                        string[] ss = s.Trim().Split(' ');
                        attrList.Add(ss[0], ss[1]);
                    }
                    //BonusList.Add(key, attrList);
                    EntitySetItemOption sio = new EntitySetItemOption(this, key);
                    sio.Option = attrList;
                    BonusList.Add(sio);
                }
            }
            // 2013-06-11


        }

        public override String GetTreeName()
        {
            return ViewName;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ViewName);
            sb.AppendLine();
            sb.AppendLine("아이템");
            foreach (EntityItem item in ItemList)
            {
                sb.AppendLine(item.ViewName);
            }
            sb.AppendLine();

            foreach (EntitySetItemOption sio in BonusList)
            {
                sb.AppendLine((((int)sio.Count == 0) ? "풀" : sio.Count + "") + "세트 효과");
                Dictionary<string, string> bonus = (Dictionary<string, string>)sio.Option;
                IDictionaryEnumerator e2 = bonus.GetEnumerator();
                while (e2.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)e2.Key) + " : " + e2.Value);
                }
                sb.AppendLine();
            }
            /*
            IDictionaryEnumerator e = NodeData.GetEnumerator();
            while (e.MoveNext())
            {
                sb.AppendLine(e.Key + " : " + e.Value);
            }
            */
            //sb.AppendLine();
            //sb.AppendLine(this.xml);
            return sb.ToString();
        }

    }


    public class EntityOption : Entity
    {
        public int GroupId = 0;
        public int Prob = 0;
        public Dictionary<string, string> AttrList = null;
        public Dictionary<string, string> AfterCalAttrList = null;
        public EntityItemRandomOption parent = null;

        public EntityOption(EntityItemRandomOption parent, XmlNode root)
        {
            this.parent = parent;
            this.xml = root.InnerXml;
            AttrList = new Dictionary<string, string>();
            AfterCalAttrList = new Dictionary<string, string>();
            GroupId = Int32.Parse(root["attr_group_id"].InnerText);
            Prob = Int32.Parse(root["prob"].InnerText);
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.StartsWith("random_attr"))
                {
                    string[] tmp = node.InnerText.Split(' ');
                    AttrList.Add(tmp[0].ToLower(), tmp[1]);
                }
            }

            // 계산후 옵션
            if (this.parent.Item != null)
            {
                IDictionaryEnumerator e1 = this.parent.Item.BonusOption.GetEnumerator();
                while (e1.MoveNext())
                {
                    AfterCalAttrList.Add((string)e1.Key, (string)e1.Value);
                }
                IDictionaryEnumerator e2 = this.AttrList.GetEnumerator();
                while (e2.MoveNext())
                {
                    if (AfterCalAttrList.ContainsKey((string)e2.Key))
                    {
                        string v1 = (string)AfterCalAttrList[(string)e2.Key];
                        string v2 = (string)e2.Value;
                        int idx1 = v1.IndexOf('%');
                        int idx2 = v2.IndexOf('%');
                        v1 = (idx1 == -1) ? v1 : v1.Substring(0, idx1);
                        v2 = (idx2 == -1) ? v2 : v2.Substring(0, idx2);
                        int i1 = 0;
                        int i2 = 0;
                        try { i1 = Int32.Parse(v1); }
                        catch { }
                        try { i2 = Int32.Parse(v2); }
                        catch { }
                        if (idx1 != -1 && idx2 != -1) AfterCalAttrList[(string)e2.Key] = (i1 + i2) + "%";
                        else AfterCalAttrList[(string)e2.Key] = (i1 + i2) + "";
                    }
                    else
                    {
                        AfterCalAttrList.Add((string)e2.Key, (string)e2.Value);
                    }
                }
            }
            else
            {
            }



        }

        public override String GetTreeName()
        {
            return "옵션 " + GroupId;
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.parent.Item.ViewName + " 옵션 " + GroupId);
            sb.AppendLine();
            
            if (this.parent.Item.BonusOption.Count > 0)
            {
                sb.AppendLine("아이템 추가옵션");
                IDictionaryEnumerator ee = this.parent.Item.BonusOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }
            

            sb.AppendLine("Prob : " + (Prob) + " (" + (Prob*100/this.parent.TotalProb) + "%)");
            IDictionaryEnumerator e = AttrList.GetEnumerator();
            while (e.MoveNext())
            {
                sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
            }
            sb.AppendLine();

            sb.AppendLine("옵션 적용 후");
            e = this.AfterCalAttrList.GetEnumerator();
            while (e.MoveNext())
            {
                sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
            }
            sb.AppendLine();

            return sb.ToString();
        }

    }

    public class EntityItemRandomOption : Entity
    {
        public EntityItem Item = null;
        public List<EntityOption> OptionList = null;
        public int TotalProb = 0;

        public EntityItemRandomOption(XMLHandler handler, XmlNode node, EntityItem item)
        {
            OptionList = new List<EntityOption>();
            this.xml = node.InnerXml;
            this.ID = Int32.Parse(node["id"].InnerText);
            this.Name = node["name"].InnerText;

            this.Item = item;
            ViewName = Item.ViewName;

            XmlNode groupNode = node.SelectSingleNode("random_attr_group_list");
            if (groupNode != null)
            {
                XmlNodeList optionList = groupNode.SelectNodes("data");
                foreach (XmlNode optionNode in optionList)
                {
                    EntityOption iro = new EntityOption(this, optionNode);
                    OptionList.Add(iro);
                    TotalProb += iro.Prob;
                }
            }
            if (this.Item != null)
            {
                this.Item.RandomOption = this;
            }
            else
            {

            }
        }

        public override String GetTreeName()
        {
            return ViewName;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ViewName);
            sb.AppendLine();
            if (this.Item.BonusOption.Count > 0)
            {
                sb.AppendLine("아이템 추가옵션");
                IDictionaryEnumerator ee = this.Item.BonusOption.GetEnumerator();
                while (ee.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)ee.Key) + " : " + ee.Value);
                }
                sb.AppendLine();
            }
            foreach (EntityOption iro in OptionList)
            {
                sb.AppendLine("ID: " + iro.GroupId);
                sb.AppendLine("Prob: " + iro.Prob + " (" + (iro.Prob * 100 / TotalProb) + "%)");

                IDictionaryEnumerator e = iro.AttrList.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
                }
                sb.AppendLine();
            }
            sb.AppendLine("Prob합계 : " + this.TotalProb);
            sb.AppendLine();
            //sb.AppendLine(this.xml);
            return sb.ToString();
        }

    }

    public enum SkillType
    {
        None, NoneActive, Passive, Toggle, Active, Charge, Provoked, Maintain
    }

    public class EntitySkill : Entity
    {
        public SkillType SType = SkillType.None;
        public string DescLong = null;
        public string DescLongString = null;
        public Dictionary<string, string> NodeData;
        public Dictionary<string, string> MainOption = null;
        public Dictionary<string, string> BonusOption = null;
        public Dictionary<string, string> PercentOption = null;

        public EntitySkill(XMLHandler handler, XmlNode node)
        {
            NodeData = new Dictionary<string, string>();
            MainOption = new Dictionary<string, string>();
            BonusOption = new Dictionary<string, string>();
            PercentOption = new Dictionary<string, string>();

            this.xml = node.InnerXml;
            this.ID = Int32.Parse(node["id"].InnerText);
            this.Name = node["name"].InnerText.ToLower();
            this.Desc = (string)node["desc"].InnerText.ToUpper();
            this.ViewName = (string)handler.StrHash[this.Desc];

            foreach (XmlNode child in node.ChildNodes)
            {
                string tagName = child.Name;
                string value = child.InnerText;
                NodeData.Add(tagName, value);
            }

            if (NodeData.ContainsKey("desc_long"))
            {
                DescLong = node["desc_long"].InnerText.ToUpper();
                if (handler.StrHash.ContainsKey(DescLong))
                {
                    DescLongString = (string)handler.StrHash[DescLong];
                }
            }

            if (NodeData.ContainsKey("activation_attribute"))
            {
                string tmp = node["activation_attribute"].InnerText.ToLower();
                switch (tmp)
                {
                    case "passive": this.SType = SkillType.Passive; break;
                    case "toggle": this.SType = SkillType.Toggle; break;
                    case "active": this.SType = SkillType.Active; break;
                    case "charge": this.SType = SkillType.Charge; break;
                    case "provoked": this.SType = SkillType.Provoked; break;
                    case "maintain": this.SType = SkillType.Maintain; break;
                    default: this.SType = SkillType.NoneActive; break;
                }
            }
            else
            {
                // 채집, 정기 추출, 오드 추출, 요리, 무기 제작, 
                SType = SkillType.None;
            }
            

            // 음식류?
            if (NodeData.ContainsKey("activation_attribute"))
            {
                string v = ((string)NodeData["activation_attribute"].ToLower());
                //if (v == "active")
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string tt = "effect" + i + "_type";
                        if (NodeData.ContainsKey(tt))
                        {
                            string t2 = NodeData[tt].ToLower();
                            //if (t2 == "statup" || t2 == "statdown" || t2 == "statboost")
                            if (t2.Contains("stat"))
                            {
                                //13 14를 메인에
                                // 13 14가 있으면 2,4
                                // 13만 있으면 1
                                // 음.. reserved1은 단계별 값이고
                                // reserved2는 단계와 상과없는 절대값이다
                                // reserved4는 퍼센트인것 같다 ;

                                string tmp1 = "effect" + i + "_reserved1";
                                string tmp2 = "effect" + i + "_reserved2";
                                string tmp3 = "effect" + i + "_reserved3";
                                string tmp4 = "effect" + i + "_reserved4";
                                string tmp13 = "effect" + i + "_reserved13";
                                string tmp14 = "effect" + i + "_reserved14";

                                string value1 = (NodeData.ContainsKey(tmp1)) ? NodeData[tmp1] : null;
                                string value2 = (NodeData.ContainsKey(tmp2)) ? NodeData[tmp2] : null;
                                string value3 = (NodeData.ContainsKey(tmp3)) ? NodeData[tmp3] : null;
                                string value4 = (NodeData.ContainsKey(tmp4)) ? NodeData[tmp4] : null;
                                string value13 = (NodeData.ContainsKey(tmp13)) ? NodeData[tmp13].ToLower() : null;
                                string value14 = (NodeData.ContainsKey(tmp14)) ? NodeData[tmp14].ToLower() : null;

                                if (t2 == "statdown")
                                {
                                    try
                                    {
                                        value1 = (value1 == null || value1 == "0") ? null : "-" + value1;
                                        value2 = (value2 == null || value2 == "0") ? null : "-" + value2;
                                        value3 = (value3 == null || value3 == "0") ? null : "-" + value3;
                                        value4 = (value4 == null || value4 == "0") ? null : "-" + value4;
                                    }
                                    catch //(Exception e)
                                    {

                                    }
                                }

                                if (SType == SkillType.Toggle && t2 == "weaponstatup")
                                {
                                    // 화살강화때문에
                                    value4 = value2; value2 = null;
                                }
                                if (SType == SkillType.Provoked && ViewName.Contains("불패의 진언") && value13 == "phyattack")
                                {
                                    value4 = value2; value2 = null;
                                }

                                if (SType == SkillType.Active && DescLongString != null && (DescLongString.Contains("e" + i + ".StatUp.Value]%%") || DescLongString.Contains("e" + i + ".StatDown.Value]%%") || DescLongString.Contains("e" + i + ".WeaponStatUp.Value]%%")))
                                {
                                    if (value4 != null && value14 != null)
                                    {
                                        // 1818181818181818
                                        PercentOption.Add(value13, value2);
                                        value13 = null;
                                    }
                                    else
                                    {
                                        value4 = value2; value2 = null;
                                    }
                                }

                               // if (SType == SkillType.Active && DescLongString != null && (DescLongString.Contains("e" + i + ".StatUp.Value2]%%") || DescLongString.Contains("e" + i + ".StatDown.Value2]%%") || DescLongString.Contains("e" + i + ".WeaponStatUp.Value2]%%")))
                                {

                                }

                                // 퍼센트옵션 ㅠ
                                if (value13 != null && value14 == null && value4 != null)
                                {
                                    if (value1 == null || value1 == "0")
                                    {
                                        if (value2 == null || value2 == "0")
                                        {
                                            if (value3 == null || value3 == "0")
                                            {
                                                PercentOption.Add(value13, value4);
                                            }
                                        }
                                    }
                                }
                                if (value13 != null)
                                {
                                    if (value1 != null && value1 != "0")
                                    {
                                        if (MainOption.ContainsKey(value13) == false)
                                        {
                                            MainOption.Add(value13, value1);
                                        }
                                    }
                                    if (value2 != null && value2 != "0")
                                    {
                                        if (BonusOption.ContainsKey(value13) == false)
                                        {
                                            BonusOption.Add(value13, value2);
                                        }
                                    }
                                }
                                if (value14 != null)
                                {
                                    if (value3 != null && value3 != "0")
                                    {
                                        if (MainOption.ContainsKey(value14) == false)
                                        {
                                            MainOption.Add(value14, value3);
                                        }
                                    }
                                    if (value4!= null && value4 != "0")
                                    {
                                        if (BonusOption.ContainsKey(value14) == false)
                                        {
                                            BonusOption.Add(value14, value4);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // 2013-06-21
                //elementaldefendall
                if (MainOption.ContainsKey("elementaldefendall"))
                {
                    string value = MainOption["elementaldefendall"];
                    MainOption.Add("elementaldefendfire", value);
                    MainOption.Add("elementaldefendair", value);
                    MainOption.Add("elementaldefendwater", value);
                    MainOption.Add("elementaldefendearth", value);
                }
            }
        }

        public override String GetTreeName()
        {
            return ViewName;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ViewName);
            sb.AppendLine(DescLongString);
            sb.AppendLine();
            if (MainOption.Count > 0)
            {
                sb.AppendLine("메인옵션 - 레벨값 옵션");
                IDictionaryEnumerator e = MainOption.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
                }
                sb.AppendLine();
            }
            if (BonusOption.Count > 0)
            {
                sb.AppendLine("추가옵션 - 절대값 옵션");
                IDictionaryEnumerator e = BonusOption.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
                }
                sb.AppendLine();
            }

            if (PercentOption.Count > 0)
            {
                sb.AppendLine("추가옵션 - 퍼센트 옵션");
                IDictionaryEnumerator e = PercentOption.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.AppendLine(Information.GetHangulAttrName((string)e.Key) + " : " + e.Value);
                }
                sb.AppendLine();
            }


            if ( NodeData.ContainsKey("conflict_id") ) sb.AppendLine("conflict_id: " + NodeData["conflict_id"]);

            /*
            IDictionaryEnumerator ee = NodeData.GetEnumerator();
            while (ee.MoveNext())
            {
                sb.AppendLine(ee.Key + " : " + ee.Value);
            }
             */
            //sb.AppendLine(this.xml);
            return sb.ToString();
        }
    }
}


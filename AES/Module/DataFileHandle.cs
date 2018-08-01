using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;


namespace AES
{
    public class DataFileHandle
    {
        public MainForm main;
        public string AppPath = null;
        public string DataPath = null;
        public string Extension = "aes";

        public DataFileHandle(MainForm main)
        {
            this.main = main;
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            DataPath = AppPath + "data";
            if (Directory.Exists(DataPath) == false) Directory.CreateDirectory(DataPath);
        }

        public bool IsExist(string filename)
        {
            string fullFilenmae = DataPath + "\\" + filename;
            return File.Exists(fullFilenmae);
        }


        public bool Save(string filename, string desc)
        {
            try
            {
                Stat item = main.CurrentStat;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes"));

                XmlNode rootNode = xmlDocument.CreateNode(XmlNodeType.Element, "AESInfo", string.Empty);
                xmlDocument.AppendChild(rootNode);

                string[] nodeNameList = { "클래스", "오른손", "왼손", "상의", "어깨", "장갑", "하의", "신발", "머리", "목걸이", "귀고리1", "귀고리2", "반지1", "반지2", "허리띠", "날개", "타이틀", "이디안", "음식", "캔디", "마석", "주문서", "스킬" };
                object[] itemList = { item.클래스, item.오른손, item.왼손, item.상의, item.어깨, item.장갑, item.하의, item.신발, item.머리, item.목걸이, item.귀고리1, item.귀고리2, item.반지1, item.반지2, item.허리띠, item.날개, item.타이틀, item.이디안, item.음식, item.캔디, item.마석목록, item.주문서목록, item.스킬목록 };

                XmlNode version = xmlDocument.CreateNode(XmlNodeType.Element, "version", string.Empty);
                rootNode.AppendChild(version);
                version.InnerText = this.main.Version;

                if (String.IsNullOrEmpty(desc) == false)
                {
                    XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "desc", string.Empty);
                    rootNode.AppendChild(node);
                    node.InnerText = desc;
                }

                for (int i = 0; i < nodeNameList.Length; i++)
                {
                    if (itemList[i] != null)
                    {
                        if (itemList[i] is EntityItem)
                        {
                            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, nodeNameList[i], string.Empty);
                            rootNode.AppendChild(node);
                            node.InnerText = ((EntityItem)itemList[i]).Name;
                            int level = ((EntityItem)itemList[i]).GetLevelOrCount();
                            if (level > 0)
                            {
                                XmlAttribute enchantAttributte = xmlDocument.CreateAttribute("enchant_count");
                                enchantAttributte.Value = ((EntityItem)itemList[i]).GetLevelOrCount() + "";
                                node.Attributes.Append(enchantAttributte);
                            }
                            if (((EntityItem)itemList[i]).RandomOptionGroupID != 0)
                            {
                                XmlAttribute roidAttributte = xmlDocument.CreateAttribute("random_option_id");
                                roidAttributte.Value = ((EntityItem)itemList[i]).RandomOptionGroupID + "";
                                node.Attributes.Append(roidAttributte);
                            }
                        }
                        else if (itemList[i] is InfoClassDefault)
                        {
                            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, nodeNameList[i], string.Empty);
                            rootNode.AppendChild(node);
                            node.InnerText = ((InfoClassDefault)itemList[i]).ClassName;
                        }
                        else if (itemList[i] is EntityTitle)
                        {
                            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, nodeNameList[i], string.Empty);
                            rootNode.AppendChild(node);
                            node.InnerText = ((EntityTitle)itemList[i]).Name;
                        }
                        else if (itemList[i] is List<EntityItem>)
                        {
                            List<EntityItem> list = (List<EntityItem>)itemList[i];
                            foreach (EntityItem ei in list)
                            {
                                XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, nodeNameList[i], string.Empty);
                                rootNode.AppendChild(node);
                                node.InnerText = ei.Name;
                                int level = ei.GetLevelOrCount();
                                if (level > 0)
                                {
                                    XmlAttribute enchantAttributte = xmlDocument.CreateAttribute("enchant_count");
                                    enchantAttributte.Value = ei.GetLevelOrCount() + "";
                                    node.Attributes.Append(enchantAttributte);
                                }
                            }
                        }
                        else if (itemList[i] is List<EntitySkill>)
                        {
                            List<EntitySkill> list = (List<EntitySkill>)itemList[i];
                            foreach (EntitySkill ei in list)
                            {
                                XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, nodeNameList[i], string.Empty);
                                rootNode.AppendChild(node);
                                node.InnerText = ei.Name;
                            }
                        }
                    }
                }
                xmlDocument.Save(DataPath + "\\" + filename + "." + Extension);

                return true;

            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public Stat Load(string filename)
        {
            Stat stat = new Stat();
            stat.FullFileName = filename;
            stat.OnlyFileName = new FileInfo(stat.FullFileName).Name;
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            XmlElement root = xml.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;

            foreach (XmlNode node in nodeList)
            {
                string s = node.InnerText;
                int level = -1;
                int gid = 0;
                XmlAttributeCollection xac = node.Attributes;
                if (xac.Count > 0)
                {
                    XmlAttribute att = xac["enchant_count"];
                    if ( att != null) level = Int32.Parse(att.Value);
                    att = xac["random_option_id"];
                    if (att != null) gid = Int32.Parse(att.Value);
                }

                switch (node.Name)
                {
                    case "desc": stat.Desc = s; break;
                    case "version": stat.Version = s; break;
                    case "클래스": stat.SetClass(s); break;
                    case "오른손":
                        stat.오른손 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.오른손.SetLevelOrCount(level);
                        stat.오른손.RandomOptionGroupID = gid;
                        break;
                    case "왼손":
                        stat.왼손 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.왼손.SetLevelOrCount(level);
                        stat.왼손.RandomOptionGroupID = gid;
                        break;
                    case "상의":
                        stat.상의 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.상의.SetLevelOrCount(level);
                        stat.상의.RandomOptionGroupID = gid;
                        break;
                    case "어깨":
                        stat.어깨 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.어깨.SetLevelOrCount(level);
                        stat.어깨.RandomOptionGroupID = gid;
                        break;
                    case "장갑":
                        stat.장갑 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.장갑.SetLevelOrCount(level);
                        stat.장갑.RandomOptionGroupID = gid;
                        break;
                    case "하의":
                        stat.하의 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.하의.SetLevelOrCount(level);
                        stat.하의.RandomOptionGroupID = gid;
                        break;
                    case "신발":
                        stat.신발 = this.main.xmlHandler.ItemList[s].Clone();
                        if (level > -1) stat.신발.SetLevelOrCount(level);
                        stat.신발.RandomOptionGroupID = gid;
                        break;
                    case "머리": stat.머리 = this.main.xmlHandler.ItemList[s].Clone(); stat.머리.RandomOptionGroupID = gid; break;
                    case "목걸이": stat.목걸이 = this.main.xmlHandler.ItemList[s].Clone(); stat.목걸이.RandomOptionGroupID = gid; break;
                    case "귀고리1": stat.귀고리1 = this.main.xmlHandler.ItemList[s].Clone(); stat.귀고리1.RandomOptionGroupID = gid; break;
                    case "귀고리2": stat.귀고리2 = this.main.xmlHandler.ItemList[s].Clone(); stat.귀고리2.RandomOptionGroupID = gid; break;
                    case "반지1": stat.반지1 = this.main.xmlHandler.ItemList[s].Clone(); stat.반지1.RandomOptionGroupID = gid; break;
                    case "반지2": stat.반지2 = this.main.xmlHandler.ItemList[s].Clone(); stat.반지2.RandomOptionGroupID = gid; break;
                    case "허리띠": stat.허리띠 = this.main.xmlHandler.ItemList[s].Clone(); stat.허리띠.RandomOptionGroupID = gid; break;
                    case "날개": stat.날개 = this.main.xmlHandler.ItemList[s].Clone(); stat.날개.RandomOptionGroupID = gid; break;
                    case "이디안": stat.이디안 = this.main.xmlHandler.ItemList[s].Clone(); stat.이디안.RandomOptionGroupID = gid; break;
                    case "음식": stat.음식 = this.main.xmlHandler.ItemList[s].Clone(); break;
                    case "캔디": stat.캔디 = this.main.xmlHandler.ItemList[s].Clone(); break;
                    case "타이틀": stat.타이틀 = this.main.xmlHandler.TitleList[s]; break;
                    case "마석":
                        EntityItem matter = this.main.xmlHandler.ItemList[s].Clone();
                        matter.SetLevelOrCount(level);
                        stat.마석목록.Add(matter);
                        break;
                    case "주문서":
                        EntityItem scroll = this.main.xmlHandler.ItemList[s].Clone();
                        stat.주문서목록.Add(scroll);
                        break;
                    case "스킬":
                        EntitySkill skill = this.main.xmlHandler.SkillList[s];
                        stat.스킬목록.Add(skill);
                        break;
                }
            }
            return stat;
        }

        public List<Stat> LoadFolder()
        {
            List<Stat> statList = new List<Stat>();
            DirectoryInfo dir = new DirectoryInfo(DataPath);
            FileInfo[] files = dir.GetFiles("*." + Extension);

            foreach (FileInfo f in files)
            {
                Stat s = Load(DataPath + "\\" + f.Name);
                statList.Add(s);
            }
            return statList;
        }
    }
}

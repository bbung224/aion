using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;

namespace AES
{
    public enum XMLEvent
    {
        스트링, 스킬, 아이템, 랜덤옵션, 세트아이템, 타이틀, 에러, 완료, 준비, 트리
    }
    public class XMLEventArgs : EventArgs
    {
        public XMLEvent EventType;
        public string Args;
        public XMLEventArgs(XMLEvent EventType)
        {
            this.EventType = EventType;
        }
        public XMLEventArgs(XMLEvent EventType, string args)
        {
            this.EventType = EventType;
            this.Args = args;
        }
    }
    public class XMLHandler
    {
        public MainForm main = null;
        public Hashtable StrHash;
        public Dictionary<string, EntitySkill> SkillList = null;
        public Dictionary<string, EntityItem> ItemList = null;
        public Dictionary<string, List<EntityItem>> ItemListKeyIsRandomOption = null;
        public Dictionary<string, EntityTitle> TitleList = null;

        public List<EntitySetItem> SetItemList = null;
        
        public List<EntityItemRandomOption> ItemRandomOptionList = null;

        public event EventHandler XMLEventHandler;

        public XMLHandler(MainForm main)
        {
            this.main = main;
        }

        private Thread xmlThread;

        public void Load()
        {
            xmlThread = new Thread(new ThreadStart(ThreadMethod));
            xmlThread.Start();
        }
        public void Stop()
        {
            if (xmlThread != null) xmlThread.Abort();
        }

        public void ThreadMethod()
        {
            try
            {
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.준비));
                LoadString();
                LoadSkill();
                LoadItem();
                LoadItemRandomOption();
                LoadSetItem();
                LoadTitle();
                Sort();
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.트리));
                this.main.CreateTree();
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.완료));
            }
            catch(Exception e)
            {
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.에러, e.ToString()));
            }
        }

        public void Sort()
        {
            {
                List<KeyValuePair<string, EntityItem>> result = new List<KeyValuePair<string, EntityItem>>(ItemList);
                result.Sort(
                    delegate(KeyValuePair<string, EntityItem> first, KeyValuePair<string, EntityItem> second)
                    {
                        return first.Value.ViewName.CompareTo(second.Value.ViewName);
                    }
                );
                ItemList = result.ToDictionary(p => p.Key, p => p.Value);
            }
            {
                List<KeyValuePair<string, EntitySkill>> result = new List<KeyValuePair<string, EntitySkill>>(SkillList);
                result.Sort(
                    delegate(KeyValuePair<string, EntitySkill> first, KeyValuePair<string, EntitySkill> second)
                    {
                        return first.Value.ViewName.CompareTo(second.Value.ViewName);
                    }
                );
                SkillList = result.ToDictionary(p => p.Key, p => p.Value);
            }
            {
                List<KeyValuePair<string, EntityTitle>> result = new List<KeyValuePair<string, EntityTitle>>(TitleList);
                result.Sort(
                    delegate(KeyValuePair<string, EntityTitle> first, KeyValuePair<string, EntityTitle> second)
                    {
                        return first.Value.ViewName.CompareTo(second.Value.ViewName);
                    }
                );
                TitleList = result.ToDictionary(p => p.Key, p => p.Value);
            }
            SetItemList.Sort(
                    delegate(EntitySetItem first, EntitySetItem second)
                    {
                        return first.ViewName.CompareTo(second.ViewName);
                    }
                );

            ItemRandomOptionList.Sort(
                    delegate(EntityItemRandomOption first, EntityItemRandomOption second)
                    {
                        return first.ViewName.CompareTo(second.ViewName);
                    }
                );
        }

        




        private void LoadString()
        {
            StrHash = new Hashtable();
            string[] urls = GenerateDataFile.fileListStrings;
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                //2013-07-29
                if (File.Exists(GenerateDataFile.dataPath + "\\" + urls[i]))
                {
                    xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                    XmlElement root = xml.DocumentElement;
                    XmlNodeList itemList = root.ChildNodes;
                    foreach (XmlNode node in itemList)
                    {
                        if (node["body"] == null) continue;
                        StrHash.Add(node["name"].InnerText.ToUpper(), node["body"].InnerText);
                    }
                    XMLEventHandler(this, new XMLEventArgs(XMLEvent.스트링, urls[i]));
                }
            }
        }

        private void LoadSkill()
        {
            SkillList = new Dictionary<string, EntitySkill>();
            string[] urls = GenerateDataFile.fileListSkills;
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                XmlElement root = xml.DocumentElement;
                XmlNodeList list = root.ChildNodes;
                foreach (XmlNode node in list)
                {
                    EntitySkill skill = new EntitySkill(this, node);
                    if (skill.ViewName == null) continue;
                    this.SkillList.Add(skill.Name, skill);
                }
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.스킬, urls[i]));
            }
        }

        //2013-09-08
        private void AddRandomOptionList(string key, EntityItem item)
        {
            if (this.ItemListKeyIsRandomOption.ContainsKey(key))
            {
                List<EntityItem> list = this.ItemListKeyIsRandomOption[key];
                list.Add(item);
            }
            else
            {
                List<EntityItem> list = new List<EntityItem>();
                list.Add(item);
                this.ItemListKeyIsRandomOption.Add(key, list);
            }
        }

        private void LoadItem()
        {
            ItemList = new Dictionary<string, EntityItem>();
            ItemListKeyIsRandomOption = new Dictionary<string, List<EntityItem>>();

            string[] urls = { GenerateDataFile.fileListItems[0], GenerateDataFile.fileListItems[1], GenerateDataFile.fileListItems[2] };
            //DataFilePath + "\\Items\\client_items_etc.xml", DataFilePath + "\\Items\\client_items_armor.xml", DataFilePath + "\\Items\\client_items_misc.xml" };
            
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                XmlElement root = xml.DocumentElement;
                XmlNodeList list = root.ChildNodes;
                foreach (XmlNode node in list)
                {
                    EntityItem item = new EntityItem(this, node);
                    if (item.ViewName == null)
                    {
                        //item.ViewName = item.Name;
                        continue;
                    }
                    if (item.NodeData.ContainsKey("random_option_set"))
                    {
                        string ros = item.NodeData["random_option_set"].ToLower();
                        AddRandomOptionList(ros, item);
                    }
                    if (item.NodeData.ContainsKey("polish_set_name"))
                    {
                        string psn = item.NodeData["polish_set_name"].ToLower();
                        AddRandomOptionList(psn, item);
                    }
                    this.ItemList.Add(item.Name, item);
                }
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.아이템, urls[i]));
            }
        }


        private void LoadItemRandomOption()
        {
            ItemRandomOptionList = new List<EntityItemRandomOption>();
            string[] urls = { GenerateDataFile.fileListItems[3], GenerateDataFile.fileListItems[4] };
            //{ DataFilePath + "\\Items\\client_item_random_option.xml", DataFilePath + "\\Items\\polish_bonus_setlist.xml" };
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                XmlElement root = xml.DocumentElement;
                XmlNodeList list = root.ChildNodes;
                foreach (XmlNode node in list)
                {
                    string optionName = node["name"].InnerText;
                    if (this.ItemListKeyIsRandomOption.ContainsKey(optionName))
                    {
                        List<EntityItem> il = this.ItemListKeyIsRandomOption[optionName];
                        foreach (EntityItem ii in il)
                        {
                            EntityItemRandomOption set = new EntityItemRandomOption(this, node, ii);
                            if (set.ViewName == null) continue;
                            //if (i == 1) continue; //이디안은 안보이게 할때
                            if (set.Name.Contains("test") == false)
                                this.ItemRandomOptionList.Add(set);
                        }
                    }
                }
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.랜덤옵션, urls[i]));
            }
        }

        private void LoadSetItem()
        {
            SetItemList = new List<EntitySetItem>();
            string[] urls = { GenerateDataFile.fileListItems[5] };
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                XmlElement root = xml.DocumentElement;
                XmlNodeList list = root.ChildNodes;
                foreach (XmlNode node in list)
                {
                    EntitySetItem set = new EntitySetItem(this, node);
                    if (set.ViewName == null) continue;
                    this.SetItemList.Add(set);
                }
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.랜덤옵션, urls[i]));
            }
        }

        private void LoadTitle()
        {
            TitleList = new Dictionary<string, EntityTitle>();
            string[] urls = GenerateDataFile.fileListPC;
            for (int i = 0; i < urls.Length; i++)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(GenerateDataFile.dataPath + "\\" + urls[i]);
                XmlElement root = xml.DocumentElement;
                XmlNodeList list = root.ChildNodes;
                foreach (XmlNode node in list)
                {
                    EntityTitle title = new EntityTitle(this, node);
                    if (title.ViewName == null) continue;
                    this.TitleList.Add(title.Name, title);
                }
                XMLEventHandler(this, new XMLEventArgs(XMLEvent.타이틀, urls[i]));
            }
        }
    }
}

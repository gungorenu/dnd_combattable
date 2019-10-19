/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CombatTable.Models.Custom;
using System.Xml;

namespace CombatTable.Models
{
    public class SessionManager : CombatTable.Models.Base.BaseObject
    {
        public static SessionManager Singleton { get; private set; }

        private ObservableCollection<Character> characters;
        private ObservableCollection<Map> maps;

        private Map selectedMap, editorMap;
        private Character selectedChar;

        public SessionManager()
        {
            characters = new ObservableCollection<Character>();
            maps = new ObservableCollection<Map>();
            Singleton = this;
        }

        public ObservableCollection<Character> Characters
        {
            get { return characters; }
        }
        public ObservableCollection<Map> Maps
        {
            get { return maps; }
        }

        public Character SelectedCharacter
        {
            get { return selectedChar; }
            set
            {
                if (selectedChar != value)
                {
                    selectedChar = value;
                    Trigger("SelectedCharacter");
                }
            }
        }
        public Map SelectedMap
        {
            get { return selectedMap; }
            set
            {
                if (selectedMap != value)
                {
                    selectedMap = value;
                    Trigger("SelectedMap");
                }
            }
        }
        public Map EditorMap
        {
            get { return editorMap; }
            set
            {
                if (editorMap != value)
                {
                    editorMap = value;
                    Trigger("EditorMap");
                }
            }
        }

        public void ReadFromFile(string file)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(file);
            ReadFromXml(xdoc.DocumentElement);
        }


        public void WriteToFile(string file)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlElement xel = xdoc.CreateElement("session");
            xdoc.AppendChild(xel);
            WriteToXml(xel);
            xdoc.Save(file);
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            //ReadCollection<SpellEffect>(xel, "session.effects", "effect", effects);
            //ReadCollection<Character>(xel, "session.characters", "character", characters, null);

            XmlNodeList xnl;

            xnl = xel.SelectNodes("session.characters/character");
            foreach (XmlNode xn in xnl)
            {
                Character chr = Character.ReadCharacterFromElement(xn as XmlElement);
                Characters.Add(chr);
            }
            ReadCollection<Map>(xel, "session.maps", "map", maps, null);

            string name = ReadAttribute(xel, "selectedMap");
            Map map = Maps.FirstOrDefault(f => f.Name == name);
            SelectedMap = map;
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            XmlElement xch = Write(xel, "session.characters");
            foreach (Character c in characters)
            {
                XmlElement xc = Write(xch, "character");
                c.WriteToXml(xc);
            }
            //WriteCollection<Character>(xel, "session.characters", "character", characters);
            WriteCollection<Map>(xel, "session.maps", "map", maps);
            if (SelectedMap != null) WriteAttribute(xel, "selectedMap", SelectedMap.Name);
        }
    }
}

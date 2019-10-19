/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CombatTable.Models.Base;
using System.Collections.ObjectModel;
using System.Xml;

namespace CombatTable.Models.Custom
{
    public class Map : BaseObject
    {
        private int sizeX, sizeY;
        private ObservableCollection<PointOfInterest> pois;
        private ObservableCollection<Door> doors;
        private ObservableCollection<Wall> walls;
        private ObservableCollection<Character> characters;
        private ObservableCollection<Block> blocks;
        private ObservableCollection<Effect> effects;
        private string name;
        private string notes;

        public Map()
        {
            pois = new ObservableCollection<PointOfInterest>();
            doors = new ObservableCollection<Door>();
            walls = new ObservableCollection<Wall>();
            characters = new ObservableCollection<Character>();
            blocks = new ObservableCollection<Block>();
            effects = new ObservableCollection<Effect>();
        }

        public ObservableCollection<Effect> Effects
        {
            get { return effects; }
        }
        public ObservableCollection<Block> Blocks
        {
            get { return blocks; }
        }
        public ObservableCollection<Character> Characters
        {
            get { return characters; }
        }
        public ObservableCollection<PointOfInterest> PointOfInterests
        {
            get { return pois; }
        }
        public ObservableCollection<Door> Doors
        {
            get { return doors; }
        }
        public ObservableCollection<Wall> Walls
        {
            get { return walls; }
        }


        public int SizeX
        {
            get { return sizeX; }
            set
            {
                if (sizeX != value)
                {
                    sizeX = value;
                    Trigger("SizeX");
                }
            }
        }
        public int SizeY
        {
            get { return sizeY; }
            set
            {
                if (sizeY != value)
                {
                    sizeY = value;
                    Trigger("SizeY");
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    Trigger("Name");
                }
            }
        }
        public string Notes
        {
            get { return notes; }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    Trigger("Notes");
                }
            }
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            Name = ReadText(xel);
            Notes = ReadCData(xel, "notes");
            SizeX = ReadIntAttribute(xel, "xsize");
            SizeY = ReadIntAttribute(xel, "ysize");
            ReadCollection<Door>(xel, "map.doors", "door", doors, null);
            ReadCollection<Wall>(xel, "map.walls", "wall", walls, null );
            ReadCollection<PointOfInterest>(xel, "map.pointofinterests", "pointofinterest", pois, null);
            ReadCollection<Block>(xel, "map.blocks", "block", blocks, null);
            ReadCollection<Effect>(xel, "map.effects", "effect", effects, null);

            XmlNodeList xnl = xel.SelectNodes("map.characters/character");
            foreach (XmlNode xn in xnl)
            {
                XmlElement x = xn as XmlElement;
                string code = ReadAttribute(x, "code");
                Character chr = SessionManager.Singleton.Characters.FirstOrDefault(f => f.Value == code);
                if (chr != null)
                {
                    Characters.Add(chr);
                }
            }
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            WriteText(xel, Name);
            WriteAttribute(xel, "xsize", SizeX);
            WriteAttribute(xel, "ysize", SizeY);
            WriteCData(xel, "notes", Notes);
            WriteCollection<Door>(xel, "map.doors", "door", doors);
            WriteCollection<Wall>(xel, "map.walls", "wall", walls);
            WriteCollection<PointOfInterest>(xel, "map.pointofinterests", "pointofinterest", pois);
            WriteCollection<Block>(xel, "map.blocks", "block", blocks);
            WriteCollection<Effect>(xel, "map.effects", "effect", effects);

            if (Characters.Count > 0)
            {
                XmlElement xchrs = Write(xel, "map.characters");
                foreach (Character chr in Characters)
                {
                    XmlElement xc = Write(xchrs, "character");
                    WriteAttribute(xc, "code", chr.Value);
                }
            }
        }

    }
}

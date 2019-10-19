/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;

namespace CombatTable.Models
{
    public class Character : NodeContainer, ICellModel
    {
        public static Character ReadCharacterFromElement(XmlElement xel)
        {
            Character chr = new Character(xel);
            chr.ReadFromXml(xel);
            if (chr.GetProperty<SingleProperty>(Nodes.Notes) != null)
            {
                chr.NewString(Nodes.Notes).PropertyChanged += (o, e) => chr.Trigger("Notes");
            }
            return chr;
        }

        private Character(XmlElement xel)
            : base(Nodes.Character)
        {
        }

        public Character(string name)
            : base(Nodes.Character)
        {
            Name = name;
            if (name.Length > 3)
                Value = name.ToUpper().Substring(0, 3);
            Add(new BaseInfo());
            Add(new Society());
            Add(new VitalStatistics());
            Add(new Stats());
            Add(new NodeContainer(Nodes.RacialTraits));
            Add(new NodeContainer(Nodes.ClassFeatures));
            Add(new NodeContainer(Nodes.Feats));
            Add(new Skills());
            Add(new Saves());
            Add(new NodeContainer(Nodes.SpecialQualities));
            Add(new CombatStances());
            Add(new Spellcasting());
            Add(new Gear());
            Add(new States());
            Add(new Session());
            NewString(Nodes.Notes);
        }

        public BaseInfo BaseInfo { get { return (BaseInfo)Properties[0]; } }
        public NodeContainer Race { get { return GetProperty<NodeContainer>(Nodes.RacialTraits); } }
        public NodeContainer Class { get { return GetProperty<NodeContainer>(Nodes.ClassFeatures); } }
        public Stats Stats { get { return GetProperty<Stats>(Nodes.Stats); } }
        public NodeContainer Feats { get { return GetProperty<NodeContainer>(Nodes.Feats); } }
        public Skills Skills { get { return GetProperty<Skills>(Nodes.Skills); } }
        public Saves Saves { get { return GetProperty<Saves>(Nodes.Saves); } }
        public Spellcasting Spellcasting { get { return GetProperty<Spellcasting>(Nodes.Spellcasting); } }
        public Gear Gear { get { return GetProperty<Gear>(Nodes.Gear); } }
        public NodeContainer SpecialQualities { get { return GetProperty<NodeContainer>(Nodes.SpecialQualities); } }
        public VitalStatistics VitalStatistics { get { return GetProperty<VitalStatistics>(Nodes.VitalStatistics); } }
        public CombatStances CombatStances { get { return GetProperty<CombatStances>(Nodes.CombatStances); } }
        public Society Society { get { return GetProperty<Society>(Nodes.Society); } }
        public States States { get { return GetProperty<States>(Nodes.States); } }
        public Session Session { get { return GetProperty<Session>(Nodes.Session); } }
        public SingleProperty Notes { get { return GetProperty<SingleProperty>(Nodes.Notes); } }

        public IEnumerable<Property> Children
        {
            get
            {
                List<Property> children = new List<Property>();
                children.AddRange(FindProperties<NodeContainer>( (c) => PropertyFactory.GetTypeOf(c) != Models.Properties.Session));
                children.Add(Notes);
                return children;
            }
        }

        Point ICellModel.Location
        {
            get
            {
                return new Point(Session.CurrentCoordinate_X.IntegerValue, Session.CurrentCoordinate_Y.IntegerValue);
            }
        }

        public void Export(string file)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlElement xel = xdoc.CreateElement("character");
            xdoc.AppendChild(xel);
            WriteToXml(xel);
            xdoc.Save(file);
        }

        public XmlDocument Export()
        {
            XmlDocument xdoc = new XmlDocument();
            XmlElement xel = xdoc.CreateElement("character");
            xdoc.AppendChild(xel);
            WriteToXml(xel);
            return xdoc;
        }

    }
}

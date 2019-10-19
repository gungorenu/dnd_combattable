/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable.Models
{
    public class Stats : NodeContainer
    {
        private Stats(XmlElement xel)
            : base(Nodes.Stats)
        {
        }

        public Stats() : base(Nodes.Stats)
        {
            Add(new IntegerProperty(Nodes.Strength.ToString(), 10));
            Add(new IntegerProperty(Nodes.Dexterity.ToString(), 10));
            Add(new IntegerProperty(Nodes.Constitution.ToString(), 10));
            Add(new IntegerProperty(Nodes.Intelligence.ToString(), 10));
            Add(new IntegerProperty(Nodes.Wisdom.ToString(), 10));
            Add(new IntegerProperty(Nodes.Charisma.ToString(), 10));
        }

        public IntegerProperty Strength { get { return GetProperty<IntegerProperty>(Nodes.Strength); } }
        public IntegerProperty Dexterity { get { return GetProperty<IntegerProperty>(Nodes.Dexterity); } }
        public IntegerProperty Constitution { get { return GetProperty<IntegerProperty>(Nodes.Constitution); } }
        public IntegerProperty Intelligence { get { return GetProperty<IntegerProperty>(Nodes.Intelligence); } }
        public IntegerProperty Wisdom { get { return GetProperty<IntegerProperty>(Nodes.Wisdom); } }
        public IntegerProperty Charisma { get { return GetProperty<IntegerProperty>(Nodes.Charisma); } }
    }
}

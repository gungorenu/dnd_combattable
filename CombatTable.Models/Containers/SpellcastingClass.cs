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
    public class SpellcastingClass : NodeContainer
    {
        private SpellcastingClass(XmlElement xel)
            : base(Nodes.SpellLevel)
        {
        }

        public SpellcastingClass(string name)
            : base(Nodes.SpellcastingClass)
        {
            Name = name;
            NewEnum(Nodes.SpellcastingType, "SpellcastingTypes", SpellcastingTypes.Spelllike.ToString());
            NewInteger(Nodes.CasterLevel, 1);
            NewInteger(Nodes.KnowLevel, 1);
            NewString(Nodes.Notes);
        }

        public EnumProperty SpellcastingType { get { return GetProperty<EnumProperty>(Nodes.SpellcastingType); } }
        public IntegerProperty CasterLevel { get { return GetProperty<IntegerProperty>(Nodes.CasterLevel); } }
        public IntegerProperty KnowLevel { get { return GetProperty<IntegerProperty>(Nodes.KnowLevel); } }
        public SingleProperty Notes { get { return GetProperty<SingleProperty>(Nodes.Notes); } }
        public IEnumerable<SpellLevel> SpellLevels { get { return FindProperties<SpellLevel>(); } }

        public SpellLevel NewSpellLevel(int level)
        {
            SpellLevel spellLevel = new SpellLevel(level);
            Add(spellLevel);
            return spellLevel;
        }


    }
}

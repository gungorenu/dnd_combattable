/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace CombatTable.Models
{
    public enum SpellcastingTypes
    {
        [Description("Spontaneous")]
        Spontaneous,
        [Description("Memorizer")]
        Memorizer,
        [Description("Spell-like")]
        Spelllike
    }

    public class Spellcasting : NodeContainer
    {
        private Spellcasting(XmlElement xel)
            : base(Nodes.Spellcasting)
        {
        }

        public Spellcasting() : base( Nodes.Spellcasting )
        {
        }

        public SpellcastingClass NewSpellcastingClass(string name)
        {
            SpellcastingClass cls = new SpellcastingClass(name);
            Add(cls);
            return cls;
        }
    }
}

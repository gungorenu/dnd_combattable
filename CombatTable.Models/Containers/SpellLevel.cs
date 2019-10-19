/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable.Models
{
    public class SpellLevel : NodeContainer
    {
        private SpellLevel(XmlElement xel)
            : base(Nodes.SpellLevel)
        {
            Properties.CollectionChanged += (o, e) => Trigger("Spells");
        }

        public SpellLevel(int level) 
            : base( Nodes.SpellLevel)
        {
            Name = level.ToString();
            NewInteger(Nodes.Slots, 1);
            NewString(Nodes.Spellbook);
            Properties.CollectionChanged += (o, e) => Trigger("Spells");
        }

        protected override void OnTrigger(string name)
        {
            if (name == "Parent")
            {
                (_parent as SpellcastingClass).SpellcastingType.PropertyChanged += (o, e) => { Trigger("Spells"); Trigger("VisibleItems"); };
            }
            base.OnTrigger(name);
        }

        public IntegerProperty Slots { get { return GetProperty<IntegerProperty>(Nodes.Slots); } }
        public SingleProperty SpellList { get { return GetProperty<SingleProperty>(Nodes.Spellbook); } }
        public IEnumerable<Spell> Spells { get { return FindProperties<Spell>(); } }
        public IEnumerable<Property> VisibleItems 
        { 
            get 
            {
                SpellcastingTypes sct = (SpellcastingTypes)Enum.Parse(typeof(SpellcastingTypes), (_parent as SpellcastingClass).SpellcastingType.Value);
                if( sct == SpellcastingTypes.Memorizer )
                    return Properties; 
                else
                    return Properties.Where(p => !(p is Spell));
            }
        }

        public Spell NewSpell(string name, bool available = true)
        {
            Spell spell = new Spell("");
            spell.Available = available;
            Add(spell);
            return spell;
        }

        public Spell NewSpellSlot(bool available = true)
        {
            Spell spell = new Spell("");
            spell.Available = available;
            Add(spell);
            return spell;
        }
    }
}

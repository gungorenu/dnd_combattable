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
    public class CombatStances : NodeContainer
    {
        private CombatStances(XmlElement xel)
            : base(Nodes.CombatStances)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("HasManeuverModifiers"); Trigger("ManeuverModifiers"); Trigger("Stances"); };
        }

        public CombatStances() : base( Nodes.CombatStances)
        {
            NewInteger(Nodes.BAB, 0);
            NewInteger(Nodes.CMB, 0);
            NewInteger(Nodes.CMD, 10);
            _properties.CollectionChanged += (o, e) => { Trigger("HasManeuverModifiers"); Trigger("ManeuverModifiers"); Trigger("Stances"); };
        }

        public Stance NewStance(string name)
        {
            Stance stance = new Stance(name);
            Add(stance);
            return stance;
        }

        public IntegerProperty BAB { get { return GetProperty<IntegerProperty>(Nodes.BAB); } }
        public IntegerProperty CMD { get { return GetProperty<IntegerProperty>(Nodes.CMD); } }
        public IntegerProperty CMB { get { return GetProperty<IntegerProperty>(Nodes.CMB); } }

        public IEnumerable<Modifier> ManeuverModifiers { get { return FindProperties<Modifier>(); } }
        public IEnumerable<Stance> Stances { get { return FindProperties<Stance>(); } }

        public bool HasManeuverModifiers {  get { return ManeuverModifiers.Count() > 0; } }
    }
}

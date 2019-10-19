/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CombatTable.Models
{
    public class VitalStatistics : NodeContainer
    {
        private VitalStatistics(XmlElement xel)
            : base(Nodes.VitalStatistics)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("SpeedTypes"); Trigger("SightModifiers"); };
        }

        public VitalStatistics()
            : base(Nodes.VitalStatistics)
        {
            NewInteger(Nodes.Age, 20);
            NewInteger(Nodes.Height, 0);
            NewInteger(Nodes.Weight, 0);
            NewInteger(Nodes.Initiative, 0);

            Add( new Modifier("Baseland Speed", 30));
            _properties.CollectionChanged += (o, e) => { Trigger("SpeedTypes"); Trigger("SightModifiers"); };
        }

        public IntegerProperty Age { get { return GetProperty<IntegerProperty>(Nodes.Age); } }
        public IntegerProperty Height { get { return GetProperty<IntegerProperty>(Nodes.Height); } }
        public IntegerProperty Weight { get { return GetProperty<IntegerProperty>(Nodes.Weight); } }
        public IEnumerable<Modifier> SpeedTypes { get { return FindProperties<Modifier>(); } }
        public IntegerProperty Initiative { get { return GetProperty<IntegerProperty>(Nodes.Initiative); } }
        public IEnumerable<Ability> SightModifiers { get { return FindProperties<Ability>(); } }

    }
}

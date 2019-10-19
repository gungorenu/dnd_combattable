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
    public class Society : NodeContainer
    {
        private Society(XmlElement xel)
            : base(Nodes.Society)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("TradeModifiers"); Trigger("HasTradeModifiers"); };
        }

        public Society()
            : base(Nodes.Society)
        {
            NewInteger(Nodes.Profession);
            NewString(Nodes.Speaks);
            NewString(Nodes.ReadWrites);
            NewInteger(Nodes.Trade);
            _properties.CollectionChanged += (o, e) => { Trigger("TradeModifiers"); Trigger("HasTradeModifiers"); };
        }

        public SingleProperty Speaks { get { return GetProperty<SingleProperty>(Nodes.Speaks); } }
        public SingleProperty ReadWrites { get { return GetProperty<SingleProperty>(Nodes.ReadWrites); } }
        public IntegerProperty Trade { get { return GetProperty<IntegerProperty>(Nodes.Trade); } }
        public IntegerProperty Profession { get { return FindProperty<IntegerProperty>((p) => p.Name != Nodes.Trade.ToString()); } }
        public IEnumerable<Modifier> TradeModifiers { get { return FindProperties<Modifier>(); } }
        public bool HasTradeModifiers { get { return TradeModifiers.Count()> 0; } }

    }
}

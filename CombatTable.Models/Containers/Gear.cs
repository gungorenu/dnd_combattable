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
    public class Gear : NodeContainer
    {
        private Gear(XmlElement xel)
            : base(Nodes.Gear)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("TotalEncumbrance"); Trigger("Equipment"); };
        }

        public Gear()
            : base( Nodes.Gear )
        {
            NewInteger(Nodes.Encumbrance, 0);
            _properties.CollectionChanged += (o, e) => { Trigger("TotalEncumbrance"); Trigger("Equipment"); };
        }

        public IEnumerable<Item> Equipment { get { return FindProperties<Item>(); } }
        public IntegerProperty Encumbrance { get { return GetProperty<IntegerProperty>(Nodes.Encumbrance); } }
        public int TotalEncumbrance
        {
            get
            {
                int enc = 0;
                enc = Equipment.Sum((i) => i.Equipped.BooleanValue ? i.Weight.IntegerValue : 0);
                decimal res = Math.Ceiling(Convert.ToDecimal(enc / 2));

                enc = Equipment.Sum((i) => i.Equipped.BooleanValue ? 0 : i.Weight.IntegerValue );
                return Convert.ToInt32(res + enc);
            }
        }
        
        public Item NewItem(string name, string description = "")
        {
            Item item = new Item(name, description);
            Add(item);
            item.Weight.PropertyChanged += UpdateEncumbrance;
            item.Equipped.PropertyChanged += UpdateEncumbrance;
            return item;
        }

        public void RemoveItem( Item i)
        {
            if( i.Parent == this )
            {
                i.Weight.PropertyChanged -= UpdateEncumbrance;
                i.Equipped.PropertyChanged -= UpdateEncumbrance;
            }
        }

        private void UpdateEncumbrance(Object o, EventArgs e)
        {
            Trigger("TotalEncumbrance");
        }
    }
}

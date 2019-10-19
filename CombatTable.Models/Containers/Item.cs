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
    public class Item : NodeContainer
    {
        private Item(XmlElement xel)
            : base(Nodes.Item)
        {
        }

        public Item(string name, string description)
            : base( Nodes.Item )
        {
            Name = name;
            Value = description;
            NewBoolean(Nodes.Equipped, false);
            NewInteger(Nodes.Weight, 1);
            NewInteger(Nodes.Stack, 1);
            NewString(Nodes.Tier);
        }

        public IntegerProperty Stack { get { return GetProperty<IntegerProperty>(Nodes.Stack); } }
        public IntegerProperty Weight { get { return GetProperty<IntegerProperty>(Nodes.Weight); } }
        public BooleanProperty Equipped { get { return GetProperty<BooleanProperty>(Nodes.Equipped); } }
        public SingleProperty Tier { get { return GetProperty<SingleProperty>(Nodes.Tier); } }
    }
}

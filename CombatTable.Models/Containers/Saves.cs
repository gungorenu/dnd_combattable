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
    public class Saves : NodeContainer
    {
        private Saves(XmlElement xel)
            : base(Nodes.Saves)
        {
        }

        public Saves() : base(Nodes.Saves)
        {
            NewInteger(Nodes.Fortitude, 0);
            NewInteger(Nodes.Reflex, 0);
            NewInteger(Nodes.Will, 0);
        }

        public IntegerProperty Fortitude { get { return GetProperty<IntegerProperty>(Nodes.Fortitude); } }
        public IntegerProperty Reflex { get { return GetProperty<IntegerProperty>(Nodes.Reflex); } }
        public IntegerProperty Will { get { return GetProperty<IntegerProperty>(Nodes.Will); } }
    }
}

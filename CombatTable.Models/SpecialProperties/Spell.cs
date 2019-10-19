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
    public class Spell : BooleanProperty
    {
        protected Spell (XmlElement xel)
            : base( xel)
        {
        }

        public Spell()
            : base("Spell")
        {
        }

        public Spell(string slotName)
            : base(slotName)
        {
        }

        public bool Available
        {
            get { return BooleanValue; }
            set { BooleanValue = value; }
        }
    }
}

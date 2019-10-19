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
    public class BooleanProperty : CombatTable.Models.Base.Property
    {
        protected BooleanProperty(XmlElement xel)
        {
        }

        public BooleanProperty(string name, bool value = false)
            : base( name , value.ToString())
        {
        }

        public bool BooleanValue
        {
            get { return Convert.ToBoolean(Value); }
            set { Value = value.ToString(); }
        }
    }
}

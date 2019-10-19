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
    public class IntegerProperty: CombatTable.Models.Base.Property
    {
        public IntegerProperty(string name, int value =0 )
            : base( name , value.ToString() )
        {
        }

        protected IntegerProperty(XmlElement xel)
        {
        }

        protected IntegerProperty()
        {

        }

        public int IntegerValue
        {
            get { return Convert.ToInt32(Value); }
            set { Value = value.ToString(); }
        }
    }
}

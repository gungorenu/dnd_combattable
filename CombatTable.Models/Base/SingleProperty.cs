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
    public class SingleProperty : CombatTable.Models.Base.Property
    {
        protected SingleProperty(XmlElement xel)
        {
        }

        protected SingleProperty()
        {
        }

        public SingleProperty(string name, string value)
            : base(name, value)
        {
        }
    }
}

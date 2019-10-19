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
    public class Modifier : IntegerProperty
    {
        private Modifier(XmlElement xel)
            : base(xel)
        {
        }

        public Modifier()
        {
        }

        public Modifier(string name, int value = 0)
            : base( name , value )
        {
        }
    }
}

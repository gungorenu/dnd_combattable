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
    public class Ability : SingleProperty
    {
        private Ability(XmlElement xel)
            : base( xel)
        {
        }

        public Ability()
        {
        }

        public Ability( string name, string description ="")
            : base( name , description )
        {
        }
    }
}

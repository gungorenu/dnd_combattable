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
    public class EnumProperty : SingleProperty
    {
        private string typeName;

        public EnumProperty(string name, string value)
            : base( name, value )
        {
        }

        private EnumProperty(XmlElement xel)
            : base(xel)
        {
        }

        public string TypeName
        {
            get { return typeName; }
            set
            {
                if (typeName != value)
                {
                    typeName = value;
                    Trigger("TypeName");
                }
            }
        }

        protected internal override void ReadSelfFromXml(XmlElement xel)
        {
            base.ReadSelfFromXml(xel);
            TypeName = ReadAttribute(xel, "enum");
        }

        protected internal override void WriteSelfToXml(XmlElement xel)
        {
            base.WriteSelfToXml(xel);
            WriteAttribute(xel, "enum", TypeName);
        }
    }
}

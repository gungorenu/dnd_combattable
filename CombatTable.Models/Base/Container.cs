/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;

namespace CombatTable.Models.Base
{
    public abstract class Container : Property
    {
        protected ObservableCollection<Property> _properties = new ObservableCollection<Property>();

        protected Container(string name, string value)
            : base(name, value)
        {
        }

        protected Container()
        {
        }

        public virtual ObservableCollection<Property> Properties
        {
            get { return _properties; }
            private set
            {
                if (_properties != value)
                {
                    _properties = value;
                    Trigger("Properties");
                }
            }
        }

        public void Add(Property prop)
        {
            prop.Parent = this;
            Properties.Add(prop);
        }

        public P New<P>() where P : Property, new()
        {
            P prop = new P();
            Add(prop);
            if (prop.Parent == null) throw new Exception("Another property with same name exists!");
            return prop;
        }

        protected SingleProperty NewString(string name, string value = "")
        {
            SingleProperty prop = new SingleProperty(name, value);
            Add(prop);
            if (prop.Parent == null) throw new Exception("Another property with same name exists!");
            return prop;
        }

        protected EnumProperty NewEnum(string name, string typeName, string value = "")
        {
            EnumProperty prop = new EnumProperty(name, value) { TypeName = typeName };
            Add(prop);
            if (prop.Parent == null) throw new Exception("Another property with same name exists!");
            return prop;
        }

        protected IntegerProperty NewInteger(string name, int value = 0)
        {
            IntegerProperty prop = new IntegerProperty(name, value);
            Add(prop);
            if (prop.Parent == null) throw new Exception("Another property with same name exists!");
            return prop;
        }

        protected BooleanProperty NewBoolean(string name, bool value = false)
        {
            BooleanProperty prop = new BooleanProperty(name, value);
            Add(prop);
            if (prop.Parent == null) throw new Exception("Another property with same name exists!");
            return prop;
        }

        protected Ability NewAbility(string name, string description = "")
        {
            Ability ab = new Ability(name, description);
            Add(ab);
            return ab;
        }

        public void Remove(Property prop)
        {
            if (prop.Parent == this)
            {
                Properties.Remove(prop);
                prop.Parent = null;
            }
        }

        protected internal override void WriteSelfToXml(System.Xml.XmlElement xel)
        {
            base.WriteSelfToXml(xel);
            XmlElement xchildren = Write(xel, "property.children");
            foreach (Property p in Properties)
            {
                XmlElement xp = Write(xchildren, "property");
                p.WriteToXml(xp);
            }
        }

        protected internal override void ReadSelfFromXml(XmlElement xel)
        {
            base.ReadSelfFromXml(xel);
            XmlNodeList xnl = xel.SelectNodes("property.children/property");
            foreach (XmlNode xn in xnl)
            {
                string subtype = ReadAttribute(xn as XmlElement, "subtype");
                Properties propType = PropertyFactory.ConvertToPropertyType(subtype);
                Property prop = PropertyFactory.CreateProperty(propType);
                prop.ReadFromXml(xn as XmlElement);
                Add(prop);
            }
        }
    }
}

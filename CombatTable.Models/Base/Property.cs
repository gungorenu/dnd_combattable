/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Xml;

namespace CombatTable.Models.Base
{
    public abstract class Property : BaseObject
    {
        protected string _key;
        protected string _name;
        protected string _value;
        protected Property _parent;

        protected Property()
        {
        }

        protected Property(string name, string value)
        {
            this._key = name;
            this._name = name;
            this._value = value;
        }

        protected override void OnTriggerAll()
        {
            if (_parent != null) _parent.TriggerAll();
            base.OnTriggerAll();
        }

        #region Properties

        public Properties PropertyType
        {
            get { return PropertyFactory.GetTypeOf(this); }
        }

        public Property Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    Trigger(nameof(Parent));
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Trigger(nameof(Name));
                }
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    Trigger(nameof(Value));
                }
            }
        }

        public string Key
        {
            get { return _key; }
        }
        #endregion

        protected internal virtual void ReadSelfFromXml(System.Xml.XmlElement xel)
        {
            string type = ReadAttribute(xel, "subtype");
            if (this.GetType().Name != type)
                throw new Exception("Property type mismatch!");

            _key = ReadText(Read(xel, "property.key"));
            Name = ReadText(Read(xel,"property.name"));
            Value = ReadCData(xel, "property.value");
        }

        protected internal virtual void WriteSelfToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            bool isSelfContainer = (this as Container) != null;
            WriteAttribute(xel, "type", isSelfContainer ? "container" : "property");
            WriteAttribute(xel, "subtype", this.GetType().Name);
            XmlElement xn = Write(xel, "property.key");
            WriteText(xn, Key);
            xn = Write(xel, "property.name");
            WriteText(xn, Name);
            WriteCData(xel, "property.value", Value);
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            ReadSelfFromXml(xel);
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            WriteSelfToXml(xel);
        }
    }
}


/*

<property type="container|property" subtype="!CLASSNAME!">
  <property.children>
    ... recursive here ...
  </property.children>
  <property.name>!NAME!</property.name> ... CHANGEABLE, defaults to !KEY!
  <property.key>!KEY!<property.key> ... FIXED
  <property.value>CDATA[ !VALUE! ]</property.value>
</property>


*/
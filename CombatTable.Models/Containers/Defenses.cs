/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable.Models
{
    public class Defenses : NodeContainer
    {
        private Defenses(XmlElement xel)
            : base(Nodes.Defenses)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("AbilitySummary"); Trigger("HasCustomACModifiers"); };
        }

        public Defenses()
            : base(Nodes.Defenses)
        {
            Name = "AC";
            Value = 10.ToString();
            NewInteger(Nodes.FlatfootedAC, 10);
            NewInteger(Nodes.TouchAC, 10);
            _properties.CollectionChanged += (o, e) => { Trigger("AbilitySummary"); Trigger("HasCustomACModifiers"); };
        }

        public IntegerProperty FlatfootedAC { get { return GetProperty<IntegerProperty>(Nodes.FlatfootedAC); } }
        public IntegerProperty TouchAC { get { return GetProperty<IntegerProperty>(Nodes.TouchAC); } }
        public IEnumerable<Modifier> ACModifiers { get { return FindProperties<Modifier>(); } }
        public bool HasCustomACModifiers
        {
            get{ return ACModifiers.Count() > 0; }
        }

        public string AbilitySummary
        {
            get
            {
                string text = "";
                foreach (Modifier a in ACModifiers)
                    text += a.Name + ", ";
                text = text.TrimEnd(' ', ',');
                return text;
            }
        }
    }
}

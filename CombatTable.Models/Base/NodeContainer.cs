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
    public class NodeContainer : Container
    {
        public NodeContainer(Nodes node, string value = "")
            : base(node.ToString(), value)
        {
        }

        private NodeContainer(XmlElement xel)
        {
        }


        public SingleProperty NewString(Nodes name, string value = "")
        {
            return NewString(name.ToString(), value);
        }

        public EnumProperty NewEnum(Nodes name, string typeName, string value = "")
        {
            return NewEnum(name.ToString(), typeName, value);
        }

        public IntegerProperty NewInteger(Nodes name, int value = 0)
        {
            return NewInteger(name.ToString(), value);
        }

        public BooleanProperty NewBoolean(Nodes name, bool value = false)
        {
            return NewBoolean(name.ToString(), value);
        }

        public Modifier NewModifier(string condition, int value = 0)
        {
            Modifier ab = new Modifier(condition + " " + this.Properties.Count, value);
            Add(ab);
            return ab;
        }

        public Ability NewTrait(string description = "")
        {
            return NewAbility(Nodes.Trait, description);
        }
        public Ability NewFeat(string description = "")
        {
            return NewAbility(Nodes.Feat, description);
        }
        public Ability NewFeature(string description = "")
        {
            return NewAbility(Nodes.Feature, description);
        }
        public Ability NewSpecialQuality(string description = "")
        {
            return NewAbility(Nodes.SpecialQuality, description);
        }
        public Ability NewVision(string description = "")
        {
            return NewAbility("New Vision Ability", description);
        }

        private Ability NewAbility(Nodes nodeName, string description = "")
        {
            Ability ab = new Ability(nodeName.ToString() + " " + this.Properties.Count, description);
            Add(ab);
            return ab;
        }

        public P GetProperty<P>(Nodes nodeName)
            where P : Property
        {
            if (nodeName == Nodes.Self)
                return this as P;
            else
            {
                // first check "node name"
                Property prop = Properties.FirstOrDefault(f => f.Key == nodeName.ToString());
                if (prop == null)
                    // if not found check "property name"
                    prop = Properties.FirstOrDefault(f => f.Name == nodeName.ToString());

                return prop as P;
            }
        }

        public P FindProperty<P>(Func<P, bool> finder = null)
            where P : Property
        {
            if (finder == null)
                return this.Properties.FirstOrDefault((p) => p is P) as P;
            else
                return this.Properties.FirstOrDefault((p) => p is P && finder((P)p)) as P;
        }

        public IEnumerable<P> FindProperties<P>(Func<P, bool> finder = null)
            where P : Property
        {
            if (finder == null)
                return this.Properties.Where((p) => p is P).Cast<P>();
            else
                return this.Properties.Where((p) => p is P && finder((P)p)).Cast<P>();
        }
    }
}

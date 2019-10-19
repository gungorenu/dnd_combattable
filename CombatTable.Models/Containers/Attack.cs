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
    public class Attack : NodeContainer
    {
        private Attack(XmlElement xel)
            : base(Nodes.Attack)
        {
        }

        public Attack() : base( Nodes.Attack)
        {
            NewInteger( Nodes.ToHit, 0);
            NewInteger(Nodes.AttackCount, 1);
            NewInteger(Nodes.Range, 5);
            NewString(Nodes.CriticalHit, "20/x2");
            Properties.CollectionChanged += (o, e) => Trigger("Damages");
        }

        public Attack(string name) : this()
        {
            Name = name;
        }

        public IntegerProperty ToHit { get { return GetProperty<IntegerProperty>(Nodes.ToHit); } }
        public IntegerProperty Count { get { return GetProperty<IntegerProperty>(Nodes.AttackCount); } }
        public IntegerProperty Range { get { return GetProperty<IntegerProperty>(Nodes.Range); } }
        public SingleProperty Critical { get { return GetProperty<SingleProperty>(Nodes.CriticalHit); } }
        public IEnumerable<Ability> Damages { get { return FindProperties<Ability>(); } }

        public Ability NewDamage(string name, string description = "")
        {
            Ability ab = new Ability(name, description);
            Add(ab);
            return ab;
        }
    }
}

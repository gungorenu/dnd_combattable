/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace CombatTable.Models
{
    public enum StanceTypes : int
    {
        [Description("Melee 1handed")]  Melee = 0,
        [Description("Melee 2handed")]  Melee2Handed = 3,
        [Description("Melee 1handed & Shield")]  Melee1HandedShield = 4,
        [Description("Ranged")] Ranged = 1,
        [Description("Thrown")] Thrown = 2,
        [Description("Thrown & Shield")] ThrownShield = 5,
        [Description("Natural Attacks")] NaturalAttacks = 6
    }

    public class Stance : NodeContainer
    {
        private Stance(XmlElement xel)
            : base(Nodes.Stance)
        {
        }

        public Stance(string name) : base(Nodes.Stance)
        {
            Name = name;
            NewEnum(Nodes.StanceType, "StanceTypes", StanceTypes.Melee.ToString());
            Add(new Defenses());
        }

        public EnumProperty Type { get { return GetProperty<EnumProperty>(Nodes.StanceType); } }
        public Defenses Defense { get { return GetProperty<Defenses>(Nodes.Defenses); } }

        public Attack NewAttack(string name)
        {
            Attack attack = new Attack(name);
            Add(attack);
            return attack;
        }
    }
}

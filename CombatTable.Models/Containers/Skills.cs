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
    public class Skills : NodeContainer
    {
        private Skills(XmlElement xel)
            : base(Nodes.Skills)
        {
        }

        public Skills()
            : base( Nodes.Skills)
        {
        }

        public Skill NewSkill(SkillTypes name, int value =0)
        {
            Skill skill = new Skill(name, value);
            Add(skill);
            return skill;
        }
    }
}

/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Description = System.ComponentModel.DescriptionAttribute;

namespace CombatTable.Models
{
    public enum SkillTypes
    {
        [Description("Acrobatics")] Acrobatics,
        [Description("Athletics")] Athletics,
        [Description("Finesse")] Finesse,
        [Description("Heal")] Heal,
        [Description("Impersonation")] Impersonation,
        [Description("Influence")] Influence,
        [Description("Nature")] Nature,
        [Description("Perception")] Perception,
        [Description("Stealth")] Stealth,
        [Description("Religion")] Religion,
        [Description("Society")] Society,
        [Description("Spellcraft")] Spellcraft,
    }


    public class Skill : IntegerProperty
    {
        public Skill(SkillTypes skillType, int value)
            : base(skillType.ToString(), value)
        {
        }

        protected Skill(XmlElement xel)
            : base( xel)
        {
        }

        public Skill()
        {
        }
    }
}

/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatTable.Models
{
    /// <summary>
    /// XML relevant property types, equals to type name
    /// </summary>
    public enum Properties
    {
        Undefined,

        SingleProperty,
        IntegerProperty,
        BooleanProperty,
        EnumProperty,
        Ability,
        Modifier,
        NodeContainer,
        VitalStatistics,
        Stats,
        Stance,
        SpellLevel,
        SpellcastingClass,
        Spellcasting,
        Society,
        Spell,
        Skills,
        Saves,
        Item,
        Gear,
        Defenses,
        CombatStances,
        BaseInfo,
        Attack,
        States,
        State,
        SessionManager,
        Skill,
        Session,
    }
}

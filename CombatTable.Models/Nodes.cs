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
    /// Names of properties for Binding relevant, not always match property type
    /// </summary>
    public enum Nodes
    {
        Undefined,

        Character,
        Self,
        Modifier,
        Trait,
        Feature,
        Feat,
        SpecialQuality,

        // Character
        BaseInfo,
        RacialTraits,
        ClassFeatures,
        Stats,
        Feats,
        Skills,
        Saves,
        Spellcasting,
        Gear,
        SpecialQualities,
        VitalStatistics,
        CombatStances,
        Society,
        States,
        Notes,
        Session,

        // BaseInfo
        Alignment,
        HitPoints,
        LevelAdjustment,
        Size,
        Player,
        Race,
        Classes,
        Gender,

        // Society
        Speaks, 
        ReadWrites,
        Profession,
        Trade,

        // VitalStatistics
        Age,
        Height,
        Weight,
        Speed,
        Initiative,

        // Stats
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,

        // Saves
        Fortitude,
        Reflex,
        Will,

        // Combat Stances
        BAB, 
        CMD, 
        CMB,

        // Stance
        Stance,
        StanceType,
        Defenses,

        // Defenses
        FlatfootedAC,
        TouchAC,
        Attack,

        // Attack
        ToHit,
        AttackCount,
        CriticalHit,
        Range,
        Damage,

        // Spellcasting
        SpellcastingClass, 

        // Spellcasting Class
        SpellcastingType,
        CasterLevel,
        KnowLevel,
        SpellLevel,

        // SpellLevel
        Slots,
        Spellbook,
        Spell,

        // Spell 
        Available,

        // Gear
        Encumbrance,

        // Item
        Item,
        Stack,
        Equipped,
        Tier,

        // State
        State,
        CustomState,
        Duration,

        // Session
        CurrentHitPoints,
        CurrentInitiative,
        CurrentPlayer,
        CurrentCoordinate_X,
        CurrentCoordinate_Y,


    }
}

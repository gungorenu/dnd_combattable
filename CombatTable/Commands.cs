/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatTable.Commands
{
    public class NewRaceAbilityCommand : NewItemCommand<NodeContainer>
    {
        protected override void AddNewItem(NodeContainer parent)
        {
            parent.NewTrait();
        }
    }

    public class NewClassAbilityCommand : NewItemCommand<NodeContainer>
    {
        protected override void AddNewItem(NodeContainer parent)
        {
            parent.NewFeature();
        }
    }

    public class NewSpecialQualityCommand : NewItemCommand<NodeContainer>
    {
        protected override void AddNewItem(NodeContainer parent)
        {
            parent.NewSpecialQuality();
        }
    }

    public class NewStanceModifierCommand : NewItemCommand<CombatStances>
    {
        protected override void AddNewItem(CombatStances parent)
        {
            parent.NewModifier("Maneuver Modifier");
        }
    }

    public class NewTradeModifierCommand : NewItemCommand<Society>
    {
        protected override void AddNewItem(Society parent)
        {
            parent.NewModifier("Trade Modifier");
        }
    }

    public class NewSpeedAbilityCommand : NewItemCommand<VitalStatistics>
    {
        protected override void AddNewItem(VitalStatistics parent)
        {
            parent.NewModifier("New Speed Ability", 30);
        }
    }

    public class NewPerceptionAbilityCommand : NewItemCommand<VitalStatistics>
    {
        protected override void AddNewItem(VitalStatistics parent)
        {
            parent.NewVision();
        }
    }

    public class DeleteModifierCommand : DeleteItemCommand<CombatTable.Models.Modifier>
    {
        protected override void DeleteItem(Models.Modifier child)
        {
            (child.Parent as NodeContainer).Remove(child);
        }
    }


    public class DeleteAbilityCommand : DeleteItemCommand<CombatTable.Models.Ability>
    {
        protected override void DeleteItem(Models.Ability child)
        {
            (child.Parent as NodeContainer).Remove(child);
        }
    }

    public class NewSaveModifierCommand : NewItemCommand<CombatTable.Models.Saves>
    {
        protected override void AddNewItem(Models.Saves parent)
        {
            parent.NewModifier("Modifier");
        }
    }

    public class NewFeatCommand : NewItemCommand<NodeContainer>
    {
        protected override void AddNewItem(NodeContainer parent)
        {
            parent.NewFeat("New Feat");
        }
    }

    public class NewACModifierCommand : NewItemCommand<CombatTable.Models.Defenses>
    {
        protected override void AddNewItem(Models.Defenses parent)
        {
            parent.NewModifier("AC Modifier");
        }
    }

    public class NewStanceCommand : NewItemCommand<CombatTable.Models.CombatStances>
    {
        protected override void AddNewItem(Models.CombatStances parent)
        {
            parent.NewStance("Stance " + parent.Properties.Count);
        }
    }

    public class NewAttackCommand : NewItemCommand<CombatTable.Models.Stance>
    {
        protected override void AddNewItem(Models.Stance parent)
        {
            parent.NewAttack("Attack " + parent.Properties.Count);
        }
    }

    public class NewDamageCommand : NewItemCommand<CombatTable.Models.Attack>
    {
        protected override void AddNewItem(Models.Attack parent)
        {
            parent.NewDamage("Damage " + parent.Properties.Count);
        }
    }

    public class NewSkillCommand : NewItemCommand<CombatTable.Models.Skills>
    {
        protected override void AddNewItem(Models.Skills parent)
        {
            parent.NewSkill(CombatTable.Models.SkillTypes.Acrobatics);
        }
    }

    public class DeleteStanceCommand : DeleteItemCommand<CombatTable.Models.Stance>
    {
        protected override void DeleteItem(Models.Stance child)
        {
            if (child.Parent is CombatTable.Models.CombatStances)
                (child.Parent as CombatTable.Models.CombatStances).Properties.Remove(child);
        }
    }

    public class DeleteSkillCommand : DeleteItemCommand<CombatTable.Models.Skill>
    {
        protected override void DeleteItem(Models.Skill child)
        {
            if (child.Parent is CombatTable.Models.Skills)
                (child.Parent as CombatTable.Models.Skills).Properties.Remove(child);
        }
    }

    public class DeleteItemCommand : DeleteItemCommand<CombatTable.Models.Item>
    {
        protected override void DeleteItem(Models.Item child)
        {
            if (child.Parent is CombatTable.Models.Gear)
            {
                (child.Parent as CombatTable.Models.Gear).Properties.Remove(child);
                (child.Parent as CombatTable.Models.Gear).RemoveItem(child);
            }
        }
    }

    public class NewItemCommand : NewItemCommand<CombatTable.Models.Gear>
    {
        protected override void AddNewItem(Models.Gear parent)
        {
            parent.NewItem("New Item");
        }
    }

    public class DeleteAttackCommand : DeleteItemCommand<CombatTable.Models.Attack>
    {
        protected override void DeleteItem(Models.Attack child)
        {
            if (child.Parent is CombatTable.Models.Stance)
                (child.Parent as CombatTable.Models.Stance).Properties.Remove(child);
        }
    }

    public class NewStateCommand : NewItemCommand<CombatTable.Models.States>
    {
        protected override void AddNewItem(Models.States parent)
        {
            parent.NewState();
        }
    }

    public class NewSpellcastingClassCommand : NewItemCommand<CombatTable.Models.Spellcasting>
    {
        protected override void AddNewItem(Models.Spellcasting parent)
        {
            parent.NewSpellcastingClass("Spellcasting Class");
        }
    }

    public class NewSpellLevelCommand : NewItemCommand<CombatTable.Models.SpellcastingClass>
    {
        protected override void AddNewItem(Models.SpellcastingClass parent)
        {
            parent.NewSpellLevel(1);
        }
    }

    public class NewSpellCommand : NewItemCommand<CombatTable.Models.SpellLevel>
    {
        protected override void AddNewItem(Models.SpellLevel parent)
        {
            CombatTable.Models.SpellcastingTypes sct = (CombatTable.Models.SpellcastingTypes)Enum.Parse(typeof(CombatTable.Models.SpellcastingTypes), (parent.Parent as CombatTable.Models.SpellcastingClass).SpellcastingType.Value);
            if (sct == Models.SpellcastingTypes.Memorizer)
                parent.NewSpell("New Memorized Spell");
            else
                parent.NewSpellSlot();
        }
    }

    public class DeleteStateCommand : DeleteItemCommand<CombatTable.Models.State>
    {
        protected override void DeleteItem(Models.State child)
        {
            if (child.Parent is CombatTable.Models.States)
                (child.Parent as CombatTable.Models.States).Properties.Remove(child);
        }
    }

    public class DeleteSpellcastingClassCommand : DeleteItemCommand<CombatTable.Models.SpellcastingClass>
    {
        protected override void DeleteItem(Models.SpellcastingClass child)
        {
            if (child.Parent is CombatTable.Models.Spellcasting)
                (child.Parent as CombatTable.Models.Spellcasting).Properties.Remove(child);
        }
    }

    public class DeleteSpellLevelCommand : DeleteItemCommand<CombatTable.Models.SpellLevel>
    {
        protected override void DeleteItem(Models.SpellLevel child)
        {
            if (child.Parent is CombatTable.Models.SpellcastingClass)
                (child.Parent as CombatTable.Models.SpellcastingClass).Properties.Remove(child);
        }
    }

    //public class DeleteCustomStateCommand : DeleteItemCommand<CombatTable.Models.CustomState>
    //{
    //    protected override void DeleteItem(Models.CustomState child)
    //    {
    //        if (child.parent is CombatTable.Models.States)
    //            (child.parent as CombatTable.Models.States).Properties.Remove(child);
    //    }
    //}

    public class DeleteSpellCommand : DeleteItemCommand<CombatTable.Models.Spell>
    {
        protected override void DeleteItem(Models.Spell child)
        {
            if (child.Parent is CombatTable.Models.SpellLevel)
                (child.Parent as CombatTable.Models.SpellLevel).Properties.Remove(child);
        }
    }
}

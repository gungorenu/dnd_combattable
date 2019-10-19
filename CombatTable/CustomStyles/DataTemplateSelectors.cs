/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CombatTable.Models.Base;
using CombatTable.Models;

namespace CombatTable.Styles
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item == null || element == null) return base.SelectTemplate(item, container);

            Property prop = item as Property;
            Nodes node = Nodes.Undefined;
            if( Enum.TryParse(prop.Key, out node))
            {
                switch (node)
                {
                    //: complex container properties
                    case Nodes.BaseInfo: return (DataTemplate)element.FindResource("DataTemplateForBaseInfo");
                    case Nodes.Stats: return (DataTemplate)element.FindResource("DataTemplateForStats");
                    case Nodes.Spellcasting: return (DataTemplate)element.FindResource("DataTemplateForSpellcasting");
                    case Nodes.SpellcastingClass: return (DataTemplate)element.FindResource("DataTemplateForSpellcastingClass");
                    case Nodes.SpellLevel: return (DataTemplate)element.FindResource("DataTemplateForSpellLevel");
                    case Nodes.ClassFeatures: return (DataTemplate)element.FindResource("DataTemplateForNodeContainerWithoutTitle");
                    case Nodes.RacialTraits: return (DataTemplate)element.FindResource("DataTemplateForNodeContainerWithoutTitle");
                    case Nodes.Feats: return (DataTemplate)element.FindResource("DataTemplateForNodeContainerWithoutTitle");
                    case Nodes.SpecialQualities: return (DataTemplate)element.FindResource("DataTemplateForNodeContainerWithoutTitle");
                    case Nodes.Saves: return (DataTemplate)element.FindResource("DataTemplateForSaves");
                    case Nodes.Skills: return (DataTemplate)element.FindResource("DataTemplateForSkills");
                    case Nodes.CombatStances: return (DataTemplate)element.FindResource("DataTemplateForCombatStances");
                    case Nodes.Stance: return (DataTemplate)element.FindResource("DataTemplateForStance");
                    case Nodes.Attack: return (DataTemplate)element.FindResource("DataTemplateForAttack");
                    case Nodes.Defenses: return (DataTemplate)element.FindResource("DataTemplateForDefenses");
                    case Nodes.Society: return (DataTemplate)element.FindResource("DataTemplateForSociety");
                    case Nodes.VitalStatistics: return (DataTemplate)element.FindResource("DataTemplateForVitalStatistics");
                    case Nodes.Item: return (DataTemplate)element.FindResource("DataTemplateForItem");
                    case Nodes.Gear: return (DataTemplate)element.FindResource("DataTemplateForGear");
                    case Nodes.States: return (DataTemplate)element.FindResource("DataTemplateForStates");
                }
            }

            //: simple single properties
            switch (PropertyFactory.GetTypeOf(prop))
            {
                case Models.Properties.IntegerProperty: return (DataTemplate)element.FindResource("DataTemplateForInteger");
                case Models.Properties.SingleProperty: return (DataTemplate)element.FindResource("DataTemplateForSingle");
                case Models.Properties.EnumProperty: return (DataTemplate)element.FindResource("DataTemplateForEnumValue");
                case Models.Properties.BooleanProperty: return (DataTemplate)element.FindResource("DataTemplateForBoolean");
                case Models.Properties.Spell: return (DataTemplate)element.FindResource("DataTemplateForSpell");
                case Models.Properties.Ability: return (DataTemplate)element.FindResource("DataTemplateForAbility");
                case Models.Properties.Modifier: return (DataTemplate)element.FindResource("DataTemplateForModifier");
                case Models.Properties.Skill: return (DataTemplate)element.FindResource("DataTemplateForSkill");
                case Models.Properties.State: return (DataTemplate)element.FindResource("DataTemplateForState");
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class CustomSummaryDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item == null || element == null) return base.SelectTemplate(item, container);

            Property prop = item as Property;
            Nodes node = Nodes.Undefined;
            if (Enum.TryParse(prop.Parent.Key, out node))
            {
                switch (node)
                {
                    //: predefined summary types from parent
                    case Nodes.Society:
                        return (DataTemplate)element.FindResource("DataTemplateForModifierSummary");
                    case Nodes.VitalStatistics:
                    {
                            if (PropertyFactory.GetTypeOf(prop) == Models.Properties.Modifier)
                                return (DataTemplate)element.FindResource("DataTemplateForSpeedSummary");
                            else break;
                    }
                    case Nodes.Stats: return (DataTemplate)element.FindResource("DataTemplateForStatSummary");
                    case Nodes.Saves:
                    {
                        if(PropertyFactory.GetTypeOf(prop) == Models.Properties.Modifier)
                            return (DataTemplate)element.FindResource("DataTemplateForModifierSummary");
                        else
                            return (DataTemplate)element.FindResource("DataTemplateForSaveSummary");
                    }
                    case Nodes.Skills: return (DataTemplate)element.FindResource("DataTemplateForSkillSummary");
                    case Nodes.CombatStances:
                        {
                            if (prop.Name == "BAB" || prop.Name == "CMB")
                                return (DataTemplate)element.FindResource("DataTemplateForIntegerWithSignSummary");
                            else if (prop.Name == "CMD")
                                return (DataTemplate)element.FindResource("DataTemplateForIntegerSummary");
                            else break;
                        }
                }
            }

            //: self summary types
            switch (PropertyFactory.GetTypeOf(prop))
            {
                case Models.Properties.EnumProperty: return (DataTemplate)element.FindResource("DataTemplateForEnumSummary");
                case Models.Properties.Spell: return (DataTemplate)element.FindResource("DataTemplateForSpellSummary");
                case Models.Properties.SpellLevel: return (DataTemplate)element.FindResource("DataTemplateForSpellLevelSummary");
                case Models.Properties.SpellcastingClass: return (DataTemplate)element.FindResource("DataTemplateForSpellcastingClassSummary");
                case Models.Properties.Ability: return (DataTemplate)element.FindResource("DataTemplateForAbilitySummary");
                case Models.Properties.Modifier: return (DataTemplate)element.FindResource("DataTemplateForModifierSummary");
                case Models.Properties.Attack: return (DataTemplate)element.FindResource("DataTemplateForAttackSummary");
                case Models.Properties.Stance: return (DataTemplate)element.FindResource("DataTemplateForStanceSummary");
                case Models.Properties.Defenses: return (DataTemplate)element.FindResource("DataTemplateForDefensesSummary");
                case Models.Properties.State: return (DataTemplate)element.FindResource("DataTemplateForStateSummary");
            }

            return base.SelectTemplate(item, container);
        }
    }
}

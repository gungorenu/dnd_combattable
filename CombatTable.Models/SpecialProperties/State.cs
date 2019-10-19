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
    public class State : NodeContainer
    {
        private State(XmlElement xel)
            : base( Nodes.State)
        {
        }

        public State()
            : base(Nodes.State)
        {
            NewInteger(Nodes.Duration,1);
            NewBoolean(Nodes.CustomState, false);
            Name = PredefinedStates.Dead.ToString();
            UpdateDescription(Name);
        }

        public IntegerProperty Duration { get { return GetProperty<IntegerProperty>(Nodes.Duration); } }
        public BooleanProperty CustomState {  get { return GetProperty<BooleanProperty>(Nodes.CustomState); } }
        public string StateName
        {
            get
            {
                if (!CustomState.BooleanValue)
                {
                    PredefinedStates s;
                    if (Enum.TryParse(Name, out s))
                        return s.ToString();
                    else
                        return PredefinedStates.Dead.ToString();
                }
                else
                    return Name;
            }
            set
            {
                if (!CustomState.BooleanValue)
                {
                    PredefinedStates s;
                    if (Enum.TryParse(value, out s))
                        Name = value;
                    else
                        Name = PredefinedStates.Dead.ToString();
                }
                else
                    Name = value;
                Trigger(nameof(StateName));
            }
        }

        protected override void OnTrigger(string name)
        {
            if (name == "Name")
                UpdateDescription(Name);
            base.OnTrigger(name);
        }

        protected override void OnTriggerAll()
        {
            UpdateDescription(Name);
            base.OnTriggerAll();
        }

        private void UpdateDescription(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (CustomState == null) return;
            if (CustomState.BooleanValue)
                return;

            PredefinedStates state = (PredefinedStates)Enum.Parse(typeof(PredefinedStates), name);
            switch (state)
            {
                case PredefinedStates.Blinded:
                    Value = "-2 AC, loses Dexterity bonus to AC, moves at half speed, -4 on Search & Strength & Dexterity based skills, cannot use vision related skills, every target have total concealment (%50 miss chance)";
                    break;
                case PredefinedStates.Confused:
                    Value = "% roll, 01-10: attack caster melee/ranged ; 11-20: normal act ; 21-50: babble incoherently ; 51-70: flee from caster full speed ; 71-100: attack nearest";
                    break;
                case PredefinedStates.Cowering:
                    Value = "no action possible, -2 AC and lose Dexterity bonus";
                    break;
                case PredefinedStates.Dazed:
                    Value = "no action possible, no other penalty";
                    break;
                case PredefinedStates.Dazzled:
                    Value = "-1 TH, Search, Spot checks";
                    break;
                case PredefinedStates.Dead:
                    Value = "dead";
                    break;
                case PredefinedStates.Dying:
                    Value = "dying and reduce HP -1 until -10, when it is then dead";
                    break;
                case PredefinedStates.Deafened:
                    Value = "-4 initiative, cannot use Listen, %20 spell miss chance with verbal";
                    break;
                case PredefinedStates.Disabled:
                    Value = "0 HP, can make single move or standard action but then -1 HP";
                    break;
                case PredefinedStates.Entangled:
                    Value = "move half speed, cannot charge, -2 TH and -4 Dexterity, spellcaster must make Concentration DC 15 + SL";
                    break;
                case PredefinedStates.Exhausted:
                    Value = "move half speed, -6 Strength and Dexterity, 1hour rest and becomes fatigue";
                    break;
                case PredefinedStates.Fatigue:
                    Value = "-2 Strength and Dexterity, cannot run or charge";
                    break;
                case PredefinedStates.Flatfooted:
                    Value = "loses Dexterity bonus to AC, cannot make attack of opportunities";
                    break;
                case PredefinedStates.Frightened:
                    Value = "flee from source, -2 TH/save/skill/ability checks, can use spells to escape";
                    break;
                case PredefinedStates.Grappling:
                    Value = "cannot make attack of opportunities, loses Dexterity bonus to AC (only to other than grappling targets)";
                    break;
                case PredefinedStates.Helpless:
                    Value = "cannot make any action, Dexterity is 0, melee attacks have +4 TH, can be target for coup-de-grace, must make Fortitude DC 10 + damage or die";
                    break;
                case PredefinedStates.Invisible:
                    Value = "+2 TH against others and ignores Dexterity bonus to AC";
                    break;
                case PredefinedStates.Nauseated:
                    Value = "can only do move action which is not attack/spellcast/concentration etc...";
                    break;
                case PredefinedStates.Panicked:
                    Value = "drop anything and flee from source, no other action possible, -2 save/skill/ability checks, cannot attack but total defense only if cornered, can use spells to escape";
                    break;
                case PredefinedStates.Paralyzed:
                    Value = "unable to act, Dexterity and Strength is 0 and helpless";
                    break;
                case PredefinedStates.Pinned:
                    Value = "immobile in grappling, not helpless";
                    break;
                case PredefinedStates.Prone:
                    Value = "-4 melee TH and cannot use ranged, prone have +4 AC against ranged but -4 against melee, stand up is move that provokoes attack opportunity";
                    break;
                case PredefinedStates.Shaken:
                    Value = "-2 TH/save/skill/ability checks";
                    break;
                case PredefinedStates.Sickened:
                    Value = "-2 TH/save/skill/weapon damage/ability checks";
                    break;
                case PredefinedStates.Stable:
                    Value = "not anymore dying, negative HP";
                    break;
                case PredefinedStates.Staggered:
                    Value = "can only make move or standard action, no restriction n free or immediate actions";
                    break;
                case PredefinedStates.Stunned:
                    Value = "drop anything held, -2 AC, loses Dexterity bonus to AC";
                    break;
                case PredefinedStates.Unconscious:
                    Value = "knocked out and helpless";
                    break;
            }
        }

    }
}

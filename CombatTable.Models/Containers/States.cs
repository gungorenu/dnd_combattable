/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable.Models
{
    public enum PredefinedStates : int
    {
        [Description("Blinded")]
        Blinded,
        [Description("Confused")]
        Confused,
        [Description("Cowering")]
        Cowering,
        [Description("Dazed")]
        Dazed,
        [Description("Dazzled")]
        Dazzled,
        [Description("Dead")]
        Dead,
        [Description("Dying")]
        Dying,
        [Description("Deafened")]
        Deafened,
        [Description("Disabled")]
        Disabled,
        [Description("Entangled")]
        Entangled,
        [Description("Exhausted")]
        Exhausted,
        [Description("Fatigue")]
        Fatigue,
        [Description("Flatfooted")]
        Flatfooted,
        [Description("Frightened")]
        Frightened,
        [Description("Grappling")]
        Grappling,
        [Description("Helpless")]
        Helpless,
        [Description("Invisible")]
        Invisible,
        [Description("Nauseated")]
        Nauseated,
        [Description("Panicked")]
        Panicked,
        [Description("Paralyzed")]
        Paralyzed,
        [Description("Pinned")]
        Pinned,
        [Description("Prone")]
        Prone,
        [Description("Shaken")]
        Shaken,
        [Description("Sickened")]
        Sickened,
        [Description("Stable")]
        Stable,
        [Description("Staggered")]
        Staggered,
        [Description("Stunned")]
        Stunned,
        [Description("Unconscious")]
        Unconscious
    }

    public class States : NodeContainer
    {
        private States(XmlElement xel)
            : base(Nodes.States)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("CurrentState"); Trigger("StateSummary"); };
        }

        public States()
            : base(Nodes.States)
        {
            _properties.CollectionChanged += (o, e) => { Trigger("CurrentState"); Trigger("StateSummary"); };
        }

        public State NewState()
        {
            State state = new State();
            Add(state);
            state.PropertyChanged += (o, e) => { Trigger("CurrentState"); Trigger("StateSummary"); };
            return state;
        }

        public string CurrentState
        {
            get
            {
                string text = "";
                foreach (var st in FindProperties<State>())
                    text += st.Name + ", ";

                return text.TrimEnd(',', ' ');
            }
        }

        public string StateSummary
        {
            get
            {
                string text = "";
                foreach (var st in FindProperties<State>((s) => !s.CustomState.BooleanValue && s.Duration.IntegerValue < 0))
                    text += string.Format("{0}, ", st.Name);
                foreach (var st in FindProperties<State>((s) => !s.CustomState.BooleanValue && s.Duration.IntegerValue > 0))
                    text += string.Format("{0} [{1} R], ", st.Name, st.Duration.IntegerValue);

                foreach (var st in FindProperties<State>((s) => s.CustomState.BooleanValue && s.Duration.IntegerValue < 0))
                    text += string.Format("{0} [{1}], ", st.Name, st.Value);

                foreach (var st in FindProperties<State>((s) => s.CustomState.BooleanValue && s.Duration.IntegerValue > 0))
                    text += string.Format("{0} [{1}, {2} R], ", st.Name, st.Value, st.Duration.IntegerValue);

                return text.TrimEnd(',', ' ');
            }
        }
    }
}

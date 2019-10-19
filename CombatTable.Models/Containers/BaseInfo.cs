/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Description = System.ComponentModel.DescriptionAttribute;
using System.Xml;

namespace CombatTable.Models
{
    public enum Alignments
    {   
        [Description("Lawful Good")]
        LawfulGood,
        [Description("Neutral Good")]
        NeutralGood,
        [Description("Chaotic Good")]
        ChaoticGood,
        [Description("Lawful Neutral")]
        LawfulNeutral,
        [Description("Neutral")]
        Neutral,
        [Description("Chaotic Neutral")]
        ChaoticNeutral,
        [Description("Lawful Evil")]
        LawfulEvil,
        [Description("Neutral Evil")]
        NeutralEvil,
        [Description("Chaotic Evil")]
        ChaoticEvil
    }

    public enum Genders : int
    {
        [Description("Female")]
        Female = 0,
        [Description("Male")]
        Male = 1
    }


    public class BaseInfo : NodeContainer
    {
        private BaseInfo(XmlElement xel)
            : base(Nodes.BaseInfo)
        {
        }

        public BaseInfo()
            : base(Nodes.BaseInfo)
        {
            NewBoolean(Nodes.Player, false);
            NewEnum(Nodes.Gender, "Genders", Genders.Female.ToString());
            NewString(Nodes.Race, "<<Race>>");
            NewString(Nodes.Size, "Medium");
            NewString(Nodes.Classes, "<<Classes>>");
            NewInteger(Nodes.HitPoints, 0);
            NewEnum(Nodes.Alignment, "Alignments", Alignments.Neutral.ToString());
            NewInteger(Nodes.LevelAdjustment, 0);
        }

        public SingleProperty Classes { get { return GetProperty<SingleProperty>(Nodes.Classes); } }
        public EnumProperty Alignment { get { return GetProperty<EnumProperty>(Nodes.Alignment); } }
        public EnumProperty Gender{ get { return GetProperty<EnumProperty>(Nodes.Gender); } }
        public SingleProperty Size { get { return GetProperty<SingleProperty>(Nodes.Size); } }
        public IntegerProperty HitPoints { get { return GetProperty<IntegerProperty>(Nodes.HitPoints); } }
        public IntegerProperty LevelAdjustment { get { return GetProperty<IntegerProperty>(Nodes.LevelAdjustment); } }
        public BooleanProperty IsPlayer { get { return GetProperty<BooleanProperty>(Nodes.Player); } }
        public SingleProperty Race { get { return GetProperty<SingleProperty>(Nodes.Race); } }
    }
}

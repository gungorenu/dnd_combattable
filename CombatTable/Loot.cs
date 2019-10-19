/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatTable
{

    public enum TreasureType
    {
        Weapon,
        Armor,
        Shield,
        Jewelry,
        Trinket,
        Gold
    }

    public class Loot : IComparable<Loot>
    {
        private int m_Tier;
        public TreasureType Type { get; set; }
        public int Tier { get { return m_Tier; } set { m_Tier = value; if (m_Tier > 20) m_Tier = 20; } }
        public bool SpecialMaterial { get; set; }
        public bool Enchanted { get; set; }

        public string FullName { get; set; }
        public string Description { get; set; }
        public string Slot { get; set; }
        public string Cost { get; set; }

        public override string ToString()
        {
            string text = "";
            switch (Type)
            {
                case TreasureType.Gold:
                    return Description + " gold";
                case TreasureType.Armor:
                    text += "[A] ";
                    break;
                case TreasureType.Jewelry:
                    text += "[J] ";
                    break;
                case TreasureType.Shield:
                    text += "[S] ";
                    break;
                case TreasureType.Trinket:
                    text += "[T] ";
                    break;
                case TreasureType.Weapon:
                    text += "[W] ";
                    break;
                default: throw new InvalidOperationException("Unknown treasure type");
            }

            if (Type == TreasureType.Weapon || Type == TreasureType.Armor || Type == TreasureType.Shield)
            {
                text += FullName;
                return text;
            }
            else if (Type == TreasureType.Jewelry)
            {
                return string.Format("{0}{1} {{T{2}.{3}}} : {4}", text, FullName, Tier, Cost, Description);
            }
            else
            {
                return string.Format("{0}{1} {{T{2}.{3}}} [{4}]: {5} ", text, FullName, Tier, Cost, Slot, Description);
            }
        }

        public int CompareTo(Loot other)
        {
            return other.Tier.CompareTo(this.Tier);
        }
    }
}

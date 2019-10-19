/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatSimulator.RollEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable
{
    /* Pattern:
    TIER X shall be rolled. if X < 5 then single roll, if 4 < X < 9 then two rolls, if 8 < X < 13 then three rolls and so on [(X+3)/4 -1, min 1]
    TIER X can try roll X, X-4, X-8 and so on rolls as long as treasure tier is larger than 4 (TIER 13 can roll T#13, T#9, T#5). If X is 3 or lower then no treasure.
    T#4 can happen T#4/T#3/T#2/T#1. pattern is T#X can roll T#X with %1, T#X-1 with %9, T#X-2 with %15. T#X-3 with %25 and rest (%50) is none
    If X can roll T#X then T#X-4 shall have no modifier on the percentage. If 'NextTierModifier' is non-zero then the next TIER roll shall have the increment 'NextTierModifier' on the percentages (T#X-4 shall have %11 and so on). this value is cumulative and also applies to secondary rolls for NPCs
    If X is 12 then T#12, T#8 and T#4 can be rolled. If T#11 rolled then T#8 roll shall have 'NextTierModifier' bonus multiplied by 1 (12-11). if T#8 rolls 5 then T#4 shall have 'NextTierModifier' bonus multiplied by 1+3 (12-11 + 8-5). No treasure roll shall make bonus multiply by 4
    If 'HighestRollSet' is set then the T#X roll cannot have empty roll (set to T#X-3 for %50 so its probability becomes %75)
    If 'NPCTreasure' is set then treasure output shall be double (two T#X roll, two T#X-4 roll and so on). gold die shall be trippled
    */
    internal class LootGenerator
    {
        private XmlDocument m_TreasureTable = null;
        private ExecutionEngine m_RollerEngine;

        public LootGenerator(ExecutionEngine roller)
        {
            m_RollerEngine = roller;
        }

        private XmlDocument TreasureTable
        {
            get
            {
                if (m_TreasureTable != null) return m_TreasureTable;
                m_TreasureTable = new XmlDocument();
                m_TreasureTable.Load(TreasureXmlPath);
                return m_TreasureTable;
            }
        }

        private string TreasureXmlPath
        {
            get
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                path = path.Substring(0, path.LastIndexOf('\\') + 1);
                return path + "treasure.xml";
            }
        }


        /// <summary>
        /// Next tier bonus modifier per tier difference
        /// </summary>
        public int NextTierModifier { get; set; }

        /// <summary>
        /// Subscribe for treasure loot rolls
        /// </summary>
        public event VerboseRollEvent VerboseLog;

        /// <summary>
        /// Entry point, rolls treasure and outputs by Verbose log
        /// </summary>
        /// <param name="tier">Tier as number</param>
        public IEnumerable<Loot> RollTreasure(int tier, bool isNPC, bool explicitTreasure, bool onlySpellcaster)
        {
            // get tiers to roll treasure
            List<int> tierListToRoll = new List<int>();
            foreach (int subtier in GetTreasureTiers(tier, isNPC))
                if (subtier != 0) tierListToRoll.Add(subtier);

            // we have what to roll now but we have not rolled anything yet

            // roll tiers so we shall have what to roll for treasure
            List<int> tiersOfTreasure = new List<int>();
            int maxtier = tierListToRoll.Max();
            Dictionary<int, int> bonuses = new Dictionary<int, int>();
            if (tierListToRoll.Count > 0)
            {
                foreach (int subtier in tierListToRoll)
                {
                    // sum bonuses for the next tier, same tier can also bonus in case of NPCs
                    int bonus = bonuses.Where((t) => t.Key >= subtier).Aggregate(0, (s, item) => s + item.Value);

                    // we get treasure tier rolled here, we shall roll treasure itself later if it is non-zero
                    int treasureTier = GetFinalTierOfSingleTreasureTier(subtier, explicitTreasure && maxtier == subtier, bonus);
                    if (treasureTier != 0) tiersOfTreasure.Add(treasureTier);
                    Verbose($"Treasure TIER {subtier} got " + (treasureTier == 0 ? "<NO TIER> !" : $"random TIER {treasureTier} !"));

                    // add bonus for treasure for the special subtier
                    if (treasureTier == 0) // no roll? set max bonus
                        bonuses[subtier] = 4 * NextTierModifier;
                    // no bonus set before or bonus was lower? update
                    else if (!bonuses.ContainsKey(subtier) || bonuses[subtier] < (subtier - treasureTier) * NextTierModifier)
                        bonuses[subtier] = (subtier - treasureTier) * NextTierModifier;
                }
            }

            List<Loot> loots = new List<Loot>();

            // at this point we have defined what tiers of treasure we shall roll, next step is to find the treasure
            for (int i = 0; i < tiersOfTreasure.Count; i++)
            {
                Verbose(String.Format("Treasure Tier {0}, type check", tiersOfTreasure[i]) + "\n");
                int tierType = RollDie(100);
                Loot loot = new Loot();
                loot.Tier = tiersOfTreasure[i];
                if ((onlySpellcaster && tierType < 6) ||
                    (!onlySpellcaster && tierType < 31))
                {
                    if (RollWeapon(loot.Tier, loot))
                        loots.Add(loot);
                }
                else if ((onlySpellcaster && tierType < 11) ||
                    (!onlySpellcaster && tierType < 46))
                {
                    if (RollArmor(loot.Tier, loot))
                        loots.Add(loot);
                }
                else if ((onlySpellcaster && tierType < 16) ||
                    (!onlySpellcaster && tierType < 51))
                {
                    if (RollShield(loot.Tier, loot))
                        loots.Add(loot);
                }
                else if ((onlySpellcaster && tierType < 66) ||
                    (!onlySpellcaster && tierType < 86))
                {
                    loot.Type = TreasureType.Trinket;
                    FinalizeLoot(loot);
                    if (!string.IsNullOrEmpty(loot.FullName))
                        loots.Add(loot);
                    else
                    {
                        Verbose(String.Format("Treasure Tier {0}, trinket not found at tier (spellcasting only?), skipping...", loot.Tier) + "\n");
                    }
                }
                else
                {
                    if (loot.Tier < 3)
                    {
                        Verbose(String.Format("Treasure Tier {0}, basic jewelry, skipping...", loot.Tier) + "\n");
                        continue;
                    }

                    loot.Type = TreasureType.Jewelry;
                    FinalizeLoot(loot);
                    if (!string.IsNullOrEmpty(loot.FullName))
                        loots.Add(loot);
                    else
                    {
                        Verbose(String.Format("Treasure Tier {0}, jewelry not found at tier (spellcasting only?), skipping...", loot.Tier) + "\n");
                    }
                }
            }

            // roll gold, final step
            Loot gold = new Loot() { Type = TreasureType.Gold };
            gold.Description = m_RollerEngine.ExecuteExpression(GetGoldRoll(tier, isNPC)).ToString();
            Verbose( "gold roll: " + gold.Description + "\r\n");
            loots.Add(gold);

            return loots;
        }

        /// <summary>
        /// This function shall return the tier of treasure or 0 for the single tier provided depending on current modifier bonus
        /// </summary>
        /// <param name="tier">Meaningful tier value</param>
        /// <returns>Tier of treasure or 0 for no treasure roll</returns>
        /// <remarks>Not starter</remarks>
        private int GetFinalTierOfSingleTreasureTier(int tier, bool explicitTreasure, int bonus = 0)
        {
            int value = m_RollerEngine.ExecuteExpression("d100");
            Verbose($"Tier {tier} rolled {value} plus bonus {bonus}, explicit treasure is '{explicitTreasure}'");
            value += bonus;

            if (value > 99)
                return tier;
            else if (value > 90)
                return tier - 1;
            else if (value > 75)
                return tier - 2;
            else if (value > 50)
                return tier - 3;
            else if (explicitTreasure)
                return tier - 3;

            return 0;
        }

        private IEnumerable<int> GetTreasureTiers(int tier, bool isNPC)
        {
            if (tier < 4)
                yield break;

            for (int i = tier; i >= 4; i -= 4)
            {
                // first roll
                yield return i;

                // NPC bonus roll
                if (isNPC)
                    yield return i;
            }
        }

        private string GetGoldRoll(int tier, bool isNPC)
        {
            int roll = tier * (isNPC ? 3 : 1);

            switch (tier)
            {
                case 1: 
                case 2: 
                case 3: return string.Format($"{roll}d50");
                case 4:
                case 5: return string.Format($"{roll}d60");
                case 6:
                case 7: return string.Format($"{roll}d70");
                case 8: return string.Format($"{roll}d80");
                case 9: return string.Format($"{roll}d90");
                case 10: return string.Format($"{roll}d100");
                case 11: return string.Format($"{roll}d125");
                case 12: return string.Format($"{roll}d150");
                case 13: return string.Format($"{roll}d200");
                case 14: return string.Format($"{roll}d250");
                case 15: return string.Format($"{roll}d350");
                case 16: return string.Format($"{roll}d500");
                case 17: return string.Format($"{roll}d750");
                default: return string.Format($"{roll}d" + 500 * (tier - 16));
            }
        }

        private void Verbose(string text, params object[] args)
        {
            VerboseLog?.Invoke(string.Format(text + "\r\n", args));
        }

        private int RollDie(int num)
        {
            return m_RollerEngine.ExecuteExpression("d" + num);
        }

        private bool RollWeapon(int tier, Loot loot)
        {
            if (tier < 3)
            {
                Verbose(String.Format("Treasure Tier {0}, basic weapon, skipping...", tier) + "\n");
                return false;
            }
            
            loot.Type = TreasureType.Weapon;

            int enchantment = 0, enchantedLimit = 0;
            EnchantmentCalculator(tier, out enchantedLimit, out enchantment);
            
            int temp = RollDie(100);
            if (temp > enchantedLimit)
                loot.Enchanted = true;

            temp = RollDie(100);
            if (temp > 90)
                loot.SpecialMaterial = true;

            FinalizeLoot(loot);
            return true;
        }

        private bool RollArmor(int tier, Loot loot)
        {
            if (tier < 3)
            {
                Verbose(String.Format("Treasure Tier {0}, basic armor, skipping...", tier) + "\n");
                return false;
            }
            
            loot.Type = TreasureType.Armor;

            int enchantment = 0, enchantedLimit = 0;
            EnchantmentCalculator(tier, out enchantedLimit, out enchantment);

            int temp = RollDie(100);
            if (temp > enchantedLimit)
                loot.Enchanted = true;

            temp = RollDie(100);
            if (temp > 90)
                loot.SpecialMaterial = true;

            FinalizeLoot(loot);
            return true;
        }

        private bool RollShield(int tier, Loot loot)
        {
            if (tier < 3)
            {
                Verbose(String.Format("Treasure Tier {0}, basic shield, skipping...", tier) + "\n");
                return false;
            }
            
            loot.Type = TreasureType.Shield;
            int enchantment = 0, enchantedLimit = 0;
            EnchantmentCalculator(tier, out enchantedLimit, out enchantment);

            int temp = RollDie(100);
            if (temp > enchantedLimit)
                loot.Enchanted = true;

            temp = RollDie(100);
            if (temp > 90)
                loot.SpecialMaterial = true;

            FinalizeLoot(loot);
            return true;
        }

        private void FinalizeLoot(Loot loot, bool onlySpellcaster = false)
        {
            switch (loot.Type)
            {
                case TreasureType.Armor:
                    FinalizeArmor(loot);
                    break;
                case TreasureType.Jewelry:
                    FinalizeJewelry(loot, onlySpellcaster);
                    break;
                case TreasureType.Shield:
                    FinalizeShield(loot);
                    break;
                case TreasureType.Trinket:
                    FinalizeWondrousItem(loot, onlySpellcaster);
                    break;
                case TreasureType.Weapon:
                    FinalizeWeapon(loot);
                    break;
            }
        }

        private void FinalizeWeapon(Loot loot)
        {
            int weaponTypeRoll = RollDie(8);
            string weaponType = "simple";
            if (weaponTypeRoll == 8)
                weaponType = "racial";
            else if (weaponTypeRoll == 7)
                weaponType = "exotic";
            else if (weaponTypeRoll > 2)
                weaponType = "martial";

            XmlNodeList candidates = TreasureTable.SelectNodes(string.Format("/treasure/weapons/weapon[@type='{0}']", weaponType));
            XmlNode weapon = RollRandomLoot(candidates);

            int enchantLimit = 0, enchantment = 0;
            EnchantmentCalculator(loot.Tier, out enchantLimit, out enchantment);

            string material = RollMaterial(weapon, loot);
            string enchanted = loot.Enchanted ? "Enchanted " : "";

            loot.FullName = string.Format("+{0} {1}{2}{3}", enchantment, enchanted, material, weapon.InnerText.Trim());
        }
        private void FinalizeArmor(Loot loot)
        {
            XmlNodeList candidates = TreasureTable.SelectNodes(string.Format("/treasure/armors/armor"));
            XmlNode armor = RollRandomLoot(candidates);

            int enchantLimit = 0, enchantment = 0;
            EnchantmentCalculator(loot.Tier, out enchantLimit, out enchantment);

            string material = RollMaterial(armor, loot);
            string enchanted = loot.Enchanted ? "Enchanted " : "";

            loot.FullName = string.Format("+{0} {1}{2}{3}", enchantment, enchanted, material, armor.InnerText.Trim());
        }
        private void FinalizeShield(Loot loot)
        {
            XmlNodeList candidates = TreasureTable.SelectNodes(string.Format("/treasure/shields/shield"));
            XmlNode shield = RollRandomLoot(candidates);

            int enchantLimit = 0, enchantment = 0;
            EnchantmentCalculator(loot.Tier, out enchantLimit, out enchantment);

            string material = RollMaterial(shield, loot);
            string enchanted = loot.Enchanted ? "Enchanted " : "";

            loot.FullName = string.Format("+{0} {1}{2}{3}", enchantment, enchanted, material, shield.InnerText.Trim());
        }
        private void FinalizeJewelry(Loot loot, bool onlySpellcaster)
        {
            string query = string.Format("/treasure/jewelry/jewel[@tier={0}]", loot.Tier > 20 ? 20 : loot.Tier);
            if (onlySpellcaster)
                query = string.Format("/treasure/jewelry/jewel[@tier={0} and @flags='S']", loot.Tier >20? 20 : loot.Tier);

            XmlNodeList candidates = TreasureTable.SelectNodes(query);
            XmlNode jewelry = RollRandomLoot(candidates);

            if (jewelry == null)
                return;

            loot.FullName = jewelry.FirstChild.InnerText.Trim();
            loot.Description = jewelry.SelectSingleNode("description").InnerText.Trim();
            loot.Cost = jewelry.Attributes["cost"].Value;
        }
        private void FinalizeWondrousItem(Loot loot, bool onlySpellcaster)
        {
            string query = string.Format("/treasure/trinkets/trinket[@tier={0}]", loot.Tier > 20 ? 20 : loot.Tier);
            if (onlySpellcaster)
                query = string.Format("/treasure/trinkets/trinket[@tier={0} and @flags='S']", loot.Tier > 20 ? 20 : loot.Tier);

            XmlNodeList candidates = TreasureTable.SelectNodes(query);
            XmlNode wondrousItem = RollRandomLoot(candidates);
            if (wondrousItem == null)
                return;

            loot.FullName = wondrousItem.FirstChild.InnerText.Trim();
            loot.Description = wondrousItem.SelectSingleNode("description").InnerText.Trim();
            loot.Cost = wondrousItem.Attributes["cost"].Value;
            switch (wondrousItem.Attributes["slot"].Value)
            {
                case "L":
                    loot.Slot = "waist";
                    break;
                case "B":
                    loot.Slot = "feet";
                    break;
                case "C":
                    loot.Slot = "back";
                    break;
                case "A":
                    loot.Slot = "torso";
                    break;
                case "G":
                    loot.Slot = "hands";
                    break;
                case "H":
                    loot.Slot = "head";
                    break;
                case "T":
                    loot.Slot = "trinket";
                    break;
            }
        }

        private string RollMaterial(XmlNode node, Loot loot)
        {
            if (!loot.SpecialMaterial)
                return "";
            if (node.Attributes["material"] != null)
            {
                string[] materials = node.Attributes["material"].Value.Split(',');

                int result = RollDie(materials.Length);
                return materials[result - 1].Trim() + " ";
            }

            return "";
        }

        private XmlNode RollRandomLoot(XmlNodeList nodes)
        {
            if (nodes.Count == 0)
                return null;
            int count = nodes.Count;
            int result = RollDie(count);
            return nodes[result - 1];
        }

        private void EnchantmentCalculator(int tier, out int enchantedLimit, out int enchantment)
        {
            enchantedLimit = 101;
            enchantment = 0;
            if (tier < 0)
                return;

            switch (tier)
            {
                case 1:
                    enchantedLimit = 101;
                    enchantment = 0;
                    break;
                case 2:
                case 3:
                    enchantedLimit = 101;
                    enchantment = 1;
                    break;
                case 4:
                case 5:
                case 6:
                    enchantedLimit = 101;
                    enchantment = 2;
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    enchantedLimit = 101;
                    enchantment = 3;
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    enchantedLimit = 101;
                    enchantment = 4;
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    enchantedLimit = 101;
                    enchantment = 5;
                    break;
                case 20:
                default:
                    enchantedLimit = 101;
                    enchantment = 6;

                    break;
            }
        }

    }
}

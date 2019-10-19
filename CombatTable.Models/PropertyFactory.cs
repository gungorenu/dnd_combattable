/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CombatTable.Models
{
    public static class PropertyFactory
    {
        private static Dictionary<Properties, Type> _knownTypes;
            
        static PropertyFactory()
        {
            _knownTypes = new Dictionary<Properties, Type>();

            _knownTypes.Add(Properties.SingleProperty, typeof(SingleProperty));
            _knownTypes.Add(Properties.IntegerProperty, typeof(IntegerProperty));
            _knownTypes.Add(Properties.BooleanProperty, typeof(BooleanProperty));
            _knownTypes.Add(Properties.EnumProperty, typeof(EnumProperty));
            _knownTypes.Add(Properties.Ability, typeof(Ability));
            _knownTypes.Add(Properties.NodeContainer, typeof(NodeContainer));
            _knownTypes.Add(Properties.VitalStatistics, typeof(VitalStatistics));
            _knownTypes.Add(Properties.Stats, typeof(Stats));
            _knownTypes.Add(Properties.Stance, typeof(Stance));
            _knownTypes.Add(Properties.SpellLevel, typeof(SpellLevel));
            _knownTypes.Add(Properties.SpellcastingClass, typeof(SpellcastingClass));
            _knownTypes.Add(Properties.Spellcasting, typeof(Spellcasting));
            _knownTypes.Add(Properties.Society, typeof(Society));
            _knownTypes.Add(Properties.Skills, typeof(Skills));
            _knownTypes.Add(Properties.Saves, typeof(Saves));
            _knownTypes.Add(Properties.Item, typeof(Item));
            _knownTypes.Add(Properties.Gear, typeof(Gear));
            _knownTypes.Add(Properties.Defenses, typeof(Defenses));
            _knownTypes.Add(Properties.CombatStances, typeof(CombatStances));
            _knownTypes.Add(Properties.BaseInfo, typeof(BaseInfo));
            _knownTypes.Add(Properties.Attack, typeof(Attack));
            _knownTypes.Add(Properties.SessionManager, typeof(SessionManager));
            _knownTypes.Add(Properties.Spell, typeof(Spell));
            _knownTypes.Add(Properties.Modifier, typeof(Modifier));
            _knownTypes.Add(Properties.Skill, typeof(Skill));
            _knownTypes.Add(Properties.State, typeof(State));
            _knownTypes.Add(Properties.States, typeof(States));
            _knownTypes.Add(Properties.Session, typeof(Session));
        }

        public static Property CreateProperty( Properties propertyType)
        {
            Type type = _knownTypes[propertyType];
            System.Reflection.ConstructorInfo ci = null;
            object propertyInstance = null;
            try
            {
                ci = type.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance, null, new Type[] { typeof(XmlElement) }, null);
                propertyInstance = ci.Invoke(new object[] { null });
                return propertyInstance as Property;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static P CreateProperty<P>(Properties propertyType)
            where P: Property
        {
            return CreateProperty(propertyType) as P;
        }

        public static Properties ConvertToPropertyType( string name)
        {
            Properties val = Properties.Undefined;
            if (Enum.TryParse(name, out val))
                return val;
            return Properties.Undefined;
        }

        public static Properties GetTypeOf(Property prop)
        {
            return _knownTypes.FirstOrDefault((kv) => prop.GetType().Equals(kv.Value)).Key;

        }

    }
}

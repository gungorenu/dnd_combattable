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
    public class Session : NodeContainer
    {
        private Session(XmlElement xel)
            : base( Nodes.Session)
        {
            
        }

        public Session()
            : base(Nodes.Session)
        {
            NewInteger(Nodes.CurrentHitPoints);
            NewInteger(Nodes.CurrentInitiative, -1);
            NewBoolean(Nodes.CurrentPlayer);
            NewInteger(Nodes.CurrentCoordinate_X);
            NewInteger(Nodes.CurrentCoordinate_Y);
        }

        public IntegerProperty CurrentHitPoints {  get { return GetProperty<IntegerProperty>(Nodes.CurrentHitPoints); } }
        public IntegerProperty CurrentInitiative {  get { return GetProperty<IntegerProperty>(Nodes.CurrentInitiative); } }
        public IntegerProperty CurrentCoordinate_X {  get { return GetProperty<IntegerProperty>(Nodes.CurrentCoordinate_X); } }
        public IntegerProperty CurrentCoordinate_Y {  get { return GetProperty<IntegerProperty>(Nodes.CurrentCoordinate_Y); } }
        public BooleanProperty CurrentPlayer {  get { return GetProperty<BooleanProperty>(Nodes.CurrentPlayer); } }


    }
}

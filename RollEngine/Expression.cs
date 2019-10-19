/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatSimulator.RollEngine
{
    /// <summary>
    /// Operation nods, represents an operation which depends on the operator and arguments, existence of this node means it is not calculated yet
    /// </summary>
    internal class Expression : Node
    {
        private int nodeCounter = 0;
        public Expression()
        {
            Nodes = new Dictionary<int, Node>();
        }

        public string Equation { get; set; }
        public Dictionary<int, Node> Nodes { get; private set; }
        public Expression Parent { get; set; }

        /// <summary>
        /// Calculates next unique id for the entire expression
        /// </summary>
        /// <returns>Unique identifier</returns>
        public int GetNextNodeId()
        {
            if (Parent != null)
                return Parent.GetNextNodeId();
            return ++nodeCounter;
        }

        override public string ToString()
        {
            if (Nodes.Count == 0)
                return Equation;

            string value = Equation;
            foreach (int key in Nodes.Keys)
            {
                value = value.Replace($"{{{key}}}", Nodes[key].ToString());
            }

            return " <" + value + "> ";
        }
    }
}

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
    /// Constant value node, represents a number
    /// </summary>
    internal class Token : Node
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return " T:" + Value + " ";
        }
    }
}

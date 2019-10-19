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
    /// Basic node that can happen inside an expression
    /// </summary>
    public abstract class Node
    {
        public int Id { get; set; }
    }
}

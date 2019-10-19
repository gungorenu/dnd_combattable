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
    /// Exception that happens during parsing the code
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Exception that happens during the execution
    /// </summary>
    public class ExecutionException : Exception
    {
        public ExecutionException(string msg) : base(msg) { }
    }
}

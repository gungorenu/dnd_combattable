/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

namespace CombatSimulator.RollEngine
{
    /// <summary>
    /// Basic supported operators, Add and Subtract are special cases and they are not considered as operators actually
    /// </summary>
    public enum Operators
    {
        Add = 0,
        Subtract,
        Multiply,
        Divide,
        Roll,
        Remainder
    }
}
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
    /// Expression execution engine
    /// </summary>
    public class ExecutionEngine
    {
        private static Random rand = new Random();
        private Parser parser = new Parser();

        /// <summary>
        /// Executes given code and returns the result
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int ExecuteExpression(string code)
        {
            Expression expr = parser.ParseCode(code);

            Token result = ExecuteExpression(expr);

            return result.Value;
        }

        #region Private Members
        /// <summary>
        /// Entry point for the expression
        /// </summary>
        /// <returns>Result token back about the calculation</returns>
        private Token ExecuteExpression(Expression expr)
        {
            // calculate results from all children
            foreach (Expression child in expr.Nodes.Values.Where((n) => n is Expression).ToArray())
            {
                // a result is always a token
                Token result = ExecuteExpression(child);
                if (result.Id != child.Id)
                    throw new ExecutionException("Expression and its result must have same id");
                // replace the tokens
                expr.Nodes.Remove(child.Id);
                expr.Nodes.Add(result.Id, result);
            }

            // at this point all of the nodes must be regular tokens and they can be added easily
            if (expr.Nodes.Values.All((n) => n is Token))
                return CalculateExpression(expr);
            else
                throw new ExecutionException("Execution engine could not execute child expressions properly");
        }

        /// <summary>
        /// Finds the first operator from the given code
        /// </summary>
        private Operators GetOperator(string code, ref char opChar)
        {
            if (code.IndexOf(Parser.cRoll) >= 0)
            {
                opChar = Parser.cRoll;
                return Operators.Roll;
            }
            if (code.IndexOf(Parser.cMultiply) >= 0)
            {
                opChar = Parser.cMultiply;
                return Operators.Multiply;
            }
            if (code.IndexOf(Parser.cDivide) >= 0)
            {
                opChar = Parser.cDivide;
                return Operators.Divide;
            }
            if (code.IndexOf(Parser.cRemainder) >= 0)
            {
                opChar = Parser.cRemainder;
                return Operators.Remainder;
            }

            // above is important only, others can be executed inline
            return Operators.Add;
        }

        /// <summary>
        /// Calculates the value of the current expression, assumption is the expression has no child expression and every node on expression is a token
        /// </summary>
        /// <returns>Result of calculation as a token</returns>
        private Token CalculateExpression(Expression expr)
        {
            Token result = null;
            // no node? parse problem
            if (expr.Nodes.Count == 0)
                throw new ExecutionException("An expression must have at least one node");
            // has single node?
            if (expr.Nodes.Count == 1)
            {
                // single node must be token
                if (!(expr.Nodes.Values.First() is Token))
                    throw new ExecutionException("Expression has single node but it is not a token");

                // create new token with the existing token's value but same id with expression
                result = new Token() { Value = (expr.Nodes.Values.First() as Token).Value, Id = expr.Id };
                return result;
            }

            // calculate complex operators, they are truly binary (has 2 token) and a single operator
            char opChar = ' ';
            Operators op = GetOperator(expr.Equation, ref opChar);
            Token left, right;
            switch (op)
            {
                case Operators.Roll:
                case Operators.Multiply:
                case Operators.Divide:
                case Operators.Remainder:
                    GetBinaryOperatorTokens(expr, opChar, out left, out right);
                    if (left == null || right == null)
                        throw new ExecutionException("Left or right token came null from binary operator");
                    result = ExecuteBinaryOperator(op, left, right);
                    result.Id = expr.Id;
                    return result;
                default:
                    break;
            }

            // remaining tokens shall be "added" based on sign/operator
            string code = expr.Equation;
            int sign;
            result = new Token() { Value = 0, Id = expr.Id };
            do
            {
                if (code[0] == Parser.cSubtract)
                    sign = -1;
                else
                    sign = 1;

                int tokenStart = code.IndexOf('{');
                int tokenEnd = code.IndexOf('}');
                string tokenId = code.Substring(tokenStart + 1, tokenEnd - tokenStart - 1);
                result.Value += sign * (expr.Nodes[Convert.ToInt32(tokenId)] as Token).Value;

                if (tokenEnd == code.Length - 1)
                    break;
                else
                    code = code.Substring(tokenEnd + 1);
            } while (!String.IsNullOrEmpty(code));

            return result;
        }

        /// <summary>
        /// Executes a binary operator using the left and right arguments of the operator
        /// </summary>
        /// <returns>Result of operation as a token</returns>
        private Token ExecuteBinaryOperator(Operators op, Token left, Token right)
        {
            switch (op)
            {
                case Operators.Roll:
                    if (right.Value <= 0)
                        throw new ExecutionException("Roll a 0 or negative dice");
                    return new Token() { Value = RollNumber(left.Value, right.Value) };
                case Operators.Multiply:
                    return new Token() { Value = left.Value * right.Value };
                case Operators.Divide:
                    if (right.Value == 0)
                        throw new ExecutionException("Divide by 0");
                    return new Token() { Value = left.Value / right.Value };
                case Operators.Remainder:
                    if (right.Value == 0)
                        throw new ExecutionException("Divide by 0");
                    return new Token() { Value = left.Value % right.Value };
                default:
                    throw new ExecutionException("Binary operator must be complex to be executed");
            }
        }

        /// <summary>
        /// Rolling a dice
        /// </summary>
        /// <param name="n1">Count of dice</param>
        /// <param name="n2">Size of dice</param>
        /// <returns>Total rolled value</returns>
        private int RollNumber(int n1, int n2)
        {
            int result = 0;
            int dieCount = n1;
            if (dieCount < 0) dieCount *= -1;

            List<double> randomNumbers = new List<double>();
            List<int> randomNumberInts = new List<int>();
            for (int k = 0; k < dieCount; k++)
            {
                double randomDouble = rand.NextDouble();
                randomNumbers.Add(randomDouble);

                int randomInt = (int)(randomDouble * n2) + 1;
                randomNumberInts.Add(randomInt);

                result += randomInt;
            }

            Log("Roll " + dieCount + "d" + n2 + " : " + result);
            if (VerboseNotification)
            {
                for (int i = 0; i < randomNumbers.Count; i++)
                    Log(" d" + n2 + " : " + randomNumberInts[i] + " (" + randomNumbers[i] + ")");
                Log(" ");
            }
            else
            {
                string logStr = " d" + n2 + " : ";
                for (int i = 0; i < randomNumberInts.Count; i++)
                {
                    logStr += randomNumberInts[i].ToString();
                    if (i != randomNumberInts.Count - 1) logStr += " , ";
                }
                Log(logStr);
                Log(" ");
            }

            return n1 < 0 ? result * -1 : result;
        }

        /// <summary>
        /// From the current expression parses the (first) found complex operator [which is supposed to be unique] and its arguments
        /// </summary>
        private void GetBinaryOperatorTokens(Expression expr, char op, out Token left, out Token right)
        {
            // get the index of operator
            int index = expr.Equation.IndexOf(op);

            // find token starts and ends
            int leftSideTokenStart = parser.FindPreviousIndexOf(expr.Equation, '{', index);
            int leftSideTokenEnd = index - 1;
            int rightSideTokenStart = index + 1;
            int rightSideTokenEnd = parser.FindNextIndexOf(expr.Equation, '}', index);

            // parse token ids
            string leftToken = expr.Equation.Substring(leftSideTokenStart + 1, leftSideTokenEnd - leftSideTokenStart - 1);
            string rightToken = expr.Equation.Substring(rightSideTokenStart + 1, rightSideTokenEnd - rightSideTokenStart - 1);
            int leftTokenId = Convert.ToInt32(leftToken);
            int rightTokenId = Convert.ToInt32(rightToken);

            left = expr.Nodes[leftTokenId] as Token;
            right = expr.Nodes[rightTokenId] as Token;
        }
        #endregion

        #region Logging Event & Flag
        private void Log(string text)
        {
            if (VerboseRollEventHandler != null) VerboseRollEventHandler(text);
        }

        /// <summary>
        /// Verbose notification flag, if set then random double values are logged too
        /// </summary>
        public bool VerboseNotification { get; set; }

        /// <summary>
        /// Event handler for verbose notification
        /// </summary>
        public event VerboseRollEvent VerboseRollEventHandler;

        #endregion
    }
}

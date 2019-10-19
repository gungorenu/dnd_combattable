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
    /// Parser class, only responsible for creating the expression tree
    /// </summary>
    internal class Parser
    {
        #region Constants
        private readonly string cContainerStarts = "([";
        private readonly string cContainerEnds = ")]";
        private readonly string cAcceptableCharacters = "0123456789/*-+%d()[]";

        internal const char cRoll = 'd';
        internal const char cAdd = '+';
        internal const char cSubtract = '-';
        internal const char cMultiply = '*';
        internal const char cDivide = '/';
        internal const char cRemainder = '%';
        #endregion

        #region Private Members
        /// <summary>
        /// Validate the given code, throws exception if finds problem. due to nature of code cannot fully check the expression
        /// </summary>
        private void PreValidate(string code)
        {
            // forbidden characters
            foreach (char c in code)
                if (cAcceptableCharacters.IndexOf(c) < 0)
                    throw new ParseException("Expression contains invalid characters");

            // count container open-close
            for (int i = 0; i < cContainerStarts.Length; i++)
                if (code.Count((c) => c == cContainerStarts[i]) !=
                    code.Count((c) => c == cContainerEnds[i]))
                    throw new ParseException("Containers in expression do not match open-close counts");

            // check invalid nested containers
            Stack<char> containers = new Stack<char>();
            for (int i = 0, j; i < code.Length; i++)
            {
                char c = code[i];
                // found end, pop start
                if ((j = cContainerEnds.IndexOf(c)) >= 0)
                {
                    if (containers.Count == 0)
                        throw new ParseException("Containers end has no start");

                    if (cContainerStarts.IndexOf(containers.Peek()) != j)
                        throw new ParseException("Containers end and start are not matching");

                    containers.Pop();
                }
                else if (cContainerStarts.IndexOf(c) >= 0)
                    containers.Push(c);
            }
            if (containers.Count > 0)
                throw new ParseException("Containers did not start-end properly");
        }

        /// <summary>
        /// Puts some text into the code and cleans up for easier handling the expression
        /// </summary>
        private string Normalize(string code)
        {
            string result = code.Replace(" ", ""); // strip empty spaces

            // add multiplications between number and containers
            for (int i = 0; i < 10; i++)
            {
                foreach (char c in cContainerStarts)
                {
                    result = result.Replace($"{i}{c}", $"{i}*{c}");
                }
                foreach (char c in cContainerEnds)
                {
                    result = result.Replace($"{c}{i}", $"{c}*{i}");
                }
            }

            // add multiplication between two container types 
            foreach (char c in cContainerEnds)
                foreach (char d in cContainerStarts)
                    result = result.Replace($"{c}{d}", $"{c}*{d}");

            // add 1 for the empty roll operators
            foreach (char c in cContainerStarts)
            {
                result = result.Replace($"{c}d", $"{c}1d");
            }

            // starts with roll
            if (result.StartsWith("d"))
                result = "1" + result;
            result = result.Replace("-d", "-+1d");
            result = result.Replace("+d", "+1d");
            result = result.Replace("/d", "/1d");
            result = result.Replace("*d", "*1d");

            // fix double sign values
            result = result.Replace("--", "");
            result = result.Replace("++", "");
            result = result.Replace("-+", "+-");
            //result = result.Replace("+-", "-");

            // unnecessary plus
            foreach (char c in cContainerStarts)
            {
                result = result.Replace($"{c}+", $"{c}");
            }

            return result.ToLower();
        }

        /// <summary>
        /// Parses the expression and puts space holders for the tokens (numbers)
        /// </summary>
        private void Tokenize(Expression exp)
        {
            string tokenRead = "";
            int tokenStart = -1;
            Func<char, int, int> tokenizeTiny = (p, i) =>
            {
                // char is digit, add
                if (Char.IsDigit(p))
                {
                    tokenStart = tokenStart >= 0 ? tokenStart : i;
                    tokenRead += p;
                }
                // minus can be part of token only if token is already empty
                else if (p == '-' && string.IsNullOrEmpty(tokenRead))
                {
                    tokenStart = tokenStart >= 0 ? tokenStart : i;
                    tokenRead += '-';
                }
                // token ended or ignore
                else if (!string.IsNullOrEmpty(tokenRead) && tokenRead != "-") // "-" means it is an operator, not sign
                {
                    int value = Convert.ToInt32(tokenRead);
                    int tokenId = exp.GetNextNodeId();
                    string tokenName = $"{{{tokenId}}}";
                    exp.Nodes.Add(tokenId, new Token() { Value = value, Id = tokenId });
                    exp.Equation = ReplaceInString(exp.Equation, tokenRead.Length, tokenStart, tokenName);

                    int temp = 1 + tokenStart + tokenName.Length;
                    tokenRead = "";
                    tokenStart = -1;
                    return temp;
                }
                // startinga new container means reset the values, previous token info was set correctly if grammar was correct
                else if (cContainerStarts.Contains(p))
                {
                    tokenStart = -1;
                    tokenRead = "";
                }

                return i + 1;
            };

            for (int i = 0; i < exp.Equation.Length;)
            {
                char c = exp.Equation[i];
                i = tokenizeTiny(c, i);
            }

            tokenizeTiny(' ', exp.Equation.Length);
        }

        /// <summary>
        /// Replaces 'count' characters in 'text' starting with index 'index' and puts the 'value'
        /// </summary>
        private string ReplaceInString(string text, int count, int index, string value)
        {
            // replace entire value
            if (count == text.Length)
            {
                return value;
            }
            // replace from beginning
            else if (index == 0)
            {
                return value + text.Substring(count);
            }
            // replace the ending
            else if (index + count == text.Length)
            {
                return text.Substring(0, index) + value;
            }
            // replace from middle
            else
            {
                string pre = text.Substring(0, index);
                string post = text.Substring(index + count);
                return pre + value + post;
            }
        }

        /// <summary>
        /// Finds token identifiers in the given expression, used when moving nodes/tokens from one node to another
        /// </summary>
        /// <returns>list of node ids</returns>
        private IEnumerable<int> FindChildNodes(string expr)
        {
            int index = expr.IndexOf('{');
            if (index < 0)
                yield break;

            int endIndex = expr.IndexOf('}');
            string value = expr.Substring(index + 1, endIndex - index - 1);
            yield return Convert.ToInt32(value);
            foreach (var res in FindChildNodes(expr.Substring(endIndex + 1)))
            {
                yield return res;
            }
        }

        /// <summary>
        /// Creates sub expressions depending on operator precedence, calls recursive
        /// </summary>
        private void PrioritizeOperators(Expression expr)
        {
            // work on children recursively
            foreach (Node child in expr.Nodes.Values)
            {
                if (child is Expression)
                    PrioritizeOperators(child as Expression);
            }

            // use prioritization on self, shall be called recursively
            PrioritizeOperatorsOnSelf(expr);
        }

        /// <summary>
        /// Creates sub expressions depending on operator precedence, operates on self only
        /// </summary>
        private void PrioritizeOperatorsOnSelf(Expression expr)
        {
            int opCountRoll = expr.Equation.Count((c) => c == cRoll);
            int opCountMultiply = expr.Equation.Count((c) => c == cMultiply);
            int opCountDivide = expr.Equation.Count((c) => c == cDivide);
            int opCountRemainder = expr.Equation.Count((c) => c == cRemainder);
            int opCountAdd = expr.Equation.Count((c) => c == cAdd);
            int opCountSubtract = expr.Equation.Count((c) => c == cSubtract);

            // if only have one operator, then done
            if (((opCountRoll + opCountMultiply + opCountDivide + opCountRemainder) + (opCountAdd + opCountSubtract)) == 1)
                return;

            // no special precedence operator, they can be executed easily, do nothing
            if ((opCountRoll + opCountMultiply + opCountDivide + opCountRemainder) == 0)
                return;

            // if we are here then we are sure there are multiple operators on current expression and at least one high precedence. parse one expression and call self

            // parse operators on self one by one
            foreach (char cop in new char[] { cRoll, cMultiply, cDivide, cRemainder })
            {
                if (PrioritizeSubOperatorOnSelf(expr, cop))
                    break;
            }

            PrioritizeOperatorsOnSelf(expr);
        }

        /// <summary>
        /// Creates sub expressions for a specific operator, true if any replacement is done
        /// </summary>
        private bool PrioritizeSubOperatorOnSelf(Expression expr, char opChar)
        {
            // get the index if operator is there, if not then ignore silently
            int index = expr.Equation.IndexOf(opChar);
            if (index < 0) return false;

            // find token starts and ends
            int leftSideTokenStart = FindPreviousIndexOf(expr.Equation, '{', index);
            int leftSideTokenEnd = index - 1;
            int rightSideTokenStart = index + 1;
            int rightSideTokenEnd = FindNextIndexOf(expr.Equation, '}', index);

            if (leftSideTokenStart == -1 ||
               leftSideTokenEnd == -1 ||
               rightSideTokenStart == -1 ||
               rightSideTokenEnd == -1)
                throw new ParseException("Sub operator prioritizing failed because tokens are not managed to be extracted");

            // parse token ids
            string leftToken = expr.Equation.Substring(leftSideTokenStart + 1, leftSideTokenEnd - leftSideTokenStart - 1);
            string rightToken = expr.Equation.Substring(rightSideTokenStart + 1, rightSideTokenEnd - rightSideTokenStart - 1);
            int leftTokenId = Convert.ToInt32(leftToken);
            int rightTokenId = Convert.ToInt32(rightToken);

            // create child node and move children to new child
            Expression child = new Expression() { Equation = $"{{{leftTokenId}}}{opChar}{{{rightTokenId}}}", Parent = expr };
            Node left = expr.Nodes[leftTokenId];
            Node right = expr.Nodes[rightTokenId];
            child.Nodes.Add(leftTokenId, left);
            child.Nodes.Add(rightTokenId, right);
            expr.Nodes.Remove(leftTokenId);
            expr.Nodes.Remove(rightTokenId);

            // replace the new expression's token in current expression command
            int next = expr.GetNextNodeId();
            expr.Equation = expr.Equation.Replace(child.Equation, $"{{{next}}}");
            child.Id = next;
            expr.Nodes.Add(next, child);

            return true;
        }

        /// <summary>
        /// Parses containers in the expression and creates sub expression nodes in a tree like form. Only checks containers (no operators)
        /// </summary>
        private void ParseContainers(Expression expr)
        {
            int[] endIndex = new int[cContainerEnds.Length];

            for (int i = 0; i < cContainerEnds.Length; i++)
            {
                endIndex[i] = expr.Equation.IndexOf(cContainerEnds[i]);
                if (endIndex[i] < 0)
                    endIndex[i] = Int32.MaxValue;
            }

            // we could not find container end, it means we do not have container in the expression, leave it
            int containerEndIndex = endIndex.Min();
            if (containerEndIndex == Int32.MaxValue)
                return;

            // cut the string so that we can find the last container start before the end
            string line = expr.Equation;
            if (containerEndIndex != line.Length)
                line = line.Substring(0, containerEndIndex + 1);

            // find corresponding start of the end we found
            int containerStartIndex = line.LastIndexOf(cContainerStarts[cContainerEnds.IndexOf(line[line.Length - 1])]);

            // add single space to prevent substring fail if end is at end of expression
            string subExpr = (line + " ").Substring(containerStartIndex + 1, containerEndIndex - containerStartIndex - 1);

            // create the child and move the sub expressions/tokens to the child
            Expression child = new Expression() { Equation = subExpr, Parent = expr };
            foreach (int key in FindChildNodes(subExpr))
            {
                Node node = expr.Nodes[key];
                child.Nodes.Add(key, node);
                expr.Nodes.Remove(key);
            }

            int tokenId = expr.GetNextNodeId();
            string tokenName = $"{{{tokenId}}}";
            child.Id = tokenId;
            expr.Nodes.Add(tokenId, child);

            expr.Equation = ReplaceInString(expr.Equation, 1 + containerEndIndex - containerStartIndex, containerStartIndex, tokenName);

            ParseContainers(expr);
        }
        #endregion

        #region Internal Members
        /// <summary>
        /// Parses the given code into an expression tree
        /// </summary>
        internal Expression ParseCode(string code)
        {
            // normalize, some text replacement
            string normalized = Normalize(code);

            // validate expression
            PreValidate(normalized);

            // tokenize the expression into easily parsable parts
            Expression expr = new Expression() { Equation = normalized };
            Tokenize(expr);

            // parse expression into more smaller pieces
            ParseContainers(expr);

            // prioritize operators on the expression's sub parts
            PrioritizeOperators(expr);

            return expr;
        }

        /// <summary>
        /// String search helper, looks for previous index of a character starting from an index
        /// </summary>
        /// <returns>Index or -1</returns>
        internal int FindPreviousIndexOf(string text, char lookup, int startIndex)
        {
            for (int i = startIndex; i >= 0; i--)
                if (text[i] == lookup)
                    return i;

            return -1;
        }

        /// <summary>
        /// String search helper, looks for next existence of a character starting from an index
        /// </summary>
        /// <returns>Index or -1</returns>
        internal int FindNextIndexOf(string text, char lookup, int startIndex)
        {
            for (int i = startIndex; i < text.Length; i++)
                if (text[i] == lookup)
                    return i;

            return -1;
        }
        #endregion
    }
}

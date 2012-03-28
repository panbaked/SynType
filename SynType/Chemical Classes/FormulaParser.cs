using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.Chemical_Classes
{
    public static class FormulaParser
    {
        static Dictionary<string, int> ParseBit(Stack<char> bit)
        {
            string element = string.Empty; string occ = string.Empty;
            int bitLength = bit.Count;
            for (int i = 0; i < bitLength; i++)
            {
                char next = bit.Pop();
                if (char.IsLetter(next))
                    element += next;
                else
                    occ += next;
            }
            int num = 1;

            if (!string.IsNullOrEmpty(occ))
            {
                num = int.Parse(occ);
            }
            Dictionary<string, int> parsedDict = new Dictionary<string, int>();
            parsedDict.Add(element, num);
            return parsedDict;

        }
        public static Dictionary<string, int> ParseFormula(string formula)
        {
            Dictionary<string, int> parsedDict = new Dictionary<string, int>();
            char currChar; int count = formula.Length - 1;
            Stack<char> holdingStack = new Stack<char>();

            while (count >= 0)
            {
                currChar = formula[count];
                if (char.IsDigit(currChar))
                {
                    if (holdingStack.Count == 0)
                    {
                        holdingStack.Push(currChar);
                    }
                    else if (char.IsDigit(holdingStack.Peek()))
                    {
                        holdingStack.Push(currChar);
                    }
                    else if (char.IsLower(holdingStack.Peek()))
                    {
                        Console.WriteLine("Error a number cannot precede a lowercase letter.\n Please check the formula.");
                    }
                    else if (char.IsUpper(holdingStack.Peek()))
                    {
                        //We have a subunit complete, so breakdown the holding stack, after parsing the stack we clear it.
                        //We then add in the number for the next stack
                        //Parse subunit
                        Dictionary<string, int> bit = ParseBit(holdingStack);
                        AppendDictionary(parsedDict, bit.First().Key, bit.First().Value);
                        holdingStack.Clear();
                        holdingStack.Push(currChar);
                    }
                }
                else if (char.IsUpper(currChar))
                {
                    if (holdingStack.Count == 0)
                    {
                        holdingStack.Push(currChar);
                    }
                    else if (char.IsDigit(holdingStack.Peek()))
                    {
                        //push as it could be a single letter element

                        holdingStack.Push(currChar);
                    }
                    else if (char.IsLower(holdingStack.Peek()))
                    {
                        //if a lower letter is present than we have a two letter element, so we push and then empty the stack
                        holdingStack.Push(currChar);
                        Dictionary<string, int> bit = ParseBit(holdingStack);
                        AppendDictionary(parsedDict, bit.First().Key, bit.First().Value);
                        holdingStack.Clear();

                    }
                    else if (char.IsUpper(holdingStack.Peek()))
                    {
                        //ditto for subunit
                        Dictionary<string, int> bit = ParseBit(holdingStack);
                        AppendDictionary(parsedDict, bit.First().Key, bit.First().Value);
                        holdingStack.Clear();
                        holdingStack.Push(currChar);
                    }
                }
                else if (char.IsLower(currChar))
                {
                    if (holdingStack.Count == 0)
                    {
                        holdingStack.Push(currChar);
                    }
                    else if (char.IsDigit(holdingStack.Peek()))
                    {
                        holdingStack.Push(currChar);
                    }
                    else if (char.IsLower(holdingStack.Peek()))
                    {
                        Console.WriteLine("Error two consecutive lower letters cannot occur.");
                    }
                    else if (char.IsUpper(holdingStack.Peek()))
                    {
                        //subunit jazz again
                        Dictionary<string, int> bit = ParseBit(holdingStack);
                        AppendDictionary(parsedDict, bit.First().Key, bit.First().Value);
                        holdingStack.Clear();
                        holdingStack.Push(currChar);
                    }
                }
                count--;
            }
            //need to parse one last time to get the first element
            Dictionary<string, int> finalBit = ParseBit(holdingStack);
            if (!string.IsNullOrEmpty(finalBit.Keys.First()))
            {
                AppendDictionary(parsedDict, finalBit.First().Key, finalBit.First().Value);
                holdingStack.Clear();
            }
            return parsedDict;
        }
        private static void AppendDictionary(Dictionary<string, int> dict, string atom, int occ)
        {
            try
            {
                dict.Add(atom, occ);
            }
            catch (ArgumentException ex)
            {
                dict[atom] += occ;

            }
        }
    }
}

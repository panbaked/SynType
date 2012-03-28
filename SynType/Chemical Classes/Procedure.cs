using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.Chemical_Classes
{
    //A procedure has text asscoiated with it, but also possible hyperlinks, commands etc.
    public class Procedure
    {
        string rawText;
        public string RawText { get { return rawText; } set { rawText = value; } }
        public string ParsedText { get { return ParseText(RawText); } }
        public Synthesis MySynthesis { get; set; }
    
        public Procedure()
        {
            rawText = "empty";
        }
        public Procedure(Synthesis mySynthesis, string text)
        {
            this.rawText = text;
            MySynthesis = mySynthesis;
        }
        //SYNTYPE COMMANDS
        //There are various commands we need to handle, the format goes $func(identifier) so we could have for example $mol(rc_0)
        //given $func(identifier), figure out the function and proceed to do the function on the compound 
        private string DoFunction(string part)
        {
            //We are given a split of the main text and it has been determined to contain a function (starts with $)
            //So we start at index 1 and go until we hit the first ( or beyond string length (in case the user messed up or the string isn't complete yet
            string function = string.Empty;
            int i = 1;
            while (part[i] != '(' || i > part.Length -1)
            {
                function += part[i];
                i++;
            }
            //the compound is given by a 4 char string starting at i + 1
            string compound = string.Empty;
            i++;
            while (part[i] != ')' || i > part.Length-1)
            {
                compound += part[i];
                i++;
            }
            switch (function)
            {
                case "mol":
                    return DoMol(compound);
                    
                case "mass":
                    return DoMass(compound);

                case "shortinfo":
                    return DoShortInfo(compound);

                case "longinfo":
                    return DoLongInfo(compound);
                default:
                    return "Cannot Parse Function";
            }
        }

        private string DoLongInfo(string compound)
        {
            throw new NotImplementedException();
        }

        private string DoShortInfo(string compound)
        {
            Compound comp = MySynthesis.GetCompound(compound);
            if (comp == null) 
                return "There is no compound named: " + compound;
            return comp.RefName + " (" + comp.Mols.ToString() + ", " + comp.Mass.ToString()+")";
        }

        private string DoMass(string compound)
        {
            Compound comp = MySynthesis.GetCompound(compound);
            if (comp == null) 
                return "There is no compound named: " + compound;
            return comp.Mass.ToString();
            
        }
        private string DoMol(string compound)
        {
            Compound comp = MySynthesis.GetCompound(compound);
            if (comp == null) 
                return "There is no compound named: " + compound;
            return comp.Mols.ToString();
        }
        private string ParseText(string RawText)
        {
            string[] parts = RawText.Split(' ');
            string parsed = string.Empty;
            int i = 0;
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                if (part[0] == '$')
                {
                    parsed += DoFunction(part) + " ";
                }
                else
                {
                    parsed += part + " ";
                }
            }
            return parsed;
        }
   
    }
}

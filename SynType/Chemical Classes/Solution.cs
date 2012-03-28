using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynType.Math_Classes;

namespace SynType.Chemical_Classes
{
    public class Solution
    {
        public string SolventName { get; set; }
        public Unit Concentration { get; set; }

        public Solution(string solventName, Unit conc)
        {
            SolventName = solventName;
            Concentration = conc;
        }
    }
}

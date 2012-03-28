using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynType.Math_Classes;

namespace SynType.Chemical_Classes
{
    public class Molecule : ICloneable
    {
        string name;
        int numberOfAtoms;
        int numberOfBonds;
        bool isChiral;
        string casNumber;
        Dictionary<int, Node> atoms; //Each atom has a number associated with it
        string formula;
        string createdBy;
        DateTime creationTime;
      
        string buildSoftware;
        string comments;
        PeriodicTable pTable;
        private List<Bond> bonds; //a list of all bonds
        private BoundingBox boundingBox;
        private AddCompoundForm form;
        public Molecule()
        {
            atoms = new Dictionary<int, Node>();
            name = "Not named.";
            numberOfAtoms = 0;
            numberOfBonds = 0;
            isChiral = false;
            createdBy = "Not given.";
            buildSoftware = "Not given.";
            comments = "No comments given.";
            bonds = new List<Bond>();
            pTable = new PeriodicTable();

        }
        public Molecule(PeriodicTable pTable)
        {
            atoms = new Dictionary<int, Node>();
            name = "Not named.";
            numberOfAtoms = 0;
            numberOfBonds = 0;
            isChiral = false;
            createdBy = "Not given.";
            buildSoftware = "Not given.";
            comments = "No comments given.";
            this.pTable = pTable;
            bonds = new List<Bond>();

        }
        public Molecule(AddCompoundForm form)
        {
            atoms = new Dictionary<int, Node>();
            name = "Not named.";
            numberOfAtoms = 0;
            numberOfBonds = 0;
            isChiral = false;
            createdBy = "Not given.";
            buildSoftware = "Not given.";
            comments = "No comments given.";
            bonds = new List<Bond>();
            pTable = new PeriodicTable();
            this.form = form;
        }
        public Molecule(Dictionary<int, Node> atoms, string name, int numberOfAtoms, int numberOfBonds, string createdBy, string buildSoftware, string comments, List<Bond> bonds, AddCompoundForm form)
        {
            this.atoms = atoms;
            this.name = name;
            this.numberOfAtoms = numberOfAtoms;
            this.numberOfBonds = numberOfBonds;
            this.createdBy = createdBy;
            this.comments = comments;
            this.bonds = bonds;
            this.form = form;
            pTable = new PeriodicTable();

        }
        public string Name { get { return name; } set { name = value; } }
        public string CASNumber { get { return casNumber; } set { casNumber = value; } }
        public int NumberOfAtoms
        {
            get { return numberOfAtoms; }
            set { numberOfAtoms = value; }
        }
        public int NumberOfBonds
        {
            get { return numberOfBonds; }
            set { numberOfBonds = value; }
        }
        public BoundingBox Bounds { get { return boundingBox; } set { boundingBox = value; } }
        public bool IsChiral { get { return isChiral; } set { isChiral = value; } }
        public Dictionary<int, Node> Atoms { get { return atoms; } set { atoms = value; } }
        public string CreatedBy { get { return createdBy; } set { createdBy = value; } }
        public DateTime CreationTime { get { return creationTime; } set { creationTime = value; } }
        public string BuildSoftware { get { return buildSoftware; } set { buildSoftware = value; } }
        public string Comments { get { return comments; } set { comments = value; } }
        public List<Bond> Bonds { get { return bonds; } set { bonds = value; } }

        public Dictionary<int, int> ImplicitHydrogens //atom a has n hydrogens
        {
            get
            {
                Dictionary<int, int> tempDict = new Dictionary<int, int>();
                if (atoms.Count > 1)
                {
                    //We go through each atom and find all of its bond partners and their order
                    //also if an atom has a number of required hydrogens we also count those
                    foreach (int i in atoms.Keys)
                    {
                        if (atoms[i].Symbol == "C" || atoms[i].Symbol == "N" || atoms[i].Symbol == "S" || atoms[i].Symbol == "O")
                        {
                            int count = 0;
                            foreach (Bond bond in bonds)
                            {
                                if (atoms[i] ==  bond.Node1 || atoms[i] ==  bond.Node2)
                                {
                                    count += bond.BondOrder;
                                }
                            }
                            if (atoms[i].RequiredHydrogens != 0)
                                count += atoms[i].RequiredHydrogens;

                            int impHs = 0;
                            if (atoms[i].Symbol == "N") impHs = 3 - count;
                            else if (atoms[i].Symbol == "C") impHs = 4 - count;
                            else if (atoms[i].Symbol == "S") impHs = 2 - count;
                            else if (atoms[i].Symbol == "O") impHs = 2 - count;
                            atoms[i].ImplicitHydrogens = impHs;
                            tempDict.Add(i, impHs);
                        }
                    }
                    return tempDict;
                }
                else
                    return new Dictionary<int, int>();


            }
        }
        public int ImpHs 
        {
            get
            {
                int count = 0;
                foreach (int i in ImplicitHydrogens.Keys)
                {
                    count += ImplicitHydrogens[i];
                }
                return count;
            }
        }
        public string Formula
        {
            get
            {
                if (string.IsNullOrEmpty(formula))
                {
                    formula = BuildFormula();
                    if (!string.IsNullOrEmpty(formula)) //we check to see if there is no formula that can be built, this means that there is no atomic data
                        return formula;
                    else
                        return string.Empty;
                }
                else
                    return formula;
            }
            set
            {
                Console.WriteLine("!!!Formula is being set to " + value + " at "+ DateTime.Now);
                formula = value;
                creationTime = DateTime.Now; //set the time we created the molecule
            }
        }
        public bool OnlyHasFormula
        {
            get;
            set;
        }
        private string BuildFormula()
        {
            string formula = string.Empty;
            if (atoms.Count == 0) return string.Empty; //If there is no atom data then no formula can be built
            Dictionary<int, int> impHydrogens = ImplicitHydrogens;
            //Next we must build a new dictionary from the atoms dict, as that relates the atom number to a specific atom
            //instance. The new dictionary is by Symbol, count
            Dictionary<string, int> tempDict = new Dictionary<string, int>();
            foreach (int i in atoms.Keys)
            {
                string symbol = atoms[i].Symbol;
                if (tempDict.ContainsKey(symbol))
                {
                    tempDict[symbol] += 1;
                }
                else
                {
                    tempDict.Add(symbol, 1);
                }
            }

            //If there are any carbon they go first
            if (tempDict.ContainsKey("C"))
            {
                if (tempDict["C"] > 1)
                    formula += "C" + tempDict["C"]; //Add C#
                else if (tempDict["C"] == 1)
                    formula += "C";

                tempDict.Remove("C");
            }
            //Hydrogens are different as there could be implicit ones
            if (tempDict.ContainsKey("H"))
            {
                //First we need to calculate the total number of impHs in the dictionary
                
                if (tempDict["H"] > 1)
                {
                    int HCount = tempDict["H"] + ImpHs;
                    formula += "H" + HCount;
                }
                else if (tempDict["H"] == 1)
                {
                    int HCount = tempDict["H"] + ImpHs;
                    if (HCount > 1)
                        formula += "H" + HCount;

                }

                tempDict.Remove("H");
            }
            else
            {
                if (ImplicitHydrogens.Count > 0)
                    formula += "H" + ImpHs;
            }

            if (tempDict.ContainsKey("N"))
            {
                if (tempDict["N"] > 1)
                    formula += "N" + tempDict["N"];
                else if (tempDict["N"] == 1)
                    formula += "N";

                tempDict.Remove("N");
            }
            else if (tempDict.ContainsKey("O"))
            {
                if (tempDict["O"] > 1)
                    formula += "O" + tempDict["O"];
                else if (tempDict["O"] == 1)
                    formula += "O";

                tempDict.Remove("O");
            }
            


            foreach (string s in tempDict.Keys)
            {
                if (tempDict[s] != 1)
                    formula += s + tempDict[s];
                else
                    formula += s;
            }
            Console.WriteLine("Formula built to " + formula + " at " + DateTime.Now);
            return formula;

        }
        public float MolecularWeight
        {
            get
            {
                if (atoms.Count == 0)
                { //if the atom count is zero then there is no molecule data, then we check to see if a formula is present
                    if (Formula != string.Empty)
                    {
                        float x = CalculateMolecularWeightFromFormula();
                        //If we're here then we have a formula we can parse for the weight
                        return CalculateMolecularWeightFromFormula();
                    }
                }
                else //otherwise we have molecular data, so first we check if we also have a formula
                {
                    if (Formula != string.Empty)
                    {
                        //we've got a formula as well so we compare the values
                        if (CalculateMolecularWeightFromFormula() == CalculateMolecularWeightFromStructure())
                            return CalculateMolecularWeightFromStructure();
                        else
                        {
                            Console.WriteLine("Error, formula and structural data disagree on the molecular weight!\n Molecule Name : "+ Formula);
                            Console.WriteLine(CalculateMolecularWeightFromFormula() + " from formula; " + CalculateMolecularWeightFromStructure() + " from structure");
                        }
                        

                    }
                    else
                        return CalculateMolecularWeightFromStructure();
                }
                return CalculateMolecularWeightFromStructure();
            }
        }
        private float CalculateMolecularWeightFromFormula()
        {
            //The formulaparser class delivers a dictionary that has the atoms listed by symbol and occurence
            Dictionary<string, int> aDict =  FormulaParser.ParseFormula(Formula);
            double weight = 0;
            foreach (string symbol in aDict.Keys)
            {
                try
                {
                    weight += pTable.Elements[symbol].Weight * aDict[symbol];
                }
                catch (KeyNotFoundException)
                {
                    form.ShowError("The formula is incorrect, make sure all atomic symbols are correct.");
                }
            }
            weight = Math.Round(weight, 2);
            return (float) weight;
            
        }
        private float CalculateMolecularWeightFromStructure()
        {
          
            //Next we must build a new dictionary from the atoms dict, as that relates the atom number to a specific atom
            //instance. The new dictionary is by Symbol, count
            Dictionary<string, int> tempDict = new Dictionary<string, int>();
            foreach (int i in atoms.Keys)
            {
                string symbol = atoms[i].Symbol;
                if (tempDict.ContainsKey(symbol))
                {
                    tempDict[symbol] += 1;
                }
                else
                {
                    tempDict.Add(symbol, 1);
                }
            }
            //now we lookup each element in the periodic table and multiply its weight by the occurence
            double weight = 0;
            foreach (string sym in tempDict.Keys)
            {
                weight += pTable.Elements[sym].Weight * tempDict[sym];
            }
            weight += pTable.Elements["H"].Weight * ImpHs;
            weight = Math.Round(weight, 2);
            return (float) weight;
        }
        //this function takes a formula and generates a molecular weight
     /*   private float ParseFormula(string formula)
        {
            //We can split the sets of atoms by numbering in between them, there are a few cases
            //C12H5 - here we split after letters and numbers, associating the numbers with the preceeding letters
            //Cu2H5 - here we again split by letters and numbers just as above - we know however that a lower case letter belong to the element
            //CH - here we split because there are two capital letters in sequence
            //CuH - here we split because a capital letter follows a lower case letter

            //We have to running stacks, one with letters and one with numbers, once an element has been completely established we purge the stack and begin again
            Stack<char> runningStack = new Stack<char>();
            Dictionary<string, int> atomDict = new Dictionary<string, int>(); //this dictionary stores all of the atoms and how many of them there are
            for (int i = 0; i < formula.Length; i++)
            {
                char c = formula[i];
                //First we consider whether the char is a letter
                if (char.IsLetter(c))
                {
                    //is the letter upper or lower
                    if (char.IsUpper(c))
                    {
                        //we check if the stack is empty, if so we add the letter
                        if (runningStack.Count == 0)
                            runningStack.Push(c); continue;

                        //is the last letter a capital? if so we add the previous atom to the dict
                        if(char.IsUpper(runningStack.Peek()))
                            //Add the function that splits the num
                    }
                    else
                    {
                    }
                }
                else if (char.IsDigit(c))
                {
                }
                else
                    Console.WriteLine("Formula could not be parsed, it contains other values than letters and numbers!");


                
            }
        }*/
        
        public override string ToString()
        {
            string rString = "-------Molecular information-------\n name:" + name + " atoms: " +
                atoms.Count + "\n Created by: " + createdBy +
                " on " + creationTime.ToString() + " with " + buildSoftware;
            if (isChiral)
                rString += "\n The molecule is chiral.";
            rString += "\n Additional comments: ";
            rString += "\n formula " + Formula;
            rString += " Weight: " + MolecularWeight;
            return rString;
        }
        public object Clone()
        {
            //First we make a copy of the atoms
            Dictionary<int, Node> atomCopy = new Dictionary<int, Node>();
            foreach (int id in atoms.Keys)
            {
                atomCopy.Add(id, (Node) atoms[id].Clone());
            }
            //Then a copy of the bonds
            List<Bond> bondCopy = new List<Bond>();
            foreach (Bond bond in bonds)
            {
                bondCopy.Add((Bond)bond.Clone());
            }
            Molecule copy = new Molecule(atomCopy, this.name, this.numberOfAtoms, this.numberOfBonds, this.createdBy, this.buildSoftware, this.comments, bondCopy, this.form);
            return copy;
        }
    }
   
}

/*
//is the char an uppercase letter?
                if (char.IsLetter(c) && char.IsUpper(c))
                {
                    //is the stack empty? if so then we add it in
                    if(runningStack.Count == 0) runningStack.Push(c); continue;
                    //Now check the stack and see if we have: any capital letter preceeding this char
                    if (char.IsLetter(runningStack.Peek()) && char.IsUpper(runningStack.Peek()))
                    {
                        //If we hit this point then there was only one of the last element, so we add this to the dictionary
                        atomDict.Add(runningStack.Pop().ToString(), 1);

                    }
                    //now we check if the stack contains a lower case letter, then we are also done
                    else if(char.IsLetter(runningStack.Peek()) && char.IsLower(letterStack.Peek()))
                    {
                    }
                    //and the last check is if the number stack has any in there, then we also add this
                    
                }
                else if (char.IsLetter(c) && char.IsLower(c)) //is it a lower case letter
                {
                }
                else if (char.IsDigit(c)) //is it a digit?
                {
                }
*/
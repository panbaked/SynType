using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SynType.Chemical_Classes;
using SynType.File_ReadWrite;
using SynType.Math_Classes;
using System.Collections.ObjectModel;

/*              This file contains all you need to parse or write a syntype synthesis file              */
/* Format to follow, ALL spaces must be present
 * Syntype Vx.y username date(format DD/MM/YY HH:mm)
 * id# idtag reactantsCount reagentsCount productCount
 * [Edit#]
 * editDT(format DD/MM/YY HH:mm)
 * [RC#] <- Reactants listed by number
 * molfilename directory id formula molweight(g/mol) state(s,l,g,solute) density(can be null) 
 * islimiting(0 or 1) mols mass(g,can be null) volume(ml, can be null)
 * [/RC#]
 * [RG#]
 * molfilename directory id formula molweight(g/mol) state(s,l,g,solute) density(can be null) 
 * islimiting(0 or 1) mols mass(g,can be null) volume(ml,can be null)
 * [/RG#]
 * [P#]
 * molfilename directory id formula molweight(g/mol) state(s,l,g,solute) density(can be null) 
 * istarget(0 or 1) mols mass(g, can be null) volume(ml, can be null)
 * [/P#]
 * [PROCEDURE]
 * text..........
 * [/PROCEDURE]
 * [MOL]
 * [id]
 * molefiledata
 * [/id]
 * */

namespace SynType.File_ReadWrite.SynType
{
  

    public static class SynTypeFileReader
    {
        /*
        List<Edit> edits; //everything is stored by edits
        public List<Edit> Edits { get { return edits; } set { edits = value; } }
        */
       
    
        //Records when an edit has occured and what the edit was
        //The above structures contain the most updated version of the synthesis
     
       
        /*   <localid>rc1</localid>
          <name>benzaldehyde</name>
          <refid>aldehyde</refid>
          <molweight>106.12</molweight>
          <state>solid</state>
          <density temp="20">null</density>
          <islimiting>true</islimiting>
          <mols unit="mmol">57</mols>
          <mass unit="g">6</mass>
          <volume>null</volume>
         * */
        public static Synthesis ReadFile(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode header = doc.GetElementById("header");
            XmlNodeList editHeader = doc.GetElementsByTagName("editheader");
            XmlNodeList reactantNodeList = doc.GetElementsByTagName("reactant");
            XmlNodeList reagentNodeList = doc.GetElementsByTagName("reagent");
            XmlNodeList productNodeList = doc.GetElementsByTagName("product");

            ObservableCollection<Compound> reactants = ReadBlock(reactantNodeList, COMPOUND_TYPES.Reactant);
            ObservableCollection<Compound> reagents = ReadBlock(reagentNodeList, COMPOUND_TYPES.Reagent);
            ObservableCollection<Compound> products = ReadBlock(productNodeList, COMPOUND_TYPES.Product);

            string synthName = GetSynthesisName(editHeader);
            int projectID = GetSynthesisProjectID(editHeader);
            SynthesisInfo previousStep = GetPreviousStep(editHeader);
            SynthesisInfo nextStep = GetNextStep(editHeader);
            string procedureText = GetProcedureText(editHeader);
            Synthesis synth = new Synthesis(reactants, reagents, products, synthName, projectID, previousStep, nextStep, procedureText);

            ReadMolData(doc, synth.AllCompounds);
            return synth;
        }

     
        private static void ReadMolData(XmlDocument doc, List<Compound> compounds)
        {
            //We read in the mol data, with the compound file id and then fill the molecule out with information
            //First we find the correct xml node
            XmlNodeList molDataNodes = doc.GetElementsByTagName("moldata");
            //now we run through the nodes

            foreach (XmlNode node in molDataNodes)
            {
                foreach (Compound c in compounds)
                {
                    if (c.FileID == int.Parse(node.Attributes["fileid"].Value))
                    {
                        Molecule m = new Molecule();
                        if (node.InnerText == string.Empty)
                        {
                            //sometimes we have no mol data, just a formula
                            m.Formula = node.Attributes["formula"].Value;
                            m.OnlyHasFormula = true;
                        }
                        else
                        {
                            MolFileReader.ReadMolData(node.InnerText, ref m);
                        }
                        
                        Console.WriteLine(m.ToString());
                        c.MoleculeData = m;
                    }
                }
            }
        }
        private static string GetSynthesisName(XmlNodeList headerList)
        {
            foreach (XmlNode node in headerList)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.Name == "name")
                    {
                        Console.WriteLine("FileReader getting synth name " + node.InnerText);
                        return innerNode.InnerText;
                    }
                }
                
            }
            return string.Empty;
        }
        private static int GetSynthesisProjectID(XmlNodeList headerList)
        {
            foreach (XmlNode node in headerList)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.Name == "projectid")
                    {
                        int id;
                         bool parsed = int.TryParse(innerNode.InnerText, out id);
                         if (parsed)
                             return id;
                    }
                }
            }
            return -1;
        }
        private static SynthesisInfo GetPreviousStep(XmlNodeList editHeader)
        {
            foreach (XmlNode node in editHeader)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.Name == "previousstep")
                    {
                        string name = innerNode.InnerText;
                        int id;
                        bool parsed = int.TryParse(innerNode.Attributes["projectid"].Value, out id);
                        if (parsed)
                            return new SynthesisInfo(name, id);
                    }
                }
            }
            return null;
        }

        private static SynthesisInfo GetNextStep(XmlNodeList editHeader)
        {
            foreach (XmlNode node in editHeader)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.Name == "nextstep")
                    {
                        string name = innerNode.InnerText;
                        int id;
                        bool parsed = int.TryParse(innerNode.Attributes["projectid"].Value, out id);
                        if (parsed)
                            return new SynthesisInfo(name, id);
                    }
                }
            }
            return null;
        }
        private static string GetProcedureText(XmlNodeList editHeader)
        {
            foreach (XmlNode node in editHeader)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.Name == "procedure")
                    {
                        string procedureText = innerNode.InnerText;
                        return procedureText;
                    }
                }
            }
            return string.Empty;
        }
        private static ObservableCollection<Compound> ReadBlock(XmlNodeList compoundList, COMPOUND_TYPES type)
        {
            ObservableCollection<Compound> returnList = new ObservableCollection<Compound>();
            foreach (XmlNode node in compoundList)
            {
                string localid = string.Empty;
                string name = string.Empty;
                string refid = string.Empty;
                int fileid = 0; //In the files each compound is recognized by a five digit number, so we can attach mol data
                int state = 0;
                string formula = string.Empty;
                float density = 0.0f;
                Unit mols = new Unit(0, "mol", (int)UNIT_POWERS.none);
                Unit mass = new Unit(0, "g", (int)UNIT_POWERS.none);
                Unit volume = new Unit(0, "l", (int)UNIT_POWERS.none);
                Solution solvent = new Solution("null", new Unit(0, "mol/l", (int)UNIT_POWERS.none));
                bool isLimiting = false;
                bool isTarget = false;
               

                foreach (XmlNode inner in node.ChildNodes)
                {

                    switch (inner.Name)
                    {
                        case "fileid":
                            fileid = int.Parse(inner.InnerText); break;
                        case "localid":
                            localid = inner.InnerText; break;
                        case "name":
                            name = inner.InnerText; break;
                        case "refid":
                            refid = inner.InnerText; break;
                        case "formula":
                            formula = inner.InnerText; break;
                        case "state":
                            string text = inner.InnerText;
                            if (text == "solid") state = 0;
                            if (text == "liquid") state = 1;
                            if (text == "gas") state = 2;
                            if (text == "solvated") state = 3;
                            break;
                        case "islimiting":
                            isLimiting = bool.Parse(inner.InnerText);
                            break;
                        case "istarget":
                            isTarget = bool.Parse(inner.InnerText);
                            break;
                        case "density":
                            if (inner.InnerText == "null") break;
                            density = float.Parse(inner.InnerText); break;
                        case "mols":
                            if(inner.InnerText == "null") break;
                            mols = new Unit(double.Parse(inner.InnerText), inner.Attributes["unit"].Value, int.Parse(inner.Attributes["unit_power"].Value)); 
                            break;
                        case "mass":
                            if (inner.InnerText == "null") break;
                            mass = new Unit(double.Parse(inner.InnerText), inner.Attributes["unit"].Value, int.Parse(inner.Attributes["unit_power"].Value)); 
                            break;
                        case "volume":
                            if (inner.InnerText == "null") break;
                            volume = new Unit(double.Parse(inner.InnerText), inner.Attributes["unit"].Value, int.Parse(inner.Attributes["unit_power"].Value)); 
                            break;
                        case "solvent":
                            if (inner.InnerText == "null") break;
                            solvent = new Solution(inner.InnerText, new Unit(float.Parse(inner.Attributes["conc"].Value), inner.Attributes["conc_unit"].Value, int.Parse(inner.Attributes["unit_power"].Value)));
                            break;
                        


                    }

                }
                Compound c = new Compound(name, refid, localid, fileid, (int)type, state, isLimiting, isTarget, formula, density, mols, mass, volume, solvent);
                Console.WriteLine(c.ToString());
                returnList.Add(c);
            }
            return returnList;

        }
       
    }
    
}

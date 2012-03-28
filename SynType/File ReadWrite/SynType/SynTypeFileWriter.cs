using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using SynType.Chemical_Classes;
using System.Collections.ObjectModel;

namespace SynType.File_ReadWrite.SynType
{
    static class SynTypeFileWriter
    {



        public static bool WriteFile(Header header, List<Edit> edits, string filename)
        {
            List<int> fileIDs = new List<int>();
            XmlDocument xmlDoc = new XmlDocument();


            //Most of the time the file won't exist so we start making a new one with the initilization data
            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            writer.WriteStartElement("synthesis");
            writer.Close();
            //Now load the newly setup file
            xmlDoc.Load(filename);

            //Now we actually build the xml file
            XmlNode root = xmlDoc.DocumentElement;


            //Create the header
            XmlElement head = xmlDoc.CreateElement("header");
            XmlElement username = xmlDoc.CreateElement("username");
            username.InnerText = header.userName;
            XmlElement creationDate = xmlDoc.CreateElement("creationdate");
            creationDate.InnerText = GetDTString(header.creationTime);
            XmlElement idnumber = xmlDoc.CreateElement("idnumber");
            idnumber.InnerText = header.idNumber.ToString();
            XmlElement idref = xmlDoc.CreateElement("idref");
            idref.InnerText = header.idTag;
            //Add all the members to the header
            head.AppendChild(username); head.AppendChild(creationDate); head.AppendChild(idnumber); head.AppendChild(idref);
            //And append the header to the synthesis
            root.AppendChild(head);
            //Now we go through each edit to add a synthesis
            int editCounter = 0;
            List<Compound> allCompounds = new List<Compound>();
            foreach (Edit edit in edits)
            {
                string editName = "edit" + editCounter;
                XmlElement editElement = xmlDoc.CreateElement(editName);
                XmlElement editHeader = xmlDoc.CreateElement("editheader");
                XmlElement editDate = xmlDoc.CreateElement("editdate");
                editDate.InnerText = GetDTString(edit.EditTime);
                XmlElement synthName = xmlDoc.CreateElement("name");
                synthName.InnerText = edit.Synth.Name;
                XmlElement projectID = xmlDoc.CreateElement("projectid");
                projectID.InnerText = edit.Synth.ProjectID.ToString();
                
                XmlElement previousStep = xmlDoc.CreateElement("previousstep");
                if (edit.Synth.PreviousStep == null)
                {
                    previousStep.InnerText = string.Empty;
                    previousStep.SetAttribute("projectid", (-1).ToString());
                }
                else
                {
                    previousStep.InnerText = edit.Synth.PreviousStep.Name;
                    previousStep.SetAttribute("projectid", edit.Synth.PreviousStep.ProjectID.ToString());
                }

                XmlElement nextStep = xmlDoc.CreateElement("nextstep");
                if (edit.Synth.NextStep == null)
                {
                    nextStep.InnerText = string.Empty;
                    nextStep.SetAttribute("projectid", (-1).ToString());
                }
                else
                {
                    nextStep.InnerText = edit.Synth.NextStep.Name;
                    nextStep.SetAttribute("projectid", edit.Synth.NextStep.ProjectID.ToString());
                }

                XmlElement procedure = xmlDoc.CreateElement("procedure");
                procedure.InnerText = edit.Synth.Proc.RawText;


                //add the parts to the header
                editHeader.AppendChild(editDate);
                editHeader.AppendChild(synthName);
                editHeader.AppendChild(projectID);
                editHeader.AppendChild(previousStep);
                editHeader.AppendChild(nextStep);
                editHeader.AppendChild(procedure);

                //Add the header to the edit
                editElement.AppendChild(editHeader);
                //And add the edit element to the synthesis
                root.AppendChild(editElement);
                //Now we go through the reactants, reagents and products and save their data, but before we insert reactant, reagent or product into the xml
                XmlElement reactants = xmlDoc.CreateElement("reactants"); editElement.AppendChild(reactants);
                XmlElement reagents = xmlDoc.CreateElement("reagents"); editElement.AppendChild(reagents);
                XmlElement products = xmlDoc.CreateElement("products"); editElement.AppendChild(products);

                WriteBlock(xmlDoc, reactants, edit, fileIDs, edit.Synth.Reactants, COMPOUND_TYPES.Reactant);
                WriteBlock(xmlDoc, reagents, edit, fileIDs, edit.Synth.Reagents, COMPOUND_TYPES.Reagent);
                WriteBlock(xmlDoc, products, edit, fileIDs, edit.Synth.Products, COMPOUND_TYPES.Product);
                
                //add all the compounds from this edit to our total compound list
                foreach(Compound c in edit.Synth.AllCompounds)
                {
                    allCompounds.Add(c);
                }
            }

            WriteMolBlock(xmlDoc, root, fileIDs, allCompounds);
            xmlDoc.Save(filename);
            return true;
        }
        private static void WriteMolBlock(XmlDocument doc, XmlNode pparent, List<int> fileIDs, List<Compound> compounds)
        {
            foreach (Compound comp in compounds)
            {
                string molString;
                XmlElement molData = doc.CreateElement("moldata");
                if (comp.MoleculeData.NumberOfAtoms == 0) //do we just have a formula for this molecule?
                {
                    molString = string.Empty;
                    molData.SetAttribute("formula", comp.MoleculeData.Formula);
                }
                else
                {
                    molString = MolFileWriter.WriteMolString(comp.MoleculeData);
                    molData.SetAttribute("formula", comp.MoleculeData.Formula);
                }
                
                molData.InnerText = molString;
                molData.SetAttribute("fileid", comp.FileID.ToString());
                pparent.AppendChild(molData);
            }

        }
        private static void WriteBlock(XmlDocument doc, XmlElement pparent, Edit edit, List<int> fileIDs, ObservableCollection<Compound> compounds, COMPOUND_TYPES type)
        {
            //First we have a counter to keep track of rc1, rc2 etc
            int counter = 0;


            foreach (Compound comp in compounds)
            {
                //First we determine what element this will belong to
                string parentname = string.Empty;
                switch (type)
                {
                    case COMPOUND_TYPES.Reactant:
                        parentname = "reactant";
                        break;
                    case COMPOUND_TYPES.Reagent:
                        parentname = "reagent";
                        break;
                    case COMPOUND_TYPES.Product:
                        parentname = "product";
                        break;
                }
                XmlElement parent = doc.CreateElement(parentname);
                //Fileid is generated on the fly
                XmlElement fileid = doc.CreateElement("fileid");
                comp.FileID = GenerateFileID(fileIDs);
                fileid.InnerText = comp.FileID.ToString();
                //Local id is also generated on the fly we simply pass our counter and type
                XmlElement localid = doc.CreateElement("localid");
                localid.InnerText = GenerateLocalID(counter, type);
                //The name is a copy of our compoud name
                XmlElement name = doc.CreateElement("name");
                name.InnerText = comp.Name;
                //Copy over ref id
                XmlElement refid = doc.CreateElement("refid");
                refid.InnerText = comp.RefName;
                //Same for formula
                XmlElement formula = doc.CreateElement("formula");
                formula.InnerText = comp.Formula;
                //And same for state
                XmlElement state = doc.CreateElement("state");
                state.InnerText = comp.State.ToString();
                //density is the same, but it has a temperature <--- NOT DONE
                XmlElement density = doc.CreateElement("density"); // at temp x
                density.InnerText = comp.Density.ToString(); //THIS IS NOT COMPLETE; DENSITY EXISTS AT A GIVEN TEMP
                density.SetAttribute("temp", "25");
                XmlElement islimiting = doc.CreateElement("islimiting");
                islimiting.InnerText = comp.IsLimiting.ToString();
                XmlElement isTarget = doc.CreateElement("istarget");
                isTarget.InnerText = comp.IsTarget.ToString();
                //The mols have a unit so we look into the dictionary to get the value at whatever unit
                XmlElement mols = doc.CreateElement("mols"); // unit
                mols.InnerText = comp.MolsValue.ToString();
                mols.SetAttribute("unit", comp.MolsUnit);
                mols.SetAttribute("unit_power", comp.Mols.Power.ToString());
                //The mass is also at a unit so just the same, however some compounds are measured 
                //not in mass but by volume so we check this and output null if thats the case
                XmlElement mass = doc.CreateElement("mass"); //unit
                if (comp.MassValue > 0)
                {
                    mass.InnerText = comp.MassValue.ToString();
                    mass.SetAttribute("unit", comp.MassUnit);
                    mass.SetAttribute("unit_power", comp.Mass.Power.ToString());
                }
                else
                {
                    mass.InnerText = "null";
                    mass.SetAttribute("unit", "mol");
                    mass.SetAttribute("unit_power", "0");
                }
                //and likewise for the volume, sometimes things are measured by mass and not volume
                XmlElement volume = doc.CreateElement("volume"); //unit
                if (comp.VolumeValue > 0)
                {
                    volume.InnerText = comp.VolumeValue.ToString();
                    volume.SetAttribute("unit", "l");
                    volume.SetAttribute("unit_power", comp.Volume.Power.ToString());
                }
                else
                {
                    volume.InnerText = "null";
                    volume.SetAttribute("unit", "l");
                    volume.SetAttribute("unit_power", "0");
                }
                XmlElement solvent = doc.CreateElement("solvent");
                if (!string.IsNullOrEmpty(comp.SolventName))
                {
                    solvent.InnerText = comp.SolventName;
                    solvent.SetAttribute("conc", comp.Solution.Concentration.Value.ToString());
                    solvent.SetAttribute("conc_unit", comp.Solution.Concentration.UnitType);
                    solvent.SetAttribute("unit_power", comp.Solution.Concentration.Power.ToString());
                }
                else
                {
                    solvent.InnerText = "null";
                    solvent.SetAttribute("conc", "null");
                    solvent.SetAttribute("conc_unit", "mol/l");
                    solvent.SetAttribute("unit_power", "0");
                }
                //Now we have to append all of these as children to the parent element given
                parent.AppendChild(fileid); parent.AppendChild(localid); parent.AppendChild(name); parent.AppendChild(refid);
                parent.AppendChild(formula); parent.AppendChild(state); parent.AppendChild(density); parent.AppendChild(islimiting); parent.AppendChild(isTarget);
                parent.AppendChild(mols); parent.AppendChild(mass); parent.AppendChild(volume); parent.AppendChild(solvent);
                //NOw that we have added all of the elements we add this parent element to our parents parent, giving us the correct format
                pparent.AppendChild(parent);
                counter++;
            }

        }
        private static int GenerateFileID(List<int> fileIDs)
        {
            //We generate a 4 number file id from 1000 to 9999 that does not conflict with the other ids
            Random random = new Random();
            int id = random.Next(1000, 9999);

            while (fileIDs.Contains(id))
                id = random.Next(1000, 9999);

            fileIDs.Add(id);
            return id;
        }
        //We need to be able to generate an local id for an edit such as rc1 or p1
        private static string GenerateLocalID(int counter, COMPOUND_TYPES type)
        {
            switch (type)
            {
                case COMPOUND_TYPES.Reactant:
                    return "rc" + counter;
                case COMPOUND_TYPES.Reagent:
                    return "rg" + counter;
                case COMPOUND_TYPES.Product:
                    return "p" + counter;
            }
            return string.Empty;
        }
        //We need a function that converts mm/dd/yyyy to yyyy/mm/dd
        private static string GetDTString(DateTime dt)
        {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            string day = dt.Day.ToString();
            return year + "/" + month + "/" + day;
        }
    }
}

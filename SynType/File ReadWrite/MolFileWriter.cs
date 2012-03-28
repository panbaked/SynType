using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SynType.Chemical_Classes;

namespace SynType.File_ReadWrite
{
    public static class MolFileWriter
    {
        public static string WriteMolString(Molecule mol)
        {
            
            //First we write to a temp file, we then open the file, read all of the text and return it as a string
            WriteMolFile("writertemp.mol", mol);
            string molString = File.ReadAllText("writertemp.mol");
            //Now we delete the temp file
            File.Delete("writertemp.mol");
            return molString;
        }
        public static void WriteMolFile(string filePath, Molecule mol)
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                WriteHeaderBlock(writer, mol);
                WriteCountsLine(writer, mol);
                WriteAtomBlock(writer, mol);
                WriteBondBlock(writer, mol);
                writer.WriteLine("M  END");
            }
        }
        private static void WriteHeaderBlock(StreamWriter writer, Molecule mol)
        {
            //Line 1 : molecule name, can be blank, no longer than 80 letters
            writer.WriteLine(mol.Name);
            /*Line 2: IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR
            *User's first and last initials (l), program name (P), 
            *date/time (M/D/Y,H:m), dimensional codes (d), scaling factors (S, s), 
            * energy (E) if modeling program input,
            * internal registry number (R) if input through MDL form.
            * can be blank */
            string line2 = string.Empty;
            if (mol.CreatedBy.Length == 2)
                line2 += mol.CreatedBy;
            else
                line2 += "  "; //no initials means two spaces added
            //The program is eight chars so we check our program name (generally SynType or ChemDraw)
            string buildSoft = mol.BuildSoftware;
            buildSoft = buildSoft.PadRight(8, ' ');
            line2 += buildSoft;

            //The next is the date
            string month; //month is not always 2 digits so we make sure it is
            if (mol.CreationTime.Month < 10)
            {
                month = mol.CreationTime.Month.ToString();
                month = month.PadLeft(2, '0');
            }
            else
                month = mol.CreationTime.Month.ToString();
            string day; //day is also not always 2 digits so we pad as well
            if (mol.CreationTime.Day < 10)
            {
                day = mol.CreationTime.Day.ToString();
                day = day.PadLeft(2, '0');
            }
            else
                day = mol.CreationTime.Day.ToString();
            
            int year = mol.CreationTime.Year;
            int hour = mol.CreationTime.Hour;
            int min = mol.CreationTime.Minute;
            Console.WriteLine("MolFileWriter: DT {0}/{1}/{2} {3}:{4}", day, month, year, hour, min);
            line2 += month + day + year.ToString().Substring(2) +hour.ToString() + min.ToString();
            //add in 2D at the end and write the line
            line2 += "2D";

            writer.WriteLine(line2);
            //Finally we add the 3rd line which is comments
            writer.WriteLine(mol.Comments);
        }
        private static void WriteCountsLine(StreamWriter writer, Molecule mol)
        {
            string line = string.Empty;
            string atomCount = mol.Atoms.Count.ToString();
            atomCount = atomCount.PadLeft(3, ' ');
            line += atomCount;

            //Next are the bonds
            string bondCount = mol.Bonds.Count.ToString();
            bondCount = bondCount.PadLeft(3, ' ');
            line += bondCount;
            //next we skip to the chiral spot
            line += "  0  ";
            if (mol.IsChiral)
                line += "1";
            else
                line += "0";
            //Then we write the rest of the line   0  0  0  0  0  0999 V2000
            line += "  0  0  0  0  0  0999 V2000";
            writer.WriteLine(line);

        }
        private static void WriteAtomBlock(StreamWriter writer, Molecule mol)
        {
            //It goes xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee for each atom
            foreach (Node atom in mol.Atoms.Values)
            {

                string line = string.Empty;
                string x = atom.Position.X.ToString();
                if (atom.Position.X == Math.Round(atom.Position.X)) //is it an intergral #
                    x += ".0000";
                x = x.PadLeft(10, ' '); //pad left so we get 10 chars
                line += x;
                string y = atom.Position.Y.ToString();
                if (atom.Position.Y == Math.Round(atom.Position.Y))
                    y += ".0000";
                y = y.PadLeft(10, ' ');
                line += y;
                string z = string.Empty;

                if (atom.Position.Z == 0 || Math.Round(atom.Position.Z) == atom.Position.Z)
                    z = atom.Position.Z.ToString() + ".0000";
                else
                    z = atom.Position.Z.ToString();
                z = z.PadLeft(10);
                line += z + " "; //one extra space after coords
                //next is atom sybmol
                string symbol = atom.Symbol;
                symbol = symbol.PadRight(4);
                line += symbol;
                line += "0  "; //mass difference is skipped
                //Next part is the charge
                line += atom.Charge + "  ";
                //next parity which is skipped
                line += "0  ";
                //next is hydrogen count
                line += atom.RequiredHydrogens + "  ";
                //skip next
                line += "0  ";
                //finally valence
                line += atom.Valence + "  0  0  0  0  0  0";
                writer.WriteLine(line);

            }
        }
        private static void WriteBondBlock(StreamWriter writer, Molecule mol)
        {
            // 111222tttsssxxxrrrccc first atom # second atom # bond type bond stereo 
            foreach (Bond bond in mol.Bonds)
            {
                Console.WriteLine("MolFileWriter.WriteBondBlock: atom {0} connected to atom {1}", bond.Node1.ID, bond.Node2.ID);
                string line = string.Empty;
                string atom1 = bond.Node1.ID.ToString();
                atom1 = atom1.PadLeft(3, ' ');
                line += atom1;
                string atom2 = bond.Node2.ID.ToString();
                atom2 = atom2.PadLeft(3, ' ');
                line += atom2;
                string order = bond.BondOrder.ToString();
                order = order.PadLeft(3, ' ');
                line += order;
                string stereo = bond.BondStereo.ToString();
                stereo = stereo.PadLeft(3, ' ');
                line += stereo;
                //For now we skip topology and reaction center status
                line += "  0  0  0";
                writer.WriteLine(line);
            }
        }
    }
}

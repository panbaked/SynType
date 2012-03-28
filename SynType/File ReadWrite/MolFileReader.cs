using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SynType.Chemical_Classes;
using SynType.Math_Classes;
namespace SynType
{
    public static class MolFileReader
    {
        public static void ReadMolData(string text, ref Molecule mol)
        {
            StringBuilder builder = new StringBuilder(text);
            System.Text.ASCIIEncoding encode = new ASCIIEncoding();
            byte[] byteArray = encode.GetBytes(text);
            using (Stream s = new MemoryStream(byteArray))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    ParseHeaderBlock(reader, ref mol);
                    ParseCountsLine(reader, ref mol);
                    ParseAtomBlock(reader, ref mol);
                    ParseBondBlock(reader, ref mol);
                    ParsePropertiesBlock(reader, ref mol);
                }
            }

        }
        public static void ReadMolFile(string filePath, ref Molecule mol)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                ParseHeaderBlock(reader, ref mol);
                ParseCountsLine(reader, ref mol);
                ParseAtomBlock(reader, ref mol);
                ParseBondBlock(reader, ref mol);
                ParsePropertiesBlock(reader, ref mol);
            }
        }
        private static void ParseHeaderBlock(StreamReader reader, ref Molecule mol)
        {
            //Line 1 : molecule name, can be blank, no longer than 80 letters
            mol.Name = reader.ReadLine();
            if (mol.Name.Length > 80) 
                Console.WriteLine("Name in mol file was too long");


            /*Line 2: IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR
             *User's first and last initials (l), program name (P), 
             *date/time (M/D/Y,H:m), dimensional codes (d), scaling factors (S, s), 
             * energy (E) if modeling program input,
             * internal registry number (R) if input through MDL form.
             * can be blank */
            string line2 = reader.ReadLine();
          
            if (!string.IsNullOrEmpty(line2))
            {
                mol.CreatedBy = line2.Substring(0, 2);
                mol.BuildSoftware = line2.Substring(2, 8);
                //To get the date time we need to shove in some characters so it parses correctly
                string originalDT = line2.Substring(10, 10);
                int month;
                int.TryParse(originalDT.Substring(0, 2), out month);
                int day;
                int.TryParse(originalDT.Substring(2, 2), out day);
                int year;
                int.TryParse(originalDT.Substring(4, 2), out year);
                year += 2000; //add 2000 to the year so we get the correct year, at least for the next 90 years
                int hour;
                int.TryParse(originalDT.Substring(6, 2), out hour);
                int min;
                int.TryParse(originalDT.Substring(8, 2), out min);
                DateTime dt = new DateTime(year, month, day, hour, min, 0);
                //To be added if needed
                Console.WriteLine("MOL creation time was changed to " + dt.ToString());
                mol.CreationTime = dt;
            }
            //Line 3 is for comments
            mol.Comments = reader.ReadLine();
             
        }
        private static void ParseCountsLine(StreamReader reader, ref Molecule mol)
        {
            //it goes
            //aaabbblllfffcccsssxxxrrrpppiiimmm
            string countsLine = reader.ReadLine();
            //We split by spaces 
            string[] delim = {" "};
            string[] parts = countsLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            
            //Only three of the parts are useful, number of atoms, number of bonds
            //and whether or not the molecule is chiral, so spots 0, 1, 4
            int nAtoms;
            int nBonds;
            int isChiral;

            bool result = int.TryParse(parts[0], out nAtoms);
            if (!result)
            {
                Console.WriteLine("Could not get the number of atoms");
            }
            else
                mol.NumberOfAtoms = nAtoms;

            result = int.TryParse(parts[1], out nBonds);
            if (!result)
            {
                Console.WriteLine("Could not get the númber of bonds");
            }
            else
                mol.NumberOfBonds = nBonds;

            result = int.TryParse(parts[4], out isChiral);
            if (!result)
            {
                Console.WriteLine("Could not get whether molecule is chiral.");
            }
            else
            {
                if (isChiral == 0) mol.IsChiral = false;
                else mol.IsChiral = true;
            }
      
        }
        private static void ParseAtomBlock(StreamReader reader, ref Molecule mol)
        {
            //It goes xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee for each atom
            //From the counts line we know how many atoms to expect
            for (int i = 1; i <= mol.NumberOfAtoms; i++)
            {
                string line = reader.ReadLine();
                //First the coords
                Node atomToAdd = new Node();
                double x, y, z;
                bool result = double.TryParse(line.Substring(0, 10), out x);
                if (!result)
                    Console.WriteLine("Could not parse x coords");
                result = double.TryParse(line.Substring(10, 10), out y);
                if (!result)
                    Console.WriteLine("Could not parse y coords");

                result = double.TryParse(line.Substring(20,10), out z);
                if (!result)
                    Console.WriteLine("Could not parse z coords");
                if (z == 0) z = 0.0000;

                Vector3 coords = new Vector3(x, y, z);
                atomToAdd.Position = coords;
                
                //Next we split the string into parts and the 3rd entry is the atomic symbol
                string[] delim = { " " };
                string[] parts = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                atomToAdd.Symbol = parts[3];
                
                //The fourth is mass difference ?!?!? NOT added
                int massDifference;
                result = int.TryParse(parts[4], out massDifference);
                if (!result)
                    Console.WriteLine("Could not parse mass difference");
                else
                    atomToAdd.MassDiff = massDifference;
                //The fifth is charge
                int charge;
                result = int.TryParse(parts[5], out charge);
                if (!result)
                    Console.WriteLine("Could not parse atomic charge");
                else
                {
                    atomToAdd.Charge = charge;
                    Console.WriteLine("ATOM " + atomToAdd.Symbol + " CHARGE " + charge);
                }

                //The sixth is atom stereo parity which is ignored

                //The seventh is hydrogen count
                int hCount;
                result = int.TryParse(parts[7], out hCount);
                if (!result)
                    Console.WriteLine("Could not parse hydrogen count");
                else
                {
                    if (hCount > 0)
                        Console.WriteLine("hcount above 0");
                    Console.WriteLine("ATOM " + atomToAdd.Symbol + " HS " + hCount);
                    atomToAdd.RequiredHydrogens = hCount;
                }
                //Stereo care box is skipped
                //The ninth is valence 0 = no marking default, includes number of bonds on this atom including implied
                //hydrogens
                int valence;
                result = int.TryParse(parts[9], out valence);
                if (!result)
                    Console.WriteLine("Could not parse valence");
                else
                    atomToAdd.Valence = valence;

                atomToAdd.ID = i;
                //finally we add the atom to the list
                mol.Atoms.Add(i, atomToAdd);
                
            }
            
        }
        private static void ParseBondBlock(StreamReader reader, ref Molecule mol)
        {
            //One bond per line in format
            // 111222tttsssxxxrrrccc first atom # second atom # bond type bond stereo 
            //Generally there will be spaces otherwise this fucks up
            string line = string.Empty;
            for (int i = 0; i < mol.NumberOfBonds; i++)
            {
                string[] delim = { " " };
                string[] parts = reader.ReadLine().Split(delim, StringSplitOptions.RemoveEmptyEntries);
                //The first two pars are atoms 1 and 2
                int atom1Key, atom2Key;
                bool result = int.TryParse(parts[0], out atom1Key);
                if (!result)
                    Console.WriteLine("Could not get the first atom number");
                result = int.TryParse(parts[1], out atom2Key);
                if (!result)
                    Console.WriteLine("Could not get the second atom number");

                int bondOrder;
                result = int.TryParse(parts[2], out bondOrder);
                if (!result)
                    Console.WriteLine("Could not get the bond order for "+atom1Key+"'s connection to"+atom2Key);

                //Now we build the bond
                try
                {
                    Bond bondToAdd = new Bond(mol.Atoms[atom1Key], mol.Atoms[atom2Key], bondOrder);
                    mol.Bonds.Add(bondToAdd);

                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("The keys found were out of range 1:" + atom1Key + " 2:" + atom2Key + " atom count" +
                        mol.Atoms.Count);
                }
                

            }
           

        }
        private static void ParsePropertiesBlock(StreamReader reader, ref Molecule mol)
        {
            //properties block is mmm lines (999) and ends with M  END
            //We check real fast to see if there is even any props
            string line = reader.ReadLine();
            if (line == "M  END")
            {
                Console.WriteLine("Mol file has been completely parsed \n");
                return; //we're done jump out
            }
        }
    }
    
}

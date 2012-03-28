using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SynType.Math_Classes;

namespace SynType.Chemical_Classes
{
    //an enumeration of types of compounds
    public enum COMPOUND_TYPES
    {
        Reactant,
        Reagent,
        Product,

    }
    public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs e);
    public class DataChangedEventArgs : EventArgs
    {
        public int field;
        public DataChangedEventArgs(int field)
        {
            this.field = field;
        }

    }
    public enum COMPOUND_FIELDS
    {
        Molecule,
        RefName,
        LocalID,
        ID,
        FileID,
        Density,
        Mols,
        Mass,
        Volume,
        Solvent,
        IsLimiting,
        Equivalency,
        State,
        Type,
        IsTarget
    }
    public class Compound
    {
        //Anytime the data for a compound is changed we fire the DataChanged event, to let program components know they need to update themselves.
        public event DataChangedEventHandler DataChanged;
        Molecule molecule;
        Molecule visualMolecule;
        string molFileName;
        string directory;

        string refName; // a reference name, this is what the user calls the compound
        string localid; //an id such as rc1, rg1, or p1
        string id;
        int fileid = 0; //In the files each compound is recognized by a five digit number, so we can attach mol data
        //A compound has a molecule referene, as well as id tags, its state, what category its in (react, etc)
        double density = 1;
        //The following properties have a number asscociated with a unit
        Unit mols;
        Unit mass;
        Unit volume;
        //The compound can also be used as a solution, kept as a string for the solvent and double for concentration dictionary
        Solution solution;
        
        public string Formula { get { return molecule.Formula; } }
        public string ID { get { return id; } set { id = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.ID));} }
        public double MolWeight { get { return Math.Round(molecule.MolecularWeight, 2); } }
        bool isLimiting;
        double equivalency = 1;

        int state;
        int type;
        bool isTarget;
        public Molecule VisualMolecule { get { return visualMolecule; } set { visualMolecule = value; } }
        public Molecule MoleculeData { get { return molecule; }
            set 
            { 
                molecule = value; 
                //Now we make a copy of this for visual molecule
                visualMolecule = (Molecule) molecule.Clone();
                OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Molecule));
            } }
        public bool DisplayRefNameWhenDrawn
        {
            get; 
            set;
        }
        public bool IsTarget { get { return isTarget; } set { isTarget = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.IsTarget)); } }
        public COMPOUND_TYPES Type { get { return (COMPOUND_TYPES)type; } 
            set { type = (int)value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Type)); } }
        public PHASE_STATE State
        {
            get { return (PHASE_STATE)state; }
            set { state = (int)value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.State)); }
        }
        public Unit Mols { get { return mols; } set { mols = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Mols)); } }
        public double MolsValue { get { return mols.Value; } set { mols.Value = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Mols)); } }
        public string MolsUnit { get { return mols.UnitPrefix + mols.UnitType;  } }

        public double Density { get { return Math.Round(density, 2); } set { density = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Density)); } }
        public Unit Mass { get { return mass; } set { mass = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Mass)); } }
        public double MassValue
        {
            get
            {
                return Mass.Value;
            }
            set
            {
                Mass.Value = value;
                OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Mass));
            }
        }
        public string MassUnit { get { return Mass.UnitPrefix + Mass.UnitType; } }
        public Unit Volume { get { return volume; } set { volume = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Volume)); } }
        public double VolumeValue
        {
            get
            {
                return Volume.Value;
            }
            set
            {
                Volume.Value = value;
            }
        }
        public string VolumeUnit { get { return Volume.UnitPrefix + Volume.UnitType; } }
        public Solution Solution { get { return solution; } set { solution = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Solvent)); } }
        public string SolventName { get { return solution.SolventName; } set { solution.SolventName = value; } }
        public Unit SolutionConc { get { return solution.Concentration; } set { solution.Concentration = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Solvent)); } }
        public string Name { get { return molecule.Name; } }
        public int FileID { get { return fileid; } set { fileid = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.FileID)); } }
        public string RefName { get { return refName; } set { refName = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.RefName)); } }
        public string LocalID { get { return localid; } set { localid = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.LocalID)); } }
        //These properties only exist for reactants
        public bool IsLimiting { get { return isLimiting; } set { isLimiting = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.IsLimiting)); } }
        public double Equivalency { get { return equivalency; } set { equivalency = value; OnDataChanged(new DataChangedEventArgs((int)COMPOUND_FIELDS.Equivalency)); } }


        public Compound(int compoundType, string formula, int compoundState, bool isTarget, double weight,
            double density, double mols, double mass, double volume)
        {
            this.type = compoundType;
            //NOT DONE ASDKJLSADKJHASDKA
            throw new NotImplementedException();
        }
        public Compound(Molecule mol, string fileName, string directory, int compoundType, string formula,
            int compoundState, bool isTarget, double weight, double density, Unit mols,
            Unit mass, Unit volume)
        {
            molFileName = fileName;
            this.directory = directory;
            this.type = compoundType;
            this.molecule = mol;
            this.state = compoundState;
            this.isTarget = isTarget;
            this.density = density;
            this.mols = mols;
            this.mass = mass;
            this.volume = volume;
            DisplayRefNameWhenDrawn = false;
        }
        public Compound(Molecule mol, string refname, int compoundType, int compoundState, bool isLimiting, bool isTarget,
            double density, Unit mols, Unit mass, Unit volume, Solution solution)
        {

            this.type = compoundType;
            this.refName = refname;
            this.molecule = mol;
            this.state = compoundState;
            this.isLimiting = isLimiting;
            this.isTarget = isTarget;
            this.density = density;
            this.mols = mols;
            this.mass = mass;
            this.volume = volume;
            this.solution = solution;
            DisplayRefNameWhenDrawn = false;
        }
        public Compound(string name, string refid, string localid, int fileid, int compoundType, int compoundState, bool isLimiting, bool isTarget, string formula,
            double density, Unit mols, Unit mass, Unit volume, Solution solution)
        {

            this.type = compoundType;
            this.molecule = new Molecule();
            this.molecule.Name = name;
            this.refName = refid;
            this.localid = localid;
            this.fileid = fileid;
            this.state = compoundState;
            this.isLimiting = isLimiting;
            this.isTarget = isTarget;
            this.molecule.Formula = formula;
            this.density = density;
            this.mols = mols;
            this.mass = mass;
            this.volume = volume;
            this.solution = solution;
            DisplayRefNameWhenDrawn = false;
        }
        public Compound(Molecule mol)
        {

            this.molecule = mol;
            this.mols = new Unit(0, "mol", (int)UNIT_POWERS.none);
            this.mass = new Unit(0, "g", (int)UNIT_POWERS.none);
            this.Volume = new Unit(0, "l", (int)UNIT_POWERS.m);
            DisplayRefNameWhenDrawn = false;
        }
        //Invoke the data changed event
        protected virtual void OnDataChanged(DataChangedEventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }
        public void SetProperties(int compoundType, int compoundState, Unit mols, Unit mass, Unit volume, Solution solution)
        {
            this.type = compoundType;
            this.state = compoundState;
            this.mols = mols;
            this.mass = mass;
            this.volume = volume;
            this.solution = solution;
        }
        public void SetFileAndDir(string filename, string directory)
        {
            this.molFileName = filename;
            this.directory = directory;
        }
        public override string ToString()
        {
            
            string molsUnit = string.Empty, massUnit = string.Empty, volUnit = string.Empty;
            string output = "Type:" + Type + " Name: " + Name + " Ref:" + RefName + " Formula:" + Formula + " MW:" + MolWeight + " State:" +
                (PHASE_STATE)State + " Mols:" + Mols.Value + " " + Mols.UnitPrefix + Mols.UnitType;
            output += " Mass:" + Mass.Value + " " + Mass.UnitPrefix + Mass.UnitType;
            output += " Volume:" + Volume.Value + " " + Volume.UnitPrefix + Volume.UnitType;
           

            return output;
        }
       
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using SynType.Math_Classes;

namespace SynType.Chemical_Classes
{
    public delegate void SynthesisChangedEventHandler(object sender, EventArgs e);
    public class Synthesis
    {
        public event SynthesisChangedEventHandler SynthesisChanged;
        //A Synthesis contains reactants, reagents, products and procedure
        ObservableCollection<Compound> reactants;
        ObservableCollection<Compound> reagents;
        ObservableCollection<Compound> products;
        Procedure procedure;
        private bool hasLimitingReactant = false;
        private bool supressCompoundEvents = false;
        public ObservableCollection<Compound> Reactants { get { return reactants; } set { reactants = value; } }
        public ObservableCollection<Compound> Reagents { get { return reagents; } set { reagents = value; } }
        public ObservableCollection<Compound> Products { get { return products; } set { products = value; } }
        public Procedure Proc { get { return procedure; } set { procedure = value; } }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public SynthesisInfo PreviousStep { get; set; }
        public SynthesisInfo NextStep { get; set; }
        public Compound LimitingReactant
        {
            get
            {
                foreach (Compound c in reactants)
                {
                    if (c.IsLimiting)
                        return c;
                }
                return null;
            }
        }
        public Compound TargetProduct
        {
            get
            {
                foreach (Compound c in products)
                {
                    if (c.IsTarget)
                        return c;
                }
                return null;
            }
        }
        public Synthesis()
        {
            reactants = new ObservableCollection<Compound>();
            reactants.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(reactants_CollectionChanged);
            reagents = new ObservableCollection<Compound>();
            reagents.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(reagents_CollectionChanged);
            products = new ObservableCollection<Compound>();
            products.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(products_CollectionChanged);
            procedure = new Procedure(this, string.Empty);

        }

        public Synthesis(string name, int projectID)
        {
            reactants = new ObservableCollection<Compound>();
            reactants.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(reactants_CollectionChanged);
            reagents = new ObservableCollection<Compound>();
            reagents.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(reagents_CollectionChanged);
            products = new ObservableCollection<Compound>();
            products.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(products_CollectionChanged);
            procedure = new Procedure(this, string.Empty);
            Name = name;
            ProjectID = projectID;
            PreviousStep = null;
            NextStep = null;
            
        }

       
        public Synthesis(ObservableCollection<Compound> reactants, ObservableCollection<Compound> reagents, ObservableCollection<Compound> products, string name, int projectID, SynthesisInfo previousStep, SynthesisInfo nextStep, string procedureText)
        {
            this.reactants = reactants;
            this.reagents = reagents;
            this.products = products;
            this.Name = name;
            this.ProjectID = projectID;
            this.PreviousStep = previousStep;
            this.NextStep = nextStep;
            this.Proc = new Procedure(this, procedureText);
        }
        public Synthesis(Dictionary<Compound, int> molDict)
        {
            reactants = new ObservableCollection<Compound>();
            reagents = new ObservableCollection<Compound>();
            products = new ObservableCollection<Compound>();

            //Set up the reactants etc by going through the dict
            foreach (Compound mol in molDict.Keys)
            {
                if ((COMPOUND_TYPES)molDict[mol] == COMPOUND_TYPES.Reactant)
                    reactants.Add(mol);
                else if ((COMPOUND_TYPES)molDict[mol] == COMPOUND_TYPES.Reagent)
                    reagents.Add(mol);
                else
                    products.Add(mol);
            }
        }
        void reactants_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
          
            //IF we add an entry hook up event listerners for the compound changing
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Compound newCompound in e.NewItems)
                {
                    newCompound.DataChanged += new DataChangedEventHandler(CompoundDataChanged);
                    if (newCompound.IsLimiting)
                    {
                        if (hasLimitingReactant)
                        {
                            newCompound.IsLimiting = false; //we can only have one limiting reactant
                        }
                        else
                        {
                            hasLimitingReactant = true;
                            BalanceFromEquivalency(newCompound);
                        }
                    }
                    else
                    {
                        SetEquivalency(newCompound);
                    }
                }
            }
                //If we are removing an entry remove the event listener for this compound
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Compound oldCompound in e.OldItems)
                {
                    oldCompound.DataChanged -= CompoundDataChanged;
                    if (oldCompound.IsLimiting)
                        hasLimitingReactant = false;
                }
            }
            OnSynthesisChanged(EventArgs.Empty);
        }
        void products_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //IF we add an entry hook up event listerners for the compound changing
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Compound newCompound in e.NewItems)
                {
                    newCompound.DataChanged += new DataChangedEventHandler(CompoundDataChanged);
                    
                }
            }
            //If we are removing an entry remove the event listener for this compound
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Compound oldCompound in e.OldItems)
                {
                    oldCompound.DataChanged -= CompoundDataChanged;
                }
            }
            OnSynthesisChanged(EventArgs.Empty);
        }

        void reagents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //IF we add an entry hook up event listerners for the compound changing
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Compound newCompound in e.NewItems)
                {
                    newCompound.DataChanged += new DataChangedEventHandler(CompoundDataChanged);
                    SetEquivalency(newCompound);

                }
            }
            //If we are removing an entry remove the event listener for this compound
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Compound oldCompound in e.OldItems)
                {
                    oldCompound.DataChanged -= CompoundDataChanged;
                }
            }
            OnSynthesisChanged(EventArgs.Empty);
        }
        void CompoundDataChanged(object sender, DataChangedEventArgs e)
        {
            //When we are changing things programatically we don't want to hear about those changes
            if (supressCompoundEvents)
                return;
            Console.WriteLine("Synthesis sees that something has changed in a reactant "+ (COMPOUND_FIELDS)e.field);
           
            Compound _sender = sender as Compound;

            if (_sender == null)
                return;
            //If the field is a change in type then we need to move this compound to a differnt list
            if (e.field == (int)COMPOUND_FIELDS.Type)
            {
                if (reactants.Contains(_sender))
                    reactants.Remove(_sender);
                if (reagents.Contains(_sender))
                    reagents.Remove(_sender);
                if (products.Contains(_sender))
                    products.Remove(_sender);

                if (_sender.Type == COMPOUND_TYPES.Reactant)
                {
                    reactants.Add(_sender);
                }
                else if (_sender.Type == COMPOUND_TYPES.Reagent)
                {
                    reagents.Add(_sender);
                }
                else
                {
                    products.Add(_sender);
                }
                OnSynthesisChanged(EventArgs.Empty); //let other components know that the synthesis has changed
                return;
            }
            //When we have mols changed and a limiting reactant (the sender) we want to balance everything with respect to the limiting reactant
            //When mols are changed and there is a limiting reactant, but it is NOT the sender we want to change this compounds equivalency with respect to the limiting reactant
            if (hasLimitingReactant && e.field == (int)COMPOUND_FIELDS.Mols)
            {
                if (_sender.IsLimiting)
                {
                    //If this is the limiting reactant we recalculate the other reactants with respect to this one based on their equivalency
                    //but first we need to calculate new mass and volume for the sender
                    supressCompoundEvents = true;
                    _sender.Mass = new Unit(_sender.Mols.BaseValue * _sender.MolWeight, "g", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Volume = new Unit(_sender.Mass.BaseValue / (_sender.Density * Math.Pow(10, 3)), "l", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Volume = new Unit(_sender.Mols.BaseValue / _sender.SolutionConc.BaseValue, "l", (int)UNIT_POWERS.none);
                    BalanceFromEquivalency(_sender);
                    supressCompoundEvents = false;
                }
                else
                {
                    //if the sender is not the limiting reactant we calculate a new mass and volume, and change the equivalency
                    //m = n * M
                    supressCompoundEvents = true;
                    _sender.Mass = new Unit(_sender.Mols.BaseValue * _sender.MolWeight, "g", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Volume = new Unit(_sender.Mass.BaseValue / (_sender.Density * Math.Pow(10, 3)), "l", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Volume = new Unit(_sender.Mols.BaseValue / _sender.SolutionConc.BaseValue, "l", (int)UNIT_POWERS.none);
                    _sender.Equivalency = _sender.Mols.BaseValue / LimitingReactant.Mols.BaseValue;
                    
                    OnSynthesisChanged(EventArgs.Empty);
                    supressCompoundEvents = false;
                }
                return;    
            }
            //When we have mass changed and a limiting reactant (the sender) we want to balance everything with respect to the limiting reactant
            //When mass are changed and there is a limiting reactant, but it is NOT the sender we want to change this compounds equivalency with respect to the limiting reactant
            if (hasLimitingReactant && e.field == (int)COMPOUND_FIELDS.Mass)
            {
                if (_sender.IsLimiting)
                {
                    //If this is the limiting reactant we recalculate the other reactants with respect to this one based on their equivalency
                    //but first we need our new mols value
                    supressCompoundEvents = true;
                    _sender.Mols = new Unit(_sender.Mass.BaseValue / _sender.MolWeight, "mol", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Volume = new Unit(_sender.Mass.BaseValue / (_sender.Density * Math.Pow(10, 3)), "l", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Volume = new Unit(_sender.Mols.BaseValue / _sender.SolutionConc.BaseValue, "l", (int)UNIT_POWERS.none);
                    BalanceFromEquivalency(_sender);
                    supressCompoundEvents = false;
                }
                else
                {
                    //if the sender is not the limiting reactant we calculate a new mols and volume, and change the equivalency
                    //m = n * M
                    supressCompoundEvents = true;
                    _sender.Mols = new Unit(_sender.Mass.BaseValue / _sender.MolWeight, "mol", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Volume = new Unit(_sender.Mass.BaseValue / (_sender.Density * Math.Pow(10, 3)), "l", (int)UNIT_POWERS.none);
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Volume = new Unit(_sender.Mols.BaseValue / _sender.SolutionConc.BaseValue, "l", (int)UNIT_POWERS.none);
                    _sender.Equivalency = _sender.Mols.BaseValue / LimitingReactant.Mols.BaseValue;
                    
                    OnSynthesisChanged(EventArgs.Empty);
                    supressCompoundEvents = false;
                }
            }
            //When we have volume changed and a limiting reactant (the sender) we want to balance everything with respect to the limiting reactant
            //When volumes are changed and there is a limiting reactant, but it is NOT the sender we want to change this compounds equivalency with respect to the limiting reactant
            if (hasLimitingReactant && e.field == (int)COMPOUND_FIELDS.Volume)
            {
                if (_sender.IsLimiting)
                {
                    //If this is the limiting reactant we recalculate the other reactants with respect to this one based on their equivalency
                    //but first we need our new mols value
                    supressCompoundEvents = true;
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Mols = new Unit(_sender.SolutionConc.BaseValue * _sender.Volume.BaseValue, "mol", (int)UNIT_POWERS.none);
                    else if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Mols = new Unit(_sender.Density * Math.Pow(10, 3) * _sender.Volume.BaseValue / _sender.MolWeight, "mol", (int)UNIT_POWERS.none);
                    else
                        return;
                   
                    _sender.Mass = new Unit(_sender.Mols.BaseValue * _sender.MolWeight, "g", (int)UNIT_POWERS.none);
                    BalanceFromEquivalency(_sender);
                    supressCompoundEvents = false;
                }
                else
                {
                    //if the sender is not the limiting reactant we calculate a new mols and mass, and change the equivalency
                    //m = n * M
                    supressCompoundEvents = true;
                    if (_sender.State == PHASE_STATE.Solution)
                        _sender.Mols = new Unit(_sender.SolutionConc.BaseValue * _sender.Volume.BaseValue, "mol", (int)UNIT_POWERS.none);
                    else if (_sender.State == PHASE_STATE.Liquid)
                        _sender.Mols = new Unit(_sender.Density * Math.Pow(10, 3) * _sender.Volume.BaseValue / _sender.MolWeight, "mol", (int)UNIT_POWERS.none);
                    else
                        return;
                    _sender.Mass = new Unit(_sender.Mols.BaseValue * _sender.MolWeight, "g", (int)UNIT_POWERS.none);

                    _sender.Equivalency = _sender.Mols.BaseValue / LimitingReactant.Mols.BaseValue;
                    OnSynthesisChanged(EventArgs.Empty);
                    supressCompoundEvents = false;
                }
            }
            //Changing density we change the volume value
            if (e.field == (int)COMPOUND_FIELDS.Density)
            {
                supressCompoundEvents = true;
                _sender.Volume = new Unit(_sender.Mols.BaseValue * _sender.MolWeight / (_sender.Density * Math.Pow(10, 3)), "l", (int)UNIT_POWERS.none); //density is g/ml so our result is in ml
                OnSynthesisChanged(EventArgs.Empty);
                supressCompoundEvents = false;
            }
            if (e.field == (int)COMPOUND_FIELDS.Solvent)
            {
                supressCompoundEvents = true;
                _sender.Volume = new Unit(_sender.Mols.BaseValue / _sender.SolutionConc.BaseValue, "l", (int)UNIT_POWERS.none);
                OnSynthesisChanged(EventArgs.Empty);
                supressCompoundEvents = false;
            }
            //When the user changes the equivalency then we get new values for mols, mass and volume 
            //if the sender is the limiting reactant then equivalency changes have no meaning, and we need a limiting reactant if we are to balance anything
            if (e.field == (int)COMPOUND_FIELDS.Equivalency && LimitingReactant != null && _sender != LimitingReactant)
            {
                _sender.Mols = new Unit(LimitingReactant.Mols.BaseValue * _sender.Equivalency, "mol", (int)UNIT_POWERS.none);
                _sender.Mass = new Unit(_sender.Mols.BaseValue * _sender.MolWeight, "g", (int)UNIT_POWERS.none);
                if (_sender.State == PHASE_STATE.Liquid)
                    _sender.Volume = new Unit(_sender.Mass.BaseValue / _sender.Density, "l", (int)UNIT_POWERS.none);

            }
        }
        /// <summary>
        /// Balances the other reactants and reagents from their equivalency to the sender which is the limiting reactant
        /// </summary>
        /// <param name="_sender">The limiting reactant</param>
        private void BalanceFromEquivalency(Compound _sender)
        {
            if (reactants.Count >= 2)
            {

                foreach (Compound reactant in reactants)
                {
                    //continue on if the reactant is this one
                    if (reactant == _sender)
                        continue;

                    reactant.Mols = new Unit(_sender.Mols.BaseValue * reactant.Equivalency, _sender.Mols.UnitType, _sender.Mols.Power);
                    reactant.Mass = new Unit(reactant.Mols.BaseValue * reactant.MolWeight, "g", (int)UNIT_POWERS.none);
                    //Volume is density / mass and we multiply by 10^-3 to get it in liters (this is expecting density as g/ml
                    reactant.Volume = new Unit(reactant.Mass.BaseValue / reactant.Density, "l", (int)UNIT_POWERS.none);
                }

            }
            foreach (Compound reagent in reagents)
            {
                if (reagent == _sender)
                    continue;
                reagent.Mols = new Unit(_sender.Mols.BaseValue * reagent.Equivalency, _sender.Mols.UnitType, _sender.Mols.Power);
                reagent.Mass = new Unit(reagent.Mols.BaseValue * reagent.MolWeight, "g", (int)UNIT_POWERS.none);
                //Volume is density / mass and we multiply by 10^-3 to get it in liters (this is expecting density as g/ml
                reagent.Volume = new Unit(reagent.Mass.BaseValue / reagent.Density, "l", (int)UNIT_POWERS.none);
            }
            OnSynthesisChanged(EventArgs.Empty);
            
        }
        private void SetEquivalency(Compound comp)
        {
            if (LimitingReactant != null)
                comp.Equivalency = comp.Mols.BaseValue / LimitingReactant.Mols.BaseValue;
            else
                comp.Equivalency = 1;
        }
        protected virtual void OnSynthesisChanged(EventArgs e)
        {
            if (SynthesisChanged != null)
                SynthesisChanged(this, e);
        }
        public void UpdateSynthesis()
        {
            Console.WriteLine("Updating synthesis now");
            //We go through each compound and check whether the user has changed 
            SetRefIDs();
            OnSynthesisChanged(EventArgs.Empty);
        }
        public void SetRefIDs()
        {
            int count = 0;
            foreach (Compound comp in reactants)
            {
                comp.LocalID = "rc" + count;
                count++;
            }
            count = 0;
            foreach (Compound comp in reagents)
            {
                comp.LocalID = "rg" + count;
                count++;
            }
            count = 0;
            foreach (Compound comp in products)
            {
                comp.LocalID = "p" + count;
                count++;
            }
        }
        public Compound GetCompound(string refid)
        {
            foreach (Compound comp in reactants)
            {
                if (comp.LocalID == refid)
                    return comp;
            }
            foreach (Compound comp in reagents)
            {
                if (comp.LocalID == refid)
                    return comp;
            }
            foreach (Compound comp in products)
            {
                if (comp.LocalID == refid)
                    return comp;
            }
            return null;
        }
        public void RemoveCompound(Compound comp)
        {
            if (reactants.Contains(comp))
                reactants.Remove(comp);
            if (reagents.Contains(comp))
                reagents.Remove(comp);
            if (products.Contains(comp))
                products.Remove(comp);

            OnSynthesisChanged(EventArgs.Empty);
        }
        public List<Compound> AllCompounds
        {
            get
            {
                
                List<Compound> all = new List<Compound>();
                foreach (Compound c in reactants)
                {
                    all.Add(c);
                }
                foreach (Compound c in reagents)
                {
                    all.Add(c);
                }
                foreach (Compound c in products)
                {
                    all.Add(c);
                }
                return all;
            }
        }
        public DataTable GetCompoundTable()
        {
            DataTable table = new DataTable("Compound Table");
            //init the columns
            table.Columns.Add("Formula", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("ID",typeof(string));
            table.Columns.Add("MW (g/mol)",typeof(float));
            table.Columns.Add("Type",typeof(string));
            table.Columns.Add("n (mol)", typeof(float));
            table.Columns.Add("equiv.",typeof(float));
            table.Columns.Add("State",typeof(string));
            table.Columns.Add("m (g)", typeof(float));
            table.Columns.Add("d (g/mol)",typeof(float));
            table.Columns.Add("vol (ml)",typeof(float));
            
            foreach (Compound c in reactants)
            {
                table.Rows.Add(c.Formula, c.Name, c.LocalID, c.MolWeight, c.Type, c.MolsValue, c.Equivalency, c.State, c.MassValue, c.Density, c.VolumeValue);
            }
            foreach (Compound c in reagents)
            {
                table.Rows.Add(c.Formula, c.Name, c.LocalID, c.MolWeight, c.Type, c.MolsValue, c.Equivalency, c.State, c.MassValue, c.Density, c.VolumeValue);
            }
            foreach (Compound c in products)
            {
                table.Rows.Add(c.Formula, c.Name, c.LocalID, c.MolWeight, c.Type, c.MolsValue, c.Equivalency, c.State, c.MassValue, c.Density, c.VolumeValue);
            }
            return table;
        }
        public override string ToString()
        {
            return "Synthesis: " + Name + " has " + AllCompounds.Count + " compounds."; 
        }
    }

    public class SynthesisInfo
    {
        public string Name { get; set; }
        public int ProjectID { get; set; }

        public SynthesisInfo(string name, int projectID)
        {
            Name = name;
            ProjectID = projectID;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynType.Math_Classes;

namespace SynType.Chemical_Classes
{
    public class Node : ICloneable
    {
        Vector3 position;
        int id;
        int zOrder;
        byte stereochem;
        bool isAtom; //nodes can be atoms or fragments
        private int pTableNumber;
        private string symbol;
        private float mass;
        private float massDiff;
        private int charge;
        private int valence;
        private int requiredHydrogens;
        private int implicitHydrogens;
        private Fragment fragment;

        

        public Vector3 Position { get { return position; } set { position = value; } }
        public int ID { get { return id; } set { id = value; } }
        public int ZOrder { get { return zOrder; } set { zOrder = value; } }
        public byte Stereochem { get { return stereochem; } set { stereochem = value; } }
        public bool IsAtom { get { return isAtom; } set { isAtom = value; } }
        public int AtomicNumber { get { return pTableNumber; } set { pTableNumber = value; } }
        public string Symbol { get { return symbol; } set { symbol = value; } }
        public float Mass { get { return mass; } set { mass = value; } }
        public float MassDiff { get { return massDiff; } set { massDiff = value; } }
        public int Charge { get { return charge; } set { charge = value; } }
        public int Valence { get { return valence; } set { valence = value; } }
        public int RequiredHydrogens { get { return requiredHydrogens; } set { requiredHydrogens = value; } }
        public int ImplicitHydrogens { get { return implicitHydrogens; } set { implicitHydrogens = value; } }
        public Fragment NodeFragment { get { return fragment; } set { fragment = value; } }

        public override string ToString()
        {
            if (isAtom)
                return "\nAtom #" + AtomicNumber + " : " + symbol + " m=" + mass + " charge " + charge;
            else
                return "This is a fragment";

        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

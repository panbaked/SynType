using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.Chemical_Classes
{
    /// <summary>
    /// Fragments are generally used for CDX file reading, as molecules are built from atoms and not fragments, fragments can however be converted to molecules
    /// </summary>
    public class Fragment 
    {
        private List<Node> nodeList;
        private List<Bond> bondList;
        private float weight;
        
        public float Weight
        {
            get
            {
                float finalWeight = 0f;
                foreach (Node node in nodeList)
                {
                    finalWeight += node.Mass;
                }
                return finalWeight;
            }
        }
        public Molecule GetMoleculeFromFrag()
        {
            throw new NotImplementedException();
        }
    }
}

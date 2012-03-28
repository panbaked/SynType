using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynType.Chemical_Classes;

namespace SynType.File_ReadWrite
{
    public struct Header
    {
        public Header(string user, DateTime dt, int id)
        {
            userName = user;
            creationTime = dt;
            idNumber = id;
            idTag = string.Empty;
            reactantCount = 0;
            reagentCount = 0;
            productCount = 0;
        }
        public string userName;
        public DateTime creationTime;
        public int idNumber;
        public string idTag; //What the user calls the synthesis
        public int reactantCount;
        public int reagentCount;
        public int productCount;

    }
    //The edit class holds a single synthesis along with when it was created
    public class Edit
    {
        DateTime editTime;
        public DateTime EditTime { get { return editTime; } set { editTime = value; } }
        Synthesis synth;
        public Synthesis Synth { get { return synth; } set { synth = value; } }

        public Edit() { }
        public Edit(Synthesis synth, DateTime dt)
        {
            this.synth = synth;
            editTime = dt;
        }
    }

}

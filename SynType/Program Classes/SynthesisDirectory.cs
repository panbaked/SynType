using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SynType.Chemical_Classes;

namespace SynType.Program_Classes
{
    public class SynthesisDirectory
    {
        public DirectoryInfo DInfo { get; set; }
        public Synthesis Synth { get; set; }
        public Project OwningProject { get; set; }
        public string Name { get { return Synth.Name; } }

        public SynthesisDirectory(string path, Project project)
        {
            DInfo = new DirectoryInfo(path);
            OwningProject = project;
            FileInfo[] synthFInfo = DInfo.GetFiles("*.syn", SearchOption.TopDirectoryOnly);
            string synthName = Path.GetFileNameWithoutExtension(synthFInfo[0].Name);
            Synth = OwningProject.FindSynthesis(synthName);
        }
    }
}

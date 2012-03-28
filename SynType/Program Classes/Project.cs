using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynType.File_ReadWrite;
using System.IO;
using SynType.Chemical_Classes;
using SynType.File_ReadWrite.SynType;

namespace SynType.Program_Classes
{
    public class Project
    {
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public User ProjectUser { get; set; }
        public List<Synthesis> AllSyntheses { get; set; }

        public Project(string projectName, string projectPath, User projectUser)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectUser = projectUser;
            AllSyntheses = new List<Synthesis>();
            LoadEdits();
        }

        public void LoadEdits()
        {
            string[] synthFilePaths = Directory.GetFiles(ProjectPath, "*.syn", SearchOption.AllDirectories);
            foreach (string path in synthFilePaths)
            {
                Synthesis synth = SynTypeFileReader.ReadFile(path);
                AllSyntheses.Add(synth);
            }
            Console.WriteLine("Loaded all syntheses for project " + ProjectName + " there were " + AllSyntheses.Count + " items.");
        }

        public Synthesis FindSynthesis(string name)
        {
            foreach (Synthesis synth in AllSyntheses)
            {
                if (synth.Name == name)
                {
                    Console.WriteLine("Returning a synthesis " + synth.ToString());
                    return synth;
                }
            }
            Console.WriteLine("Could not find the synthesis in the project!");
            return null;
        }
        /// <summary>
        /// Gets the project ID that would be given to a new compound
        /// </summary>
        /// <returns></returns>
        public int GetNewProjectID()
        {
            return AllSyntheses.Count;
        }
    }
    [Serializable]
    public class ProjectInfo
    {
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectUser { get; set; }
        public ProjectInfo(string projectName, string projectPath, string projectUser)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectUser = projectUser;
        }
    }
}

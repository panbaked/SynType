using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.Program_Classes
{
    [Serializable]
    public class StartupInfo
    {
        public User LastUser { get; set; }
        public ProjectInfo LastProject { get; set; }

        public StartupInfo(User lastUser, ProjectInfo lastProject)
        {
            LastUser = lastUser;
            LastProject = lastProject;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.Program_Classes
{
    [Serializable]
    public class User
    {
        public string UserID { get; set; }
        public string UserFullName { get; set; }
        public string UserInitials { get; set; }

        public User(string userid, string userfullname, string userinit)
        {
            UserID = userid;
            UserFullName = userfullname;
            UserInitials = userinit;
        }
    }
}

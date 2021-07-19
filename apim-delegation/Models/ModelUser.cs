using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apim_delegation.Models
{
    public class ModelUsers
    {
        public class User
        {
            public string email { get; set; }
            public string displayName { get; set; }
            public string id { get; set; }

        }
        public User[] value { get; set; }
    }
}

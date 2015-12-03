using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain
{
    [Serializable]
    public class Customer
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}

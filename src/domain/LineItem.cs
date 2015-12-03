using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain
{
    [Serializable]
    public class LineItem
    {
        public string Id { get; set; }

        public string Sku { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}

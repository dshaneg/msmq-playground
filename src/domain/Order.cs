using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain
{
    [Serializable]
    public class Order
    {
        public Order()
        {
            OrderDate = DateTime.Now;
        }

        public string Id { get; set; }

        public DateTime OrderDate { get; set; }

        public Customer Customer { get; set; }

        public IEnumerable<LineItem> LineItems { get; set; }

        public override string ToString()
        {
            return string.Format("Order {0}: {1}, Customer {2}, Items {3}", Id, OrderDate, Customer.Name, LineItems.Count());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public List<CartItems> Products { get; set; }
        public int __v { get; set; }

    }

    public class CartItems
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

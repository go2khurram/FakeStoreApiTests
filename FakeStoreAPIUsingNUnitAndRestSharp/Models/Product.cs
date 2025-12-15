using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public Rating Rating { get; set; }

        // FakeStoreAPI doesn’t return quantity — we add this locally to simulate stock.
        public int Quantity { get; set; }
    }


    public class Rating
    {
        public decimal Rate { get; set; }
        public int Count { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public int __v { get; set; }
    }


    public class Name
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string Zipcode { get; set; }
        public Geolocation Geolocation { get; set; }
    }
    public class Geolocation
    {
        public string Lat { get; set; }
        public string Long { get; set; }
    }
}

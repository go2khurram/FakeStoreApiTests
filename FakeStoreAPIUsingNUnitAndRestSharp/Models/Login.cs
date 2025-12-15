using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class LoginResponse
    {
        public string Token { get; set; }
    }
}

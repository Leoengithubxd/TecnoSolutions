using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TecnoSolutions.Dtos
{
    public class EmployeesDto
    {
        public int IdPerson { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
    }
}
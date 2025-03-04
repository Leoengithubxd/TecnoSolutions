using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TecnoSolutions.Models;

namespace TechnoSolutions.Dtos
{
    public class QuoteDto
    {
        public int IdQuote { get; set; }
        public int IdPerson { get; set; }
        public int IdState { get; set; }
        public int IdCrewPerson { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceDepartment { get; set; }
        public string ServiceCity { get; set; }
        public string PersonName { get; set; }
        public string StateName { get; set; }
        public string State { get; set; }
        public int IdCrew { get; set; }
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
        public virtual ICollection<QUOTE_SERVICE> QuoteServices { get; set; }


    }
    public class QuoteProduct
    {
        public int IdQuoteProduct { get; set; }
        public int IdQuote { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class Product
    {
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public float Stock { get; internal set; }
        public string UnitPrice { get; internal set; }
    }
}
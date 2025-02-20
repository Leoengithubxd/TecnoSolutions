using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TecnoSolutions.Dtos
{
    public class QuoteDto
    {
        public int IdQuote { get; set; }
        public string PersonName { get; set; }
        public string State { get; set; }
        public int IdCrew { get; set; }
        public string ServiceAddress { get; set; }
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
    }
}
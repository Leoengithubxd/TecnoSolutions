﻿using System;
using System.Data;

namespace TechnoSolutions.Dtos
{
    public class QuoteDto
    {
        public int IdQuote { get; set; }
        public int IdPerson { get; set; }
        public int IdState { get; set; }
        public bool IsSelected { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceDepartment { get; set; }
        public string ServiceCity { get; set; }
        public DateTime? StarDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Price { get; set; }

    }
}
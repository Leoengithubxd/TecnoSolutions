using System;
using System.Collections.Generic;
using TecnoSolutions.Dtos;

namespace TechnoSolutions.Dtos
{
    public class InvoiceDto
    {
        public int IdInvoice { get; set; }
        public int IdProduct { get; set; }
        public string NameProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPriceProduct { get; set; }
        public bool IsSelected { get; set; }
        public string ProductsAddress { get; set; }
        public string ProductsDepartment { get; set; }
        public string ProductsCity { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public List<ProductSelectionDto> Products { get; internal set; }
    }
}
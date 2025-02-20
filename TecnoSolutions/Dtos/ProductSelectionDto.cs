﻿namespace TechnoSolutions.Dtos
{
    public class ProductSelectionDto
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPriceProduct { get; set; }
        public bool IsSelected { get; set; }
        public string ProductsAddress { get; set; }
        public string ProductsDepartment { get; set; }
        public string ProductsCity { get; set; }
    }
}
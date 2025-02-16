namespace TechnoSolutions.Dtos
{
    public class ProductSelectionDto
    {
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPriceProduct => Quantity * UnitPrice;
        public bool IsSelected { get; set; }
    }
}
namespace TechnoSolutions.Dtos
{
    public class ServiceSelectionDto
    {
        public int IdService { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int Quantity { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceDepartment { get; set; }
        public string ServiceCitY { get; set; }
        public int IdState { get; set; }
    }
}
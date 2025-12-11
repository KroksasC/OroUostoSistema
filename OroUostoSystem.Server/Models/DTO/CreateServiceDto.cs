namespace OroUostoSystem.Server.Models.DTO
{
    public class CreateServiceDTO
    {
        public string Title { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
    }
}

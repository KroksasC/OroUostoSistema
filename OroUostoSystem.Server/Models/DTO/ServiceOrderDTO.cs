using Microsoft.AspNetCore.Mvc;

namespace OroUostoSystem.Server.Models.DTO
{
    public class ServiceOrderDTO 
    {
        public string Email { get; set; }
        public string ServiceName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string UserId { get; set; }
        public int ServiceId { get; set; }
    }
}

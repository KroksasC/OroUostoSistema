namespace OroUostoSystem.Server.Models.DTO
{
    public class BaggageLiveLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

}

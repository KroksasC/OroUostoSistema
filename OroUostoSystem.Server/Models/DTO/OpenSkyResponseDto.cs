namespace OroUostoSystem.Server.Models.DTO
{
    public class OpenSkyResponseDto
    {
        public int Time { get; set; }
        public List<List<object>> States { get; set; } = new();
    }
}

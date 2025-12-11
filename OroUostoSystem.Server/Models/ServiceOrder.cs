public class ServiceOrder
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }

    // FK — Client
    public int ClientId { get; set; }
    public Client Client { get; set; }

    // FK — Service
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}

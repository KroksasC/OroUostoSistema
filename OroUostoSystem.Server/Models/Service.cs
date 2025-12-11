using OroUostoSystem.Server.Models;

public class Service
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // FK - Employee
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    // Navigation
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
}

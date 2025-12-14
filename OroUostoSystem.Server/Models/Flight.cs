﻿public class Flight
{
    public int Id { get; set; }

    // Changed from bool to nullable int - Foreign Keys to Pilots
    public int? AssignedPilot { get; set; }
    public Pilot? AssignedPilotNavigation { get; set; }

    public int? AssignedMainPilot { get; set; }
    public Pilot? AssignedMainPilotNavigation { get; set; }

    public float WorkingHours { get; set; }
    public DateTime FlightDate { get; set; }
    public string Aircraft { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Route Route { get; set; } 
    
    public int RouteId { get; set; }

    // NEW: Repeat interval in hours (NULL = one-time flight)
    public int? RepeatIntervalHours { get; set; }

    // Navigation
    public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
    public ICollection<Route> Routes { get; set; } = new List<Route>();
}
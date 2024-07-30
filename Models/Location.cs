using System;
using TravelLocationManagement.Models;

public class Location
{
    public Guid LocationId { get; set; }
    public string? GPS { get; set; } = string.Empty;
    public Address? LocationAddress { get; set; } = new Address(); 
    public Guid LocationAddressId { get; set; }
    
    // Add these properties
    public string PlaceId { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}




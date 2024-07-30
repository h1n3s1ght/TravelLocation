using System;

namespace TravelLocationManagement.Models
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string StateProv { get; set; } = string.Empty;
        public string CityTown { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
    }
}


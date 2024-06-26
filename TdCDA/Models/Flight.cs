﻿namespace TdCDA.Models
{
    public class Flight
    {
        public string? IdFlight { get; set; }
        public string? Reference { get; set; }
        public DateTime? DepartureDate { get; set; }
        public  DateTime? ArrivalDate { get; set; }

        public virtual ICollection<Flight>? Flights { get; set; }

        public string? IdDepartureCity{ get; set; }
        public string? IdArrivalCity { get; set; }
        public virtual City? City { get; set; }
    }
}

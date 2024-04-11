using TdCDA.Models;

namespace TdCDA.Manager
{
    public class FlightManager
    {
        public Flight Flight { get; set; }

        public string DepartureCityName  { get; set; }
        public string ArrivalCityName  { get; set; }

        public FlightManager(Flight flight, string departureCityName, string arrivalCityName)
        {
            Flight = flight;
            DepartureCityName = departureCityName;
            ArrivalCityName = arrivalCityName;
        }
    }
}

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TdCDA.Models;
using TdCDA.Manager;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TdCDA.Controllers
{
    public class FlightController : Controller
    {
        IFirebaseClient? client;
        // Interface de manipulation de la BDD Firebase
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "ZkMCzQKJrloO6GDnEWlx5qcwJjHJaNZ8ZcNZ2yaO",
            BasePath = "https://bddtdcda-default-rtdb.europe-west1.firebasedatabase.app"
        };

        // GET: FlightController
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Flights");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<FlightManager>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    Flight flight = JsonConvert.DeserializeObject<Flight>(((JProperty)item).Value.ToString()); 
                    FirebaseResponse DepartureCityResponse = client.Get("Cities/" + flight.IdDepartureCity);
                    City? depCity = JsonConvert.DeserializeObject<City>(DepartureCityResponse.Body);

                    FirebaseResponse ArrivalCityResponse = client.Get("Cities/" + flight.IdArrivalCity);
                    City? arrCity = JsonConvert.DeserializeObject<City>(ArrivalCityResponse.Body);

                    FlightManager flightManager = new FlightManager(flight, depCity.Name, arrCity.Name);

                    list.Add(flightManager);
                    
                }
            }

            return View(list);
        }

/*------------------------------------------- DETAILS ----------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/


        // GET: FlightController/Details/id
        public ActionResult Details(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Flights/" + id);
            Flight? flight = JsonConvert.DeserializeObject<Flight>(response.Body);

            FirebaseResponse DepartureCityResponse = client.Get("Cities/" + flight.IdDepartureCity);
            City? depCity = JsonConvert.DeserializeObject<City>(DepartureCityResponse.Body);

            FirebaseResponse ArrivalCityResponse = client.Get("Cities/" + flight.IdArrivalCity);
            City? arrCity = JsonConvert.DeserializeObject<City>(ArrivalCityResponse.Body);

            FlightManager flightManager = new FlightManager(flight, depCity.Name, arrCity.Name);

            return View(flightManager);
        }

/*------------------------------------------------------- CREATE ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/
        
        // GET: FlightController/Create
        public ActionResult Create()
        {
            ViewData["IdDepartureCity"] = new SelectList(GetListCities(), "IdCity", "Name");
            ViewData["IdArrivalCity"] = new SelectList(GetListCities(), "IdCity", "Name");
            return View();
        }

        // POST: FlightController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Flight flight)
        {
            if(flight.IdDepartureCity == flight.IdArrivalCity)
            {
                ModelState.AddModelError("IdArrivalCity", "La ville de départ et d'arrivée ne peuvent pas être identiques.");
                ViewData["IdDepartureCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdDepartureCity);
                ViewData["IdArrivalCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdArrivalCity);
                return View(flight);
            }
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = flight;
                PushResponse response = client.Push("Flights/", flight);
                flight.IdFlight = response.Result.name;
                SetResponse setResponse = client.Set("Flights/" + flight.IdFlight, flight);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    ModelState.AddModelError(string.Empty, "OK");
                else
                    ModelState.AddModelError(string.Empty, "KO!!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("Index");
        }

/*------------------------------------------------------- EDIT --------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/

        // GET: FlightController/Edit/id
        public ActionResult Edit(string id)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Flights/" + id);
            Flight flight = JsonConvert.DeserializeObject<Flight>(response.Body);

            ViewData["IdDepartureCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdDepartureCity);
            ViewData["IdArrivalCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdArrivalCity);

            return View(flight);
        }

        // POST: FlightController/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Flight flight)
        {
            if (flight.IdDepartureCity == flight.IdArrivalCity)
            {
                ModelState.AddModelError("IdArrivalCity", "La ville de départ et d'arrivée ne peuvent pas être identiques.");
                ViewData["IdDepartureCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdDepartureCity);
                ViewData["IdArrivalCity"] = new SelectList(GetListCities(), "IdCity", "Name", flight.IdArrivalCity);
                return View(flight);
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Set("Flights/" + flight.IdFlight, flight);
            return RedirectToAction("Index");
        }

/*------------------------------------------------------- DELETE ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/
        // GET: LivresController/Delete/id - pour confirmer la suppression d'un vol en fonction de son Id
        // Affichage de la vue suppression
        // GET: FlightController/Delete/id
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Flights/" + id);
            Flight flight = JsonConvert.DeserializeObject<Flight>(response.Body);

            FirebaseResponse DepartureCityResponse = client.Get("Cities/" + flight.IdDepartureCity);
            City? depCity = JsonConvert.DeserializeObject<City>(DepartureCityResponse.Body);

            FirebaseResponse ArrivalCityResponse = client.Get("Cities/" + flight.IdArrivalCity);
            City? arrCity = JsonConvert.DeserializeObject<City>(ArrivalCityResponse.Body);

            FlightManager flightManager = new FlightManager(flight, depCity.Name, arrCity.Name);

            return View(flightManager);
        }

        // POST: FlightController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, Flight flight)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Flights/" + id);
            return RedirectToAction("Index");
        }

        // <summary>
        // Récupérere la liste des villes
        // </summary>
        // <returns></returns>
        private List<City> GetListCities()
        {
            var list = new List<City>();

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<City>(((JProperty)item).Value.ToString()));
                }
            }
            return list;
        }

        // 
        private City GetCityById(string cityId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities/" + cityId);
            return response.ResultAs<City>();
        }

    }
}

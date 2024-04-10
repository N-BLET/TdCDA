using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TdCDA.Models;

namespace TdCDA.Controllers
{
    public class CityController : Controller
    {
        IFirebaseClient? client;
        // Interface de manipulation de la BDD Firebase
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "ZkMCzQKJrloO6GDnEWlx5qcwJjHJaNZ8ZcNZ2yaO",
            BasePath = "https://bddtdcda-default-rtdb.europe-west1.firebasedatabase.app"
        };

        // GET: CityController
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<City>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<City>(((JProperty)item).Value.ToString()));
                }
            }
            return View(list);
        }

/*----------------------------------------------------- DETAILS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/


        // GET: CityController/Details/id
        public ActionResult Details(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities/" + id);
            City? city = JsonConvert.DeserializeObject<City>(response.Body);
            return View(city);
        }

/*------------------------------------------------------- CREATE ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/

        // GET: CityController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(City city)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = city;
                PushResponse response = client.Push("Cities/", city);
                city.IdCity = response.Result.name;
                SetResponse setResponse = client.Set("Cities/" + city.IdCity, city);

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

        // GET: CityController/Edit/id
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities/" + id);
            City city = JsonConvert.DeserializeObject<City>(response.Body);

            return View(city);
        }

        // POST: CityController/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(City city)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Set("Cities/" + city.IdCity, city);
            return RedirectToAction("Index");
        }

        /*------------------------------------------------------- DELETE ------------------------------------------------------
        ---------------------------------------------------------------------------------------------------------------------*/

        // GET: CityController/Delete/id - pour confirmer la suppression d'une ville en fonction de son Id
        // Affichage de la vue suppression
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cities/" + id);
            City city = JsonConvert.DeserializeObject<City>(response.Body);

            return View(city);
        }

        // POST: CityController/Delete/id - pour supprimer dans la bdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, City city)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Cities/" + id);
            return RedirectToAction("Index");
        }
    }
}

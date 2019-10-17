using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sportstrackerdemo.Models;
using System.Globalization;
using System.IO;

namespace Sportstrackerdemo.Controllers
{
    public class HomeController : Controller
    {
        // luodaan staattinen muuttuja, jotta tiedostoa ei tarvitse lukea kuin kerran
        static List<Harjoitus> Harjoitukset = new List<Harjoitus>();

        private List<Harjoitus> LueTiedosto()
        {
            // tarkistetaan, onko tiedoston data jo aiemmin luettu
            if (Harjoitukset.Count > 0)
            {
                return Harjoitukset;
            }

            string[] lines = System.IO.File.ReadAllLines("wwwroot/filut/straakadata.txt");

            //luetaan data tiedostosta riveittäin
            List<Harjoitus> harjoitukset = new List<Harjoitus>();
            foreach (string line in lines)
            {
                Harjoitus h = new Harjoitus();
                // etsitään tiedoston riviltä pilkku, jonka mukaan tekstijonosta voi lukea indeksillä eteen- ja taaksepäin
                int index1 = line.IndexOf(',');
                //luetaan harjotuksen nimi
                h.Nimi = line.Substring(0, index1 - 6).Trim();
                //luetaan päivämäärä
                string pvm = line.Substring(index1 - 6, 6).Trim();
                string vuosi = line.Substring(index1 + 2, 4);
                h.Vuosi = vuosi;
                h.Päiväys = (pvm + " " + vuosi);
                //luetaan harjoituksen kestoaika
                h.Kesto = TimeSpan.Parse(line.Substring(index1 + 6, 8));
                //luetaan harjoituksen pituus kilometreinä ja muutetaan desimaaliseksi
                string pituus = line.Substring(index1 + 14, 5).Trim();
                CultureInfo cul = new CultureInfo("en-US");
                h.Matka = Convert.ToDecimal(pituus, cul);
                //luetaan harjoituksen keskinopeus ja muutetaan desimaaliseksi
                string väli = line.Substring(index1 + 18,1);
                string keski = "";
                //tarkistus: Jos matka on ollut alle 10 km, niin merkkijonossa 1 merkki vähemmän
                if (väli == " " )
                keski = line.Substring(index1 + 21, 4).Trim();
                else
                keski = line.Substring(index1 + 22, 4).Trim();
                
                h.Keskinopeus = Convert.ToDecimal(keski, cul);

                harjoitukset.Add(h);
            }

            // tallennetaan tiedot myöhempää käyttöä varten
            Harjoitukset = harjoitukset;

            return harjoitukset;
        }
        public IActionResult Index()
        {
            List<Harjoitus> harjoitukset = LueTiedosto();
            TimeSpan kokoaika = TimeSpan.Zero;

            //Lasketaan kaikkien harjoituskokonaismatka
            decimal totmatka = harjoitukset.Sum(p => p.Matka);
            ViewBag.Totmatka = totmatka;

            //Lasketaan kaikkien keskinopeuksien keskiarvo ja pyöristetään 2:een desimaaliin
            decimal totkeskinop = Math.Round(harjoitukset.Average(p => p.Keskinopeus), 2);
            ViewBag.keskinopeuska = totkeskinop;
          
            //lasketaan kokonaiskesto
            foreach (Harjoitus h in harjoitukset)
            {
                kokoaika = kokoaika.Add(h.Kesto);
            }
            ViewBag.lkm = Harjoitukset.Count;
            ViewBag.Kokoaika = kokoaika;
            ViewBag.Kaikkiharjoitukset = harjoitukset;

            //haetaan kaikki vuosiluvut
            var vuodet = harjoitukset.Select(p => p.Vuosi).Distinct();
            ViewBag.Vuodet = vuodet;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Reitit()
        {
            List<Harjoitus> harjoitukset = LueTiedosto();

            ViewBag.Kaikkiharjoitukset = harjoitukset;
            //Järjestetään kaikki harjoitukset keston mukaan nousevaan järjestykseen
            var result = harjoitukset.OrderBy(c => c.Kesto);
            ViewBag.Nopeimmat = result;
            //Etsitään kaikki harjoitusten instanssit eli kaikki eri reitit ja laitetaan ne aakkosjärjestykseen
            var reitit = harjoitukset.OrderBy(h => h.Nimi).Select(p => p.Nimi).Distinct();
            ViewBag.Reitit = reitit;

            return View();
        }
        public IActionResult Reitit2()
        {
            List<Harjoitus> harjoitukset = LueTiedosto();

            ViewBag.Kaikkiharjoitukset = harjoitukset;
            //Järjestetään kaikki harjoitukset keston mukaan nousevaan järjestykseen
            var result = harjoitukset.OrderBy(c => c.Kesto);
            ViewBag.Nopeimmat = result;
            //Etsitään kaikki harjoitusten instanssit eli kaikki eri reitit ja laitetaan ne aakkosjärjestykseen
            var reitit = harjoitukset.OrderBy(h => h.Nimi).Select(p => p.Nimi).Distinct();
            ViewBag.Reitit = reitit;
            //haetaan kaikki vuosiluvut
            var vuodet = harjoitukset.Select(p => p.Vuosi).Distinct();
            ViewBag.Vuodet = vuodet;

            return View();
        }
        public IActionResult kuvat()
        {
            return View();
        }
        public IActionResult testi1()
       {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
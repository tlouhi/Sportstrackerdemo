using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sportstrackerdemo.Models
{
    public class Harjoitus
    {
        public string Nimi { get; set; }
        public string Vuosi { get; set; }
        public string Päiväys { get; set; }
        public TimeSpan Kesto { get; set; }
        public decimal Matka { get; set; }
        public decimal Keskinopeus { get; set; }
    }
}

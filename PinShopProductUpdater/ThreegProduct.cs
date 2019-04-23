using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinShopProductUpdater
{
    public class ThreegProduct
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public string Kategorija1 { get; set; }
        public string Kategorija2 { get; set; }
        public string Kategorija3 { get; set; }
        public double VpCena { get; set; }
        public double MpCena { get; set; }
        public double Rabat { get; set; }
        public bool Dostupan { get; set; }
        public bool NaAkciji { get; set; }
        public string Opis { get; set; }
        public string Barkod { get;set; }
        public string Slike { get; set; }
        public string Brand { get; set; }
    }
}

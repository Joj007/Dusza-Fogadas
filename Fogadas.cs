// nullReference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dusza_Fogadas
{
    internal class Fogadas
    {
        string fogadoNeve;
        string jatekMegnevezese;
        int fogadasOsszeg;
        string alany;
        string esemeny;
        string eredmeny;

        public Fogadas(string fogadoNeve, string jatekMegnevezese, int fogadasOsszeg, string alany, string esemeny, string eredmeny)
        {
            this.fogadoNeve = fogadoNeve;
            this.jatekMegnevezese = jatekMegnevezese;
            this.fogadasOsszeg = fogadasOsszeg;
            this.alany = alany;
            this.esemeny = esemeny;
            this.eredmeny = eredmeny;
        }

        public Fogadas(string sor) {
            string[] mezok = sor.Split(';');
            this.fogadoNeve = mezok[0];
            this.jatekMegnevezese = mezok[1];
            this.fogadasOsszeg = int.Parse(mezok[2]);
            this.alany = mezok[3];
            this.esemeny = mezok[4];
            this.eredmeny = mezok[5];
        }

        public string FogadoNeve { get => fogadoNeve; }
        public string JatekMegnevezese { get => jatekMegnevezese; }
        public int FogadasOsszeg { get => fogadasOsszeg; }
        public string Alany { get => alany; set => alany = value; }
        public string Esemeny { get => esemeny; }
        public string Eredmeny { get => eredmeny; }

        public override string ToString()
        {
            return $"{FogadoNeve};{JatekMegnevezese};{FogadasOsszeg};{Alany};{Esemeny};{Eredmeny}";
        }
    }
}

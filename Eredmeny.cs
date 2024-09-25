// nullReference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dusza_Fogadas
{
    internal class Eredmeny
    {
        string alany;
        string esemeny;
        string ertek;
        double szorzo;

        public Eredmeny(string alany, string esemeny, string ertek, double szorzo)
        {
            this.alany = alany;
            this.esemeny = esemeny;
            this.ertek = ertek;
            this.szorzo = szorzo;
        }

        public Eredmeny(string sor)
        {
            string[] mezok = sor.Split(';');
            alany = mezok[0];
            esemeny = mezok[1];
            ertek = mezok[2];
            szorzo = Convert.ToDouble(mezok[3]);
        }

        public string Alany => alany;
        public string Esemeny => esemeny;
        public string Ertek => ertek;
        public double Szorzo => szorzo;

        public override string ToString()
        {
            return $"{alany};{esemeny};{ertek};{szorzo}";
        }
    }
}

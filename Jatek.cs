using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dusza_Fogadas
{
    internal class Jatek
    {
        string szervezo;
        string nev;
        List<string> alanyok;
        List<string> esemenyek;

        List<Eredmeny> eredmenyek = new List<Eredmeny>();

        public Jatek(string szervezo, string nev, List<string> alanyok, List<string> esemenyek)
        {
            this.szervezo = szervezo;
            this.nev = nev;
            this.alanyok = alanyok;
            this.esemenyek = esemenyek;
        }

        public string Szervezo => szervezo;
        public string Nev => nev;
        public List<string> Alanyok => alanyok;
        public List<string> Esemenyek => esemenyek;
        public List<Eredmeny> Eredmenyek { get => eredmenyek; set => eredmenyek = value; }
        public int AlanyokSzama => alanyok.Count;
        public int EsemenyekSzama => esemenyek.Count;
        public bool Lezart => eredmenyek.Count != 0;

        public override string ToString()
        {
            return $"{szervezo};{nev};{AlanyokSzama};{EsemenyekSzama}\n{String.Join('\n', alanyok)}\n{String.Join('\n', esemenyek)}";
        }

        public static List<Jatek> Szovegbol(string jatekokSzoveg, string eredmenyekSzoveg)
        {
            List<Jatek> jatekok = new List<Jatek>();
            string[] sorok = jatekokSzoveg.Split('\n');
            int i = 0;
            while (i < sorok.Length)
            {
                string[] mezok = sorok[i].Split(';');
                int alanyokSzama = Convert.ToInt32(mezok[2]);
                int esemenyekSzama = Convert.ToInt32(mezok[3]);

                i++;

                List<string> alanyok = new List<string>();
                List<string> esemenyek = new List<string>();

                for (int j = 0; j < alanyokSzama; j++)
                {
                    alanyok.Add(sorok[i + j]);
                }
                i += alanyokSzama;

                for (int j = 0; j < esemenyekSzama; j++)
                {
                    esemenyek.Add(sorok[i + j]);
                }
                i += esemenyekSzama;

                jatekok.Add(new Jatek(mezok[0], mezok[1], alanyok, esemenyek));
            }

            if (eredmenyekSzoveg != null)
            {
                string[] eredmenySorok = eredmenyekSzoveg.Split('\n');
                foreach (Jatek jatek in jatekok)
                {
                    int index = eredmenySorok.ToList().FindIndex(0, eredmenySorok.Count() - 1, x => x == jatek.Nev);
                    if (index == -1)
                    {
                        continue;
                    }
                    jatek.Eredmenyek = eredmenySorok.Skip(index + 1).Take(jatek.AlanyokSzama * jatek.EsemenyekSzama).Select(x => new Eredmeny(x)).ToList();
                }
            }

            return jatekok;
        }

    }
}

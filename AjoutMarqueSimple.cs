using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategorieProject
{
    public class AjoutMarqueSimple
    {
        private List<string> motcles = new List<string>();
        private string Nom = "";
        private Marque marque = new Marque();
        private string NomFichierTexte = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "AjoutMarqueSimple");
        private StreamReader? streamReader = null;

        public Marque AjouterMarqueSimple(bool ANGLAIS)
        {
            if (ANGLAIS)
                NomFichierTexte += "EN.txt";
            else
                NomFichierTexte += "FR.txt";
            GetInfos();
            marque.Nom = Nom;
            marque.keywords = motcles;

            return marque;
        }

        private void GetInfos()
        {
            // Check if the file exists
            if (!File.Exists(NomFichierTexte))
            {
                Console.WriteLine($"Le fichier {NomFichierTexte} n'existe pas.");
                return;
            }
            streamReader = new StreamReader(NomFichierTexte);
            Nom = streamReader.ReadLine() ?? "";
            string line = streamReader.ReadLine();
            motcles = line?.Split(',').Select(m => m.Trim()).ToList() ?? new List<string>();

        }
    }
}

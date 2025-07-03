using System.Globalization;

public class AfficherConsole{
    public void AfficherMarques(List<Marque> list){
        if(list == null)
            return;

        for(int i = 0; i < list.Count(); i++){
            Marque marque = list[i];
            Console.WriteLine(marque.Nom);
            
        }
    }

    public static void Afficher20Marques(List<Marque> list){
        if(list == null)
            return;

        for(int i = 0; i < 21; i++){
            Marque marque = list[i];
            Console.Write(marque.keywords + ", [");
            if(marque.keywords != null){
                for(int j = 1; j < marque.keywords.Count(); j++){
                    Console.Write(marque.keywords[j] + ", ");
                }
                Console.WriteLine("]");
            }
            
        }
    }

    public void AfficherNomMarque(List<Marque> list){
        for(int i = 0; i < list.Count(); i++){
            Console.WriteLine(list[i].Nom);     
        }
    }

    public void AfficherCategorie(Marque marque){
        Console.Write(marque.Nom + " [");
        foreach (string str in marque.keywords)
            Console.Write(str + ", ");
        
        Console.WriteLine(" ]");

    }

    public void AfficherRequeteCategorie(Marque marque){
        Console.WriteLine(marque.Nom + " ; " + marque.modele + " ; " + marque.tokens);
        Console.WriteLine("======= DESCRIPTION =======");
        Console.WriteLine(marque.Description);
        Console.WriteLine("======= REQUETE =======");
        Console.WriteLine(marque.Requete);
    }
    
}
using CategorieProject;
using System.Xml.Serialization;

internal class Program
{
    public static List<Marque> marques;
    public static List<int> Generated;
    private static GestionExcel gestionExcel;
    private static AfficherConsole afficherConsole;
    private static GestionRequete gestionRequete;
    private static GestionLog gestionLog;
    private static GestionCsv gestionCSV;
    private static AjoutMarqueSimple ajoutMarqueSimple;

    private static bool ANGLAIS = false;
    private async static Task Main(string[] args)
    {
        Generated = new List<int>();
        string filePathProd1 = Path.Combine(Directory.GetCurrentDirectory(), "data", "Export1.xlsx");
        string filePathProd2 = Path.Combine(Directory.GetCurrentDirectory(), "data", "Export2.xlsx");

        gestionExcel = new GestionExcel { ExcelFilePathProd1 = filePathProd1, ExcelFilePathProd2 = filePathProd2 };
        afficherConsole = new AfficherConsole();
        gestionRequete = new GestionRequete();
        gestionLog = new GestionLog();
        gestionCSV = new GestionCsv();
        ajoutMarqueSimple = new AjoutMarqueSimple();
        marques = new List<Marque>();
        //gestionExcel.setupFile(ANGLAIS);
        //marques = gestionExcel.RemplirListeMarques();
        marques.Add(ajoutMarqueSimple.AjouterMarqueSimple(ANGLAIS));
        for (int i = 0; i < marques.Count; i++)
        {
            await GenDescription(i);
        }
        //gestionLog.CreateLog(Generated);
        gestionCSV.CreateLog(Generated, ANGLAIS);
        gestionCSV.AddMetaTitles(ANGLAIS);
        
    }

    private static async Task GenDescription(int indexCategorie){
        marques[indexCategorie] = await gestionRequete.GenererDescriptionChatGPT(marques[indexCategorie], ANGLAIS);
        Generated.Add(indexCategorie);

    }

}
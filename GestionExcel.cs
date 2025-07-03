using System.Runtime.CompilerServices;
using OfficeOpenXml;

class GestionExcel{
    public required string ExcelFilePathProd1;
    public required string ExcelFilePathProd2;

    private FileInfo? fileInfoProd1;
    private FileInfo? fileInfoProd2;
    private ExcelPackage? packageProd1;
    private ExcelPackage? packageProd2;
    private ExcelWorksheet? worksheetProd1;
    private ExcelWorksheet? worksheetProd2;

    private ExcelWorksheet? currentWorksheet;

    private List<Marque>? marques = new List<Marque>();
    private int row;
    
    private int MarqueInternetColumn = 7;
    private int DescriptionColumn = 4;

    public void setupFile(bool ANGLAIS){
        fileInfoProd1 = new FileInfo(ExcelFilePathProd1);
        packageProd1 = new ExcelPackage(fileInfoProd1);
        worksheetProd1 = packageProd1.Workbook.Worksheets[0];
        if (worksheetProd1 == null)
            Environment.Exit(0);

        fileInfoProd2 = new FileInfo(ExcelFilePathProd2);
        packageProd2 = new ExcelPackage(fileInfoProd2);
        worksheetProd2 = packageProd2.Workbook.Worksheets[0];
        if(worksheetProd2 == null)
            Environment.Exit(0);

        row = 2;
        DescriptionColumn = ANGLAIS ? 5 : 4;
    }

    public List<Marque> RemplirListeMarques(){
        List<string> keywords = new List<string>();
        currentWorksheet = worksheetProd1;
        bool fini = false;

        while(!fini){
            Marque marque = new Marque();
            if (row >= 64999 && currentWorksheet == worksheetProd1){
                currentWorksheet = worksheetProd2;
                row = 2;
            }
            else if (row >= 43180 && currentWorksheet == worksheetProd2){
                break;
            }

            if(NouvelleMarque(((string)currentWorksheet.Cells[row, MarqueInternetColumn].Value).Trim())){
                marque.Nom = ((string)currentWorksheet.Cells[row, MarqueInternetColumn].Value).Trim();
                marque.keywords = GetMotscles(marque.Nom);
                marques.Add(marque);
            }
            row++;
        }


        return marques;
    }

    private List<string> GetMotscles(string marque){
        List<string> strings = new List<string>();
        int rowLocal = row;
        bool fini = false;

        while (!fini){
            if(((string)currentWorksheet.Cells[rowLocal, MarqueInternetColumn].Value).Trim() == marque){
                var cell = currentWorksheet.Cells[rowLocal, DescriptionColumn];
                if (cell != null && cell.Value != null)
                {
                    strings.Add(((string)cell.Value).Trim());
                }
                
            }
            else{
                break;
            }
            if(strings.Count >= 20)
                break;
            rowLocal++;
        }
        


        return strings;
    }

    private bool NouvelleMarque(string marqueNew){
        foreach (Marque marque in marques){
            if(marqueNew == marque.Nom)
                return false;
        }
        return true;
    }

}
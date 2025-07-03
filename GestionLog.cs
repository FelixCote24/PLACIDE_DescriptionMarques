using OfficeOpenXml;
using OfficeOpenXml.Style;

class GestionLog{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "logs.xlsx");
    public void CreateLog(List<int> generatedIndexList){
        List<Marque> list = Program.marques;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        FileInfo file = new FileInfo(filePath);
        ExcelPackage package = new ExcelPackage(file);

        using (package = file.Exists ? new ExcelPackage(file) : new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Count > 0 
                ? package.Workbook.Worksheets[0] 
                : package.Workbook.Worksheets.Add("LogSheet");
            InitializeLogFile(worksheet);

            int row = FindFirstEmptyRow(worksheet);
            
            foreach (int i in generatedIndexList)
            {
                Marque marque = list[i];

                worksheet.Cells[row, 1].Value = marque.Nom; 
                worksheet.Cells[row, 3].Value = marque.Requete;
                worksheet.Cells[row, 4].Value = marque.Description;
                worksheet.Cells[row, 5].Value = marque.meta_requete;
                worksheet.Cells[row, 6].Value = marque.meta_description;
                worksheet.Cells[row, 7].Value = marque.modele;
                worksheet.Cells[row, 8].Value = marque.tokens;
                
                // Set alignment to top for all cells in the row
                for (int col = 1; col <= 6; col++)
                {
                    worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[row, col].Style.WrapText = true; // Enable text wrapping
                }

                // Adjust column width
                worksheet.Column(1).Width = 20; // Name
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 60; // Requete
                worksheet.Column(4).Width = 100; // Description
                worksheet.Column(5).Width = 60; // Requete meta
                worksheet.Column(6).Width = 100; // Description meta
                worksheet.Column(7).Width = 20; // Model
                worksheet.Column(8).Width = 10; // Tokens

                // Adjust row height
                worksheet.Row(row).Height = 150; // Increase row height for better readability

                // Adding dropdown for the column 2 (Requete) - Option: "Accepte", "Refusée"
                var validation = worksheet.Cells[row, 2].DataValidation.AddListDataValidation();
                validation.Formula.Values.Add("Accepte");
                validation.Formula.Values.Add("Refusée");

                var conditionalFormat = worksheet.ConditionalFormatting.AddExpression(worksheet.Cells[row, 1, row, 5]);
                conditionalFormat.Formula = "=$B" + row + "=\"Refusée\""; 
                conditionalFormat.Style.Fill.PatternType = ExcelFillStyle.Solid;
                conditionalFormat.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                if ((string)worksheet.Cells[row, 2].Value == "Refusée")
                {
                    worksheet.Row(row).Hidden = true;
                }

                row++;
            }

            package.SaveAs(file);
        }
    }

    private int FindFirstEmptyRow(ExcelWorksheet worksheet)
    {
        int row = 1;
        while (worksheet.Cells[row, 1].Value != null)
        {
            row++;
        }
        return row;
    }

    private void InitializeLogFile(ExcelWorksheet worksheet)
    {
        // Add headers in the first row
        worksheet.Cells[1, 1].Value = "Chemin";
        worksheet.Cells[1, 2].Value = "Décision";
        worksheet.Cells[1, 3].Value = "Requête";
        worksheet.Cells[1, 4].Value = "Description";
        worksheet.Cells[1, 5].Value = "Requête_meta";
        worksheet.Cells[1, 6].Value = "meta_description";
        worksheet.Cells[1, 7].Value = "Modèle";
        worksheet.Cells[1, 8].Value = "Tokens";

        // Set column widths for better readability
        worksheet.Column(1).Width = 20; // Chemin
        worksheet.Column(2).Width = 20; // Décision
        worksheet.Column(3).Width = 60; // Requête
        worksheet.Column(4).Width = 120; // Description
        worksheet.Column(5).Width = 60; // requete meta
        worksheet.Column(6).Width = 120; // meta_description
        worksheet.Column(7).Width = 20; // Modèle
        worksheet.Column(8).Width = 10; // Tokens


        using (var headerRange = worksheet.Cells[1, 1, 1, 6])
        {
            headerRange.Style.Font.Bold = true;

            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            headerRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);

            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
    }
}
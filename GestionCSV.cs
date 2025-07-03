using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

class GestionCsv
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "csv", "descriptionEN.csv");

    public void CreateLog(List<int> generatedIndexList, bool bLanguage)
    {
        List<Marque> list = Program.marques;

        // S'assurer que le dossier "csv" existe
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "csv"));

        bool fileExists = File.Exists(filePath);
        using (var writer = new StreamWriter(filePath, append: true, Encoding.UTF8))
        {
            // Écrire l'en-tête si le fichier est nouveau
            if (!fileExists)
            {
                writer.WriteLine("langue,marque,description,meta_description");
            }

            foreach (int i in generatedIndexList)
            {
                Marque marque = list[i];

                string langue = bLanguage ? "en" : "fr";
                string nom = EscapeCsv(marque.Nom);
                string description = EscapeCsv(marque.Description);
                string metaDescription = EscapeCsv(marque.meta_description);

                string line = $"{langue},{nom},{description},{metaDescription}";
                writer.WriteLine(line);
            }
        }
    }

    // Protège le CSV contre les virgules, guillemets, etc.
    private string EscapeCsv(string input)
    {
        if (input == null) return "";
        if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
        {
            input = input.Replace("\"", "\"\"");
            return $"\"{input}\"";
        }
        return input;
    }

    public void AddMetaTitles()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "csv", "MarqueCsvComplet.csv");
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "csv", "MarqueCsvComplet_with_meta.csv");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            Quote = '"',
            Encoding = Encoding.UTF8,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var reader = new StreamReader(filePath, Encoding.UTF8);
        using var csvReader = new CsvReader(reader, config);
        var records = csvReader.GetRecords<dynamic>().ToList();

        using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
        using var csvWriter = new CsvWriter(writer, config);

        // Write headers
        var firstRecord = (IDictionary<string, object>)records[0];
        foreach (var header in firstRecord.Keys)
        {
            csvWriter.WriteField(header);
        }
        csvWriter.WriteField("meta_title");
        csvWriter.NextRecord();

        // Write records with new meta_title column
        foreach (IDictionary<string, object> record in records)
        {
            foreach (var value in record.Values)
            {
                csvWriter.WriteField(value);
            }

            string brand = record.ContainsKey("marque") ? record["marque"]?.ToString() ?? "" : "";
            string metaTitle = $"{brand} | Outillage Placide Mathieu";
            csvWriter.WriteField(metaTitle);
            csvWriter.NextRecord();
        }

        Console.WriteLine("Meta titles successfully added with CsvHelper.");
    }
}
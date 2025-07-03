using System.Text;
using Newtonsoft.Json;

class GestionRequete{
    string apiKey = ""; // Replace with your actual OpenAI API key
    string apiKeyGemini = "";
    string apiUrlGemini = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=";
    string apiUrl = "https://api.openai.com/v1/chat/completions";

    //string modelChosen = "gpt-3.5-turbo-0125";
    //string modelChosen = "gpt-4o-2024-08-06";
    string modelChosen = "gpt-4o-mini-2024-07-18";

    public async Task<Marque> GenererDescriptionChatGPT(Marque marque, bool ANGLAIS){
        string requete = "Nous sommes un magasin d'outillage/outils. Rédigez un texte descriptif d’environ 80-100 mots sur la marque de produit que nous distribuons: " + marque.Nom + ". Le texte à lire en max 1 min doit comporter 3 paragraphes: le premier sur l'origine et la mission de la marque, le 2e sur le genre d'outillage qu'ils offrent, et le 3e sur un fait saillant sur le fournisseur. Utilisez un ton informatif, fluide et naturel, sans style promotionnel ni titres. Voici quelques produits que nous gardons en référence (ne pas inclure de numéros ni de listes dans la description): ";
        
        string requeteMETA = "Nous sommes un magasin d'outil appelé Outillage Placide Mathieu, Créé une meta-description internet de la page de produit de la marque " + marque.Nom + ", Maximum 250 caractères, insérez un Visitez Placide.com à la fin, voici des produits que nous offrons de cette marque mais n'inclu aucun chiffres dans la description : ";
        
        string requeteANG = "We are a hardware/tools store. Write a descriptive text of about 80-100 words about the product brand we distribute: " + marque.Nom + ". The text to be read in max 1 min must have 3 paragraphs: the first on the origin and mission of the brand, the 2nd on the type of tools they offer, and the 3rd on a key fact about the supplier. Use an informative, fluid and natural tone, without promotional style or titles. Here are some products that we keep for reference (do not include numbers or lists in the description): ";

        string requeteMETAANG = "We are a tool store called Outillage Placide Mathieu, Created a meta description of the internet product page for the brand " + marque.Nom + ", Maximum 250 characters, insert a Visit Placide.com at the end, here are some products we offer from this brandbut do not include any numbers in the description: ";

        int iNbKeywords = marque.keywords.Count() < 15 ? marque.keywords.Count() : 15;
        int iNbKeywordsMETA = marque.keywords.Count() < 5 ? marque.keywords.Count() : 5;
        int tokensUseFirst = 0;
            string requeteFINAL = ANGLAIS ? requeteANG : requete;
            string requeteMETAFINAL = ANGLAIS ? requeteMETAANG : requeteMETA;
            for (int i = 0; i < iNbKeywords; i++){
                requeteFINAL += marque.keywords[i];
                requeteFINAL += ", ";
            }
            for (int i = 0; i < iNbKeywordsMETA; i++){
                requeteMETAFINAL += marque.keywords[i];
                requeteMETAFINAL += ", ";
            }
            
            marque.Requete = requeteFINAL;
            marque.meta_requete = requeteMETAFINAL;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = modelChosen,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a SEO expert on tool categories" },
                        new { role = "user", content = requeteFINAL }
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                string responseBody = await response.Content.ReadAsStringAsync();
                OpenAIResponse openAIResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseBody);

                marque.modele = openAIResponse?.Model ?? "Unknown Model";
                marque.Description = openAIResponse?.Choices?[0]?.Message?.Content ?? "No content";
                tokensUseFirst = openAIResponse?.Usage?.TotalTokens ?? 0;
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody2 = new
                {
                    model = modelChosen,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a SEO expert on tool categories" },
                        new { role = "user", content = requeteMETAFINAL }
                    }
                };

                string jsonContent2 = JsonConvert.SerializeObject(requestBody2);
                var content2 = new StringContent(jsonContent2, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response2 = await client.PostAsync(apiUrl, content2);

                string responseBody2 = await response2.Content.ReadAsStringAsync();

                if (!response2.IsSuccessStatusCode)
                {
                    // Log or throw a detailed error
                    throw new Exception($"Erreur API OpenAI ({(int)response2.StatusCode}): {responseBody2}");
                }

                // Optionally: check if the response starts with '{' to confirm it's JSON
                if (!responseBody2.TrimStart().StartsWith("{"))
                {
                    throw new Exception("La réponse n'est pas en format JSON : " + responseBody2.Substring(0, Math.Min(500, responseBody2.Length)));
                }

                OpenAIResponse openAIResponse2 = JsonConvert.DeserializeObject<OpenAIResponse>(responseBody2);
                int tokenUseSecond = openAIResponse2?.Usage?.TotalTokens ?? 0;
                marque.meta_description = openAIResponse2?.Choices?[0]?.Message?.Content ?? "No content";
                marque.tokens = tokensUseFirst + tokenUseSecond;
            }
        
        return marque;
    }

    public async Task<Marque> GenererDescriptionGemini(Marque marque, bool ANGLAIS){
        string requete = "Rédigez un texte descriptif d’environ 80 mots (maximum 100) sur la marque suivante : " + marque.Nom + ". Le texte doit comporter deux très courts paragraphes de deux phrases: le premier sur les types d’outils qu’elle propose, et le deuxième sur ce qui la distingue sur le marché. Utilisez un ton informatif, fluide et naturel, sans style promotionnel ni titres. Voici quelques produits que nous gardons en référence (ne pas inclure de numéros ni de listes dans la description) : ";
        
        string requeteMETA = "Nous sommes un magasin d'outil appelé Outillage Placide Mathieu, Créé une meta-description internet de la page de produit de la marque " + marque.Nom + ", Maximum 250 caractères, insérez un Visitez Placide.com à la fin, voici des produits que nous offrons de cette marque mais n'inclu aucun chiffres dans la description : ";
        
        string requeteANG = "Write a descriptive text of approximately 80 words (maximum 100) about the following brand: " + marque.Nom + ". The text must include two very short 2 sentence paragraphs: the first about the types of tools it offers, and the second about what makes it stand out in the market. Use an informative, natural, and fluent tone, with no promotional style or titles. Here are some reference products we carry (do not include any numbers or lists in the description): ";
        
        string requeteMETAANG = "We are a tool store called Outillage Placide Mathieu, Created a meta description of the internet product page for the brand " + marque.Nom + ", Maximum 250 characters, insert a Visit Placide.com at the end, here are some products we offer from this brandbut do not include any numbers in the description: ";
        
        int iNbKeywords = marque.keywords.Count() < 15 ? marque.keywords.Count() : 15;
        int iNbKeywordsMETA = marque.keywords.Count() < 5 ? marque.keywords.Count() : 5;
        
            string requeteFINAL = ANGLAIS ? requeteANG : requete;
            string requeteMETAFINAL = ANGLAIS ? requeteMETAANG : requeteMETA;
            for (int i = 0; i < iNbKeywords; i++){
                requeteFINAL += marque.keywords[i];
                requeteFINAL += ", ";
            }
            for (int i = 0; i < iNbKeywordsMETA; i++){
                requeteMETAFINAL += marque.keywords[i];
                requeteMETAFINAL += ", ";
            }
            
            marque.Requete = requeteFINAL;
            marque.meta_requete = requeteMETAFINAL;
            using (HttpClient client = new HttpClient())
            {

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = requeteFINAL }
                            }
                        }
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrlGemini, content);

                string responseBody = await response.Content.ReadAsStringAsync();
                GeminiResponse geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseBody);

                marque.modele = "gemini-2.0-flash";
                marque.Description = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "No response";
                marque.tokens = 0;
            }
            using (HttpClient client = new HttpClient())
            {

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = requeteMETAFINAL }
                            }
                        }
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrlGemini, content);

                string responseBody = await response.Content.ReadAsStringAsync();
                GeminiResponse geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseBody);

                marque.modele = "gemini-2.0-flash";
                marque.meta_description = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "No response";
                marque.tokens = 0;
            }
        
        return marque;
    }
}
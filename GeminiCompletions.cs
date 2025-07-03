using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class GeminiResponse
{
    [JsonProperty("candidates")]
    public Candidate[] Candidates { get; set; }
}

public class Candidate
{
    [JsonProperty("content")]
    public Content Content { get; set; }
}

public class Content
{
    [JsonProperty("parts")]
    public Part[] Parts { get; set; }
}

public class Part
{
    [JsonProperty("text")]
    public string Text { get; set; }
}

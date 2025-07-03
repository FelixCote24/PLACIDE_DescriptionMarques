using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class OpenAIResponse
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("choices")]
    public required Choice[] Choices { get; set; }

    [JsonPropertyName("usage")]
    public required Usage Usage { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public required Message Message { get; set; }
}

public class Message
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}

public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}
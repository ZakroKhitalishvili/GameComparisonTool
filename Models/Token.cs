using System;
using System.Text.Json.Serialization;

namespace GameComparisonTool.Models;

public class Token
{
    public required string AccessToken { get; set; }
    public DateTime Expiry { get; set; }
}

public class IgdbTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
}


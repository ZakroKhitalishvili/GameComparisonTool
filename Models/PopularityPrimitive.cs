using System.Text.Json.Serialization;

namespace GameComparisonTool.Models;

public class PopularityPrimitive
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("game_id")]
    public int GameId { get; set; }

    [JsonPropertyName("popularity_type")]
    public PopularityType PopularityType { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }
}

public enum PopularityType
{
    Visits = 1,
    WantsToPlay = 2,
    Playing = 3,
    Played = 4,
}

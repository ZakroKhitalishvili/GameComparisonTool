using System.Text.Json.Serialization;

namespace GameComparisonTool.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("age_ratings")]
    public List<AgeRating>? AgeRatings { get; set; }

    [JsonPropertyName("category")]
    public GameCategory Category { get; set; }

    [JsonPropertyName("cover")]
    public Cover? Cover { get; set; }

    [JsonPropertyName("genres")]
    public List<Genre>? Genres { get; set; }

    [JsonPropertyName("keywords")]
    public List<Keyword>? Keywords { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("platforms")]
    public List<Platform>? Platforms { get; set; }

    [JsonPropertyName("player_perspectives")]
    public List<PlayerPerspective>? PlayerPerspectives { get; set; }

    [JsonPropertyName("release_dates")]
    public List<ReleaseDate>? ReleaseDates { get; set; }

    [JsonPropertyName("screenshots")]
    public List<Screenshot>? Screenshots { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("total_rating")]
    public double TotalRating { get; set; }

    [JsonPropertyName("total_rating_count")]
    public int TotalRatingCount { get; set; }

    [JsonPropertyName("websites")]
    public List<Website>? Websites { get; set; }

    [JsonPropertyName("language_supports")]
    public List<LanguageSupport>? LanguageSupports { get; set; }

    [JsonPropertyName("franchises")]
    public List<Franchise>? Franchises { get; set; }

    [JsonPropertyName("involved_companies")]
    public List<InvolvedCompany>? InvolvedCompanies { get; set; }

    [JsonPropertyName("videos")]
    public List<Video>? Videos { get; set; }
}

public enum GameCategory
{
    MainGame = 0,
    DlcAddon = 1,
    Expansion = 2,
    Bundle = 3,
    StandaloneExpansion = 4,
    Mod = 5,
    Episode = 6,
    Season = 7,
    Remake = 8,
    Remaster = 9,
    ExpandedGame = 10,
    Port = 11,
    Fork = 12,
    Pack = 13,
    Update = 14
}


public class AgeRating
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("category")]
    public AgeRatingCategory Category { get; set; }

    [JsonPropertyName("rating")]
    public AgeRatingValue Rating { get; set; }
}

public enum AgeRatingCategory
{
    ESRB = 1,
    PEGI = 2,
    CERO = 3,
    USK = 4,
    GRAC = 5,
    CLASS_IND = 6,
    ACB = 7
}

public enum AgeRatingValue
{
    Three = 1,
    Seven = 2,
    Twelve = 3,
    Sixteen = 4,
    Eighteen = 5,
    RP = 6,
    EC = 7,
    E = 8,
    E10 = 9,
    T = 10,
    M = 11,
    AO = 12,
    CERO_A = 13,
    CERO_B = 14,
    CERO_C = 15,
    CERO_D = 16,
    CERO_Z = 17,
    USK_0 = 18,
    USK_6 = 19,
    USK_12 = 20,
    USK_16 = 21,
    USK_18 = 22,
    GRAC_ALL = 23,
    GRAC_Twelve = 24,
    GRAC_Fifteen = 25,
    GRAC_Eighteen = 26,
    GRAC_TESTING = 27,
    CLASS_IND_L = 28,
    CLASS_IND_Ten = 29,
    CLASS_IND_Twelve = 30,
    CLASS_IND_Fourteen = 31,
    CLASS_IND_Sixteen = 32,
    CLASS_IND_Eighteen = 33,
    ACB_G = 34,
    ACB_PG = 35,
    ACB_M = 36,
    ACB_MA15 = 37,
    ACB_R18 = 38,
    ACB_RC = 39
}

public class Cover
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class Keyword
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class Platform
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class PlayerPerspective
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("updated_at")]
    public long UpdatedAt { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("checksum")]
    public string? Checksum { get; set; }
}

public class ReleaseDate
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("y")]
    public int? Year { get; set; }
}

public class Screenshot
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class Website
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("category")]
    public WebsiteCategory Category { get; set; }

    [JsonPropertyName("trusted")]
    public bool Trusted { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class LanguageSupport
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("language")]
    public Language? Language { get; set; }
}

public class Language
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}


public enum WebsiteCategory
{
    Official = 1,
    Wikia = 2,
    Wikipedia = 3,
    Facebook = 4,
    Twitter = 5,
    Twitch = 6,
    Instagram = 8,
    YouTube = 9,
    iPhone = 10,
    iPad = 11,
    Android = 12,
    Steam = 13,
    Reddit = 14,
    Itch = 15,
    EpicGames = 16,
    Gog = 17,
    Discord = 18
}

public class Franchise
{

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("cover")]
    public Cover? Cover { get; set; }

}

public class InvolvedCompany
{

    [JsonPropertyName("developer")]
    public bool IsDeveloper { get; set; }

    [JsonPropertyName("publisher")]
    public bool IsPublisher { get; set; }

    [JsonPropertyName("company")]
    public Company? Company { get; set; }
}

public class Company
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("logo")]
    public Cover? Logo { get; set; }

    [JsonPropertyName("websites")]
    public List<Website>? Websites { get; set; }

}


public class Video
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("video_id")]
    public string? VideoId { get; set; }
}

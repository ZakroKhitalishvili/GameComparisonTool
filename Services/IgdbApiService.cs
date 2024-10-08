using System.Collections.Concurrent;
using System.Text;
using GameComparisonTool.Models;
using Microsoft.Extensions.Caching.Memory;

public class IgdbApiService
{
    private readonly HttpClient _httpClient;
    private const string GameFields =
    """
    fields 
    created_at,
    updated_at,
    summary,
    storyline,
    player_perspectives.*,
    release_dates.y, 
    platforms.name ,
    name,
    genres.name,
    cover.url, 
    screenshots.url,
    category,id,
    total_rating,
    total_rating_count,
    age_ratings.category,
    age_ratings.rating,
    hypes,
    keywords.name,
    language_supports.language.name,
    websites.category,
    websites.url,
    websites.trusted,
    franchises.name,
    franchises.games.name,
    franchises.games.cover.url,
    involved_companies,
    involved_companies.developer,
    involved_companies.publisher,
    involved_companies.company.name,
    involved_companies.company.logo.url,
    involved_companies.company.websites.category,
    involved_companies.company.websites.url,
    videos.name,
    videos.video_id
    ;
    """;

    public IgdbApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedList<Game>?> GetGamesAsync(string? searchTerm, int pageSize = 10, int page = 1)
    {
        int limit = pageSize;

        int offset = (page - 1) * pageSize;

        string requestBody;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            requestBody = $"{GameFields} sort created_at desc; limit {limit}; offset: {offset};";
        }
        else
        {
            requestBody = $"{GameFields} search \"{searchTerm}\"; limit {limit}; offset: {offset};";
        }

        var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
        var response = await _httpClient.PostAsync("games", content);

        response.EnsureSuccessStatusCode();

        var games = await response.Content.ReadFromJsonAsync<List<Game>>();
        int totalCount = int.Parse(response.Headers.GetValues("x-count").First());

        return new PagedList<Game>
        {
            Items = games ?? new(),
            Page = page,
            PageSize = pageSize,
            Total = totalCount,
        };
    }

    public async Task<IList<Game>?> GetRecommendedGamesAsync(int size, CancellationToken cancellationToken)
    {

        //get top 500 most recently updated games
        var gameRequestBody = "fields id; sort updated_at desc; limit 500;";
        var content = new StringContent(gameRequestBody, Encoding.UTF8, "text/plain");
        var response = await _httpClient.PostAsync("games", content);

        response.EnsureSuccessStatusCode();

        var games = await response.Content.ReadFromJsonAsync<List<Game>>();

        if (games is null)
            return null;

        var gameMap = games.ToDictionary(x => x.Id, x => x);
        var gameIds = games.Select(x => x.Id);

        // fetch popularity records for the given games, limit max size by 10,000
        var popularityPrimitives = await GetPopularityPrimitives(10_000, gameIds.ToList(), cancellationToken);

        if (popularityPrimitives is null) return null;

        var ranking = new SortedSet<(double score, Game game)>(
            Comparer<(double score, Game game)>.Create(
        (x, y) =>
        {
            return y.score.CompareTo(x.score);
        }));

        foreach (var gamePopularityPrimitives in popularityPrimitives.GroupBy(x => x.GameId))
        {
            double? visitValue = gamePopularityPrimitives.FirstOrDefault(x => x.PopularityType == PopularityType.Visits)?.Value;
            double? wantsToPlayValue = gamePopularityPrimitives.FirstOrDefault(x => x.PopularityType == PopularityType.WantsToPlay)?.Value;
            double? playedValue = gamePopularityPrimitives.FirstOrDefault(x => x.PopularityType == PopularityType.Played)?.Value;
            double? playingValue = gamePopularityPrimitives.FirstOrDefault(x => x.PopularityType == PopularityType.Playing)?.Value;

            var game = gameMap[gamePopularityPrimitives.Key];

            var score = CalculateScore(game, visitValue, wantsToPlayValue, playedValue, playedValue);

            if (score > 0)
            {
                ranking.Add((score, game));
            }
        }

        var finalGameIds = ranking.Take(size).Select(x => x.game.Id).ToList();
        var finalGames = await GetGamesByIdsAsync(finalGameIds);


        return finalGames;

    }

    public async Task<Game?> GetGameByIdAsync(int gameId)
    {

        var requestBody = $"{GameFields} where id = {gameId};";

        var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");

        var response = await _httpClient.PostAsync("games", content);

        response.EnsureSuccessStatusCode();

        var games = await response.Content.ReadFromJsonAsync<List<Game>>();

        return games?.Count > 0 ? games[0] : null;
    }

    public async Task<List<Game>?> GetGamesByIdsAsync(IList<int> gameIds)
    {

        var idsString = string.Join(", ", gameIds);

        var query = $"{GameFields} where id = ({idsString}); limit 500;";

        var content = new StringContent(query);

        var response = await _httpClient.PostAsync("games", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Game>>();
    }

    /// <summary>
    /// Calculates weighted score for the game based on the popularity primitives.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="visitValue">Popularity primitive based on how many visits the game</param>
    /// <param name="wantsToPlayValue">Popularity primitive based on how many wants to play the game</param>
    /// <param name="playedValue">Popularity primitive based on how many have played the game before</param>
    /// <param name="playingValue">Popularity primitive based on how many play actively the game</param>
    /// <returns>Floating number between <c>0</c> and <c>1</c></returns>
    private double CalculateScore(Game game, double? visitValue, double? wantsToPlayValue, double? playedValue, double? playingValue)
    {
        double totalWeight = 0;
        double score = 0;

        if (visitValue.HasValue)
        {
            score += visitValue.Value * 0.1;
            totalWeight += 0.1;
        }

        if (wantsToPlayValue.HasValue)
        {
            score += wantsToPlayValue.Value * 0.2;
            totalWeight += 0.2;
        }

        if (playedValue.HasValue)
        {
            score += playedValue.Value * 0.3;
            totalWeight += 0.3;
        }

        if (playingValue.HasValue)
        {
            score += playingValue.Value * 0.3;
            totalWeight += 0.3;
        }

        var recentYear = game.ReleaseDates?.MaxBy(x => x.Year)?.Year;

        if (recentYear.HasValue)
        {
            score += 0.1 / Math.Max(1, DateTime.UtcNow.Year - recentYear.Value + 1);
            totalWeight += 0.1;
        }

        if (totalWeight > 0)
        {
            score /= totalWeight;
        }

        return score;
    }

    private async Task<IEnumerable<PopularityPrimitive>> GetPopularityPrimitives(int maxSize, IList<int>? gameIds, CancellationToken cancellationToken)
    {
        int limit = 500;
        var result = new ConcurrentBag<PopularityPrimitive>();
        string filterRequest = string.Empty;

        if (gameIds is null)
        {
            filterRequest = $"fields game_id,value,popularity_type;";
        }
        else
        {
            filterRequest = $"fields game_id,value,popularity_type; where game_id = ({string.Join(',', gameIds)});";
        }

        // make an initial call to find out total count
        var totalCountRequestBody = $"{filterRequest} limit 1;";
        var totalCountRequestContent = new StringContent(totalCountRequestBody, Encoding.UTF8, "text/plain");
        var totalCountResponse = await _httpClient.PostAsync("popularity_primitives", totalCountRequestContent, cancellationToken);
        totalCountResponse.EnsureSuccessStatusCode();
        int totalCount = int.Parse(totalCountResponse.Headers.GetValues("x-count").First());

        // calculate necessary amount for requests
        var requestCount = (int)Math.Ceiling(Math.Min(maxSize, totalCount) * 1.0 / limit);

        ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = 5 };

        await Parallel.ForAsync(0, requestCount - 1, async (index, parallelCancellationToken) =>
        {
            int offset = index * limit;
            var requestBody = $"{filterRequest} limit {limit}; offset {offset};";
            var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("popularity_primitives", content, parallelCancellationToken);
            response.EnsureSuccessStatusCode();

            var popularityPrimitives = await response.Content.ReadFromJsonAsync<List<PopularityPrimitive>>();

            if (popularityPrimitives is null)
                return;

            foreach (var primitive in popularityPrimitives)
            {
                result.Add(primitive);
            }

        });

        return result;

    }
}

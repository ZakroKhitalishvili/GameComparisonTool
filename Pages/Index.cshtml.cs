using System.Text.Json;
using GameComparisonTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;

[OutputCache]
public class IndexModel : PageModel
{
    private readonly int _pageSize = 12;
    private readonly IgdbApiService _igdbService;
    public IList<Game> Games { get; set; } = new List<Game>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; } = 0;

    public IndexModel(IgdbApiService igdbService)
    {
        _igdbService = igdbService;
    }

    [OutputCache(PolicyName = "ExpireHour")]
    public async Task OnGetAsync()
    {
        var pagedList = await _igdbService.GetGamesAsync(SearchTerm, page: PageNumber, pageSize: _pageSize);
        if (pagedList is not null)
        {
            TotalPages = (int)Math.Ceiling(pagedList.Total * 1.0 / _pageSize);
            Games = pagedList.Items;
        }
    }

    public RedirectToPageResult OnPost(string operation, int id)
    {
        switch (operation)
        {
            case "Compare":
                return AddForComparison(id);
        }

        return new RedirectToPageResult("Index");
    }

    private RedirectToPageResult AddForComparison(int id)
    {
        string sessionKey = nameof(GameComparison);

        var comparisonStr = HttpContext.Session.GetString(sessionKey);
        GameComparison? gameComparison = null;

        if (!string.IsNullOrWhiteSpace(comparisonStr))
            gameComparison = JsonSerializer.Deserialize<GameComparison>(comparisonStr);

        gameComparison ??= new GameComparison
        {
            Games = new()
        };

        if (gameComparison.Games.Count >= 5)
        {
            TempData["Error"] = "You already have too many games in the comparison list. Remove others in order to add new ones.";
            return new RedirectToPageResult("GameCompare");
        }

        gameComparison.Games.Add(id);

        comparisonStr = JsonSerializer.Serialize(gameComparison);
        HttpContext.Session.SetString(sessionKey, comparisonStr);

        if (gameComparison.Games.Count > 1)
        {
            return new RedirectToPageResult("GameCompare");
        }

        TempData["Success"] = "Successfully added the game to the comparison list.";

        return new RedirectToPageResult("GameCompare");
    }
}

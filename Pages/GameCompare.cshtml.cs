using System.Text.Json;
using GameComparisonTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class GameCompareModel : PageModel
{
    private readonly IgdbApiService _apiService;

    public GameCompareModel(IgdbApiService apiService)
    {
        _apiService = apiService;
    }

    public List<Game>? Games { get; set; }

    public async Task OnGetAsync()
    {

        string sessionKey = nameof(GameComparison);

        var comparisonStr = HttpContext.Session.GetString(sessionKey);

        GameComparison? gameComparison = null;

        if (!string.IsNullOrWhiteSpace(comparisonStr))
            gameComparison = JsonSerializer.Deserialize<GameComparison>(comparisonStr);

        if (gameComparison is not null && gameComparison.Games.Any())
        {
            Games = await _apiService.GetGamesByIdsAsync(gameComparison.Games.ToList());
        }

        ViewData["Success"] = TempData["Success"];
        ViewData["Error"] = TempData["Error"];
    }

    public RedirectToPageResult OnPost(string operation, int id)
    {
        switch (operation)
        {
            case "Remove":
                return RemoveFromComparison(id);
        }

        return new RedirectToPageResult("GameCompare");
    }

    private RedirectToPageResult RemoveFromComparison(int id)
    {
        string sessionKey = nameof(GameComparison);
        var comparisonStr = HttpContext.Session.GetString(sessionKey);
        GameComparison? gameComparison = null;

        if (!string.IsNullOrWhiteSpace(comparisonStr))
            gameComparison = JsonSerializer.Deserialize<GameComparison>(comparisonStr);

        if (gameComparison is null)
            return new RedirectToPageResult("GameCompare");

        gameComparison.Games.Remove(id);
        comparisonStr = JsonSerializer.Serialize(gameComparison);

        HttpContext.Session.SetString(sessionKey, comparisonStr);
        TempData["Success"] = "Game successfully removed";

        return new RedirectToPageResult("GameCompare");
    }
}

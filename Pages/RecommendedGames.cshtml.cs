using GameComparisonTool.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;

[OutputCache(PolicyName = "ExpireHour")]
public class RecommendedGames : PageModel
{
    private readonly int _pageSize = 12;
    private readonly IgdbApiService _igdbService;
    public IList<Game>? Games { get; set; } = new List<Game>();


    public RecommendedGames(IgdbApiService igdbService)
    {
        _igdbService = igdbService;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Games = await _igdbService.GetRecommendedGamesAsync(_pageSize, cancellationToken);
    }
}

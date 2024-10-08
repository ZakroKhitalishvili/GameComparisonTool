
using GameComparisonTool.Models;
using Microsoft.AspNetCore.Mvc;

public class IgdbTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private Token? _token;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public IgdbTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Token?> GetAccessTokenAsync()
    {
        if (_token is not null && _token.Expiry > DateTime.UtcNow)
        {
            return _token;
        }

        await _semaphore.WaitAsync();

        try
        {
            // Double-check to ensure token wasn't refreshed while waiting
            if (_token is not null && _token.Expiry > DateTime.UtcNow)
            {
                return _token;
            }

            var httpClient = _httpClientFactory.CreateClient("IgdbAuth");

            var clientId = _configuration["IGDB:ClientId"]!;
            var clientSecret = _configuration["IGDB:ClientSecret"]!;

            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            ]);

            var response = await httpClient.PostAsync("token", content);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<IgdbTokenResponse>();

            _token = new Token
            {
                AccessToken = tokenResponse!.AccessToken,
                Expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            };

            return _token;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

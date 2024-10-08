using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("ExpireHour", builder =>
    builder.Expire(TimeSpan.FromHours(1)));
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddRazorPages();
}

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IgdbTokenService>();
builder.Services.AddTransient<IgdbApiService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("IgdbAuth", client =>
{
    client.BaseAddress = new Uri("https://id.twitch.tv/oauth2/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IgdbApiService>((sp, client) =>
{
    var tokenService = sp.GetRequiredService<IgdbTokenService>();

    client.BaseAddress = new Uri("https://api.igdb.com/v4/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var token = tokenService.GetAccessTokenAsync().Result;

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
    client.DefaultRequestHeaders.Add("Client-ID", [builder.Configuration["IGDB:ClientId"]]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseOutputCache();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();

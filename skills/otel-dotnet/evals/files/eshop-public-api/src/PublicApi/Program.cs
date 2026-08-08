using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseInMemoryDatabase("Catalog"));
builder.Services.AddHttpClient<CatalogClient>(client =>
    client.BaseAddress = new Uri("https://catalog.example.test"));

var app = builder.Build();

app.MapGet("/catalog/{id:int}", async (
    int id,
    CatalogContext database,
    CatalogClient catalog,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("Loading catalog item {CatalogItemId}", id);
    var local = await database.Items.FindAsync([id], cancellationToken);
    return local ?? await catalog.GetAsync(id, cancellationToken);
});

app.Logger.LogInformation("Public API starting");
app.Run();

sealed class CatalogClient(HttpClient client)
{
    public Task<CatalogItem?> GetAsync(int id, CancellationToken cancellationToken) =>
        client.GetFromJsonAsync<CatalogItem>($"/items/{id}", cancellationToken);
}

sealed class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
{
    public DbSet<CatalogItem> Items => Set<CatalogItem>();
}

sealed record CatalogItem(int Id, string Name);

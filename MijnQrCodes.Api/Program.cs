using MijnQrCodes.Application._di;
using MijnQrCodes.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

app.MapGet("/r/{shortCode}", async (string shortCode, IShortUrlRepository repository, IShortUrlVisitRepository visitRepository) =>
{
    var shortUrl = await repository.GetByShortCode(shortCode);
    if (shortUrl is null)
    {
        return Results.NotFound();
    }

    await visitRepository.RecordVisit(shortUrl.Id);
    return Results.Redirect(shortUrl.OriginalUrl);
});

app.Run();

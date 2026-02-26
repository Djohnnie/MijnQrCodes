using System.Globalization;
using MediatR;
using MijnQrCodes.Contracts.Dashboard;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Dashboard.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, GetDashboardStatsResponse>
{
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IShortUrlVisitRepository _visitRepository;

    private static readonly string[] DayNames =
        ["Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag"];

    public GetDashboardStatsQueryHandler(
        IShortUrlRepository shortUrlRepository,
        IShortUrlVisitRepository visitRepository)
    {
        _shortUrlRepository = shortUrlRepository;
        _visitRepository = visitRepository;
    }

    public async Task<GetDashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var links = await _shortUrlRepository.GetAll();
        var visits = await _visitRepository.GetAllVisits();

        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);

        var topLinks = visits
            .GroupBy(v => v.ShortUrlId)
            .Select(g =>
            {
                var link = links.FirstOrDefault(l => l.Id == g.Key);
                return new TopLinkDto
                {
                    Title = link?.Title ?? "Onbekend",
                    ShortCode = link?.ShortCode ?? "",
                    Clicks = g.Count()
                };
            })
            .OrderByDescending(t => t.Clicks)
            .Take(5)
            .ToList();

        var bestDay = visits
            .GroupBy(v => ((int)v.VisitedAt.DayOfWeek + 6) % 7)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return new GetDashboardStatsResponse
        {
            TotalLinks = links.Count,
            TotalClicks = visits.Count,
            ClicksToday = visits.Count(v => v.VisitedAt.Date == today),
            ClicksThisWeek = visits.Count(v => v.VisitedAt.Date >= weekStart),
            ClicksThisMonth = visits.Count(v => v.VisitedAt.Date >= monthStart),
            ClicksThisYear = visits.Count(v => v.VisitedAt.Date >= yearStart),
            TopLinks = topLinks,
            BestDayOfWeek = bestDay is not null ? DayNames[bestDay.Key] : "-",
            BestDayOfWeekClicks = bestDay?.Count() ?? 0
        };
    }
}

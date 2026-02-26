using System.Globalization;
using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetVisitStatsQueryHandler : IRequestHandler<GetVisitStatsQuery, GetVisitStatsResponse>
{
    private readonly IShortUrlVisitRepository _visitRepository;

    public GetVisitStatsQueryHandler(IShortUrlVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<GetVisitStatsResponse> Handle(GetVisitStatsQuery request, CancellationToken cancellationToken)
    {
        var visits = await _visitRepository.GetVisits(request.ShortUrlId);

        var now = DateTime.UtcNow.Date;

        var dailyVisits = Enumerable.Range(0, 30)
            .Select(i => now.AddDays(-29 + i))
            .Select(date => new VisitDataPoint
            {
                Date = date,
                Count = visits.Count(v => v.VisitedAt.Date == date)
            })
            .ToList();

        var weeklyVisits = Enumerable.Range(0, 12)
            .Select(i =>
            {
                var weekStart = now.AddDays(-(int)now.DayOfWeek + 1).AddDays(-7 * (11 - i));
                return new VisitDataPoint
                {
                    Date = weekStart,
                    Count = visits.Count(v => v.VisitedAt.Date >= weekStart && v.VisitedAt.Date < weekStart.AddDays(7))
                };
            })
            .ToList();

        var monthlyVisits = Enumerable.Range(0, 12)
            .Select(i =>
            {
                var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-11 + i);
                var monthEnd = monthStart.AddMonths(1);
                return new VisitDataPoint
                {
                    Date = monthStart,
                    Count = visits.Count(v => v.VisitedAt.Date >= monthStart && v.VisitedAt.Date < monthEnd)
                };
            })
            .ToList();

        return new GetVisitStatsResponse
        {
            TotalVisits = visits.Count,
            DailyVisits = dailyVisits,
            WeeklyVisits = weeklyVisits,
            MonthlyVisits = monthlyVisits
        };
    }
}

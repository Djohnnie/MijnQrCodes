namespace MijnQrCodes.Contracts.ShortUrls;

public class GetVisitStatsResponse
{
    public int TotalVisits { get; set; }
    public List<VisitDataPoint> DailyVisits { get; set; } = [];
    public List<VisitDataPoint> WeeklyVisits { get; set; } = [];
    public List<VisitDataPoint> MonthlyVisits { get; set; } = [];
}

public class VisitDataPoint
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

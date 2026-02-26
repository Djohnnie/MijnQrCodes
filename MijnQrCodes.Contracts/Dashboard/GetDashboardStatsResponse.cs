namespace MijnQrCodes.Contracts.Dashboard;

public class GetDashboardStatsResponse
{
    public int TotalLinks { get; set; }
    public int TotalClicks { get; set; }
    public int ClicksToday { get; set; }
    public int ClicksThisWeek { get; set; }
    public int ClicksThisMonth { get; set; }
    public int ClicksThisYear { get; set; }
    public List<TopLinkDto> TopLinks { get; set; } = [];
    public string BestDayOfWeek { get; set; } = string.Empty;
    public int BestDayOfWeekClicks { get; set; }
}

public class TopLinkDto
{
    public string Title { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public int Clicks { get; set; }
}

namespace MijnQrCodes.Domain;

public class Link
{
    public Guid Id { get; set; }
    public int SysId { get; set; }
    public string Code { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public bool HasIcon { get; set; }
    public string Icon { get; set; }
    public long NumberOfVisits { get; set; }
}
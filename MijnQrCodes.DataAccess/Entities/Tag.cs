namespace MijnQrCodes.DataAccess.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public long SysId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; } = "#6366f1";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ShortUrlTag> ShortUrlTags { get; set; } = [];
}

namespace MijnQrCodes.Contracts.Tags;

public class GetTagsResponse
{
    public List<TagDto> Tags { get; set; } = [];
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
